using Frosh.Core.Common;
using Frosh.Core.Entities;
using Frosh.Core.Interface;


namespace Frosh.Core.Service;

public class FroshService
{
    private readonly IMemberRepository _members;
    private readonly IProductRepository _products;
    private readonly IProjectRepository _projects;
    private readonly IQuoteRepository _quotes;


    public FroshService(IMemberRepository members, IProductRepository products, IProjectRepository projects, IQuoteRepository quote)
    {
        _members = members;
        _products = products;
        _projects = projects;
        _quotes = quote;
    }
    
        // ---------- Members ----------

    public IEnumerable<Member> GetAllMembers()
    {
        return _members.GetAll();
    }

    public Member? GetMemberById(Guid id)
    {
        return _members.GetById(id);
    }

    public void AddMember(Member member)
    {
        ArgumentNullException.ThrowIfNull(member);

        if (string.IsNullOrWhiteSpace(member.Name))
        {
            throw new ArgumentException("نام عضو الزامی است.");
        }

        _members.Add(member);
    }

    public bool UpdateMember(Member member)
    {
        ArgumentNullException.ThrowIfNull(member);

        return _members.Update(member);
    }

    public bool DeleteMember(Guid id)
    {
        return _members.Delete(id);
    }


    // ---------- Products ----------

    public IEnumerable<Product> GetAllProducts()
    {
        return _products.GetAll();
    }

    public Product? GetProductById(Guid id)
    {
        return _products.GetById(id);
    }
    public void AddProduct(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);

        if (string.IsNullOrWhiteSpace(product.Name))
        {
            throw new ArgumentException("نام عضو الزامی است.");
        }

        _products.Add(product);
    }

    // ---------- Projects ----------

    public IEnumerable<Project> GetAllProjects()
    {
        return _projects.GetAll();
    }

    public Project? GetProjectById(Guid id)
    {
        return _projects.GetById(id);
    }


    // ---------- Quotes ----------

    public IEnumerable<Quote> GetAllQuotes()
    {
        return _quotes.GetAll();
    }

    public Quote? GetQuoteById(Guid id)
    {
        return _quotes.GetById(id);
    }
}
