using Frosh.Core.Entities;
using Frosh.Core.Interface;
using Frosh.Data.SqlServer.Context;
using Microsoft.EntityFrameworkCore;

namespace Frosh.Data.SqlServer.Repository;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Product> GetAll()
    {
        return _context.Products.AsNoTracking().ToList();
    }

    public Product? GetById(Guid id)
    {
        return _context.Products
            .AsNoTracking()
            .FirstOrDefault(p => p.Id == id);
    }

    public void Add(Product entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _context.Products.Add(entity);
        _context.SaveChanges();
    }

    public bool Update(Product entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var existingProduct = _context.Products.Find(entity.Id);

        if (existingProduct is null)
            return false;

        _context.Entry(existingProduct).CurrentValues.SetValues(entity);

        _context.SaveChanges();
        return true;
    }

    public bool Delete(Guid id)
    {
        var product = _context.Products.Find(id);

        if (product is null)
            return false;

        _context.Products.Remove(product);
        _context.SaveChanges();

        return true;
    }

    // ورود محصول از اکسل
    public void AddOrUpdateFromExcel(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);

        if (string.IsNullOrWhiteSpace(product.ProductCode))
            throw new ArgumentException("کد محصول معتبر نیست.");

        string code = product.ProductCode.Trim();

        var existingProduct = _context.Products
            .FirstOrDefault(p => p.ProductCode == code);

        if (existingProduct is null)
        {
            product.ProductCode = code;
            _context.Products.Add(product);
        }
        else
        {
            existingProduct.UnitPrice = product.UnitPrice;
            existingProduct.Specification = product.Specification;
        }

        _context.SaveChanges();
    }
}