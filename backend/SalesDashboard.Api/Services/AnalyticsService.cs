using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SalesDashboard.Api.Data;
using SalesDashboard.Api.Dtos;
using SalesDashboard.Api.Entities;

namespace SalesDashboard.Api.Services;

public class AnalyticsService
{
    private readonly AppDbContext _db;
    private readonly IMemoryCache _cache;
    private readonly ILogger<AnalyticsService> _logger;

    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(60);

    public AnalyticsService(
        AppDbContext db,
        IMemoryCache cache,
        ILogger<AnalyticsService> logger)
    {
        _db = db;
        _cache = cache;
        _logger = logger;
    }

    // Бизнес-правило: в выручку попадают только Paid-продажи.
    // Cancelled и Refunded исключаются из всех агрегатов.
    private IQueryable<Sale> Paid(IQueryable<Sale> q) =>
        q.Where(s => s.Status == SaleStatus.Paid);

    /// <summary>
    /// Кэширует результат, если запрошенный период полностью в прошлом.
    /// Текущий день не кэшируется — данные могут меняться.
    /// </summary>
    private async Task<T> CachedAsync<T>(
        string keyPrefix,
        DateTime from,
        DateTime to,
        Func<Task<T>> factory)
    {
        var today = DateTime.UtcNow.Date;
        var isPastPeriod = to <= today;

        if (!isPastPeriod)
            return await factory();

        var key = $"{keyPrefix}:{from:O}:{to:O}";
        if (_cache.TryGetValue(key, out T? cached) && cached != null)
        {
            _logger.LogDebug("Cache hit: {Key}", key);
            return cached;
        }

        var result = await factory();
        _cache.Set(key, result, CacheTtl);
        return result;
    }

    private async Task<(decimal rev, decimal gp, int cnt, decimal margin, decimal avg)>
        Aggregate(DateTime from, DateTime to, CancellationToken ct)
    {
        var query = Paid(_db.Sales).Where(s => s.Date >= from && s.Date < to);

        var rev = await query
            .SelectMany(s => s.Items)
            .SumAsync(i => (decimal?)(i.Quantity * i.UnitPrice), ct) ?? 0m;

        var cost = await query
            .SelectMany(s => s.Items)
            .SumAsync(i => (decimal?)(i.Quantity * i.UnitCost), ct) ?? 0m;

        var cnt = await query.CountAsync(ct);
        var gp = rev - cost;
        var margin = rev == 0 ? 0m : Math.Round(gp / rev * 100, 2);
        var avg = cnt == 0 ? 0m : Math.Round(rev / cnt, 2);
        return (rev, gp, cnt, margin, avg);
    }

    private static decimal Delta(decimal curr, decimal prev) =>
        prev == 0 ? 0 : Math.Round((curr - prev) / prev * 100, 2);

    public async Task<KpiDto> GetKpiAsync(DateTime from, DateTime to, CancellationToken ct)
    {
        return await CachedAsync("kpi", from, to, async () =>
        {
            var (rev, gp, cnt, margin, avg) = await Aggregate(from, to, ct);

            var len = to - from;
            var prevFrom = from - len;
            var prevTo = from;
            var (prevRev, prevGp, _, prevMargin, prevAvg) = await Aggregate(prevFrom, prevTo, ct);

            var ratings = await GetManagerRatingsAsync(from, to, "grossProfit", ct);
            var best = ratings.FirstOrDefault();

            return new KpiDto(
                rev, gp, margin, cnt, avg,
                best?.FullName,
                best?.GrossProfit,
                Delta(rev, prevRev),
                Delta(gp, prevGp),
                prevMargin == 0 ? 0 : Math.Round((margin - prevMargin) / prevMargin * 100, 2),
                Delta(avg, prevAvg));
        });
    }

    public async Task<List<ManagerRatingDto>> GetManagerRatingsAsync(
        DateTime from, DateTime to, string sortBy, CancellationToken ct)
    {
        var raw = await Paid(_db.Sales)
            .Where(s => s.Date >= from && s.Date < to)
            .GroupBy(s => s.Manager)
            .Select(g => new
            {
                Manager = g.Key,
                SalesCount = g.Count(),
                Revenue = g.SelectMany(s => s.Items)
                    .Sum(i => (decimal?)(i.Quantity * i.UnitPrice)) ?? 0m,
                Cost = g.SelectMany(s => s.Items)
                    .Sum(i => (decimal?)(i.Quantity * i.UnitCost)) ?? 0m,
            })
            .ToListAsync(ct);

        var list = raw.Select(x => new ManagerRatingDto(
            0,
            x.Manager.Id,
            x.Manager.FullName,
            x.Manager.Team,
            x.Manager.Position,
            x.Manager.AvatarColor,
            x.SalesCount,
            x.Revenue,
            x.Revenue - x.Cost,
            x.SalesCount == 0 ? 0 : Math.Round(x.Revenue / x.SalesCount, 2),
            x.Revenue == 0 ? 0 : Math.Round((x.Revenue - x.Cost) / x.Revenue * 100, 2),
            0
        )).ToList();

        list = sortBy == "avgCheck"
            ? list.OrderByDescending(x => x.AverageCheck).ThenBy(x => x.FullName).ToList()
            : list.OrderByDescending(x => x.GrossProfit).ThenBy(x => x.FullName).ToList();

        for (int i = 0; i < list.Count; i++)
        {
            var same = i > 0 && (
                (sortBy == "avgCheck" && list[i - 1].AverageCheck == list[i].AverageCheck) ||
                (sortBy != "avgCheck" && list[i - 1].GrossProfit == list[i].GrossProfit));
            list[i] = list[i] with { Rank = same ? list[i - 1].Rank : i + 1 };
        }
        return list;
    }

