using Microsoft.EntityFrameworkCore;
using SalesDashboard.Api.Data;
using SalesDashboard.Api.Entities;
using SalesDashboard.Api.Services;
using Xunit;

namespace SalesDashboard.Tests;

public class AnalyticsServiceTests
{
    private static AppDbContext NewDb()
    {
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(opts);
    }

    private static void SeedBasic(AppDbContext db)
    {
        db.Managers.Add(new Manager { Id = 1, FullName = "Иван Петров", Team = "Alpha", Position = "Sales", IsActive = true, AvatarColor = "#000" });
        db.Managers.Add(new Manager { Id = 2, FullName = "Мария Смирнова", Team = "Bravo", Position = "Sales", IsActive = true, AvatarColor = "#111" });
        db.Customers.Add(new Customer { Id = 1, Name = "Клиент", Company = "ООО Тест", Segment = "SMB" });
        db.Categories.Add(new Category { Id = 1, Name = "Кат" });
        db.Products.Add(new Product { Id = 1, Name = "Товар", CategoryId = 1, BasePrice = 100, BaseCost = 50 });
        db.SaveChanges();
    }

    private static Sale MakeSale(int id, int managerId, DateTime date, SaleStatus status, decimal unitPrice, decimal unitCost, int qty = 1)
    {
        return new Sale
        {
            Id = id,
            ManagerId = managerId,
            CustomerId = 1,
            Date = date,
            Status = status,
            Items = new List<SaleItem>
            {
                new() { ProductId = 1, Quantity = qty, UnitPrice = unitPrice, UnitCost = unitCost }
            }
        };
    }

    [Fact]
    public async Task Kpi_ExcludesCancelledAndRefunded()
    {
        var db = NewDb();
        SeedBasic(db);
        var date = new DateTime(2026, 9, 1, 12, 0, 0, DateTimeKind.Utc);

        db.Sales.Add(MakeSale(1, 1, date, SaleStatus.Paid,      100, 50, qty: 2));
        db.Sales.Add(MakeSale(2, 1, date, SaleStatus.Cancelled, 100, 50, qty: 5));
        db.Sales.Add(MakeSale(3, 1, date, SaleStatus.Refunded,  100, 50, qty: 5));
        await db.SaveChangesAsync();

        var svc = new AnalyticsService(db);
        var kpi = await svc.GetKpiAsync(date.AddDays(-1), date.AddDays(1), default);

        Assert.Equal(200m, kpi.Revenue);
        Assert.Equal(100m, kpi.GrossProfit);
        Assert.Equal(1, kpi.SalesCount);
        Assert.Equal(50m, kpi.Margin);
        Assert.Equal(200m, kpi.AverageCheck);
    }

    [Fact]
    public async Task Kpi_Margin_IsCorrect()
    {
        var db = NewDb();
        SeedBasic(db);
        var date = new DateTime(2026, 9, 1, 12, 0, 0, DateTimeKind.Utc);

        db.Sales.Add(MakeSale(1, 1, date, SaleStatus.Paid, 1000, 600));
        await db.SaveChangesAsync();

        var svc = new AnalyticsService(db);
        var kpi = await svc.GetKpiAsync(date.AddDays(-1), date.AddDays(1), default);

        Assert.Equal(40m, kpi.Margin);
    }

    [Fact]
    public async Task Kpi_AverageCheck_IsRevenueDividedByCount()
    {
        var db = NewDb();
        SeedBasic(db);
        var date = new DateTime(2026, 9, 1, 12, 0, 0, DateTimeKind.Utc);

        db.Sales.Add(MakeSale(1, 1, date, SaleStatus.Paid, 100, 50));
        db.Sales.Add(MakeSale(2, 1, date, SaleStatus.Paid, 200, 100));
        db.Sales.Add(MakeSale(3, 1, date, SaleStatus.Paid, 300, 150));
        await db.SaveChangesAsync();

        var svc = new AnalyticsService(db);
        var kpi = await svc.GetKpiAsync(date.AddDays(-1), date.AddDays(1), default);

        Assert.Equal(600m, kpi.Revenue);
        Assert.Equal(3, kpi.SalesCount);
        Assert.Equal(200m, kpi.AverageCheck);
    }

