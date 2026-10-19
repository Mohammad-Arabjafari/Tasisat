
using Frosh.Core.Entities;
using Frosh.Core.Interface;

namespace Frosh.Data.InMemori.Repository;

public class QuoteRepository
    : BaseRepository<Quote, Guid>, IQuoteRepository
{
}

