
using Frosh.Core.Common;
using Frosh.Core.Entities;
using Frosh.Core.Interface;
namespace Frosh.Data.InMemori.Repository;

public class QuoteRepository : IQuoteRepository
{
    void IRepository<Quote, Guid>.Add(Quote entity)
    {
        throw new NotImplementedException();
    }

    bool IRepository<Quote, Guid>.Delete(Guid id)
    {
        throw new NotImplementedException();
    }

    IEnumerable<Quote> IRepository<Quote, Guid>.GetAll()
    {
        throw new NotImplementedException();
    }

    Quote? IRepository<Quote, Guid>.GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    bool IRepository<Quote, Guid>.Update(Quote entity)
    {
        throw new NotImplementedException();
    }

}

