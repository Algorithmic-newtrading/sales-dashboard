namespace SalesDashboard.Api.Entities;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Company { get; set; } = "";
    public string Segment { get; set; } = "";
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}