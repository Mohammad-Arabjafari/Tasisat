using Frosh.Core.Common;

namespace Frosh.Core.Entities;

public class Product : Entity<Guid>
{
    public Product() : base(Guid.NewGuid())
    {
    }

    // اطلاعات اصلی محصول
    public string? ProductCode { get; set; }
    public string Name { get; set; } = string.Empty;

    // مشخصات فنی
    public string? Specification { get; set; }

    // اطلاعات قیمت و فروش
    public decimal UnitPrice { get; set; }
    public string Unit { get; set; } = string.Empty;

    // اطلاعات بسته‌بندی
    public decimal? QuantityPerBox { get; set; }
    public decimal? QuantityPerCarton { get; set; }
    public string? PackagingNote { get; set; }

    // وضعیت محصول
    public bool IsActive { get; set; } = true;

    // دسته‌بندی‌های قبلی
    public PipePurpose? Purpose { get; set; }
    public PipeLayer? Layer { get; set; }
}

public enum PipePurpose
{
    Water,
    Sewage
}

public enum PipeLayer
{
    SingleLayer,
    FiveLayer
}