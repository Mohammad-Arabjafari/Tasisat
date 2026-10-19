
using Frosh.Core.Entities;
using Frosh.Core.Interface;

namespace Frosh.Data.InMemori.Repository;

public class QuoteItemRepository
    : BaseRepository<QuoteItem, Guid>, IQuoteItemRepository
{
}

