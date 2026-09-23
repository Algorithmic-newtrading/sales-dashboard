namespace SalesDashboard.Api.Dtos;

// KPI-карточки верхнего уровня
public record KpiDto(
    decimal Revenue,
    decimal GrossProfit,
    decimal Margin,
    int SalesCount,
    decimal AverageCheck,
    string? BestManagerName,
    decimal? BestManagerGrossProfit,
    decimal RevenueDelta,        // % к предыдущему периоду
    decimal GrossProfitDelta,
    decimal MarginDelta,
    decimal AverageCheckDelta);

// Строка рейтинга менеджеров
public record ManagerRatingDto(
    int Rank,
    int ManagerId,
    string FullName,
    string Team,
    string Position,
    string AvatarColor,
    int SalesCount,
    decimal Revenue,
    decimal GrossProfit,
    decimal AverageCheck,
    decimal Margin,
    decimal DeltaPercent);

// Точка графика динамики
public record TimelinePointDto(
    DateTime Date,
    decimal Revenue,
    decimal GrossProfit,
    int SalesCount);

// Статистика по категории
public record CategoryStatDto(
    int CategoryId,
    string CategoryName,
    decimal Revenue,
    decimal GrossProfit,
    int SalesCount);

// Топ-продукт
public record TopProductDto(
    int ProductId,
    string ProductName,
    string CategoryName,
    decimal Revenue,
    int Quantity);

// Строка последних продаж
public record RecentSaleDto(
    int Id,
    DateTime Date,
    string ManagerName,
    string CustomerName,
    string Products,
    string Status,
    decimal Total,
    decimal GrossProfit);

// Постраничный результат
public record PagedResult<T>(
    List<T> Items,
    int Total,
    int Offset,
    int Limit);