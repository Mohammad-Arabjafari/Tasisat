using Frosh.Core.Entities;
using Frosh.Core.Interface;
using Frosh.Data.SqlServer.Context;
using Microsoft.EntityFrameworkCore;

namespace Frosh.Data.SqlServer.Repository;

public class MemberRepository : IMemberRepository
{
    private readonly AppDbContext _context;

    public MemberRepository(AppDbContext context)
    {
        _context = context;
    }

    // دریافت تمام اعضا
    public IEnumerable<Member> GetAll()
    {
        return _context.Members
            .AsNoTracking()
            .ToList();
    }

    // دریافت عضو با شناسه
    public Member? GetById(Guid id)
    {
        return _context.Members
            .AsNoTracking()
            .FirstOrDefault(m => m.Id == id);
    }

    // افزودن عضو
    public void Add(Member entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _context.Members.Add(entity);

        _context.SaveChanges();
    }

    // ویرایش عضو
    public bool Update(Member entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var existingMember = _context.Members
            .Find(entity.Id);

        if (existingMember is null)
            return false;

        _context.Entry(existingMember)
            .CurrentValues.SetValues(entity);

        _context.SaveChanges();

        return true;
    }

    // حذف عضو
    public bool Delete(Guid id)
    {
        var member = _context.Members.Find(id);

        if (member is null)
            return false;

        _context.Members.Remove(member);

        _context.SaveChanges();

        return true;
    }
}