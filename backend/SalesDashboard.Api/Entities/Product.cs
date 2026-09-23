namespace SalesDashboard.Api.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public decimal BasePrice { get; set; }
    public decimal BaseCost { get; set; }
    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
}