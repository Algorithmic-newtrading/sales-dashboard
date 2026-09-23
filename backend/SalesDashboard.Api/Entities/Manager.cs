namespace SalesDashboard.Api.Entities;

public class Manager
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string Team { get; set; } = "";
    public string Position { get; set; } = "";
    public bool IsActive { get; set; } = true;
    public string AvatarColor { get; set; } = "#6366f1";

    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}