using Frosh.Core.Common;

namespace Frosh.Core.Entities;

public class Project : Entity<Guid>
{
    public Project() : base(Guid.NewGuid())
    {
    }

    public string Name { get; set; }

    public Guid MemberId { get; set; }

    public string Address { get; set; }

    // ارتباط پروژه با مشتری
    public Member Member { get; set; }
}