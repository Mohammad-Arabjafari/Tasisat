using Frosh.Core.Common;
namespace Frosh.Core.Entities;

public class Product : Entity<Guid>
{
    public Product() : base(Guid.NewGuid())
    {
    }
    public string Name { get; set; }
    public string PipePurpose { get; set; }
    public string PipeLayer { get; set; }
    public decimal UnitPrice { get; set; }
    public bool IsActive { get; set; } = true;
    public string Unit { get; set; }
    public PipePurpose Purpose { get; set; }
    public PipeLayer Layer { get; set; }

}
public enum PipePurpose
{
    Water,  // آب
    Sewage  // فاضلاب
}

public enum PipeLayer
{
    SingleLayer, // تک‌لایه
    FiveLayer    // پنج‌لایه
}