    [Fact]
    public async Task Kpi_EmptyPeriod_ReturnsZeros()
    {
        var db = NewDb();
        SeedBasic(db);
        var svc = new AnalyticsService(db);

        var kpi = await svc.GetKpiAsync(
            new DateTime(2030, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2030, 1, 2, 0, 0, 0, DateTimeKind.Utc),
            default);

        Assert.Equal(0m, kpi.Revenue);
        Assert.Equal(0m, kpi.GrossProfit);
        Assert.Equal(0m, kpi.Margin);
        Assert.Equal(0, kpi.SalesCount);
        Assert.Equal(0m, kpi.AverageCheck);
    }

    [Fact]
    public async Task Managers_SortBy_GrossProfit_And_AvgCheck()
    {
        var db = NewDb();
        SeedBasic(db);
        var date = new DateTime(2026, 9, 1, 12, 0, 0, DateTimeKind.Utc);

        db.Sales.Add(MakeSale(1, 1, date, SaleStatus.Paid, 1000, 500));
        db.Sales.Add(MakeSale(2, 2, date, SaleStatus.Paid, 200, 100));
        db.Sales.Add(MakeSale(3, 2, date, SaleStatus.Paid, 200, 100));
        await db.SaveChangesAsync();

        var svc = new AnalyticsService(db);

        var byGp = await svc.GetManagerRatingsAsync(date.AddDays(-1), date.AddDays(1), "grossProfit", default);
        Assert.Equal("Иван Петров", byGp[0].FullName);

        var byAvg = await svc.GetManagerRatingsAsync(date.AddDays(-1), date.AddDays(1), "avgCheck", default);
        Assert.Equal("Иван Петров", byAvg[0].FullName);
    }

    [Fact]
    public async Task Managers_EqualResults_SameRank()
    {
        var db = NewDb();
        SeedBasic(db);
        var date = new DateTime(2026, 9, 1, 12, 0, 0, DateTimeKind.Utc);

        db.Sales.Add(MakeSale(1, 1, date, SaleStatus.Paid, 1000, 500));
        db.Sales.Add(MakeSale(2, 2, date, SaleStatus.Paid, 1000, 500));
        await db.SaveChangesAsync();

        var svc = new AnalyticsService(db);
        var list = await svc.GetManagerRatingsAsync(date.AddDays(-1), date.AddDays(1), "grossProfit", default);

        Assert.Equal(2, list.Count);
        Assert.Equal(list[0].Rank, list[1].Rank);
        Assert.Equal(1, list[0].Rank);
    }

    [Fact]
    public async Task Timeline_GroupsByDay()
    {
        var db = NewDb();
        SeedBasic(db);
        var d1 = new DateTime(2026, 9, 1, 12, 0, 0, DateTimeKind.Utc);
        var d2 = new DateTime(2026, 9, 2, 12, 0, 0, DateTimeKind.Utc);

        db.Sales.Add(MakeSale(1, 1, d1, SaleStatus.Paid, 100, 50));
        db.Sales.Add(MakeSale(2, 1, d1, SaleStatus.Paid, 200, 100));
        db.Sales.Add(MakeSale(3, 1, d2, SaleStatus.Paid, 300, 150));
        await db.SaveChangesAsync();

        var svc = new AnalyticsService(db);
        var timeline = await svc.GetTimelineAsync(d1.AddDays(-1), d2.AddDays(1), "day", default);

        Assert.Equal(2, timeline.Count);
        Assert.Equal(300m, timeline[0].Revenue);
        Assert.Equal(300m, timeline[1].Revenue);
    }
}