using Frosh.Core.Entities;
using Frosh.Core.Common;
namespace Frosh.Core.Entities;

public class QuoteItem : Entity<Guid>
{
    public QuoteItem() : base(Guid.NewGuid())
    {
    }

    public Guid QuoteId { get; set; }
    public Guid ProductId { get; set; }

    public decimal Quantity { get; set; }

    // نام کالا در زمان ثبت پیش‌فاکتور
    public string ProductName { get; set; } = string.Empty;

    public Quote? Quote { get; set; }
    public Product? Product { get; set; }
}
