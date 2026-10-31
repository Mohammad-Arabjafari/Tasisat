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

    // دریافت تمام کالاها
    public IEnumerable<Product> GetAll()
    {
        return _context.Products
            .AsNoTracking()
            .ToList();
    }

    // دریافت کالا با شناسه
    public Product? GetById(Guid id)
    {
        return _context.Products
            .AsNoTracking()
            .FirstOrDefault(p => p.Id == id);
    }

    // افزودن کالا
    public void Add(Product entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _context.Products.Add(entity);
        _context.SaveChanges();
    }

    // ویرایش کالا
    public bool Update(Product entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var existingProduct = _context.Products.Find(entity.Id);

        if (existingProduct is null)
            return false;

        _context.Entry(existingProduct)
            .CurrentValues.SetValues(entity);

        _context.SaveChanges();

        return true;
    }

    // حذف کالا
    public bool Delete(Guid id)
    {
        var product = _context.Products.Find(id);

        if (product is null)
            return false;

        _context.Products.Remove(product);
        _context.SaveChanges();

        return true;
    }
}
