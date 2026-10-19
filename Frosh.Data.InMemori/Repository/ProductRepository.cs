
using Frosh.Core.Entities;
using Frosh.Core.Interface;

namespace Frosh.Data.InMemori.Repository;

public class ProductRepository : IProductRepository
{
    private readonly List<Product> _products = new();

    public void Add(Product entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _products.Add(entity);
    }

    public bool Delete(Guid id)
    {
        var product = GetById(id);

        if (product is null)
            return false;

        return _products.Remove(product);
    }

    public IEnumerable<Product> GetAll()
    {
        return _products;
    }

    public Product? GetById(Guid id)
    {
        return _products.FirstOrDefault(product => product.Id == id);
    }

    public bool Update(Product entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var index = _products.FindIndex(product => product.Id == entity.Id);

        if (index == -1)
            return false;

        _products[index] = entity;

        return true;
    }
}

