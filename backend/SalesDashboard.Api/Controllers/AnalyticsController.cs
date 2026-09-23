using Microsoft.AspNetCore.Mvc;
using SalesDashboard.Api.Services;

namespace SalesDashboard.Api.Controllers;

[ApiController]
[Route("api/analytics")]
public class AnalyticsController : ControllerBase
{
    private readonly AnalyticsService _svc;
    public AnalyticsController(AnalyticsService svc) => _svc = svc;

    // Нормализуем DateTime: ASP.NET Core парсит query-параметры как Local/Unspecified,
    // а Npgsql требует UTC для 'timestamp with time zone'.
    private static DateTime Normalize(DateTime d) =>
        d.Kind == DateTimeKind.Utc ? d : DateTime.SpecifyKind(d, DateTimeKind.Utc);

    [HttpGet("kpi")]
    public async Task<IActionResult> Kpi(
        [FromQuery] DateTime from, [FromQuery] DateTime to, CancellationToken ct)
    {
        from = Normalize(from);
        to = Normalize(to);
        if (from >= to)
            return BadRequest(new { error = "Параметр 'from' должен быть меньше 'to'" });
        return Ok(await _svc.GetKpiAsync(from, to, ct));
    }

    [HttpGet("managers")]
    public async Task<IActionResult> Managers(
        [FromQuery] DateTime from, [FromQuery] DateTime to,
        [FromQuery] string sortBy = "grossProfit", CancellationToken ct = default)
    {
        from = Normalize(from);
        to = Normalize(to);
        if (from >= to)
            return BadRequest(new { error = "Параметр 'from' должен быть меньше 'to'" });

        if (sortBy != "grossProfit" && sortBy != "avgCheck")
            return BadRequest(new { error = "sortBy должен быть 'grossProfit' или 'avgCheck'" });

        return Ok(await _svc.GetManagerRatingsAsync(from, to, sortBy, ct));
    }

    [HttpGet("timeline")]
    public async Task<IActionResult> Timeline(
        [FromQuery] DateTime from, [FromQuery] DateTime to,
        [FromQuery] string granularity = "day", CancellationToken ct = default)
    {
        from = Normalize(from);
        to = Normalize(to);
        if (from >= to)
            return BadRequest(new { error = "Параметр 'from' должен быть меньше 'to'" });

        if (granularity != "day" && granularity != "month")
            return BadRequest(new { error = "granularity должен быть 'day' или 'month'" });

        return Ok(await _svc.GetTimelineAsync(from, to, granularity, ct));
    }

    [HttpGet("categories")]
    public async Task<IActionResult> Categories(
        [FromQuery] DateTime from, [FromQuery] DateTime to, CancellationToken ct)
    {
        from = Normalize(from);
        to = Normalize(to);
        if (from >= to)
            return BadRequest(new { error = "Параметр 'from' должен быть меньше 'to'" });
        return Ok(await _svc.GetCategoriesAsync(from, to, ct));
    }

    [HttpGet("top-products")]
    public async Task<IActionResult> TopProducts(
        [FromQuery] DateTime from, [FromQuery] DateTime to,
        [FromQuery] int limit = 10, CancellationToken ct = default)
    {
        from = Normalize(from);
        to = Normalize(to);
        if (from >= to)
            return BadRequest(new { error = "Параметр 'from' должен быть меньше 'to'" });
        if (limit <= 0 || limit > 100) limit = 10;
        return Ok(await _svc.GetTopProductsAsync(from, to, limit, ct));
    }

    [HttpGet("recent-sales")]
    public async Task<IActionResult> RecentSales(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        [FromQuery] int offset = 0,
        [FromQuery] int limit = 20,
        CancellationToken ct = default)
    {
        from = Normalize(from);
        to = Normalize(to);
        if (from >= to)
            return BadRequest(new { error = "Параметр 'from' должен быть меньше 'to'" });
        return Ok(await _svc.GetRecentSalesAsync(from, to, offset, limit, ct));
    }
}