using Frosh.Core.Common;
using System.Diagnostics.Contracts;

namespace Frosh.Core.Entities;

public class Project : Entity<Guid>
{ 
   
    public Project() : base(Guid.NewGuid())
    {
    }
     public string Name { get; set; }
    public Guid MemberId { get; set; }
    public  string Address { get; set; }
}
