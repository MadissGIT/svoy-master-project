namespace SvoyMaster;

public class Master
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Service { get; set; } = "";
    public string About { get; set; } = "";
    public int Price { get; set; }
    public bool IsAvailable { get; set; } = true;
    public bool IsVerified { get; set; }
    public string Summary => $"{Name} — {Service}, {Price} ₽";
}
