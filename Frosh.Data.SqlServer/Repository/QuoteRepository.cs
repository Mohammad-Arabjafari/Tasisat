using Frosh.Core.Entities;
using Frosh.Core.Interface;
using Frosh.Data.SqlServer.Context;
using Microsoft.EntityFrameworkCore;

namespace Frosh.Data.SqlServer.Repository;

public class QuoteRepository : IQuoteRepository
{
    private readonly AppDbContext _context;

    public QuoteRepository(AppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Quote> GetAll()
    {
        return _context.Quotes
            .AsNoTracking()
            .Include(q => q.Items)
            .ToList();
    }

    public Quote? GetById(Guid id)
    {
        return _context.Quotes
            .AsNoTracking()
            .Include(q => q.Items)
            .FirstOrDefault(q => q.Id == id);
    }

    public void Add(Quote entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _context.Quotes.Add(entity);
        _context.SaveChanges();
    }

    public bool Update(Quote entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var existingQuote = _context.Quotes
            .Include(q => q.Items)
            .FirstOrDefault(q => q.Id == entity.Id);

        if (existingQuote is null)
            return false;

        _context.Entry(existingQuote).CurrentValues.SetValues(entity);

        _context.SaveChanges();

        return true;
    }

    public bool Delete(Guid id)
    {
        var quote = _context.Quotes.Find(id);

        if (quote is null)
            return false;

        _context.Quotes.Remove(quote);
        _context.SaveChanges();

        return true;
    }
}