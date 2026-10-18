
using Frosh.Core.Common;
using Frosh.Core.Entities;
using Frosh.Core.Interface;
namespace Frosh.Data.InMemori.Repository;

public class MemberRepository : IMemberRepository
{
    void IRepository<Member, Guid>.Add(Member entity)
    {
        throw new NotImplementedException();
    }

    bool IRepository<Member, Guid>.Delete(Guid id)
    {
        throw new NotImplementedException();
    }

    IEnumerable<Member> IRepository<Member, Guid>.GetAll()
    {
        throw new NotImplementedException();
    }

    Member? IRepository<Member, Guid>.GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    bool IRepository<Member, Guid>.Update(Member entity)
    {
        throw new NotImplementedException();
    }

}
