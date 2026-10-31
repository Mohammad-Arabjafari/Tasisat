using Frosh.Core.Entities;
using Frosh.Core.Interface;
using Frosh.Data.SqlServer.Context;
using Microsoft.EntityFrameworkCore;

namespace Frosh.Data.SqlServer.Repository;

public class QuoteItemRepository : IQuoteItemRepository
{
    private readonly AppDbContext _context;

    public QuoteItemRepository(AppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<QuoteItem> GetAll()
    {
        return _context.QuoteItems
            .AsNoTracking()
            .ToList();
    }

    public QuoteItem? GetById(Guid id)
    {
        return _context.QuoteItems
            .AsNoTracking()
            .FirstOrDefault(q => q.Id == id);
    }

    public void Add(QuoteItem entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _context.QuoteItems.Add(entity);
        _context.SaveChanges();
    }

    public bool Update(QuoteItem entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var existingItem = _context.QuoteItems.Find(entity.Id);

        if (existingItem is null)
            return false;

        _context.Entry(existingItem).CurrentValues.SetValues(entity);

        _context.SaveChanges();

        return true;
    }

    public bool Delete(Guid id)
    {
        var item = _context.QuoteItems.Find(id);

        if (item is null)
            return false;

        _context.QuoteItems.Remove(item);
        _context.SaveChanges();

        return true;
    }
}