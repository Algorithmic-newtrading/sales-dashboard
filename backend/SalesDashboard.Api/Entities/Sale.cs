namespace SalesDashboard.Api.Entities;

public enum SaleStatus { Paid = 0, Cancelled = 1, Refunded = 2 }

public class Sale
{
    public int Id { get; set; }
    public int ManagerId { get; set; }
    public Manager Manager { get; set; } = null!;
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public DateTime Date { get; set; }
    public SaleStatus Status { get; set; }
    public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();
}