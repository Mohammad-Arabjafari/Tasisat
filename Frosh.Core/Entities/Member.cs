using Frosh.Core.Common;
namespace Frosh.Core.Entities;

public class Member : Entity<Guid>
{
    public Member() : base(Guid.NewGuid())
    {
    }
    public string Name { get; set; }
    public int phoneNumber { get; set; }
    public string MemberType { get; set; }
    public string City { get; set; }
}
public enum MemberType
{
    Executor,   // مجری
    Contractor, // پیمانکار
    Employer    // صاحب‌کار
}