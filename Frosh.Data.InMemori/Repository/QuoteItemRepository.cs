
using Frosh.Core.Common;
using Frosh.Core.Entities;
using Frosh.Core.Interface;
namespace Frosh.Data.InMemori.Repository;

public class QuoteItemRepository : IQuoteItemRepository
{
    void IRepository<QuoteItem, Guid>.Add(QuoteItem entity)
    {
        throw new NotImplementedException();
    }

    bool IRepository<QuoteItem, Guid>.Delete(Guid id)
    {
        throw new NotImplementedException();
    }

    IEnumerable<QuoteItem> IRepository<QuoteItem, Guid>.GetAll()
    {
        throw new NotImplementedException();
    }

    QuoteItem? IRepository<QuoteItem, Guid>.GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    bool IRepository<QuoteItem, Guid>.Update(QuoteItem entity)
    {
        throw new NotImplementedException();
    }

}

