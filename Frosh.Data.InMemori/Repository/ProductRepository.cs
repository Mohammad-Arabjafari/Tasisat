
using Frosh.Core.Entities;
using Frosh.Core.Interface;
using System.Linq;
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
    public void AddOrUpdateFromExcel(Product product)
{
    ArgumentNullException.ThrowIfNull(product);

    if (string.IsNullOrWhiteSpace(product.ProductCode))
        throw new ArgumentException("کد محصول معتبر نیست.");

    var existingProduct = GetAll()
        .FirstOrDefault(p =>
            string.Equals(
                p.ProductCode?.Trim(),
                product.ProductCode.Trim(),
                StringComparison.OrdinalIgnoreCase));

    if (existingProduct is null)
    {
        Add(product);
    }
    else
    {
        existingProduct.UnitPrice = product.UnitPrice;
        existingProduct.Specification = product.Specification;

        Update(existingProduct);
    }
}
}

