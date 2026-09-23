using Microsoft.EntityFrameworkCore;
using SalesDashboard.Api.Entities;

namespace SalesDashboard.Api.Data;

public static class SaleSeeder
{
    public static async Task SeedSalesAsync(AppDbContext db, CancellationToken ct = default)
    {
        // Идемпотентность: если в БД уже есть данные — не трогаем
        if (await db.Sales.AnyAsync(ct)) return;

        var rnd = new Random(2024);
        var managers = await db.Managers.ToListAsync(ct);
        var customers = await db.Customers.ToListAsync(ct);
        var products = await db.Products.ToListAsync(ct);

        var strength = managers.ToDictionary(m => m.Id, _ => 0.4 + rnd.NextDouble() * 1.6);

        var today = DateTime.UtcNow.Date;
        var start = today.AddMonths(-12);

        // Собираем всё в память
        var sales = new List<Sale>(capacity: 5000);

        for (var date = start; date <= today; date = date.AddDays(1))
        {
            var seasonal = 1.0 + 0.35 * Math.Sin((date.Month - 1) / 12.0 * 2 * Math.PI);
            var weekday = date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday ? 0.4 : 1.0;

            foreach (var m in managers.Where(m => m.IsActive))
            {
                var deals = (int)Math.Round((rnd.NextDouble() * 2 + 0.5) * strength[m.Id] * seasonal * weekday);
                for (int d = 0; d < deals; d++)
                {
                    var status = rnd.NextDouble() switch
                    {
                        < 0.88 => SaleStatus.Paid,
                        < 0.95 => SaleStatus.Cancelled,
                        _      => SaleStatus.Refunded,
                    };

                    var sale = new Sale
                    {
                        ManagerId = m.Id,
                        CustomerId = customers[rnd.Next(customers.Count)].Id,
                        Date = date.AddHours(rnd.Next(9, 19)).AddMinutes(rnd.Next(60)),
                        Status = status,
                    };

                    var itemsCount = rnd.Next(1, 5);
                    var chosen = products.OrderBy(_ => rnd.Next()).Take(itemsCount).ToList();
                    foreach (var p in chosen)
                    {
                        var qty = rnd.Next(1, 6);
                        var priceFactor = 0.9m + (decimal)rnd.NextDouble() * 0.25m;
                        sale.Items.Add(new SaleItem
                        {
                            ProductId = p.Id,
                            Quantity = qty,
                            UnitPrice = Math.Round(p.BasePrice * priceFactor, 2),
                            UnitCost = p.BaseCost,
                        });
                    }
                    sales.Add(sale);
                }
            }
        }

        // Одна транзакция на всё — в разы быстрее и безопаснее
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        db.Sales.AddRange(sales);
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        // Отчёт в лог
        Console.WriteLine($"[SaleSeeder] Создано продаж: {sales.Count}");
    }
}