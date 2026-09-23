using SalesDashboard.Api.Entities;

namespace SalesDashboard.Api.Data;

public static class SeedData
{
    public static readonly Category[] Categories =
    {
        new() { Id = 1, Name = "Ноутбуки" },
        new() { Id = 2, Name = "Смартфоны" },
        new() { Id = 3, Name = "Аксессуары" },
        new() { Id = 4, Name = "Периферия" },
        new() { Id = 5, Name = "Мониторы" },
        new() { Id = 6, Name = "Сетевое оборудование" },
    };

    public static readonly Product[] Products = BuildProducts();
    public static readonly Manager[] Managers = BuildManagers();
    public static readonly Customer[] Customers = BuildCustomers();

    private static Product[] BuildProducts()
    {
        // Фиксированный seed → одинаковая генерация при каждом запуске
        var rnd = new Random(42);
        var list = new List<Product>();
        int id = 1;
        foreach (var cat in Categories)
        {
            for (int i = 1; i <= 10; i++)
            {
                var cost = Math.Round((decimal)(rnd.NextDouble() * 800 + 50), 2);
                var price = Math.Round(cost * (decimal)(1.2 + rnd.NextDouble() * 0.6), 2);
                list.Add(new Product
                {
                    Id = id++,
                    Name = $"{cat.Name} модель {i}",
                    CategoryId = cat.Id,
                    BaseCost = cost,
                    BasePrice = price,
                });
            }
        }
        return list.ToArray();
    }

    private static Manager[] BuildManagers()
    {
        var names = new[]
        {
            "Иван Петров", "Мария Смирнова", "Алексей Кузнецов", "Ольга Попова",
            "Дмитрий Соколов", "Екатерина Лебедева", "Сергей Новиков", "Анна Морозова",
            "Павел Волков", "Юлия Зайцева", "Артём Егоров", "Наталья Павлова",
            "Роман Козлов", "Виктория Степанова", "Михаил Николаев", "Татьяна Орлова",
            "Игорь Фёдоров", "Светлана Михайлова", "Никита Беляев", "Елена Виноградова"
        };
        var teams = new[] { "Alpha", "Bravo", "Charlie", "Delta" };
        var positions = new[] { "Junior Sales", "Sales Manager", "Senior Sales", "Team Lead" };
        var colors = new[] { "#6366f1", "#0ea5e9", "#10b981", "#f59e0b", "#ef4444", "#8b5cf6" };
        var rnd = new Random(1);

        return names.Select((n, i) => new Manager
        {
            Id = i + 1,
            FullName = n,
            Team = teams[i % teams.Length],
            Position = positions[rnd.Next(positions.Length)],
            IsActive = i % 10 != 0, // каждый 10-й неактивен
            AvatarColor = colors[i % colors.Length],
        }).ToArray();
    }

    private static Customer[] BuildCustomers()
    {
        var companies = new[]
        {
            "ООО Ромашка", "АО Вектор", "ИП Сидоров", "ООО ТехноМир", "АО Прогресс",
            "ООО Альфа", "ООО Бета", "ЗАО Гамма", "ООО Дельта", "АО Эпсилон"
        };
        var segments = new[] { "SMB", "Mid-Market", "Enterprise" };
        var rnd = new Random(7);
        var list = new List<Customer>();
        for (int i = 1; i <= 80; i++)
        {
            list.Add(new Customer
            {
                Id = i,
                Name = $"Контакт {i}",
                Company = $"{companies[i % companies.Length]} #{i}",
                Segment = segments[rnd.Next(segments.Length)],
            });
        }
        return list.ToArray();
    }
}