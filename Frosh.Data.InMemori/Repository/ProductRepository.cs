
using Frosh.Core.Common;
using Frosh.Core.Entities;
using Frosh.Core.Interface;
namespace Frosh.Data.InMemori.Repository;

public class ProductRepository : IProductRepository
{
    void IRepository<Product, Guid>.Add(Product entity)
    {
        throw new NotImplementedException();
    }

    bool IRepository<Product, Guid>.Delete(Guid id)
    {
        throw new NotImplementedException();
    }

    IEnumerable<Product> IRepository<Product, Guid>.GetAll()
    {
        throw new NotImplementedException();
    }

    Product? IRepository<Product, Guid>.GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    bool IRepository<Product, Guid>.Update(Product entity)
    {
        throw new NotImplementedException();
    }

}

