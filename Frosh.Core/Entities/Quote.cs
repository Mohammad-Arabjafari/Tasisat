using Frosh.Core.Entities;
using Frosh.Core.Common;
namespace Frosh.Core.Entities;


public class Quote : Entity<Guid>
{
    public Quote() : base(Guid.NewGuid())
    {
    }

    public Guid ProjectId { get; set; }

    public PipePurpose PipePurpose { get; set; }
    public PipeLayer PipeLayer { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Project? Project { get; set; }
    public List<QuoteItem> Items { get; set; } = new();
}