    public async Task<List<TimelinePointDto>> GetTimelineAsync(
        DateTime from, DateTime to, string granularity, CancellationToken ct)
    {
        return await CachedAsync($"timeline:{granularity}", from, to, async () =>
        {
            var raw = await Paid(_db.Sales)
                .Where(s => s.Date >= from && s.Date < to)
                .SelectMany(s => s.Items.Select(i => new { s.Date, i.Quantity, i.UnitPrice, i.UnitCost }))
                .GroupBy(x => x.Date.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(x => (decimal?)(x.Quantity * x.UnitPrice)) ?? 0m,
                    Cost = g.Sum(x => (decimal?)(x.Quantity * x.UnitCost)) ?? 0m,
                    SalesCount = g.Count(),
                })
                .OrderBy(x => x.Date)
                .ToListAsync(ct);

            if (granularity == "month")
            {
                return raw
                    .GroupBy(x => new DateTime(x.Date.Year, x.Date.Month, 1))
                    .Select(g => new TimelinePointDto(
                        g.Key,
                        g.Sum(x => x.Revenue),
                        g.Sum(x => x.Revenue - x.Cost),
                        g.Sum(x => x.SalesCount)))
                    .OrderBy(x => x.Date)
                    .ToList();
            }

            return raw.Select(x =>
                new TimelinePointDto(x.Date, x.Revenue, x.Revenue - x.Cost, x.SalesCount)).ToList();
        });
    }

    public async Task<List<CategoryStatDto>> GetCategoriesAsync(DateTime from, DateTime to, CancellationToken ct)
    {
        var raw = await Paid(_db.Sales)
            .Where(s => s.Date >= from && s.Date < to)
            .SelectMany(s => s.Items)
            .GroupBy(i => i.Product.Category)
            .Select(g => new
            {
                Category = g.Key,
                Revenue = g.Sum(i => (decimal?)(i.Quantity * i.UnitPrice)) ?? 0m,
                Cost = g.Sum(i => (decimal?)(i.Quantity * i.UnitCost)) ?? 0m,
                Count = g.Count(),
            })
            .ToListAsync(ct);

        return raw
            .Select(x => new CategoryStatDto(
                x.Category.Id, x.Category.Name,
                x.Revenue, x.Revenue - x.Cost, x.Count))
            .OrderByDescending(x => x.Revenue)
            .ToList();
    }

    public async Task<List<TopProductDto>> GetTopProductsAsync(DateTime from, DateTime to, int limit, CancellationToken ct)
    {
        var raw = await Paid(_db.Sales)
            .Where(s => s.Date >= from && s.Date < to)
            .SelectMany(s => s.Items)
            .GroupBy(i => i.Product)
            .Select(g => new
            {
                Product = g.Key,
                Revenue = g.Sum(i => (decimal?)(i.Quantity * i.UnitPrice)) ?? 0m,
                Qty = g.Sum(i => i.Quantity),
            })
            .OrderByDescending(x => x.Revenue)
            .Take(limit)
            .ToListAsync(ct);

        return raw.Select(x => new TopProductDto(
            x.Product.Id, x.Product.Name,
            x.Product.Category != null ? x.Product.Category.Name : "",
            x.Revenue, x.Qty)).ToList();
    }

    public async Task<PagedResult<RecentSaleDto>> GetRecentSalesAsync(
        DateTime from, DateTime to, int offset, int limit, CancellationToken ct)
    {
        if (offset < 0) offset = 0;
        if (limit <= 0 || limit > 200) limit = 20;

        var query = Paid(_db.Sales).Where(s => s.Date >= from && s.Date < to);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(s => s.Date)
            .Skip(offset)
            .Take(limit)
            .Select(s => new RecentSaleDto(
                s.Id,
                s.Date,
                s.Manager.FullName,
                s.Customer.Company,
                string.Join(", ", s.Items.Select(i => i.Product.Name)),
                s.Status.ToString(),
                s.Items.Sum(i => (decimal?)(i.Quantity * i.UnitPrice)) ?? 0m,
                s.Items.Sum(i => (decimal?)(i.Quantity * i.UnitPrice - i.Quantity * i.UnitCost)) ?? 0m))
            .ToListAsync(ct);

        return new PagedResult<RecentSaleDto>(items, total, offset, limit);
    }
}