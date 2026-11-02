using Frosh.Core.Entities;
using Frosh.Core.Common;

namespace Frosh.Core.Interface;

public interface IProductRepository : IRepository<Product, Guid>
{
    void AddOrUpdateFromExcel(Product product);
}