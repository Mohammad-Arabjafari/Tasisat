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


public FroshService(
    IMemberRepository members,
    IProductRepository products,
    IProjectRepository projects,
    IQuoteRepository quotes )
{
    _members = members;
    _products = products;
    _projects = projects;
    _quotes = quotes;

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
        throw new ArgumentException("نام عضو الزامی است.");

    if (string.IsNullOrWhiteSpace(member.City))
        throw new ArgumentException("شهر عضو الزامی است.");

    if (string.IsNullOrWhiteSpace(member.MemberType))
        throw new ArgumentException("نوع عضو را انتخاب کنید.");

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
            throw new ArgumentException("نام محصول الزامی است.");

        if (product.UnitPrice <= 0)
            throw new ArgumentException("قیمت محصول باید بیشتر از صفر باشد.");

        if (string.IsNullOrWhiteSpace(product.Unit))
            throw new ArgumentException("واحد محصول الزامی است.");

        _products.Add(product);
    }
    // ---------- Excel Import ----------

    public void ImportProductsFromExcel(IEnumerable<Product> products)
    {
    ArgumentNullException.ThrowIfNull(products);

    foreach (var product in products)
    {
        if (string.IsNullOrWhiteSpace(product.ProductCode))
            throw new ArgumentException("کد محصول الزامی است.");

        if (string.IsNullOrWhiteSpace(product.Name))
            throw new ArgumentException( $"نام محصول با کد {product.ProductCode} خالی است.");

        if (product.UnitPrice <= 0)
            throw new ArgumentException( $"قیمت محصول با کد {product.ProductCode} معتبر نیست.");

        _products.AddOrUpdateFromExcel(product);
    }
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

    public void AddProject(Project project)
    {
    ArgumentNullException.ThrowIfNull(project);

    if (string.IsNullOrWhiteSpace(project.Name))
        throw new ArgumentException("نام پروژه الزامی است.");

    if (string.IsNullOrWhiteSpace(project.Address))
        throw new ArgumentException("آدرس پروژه الزامی است.");

    if (_members.GetById(project.MemberId) is null)
        throw new ArgumentException("عضو انتخاب‌شده در سیستم وجود ندارد.");

    _projects.Add(project);
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

public Quote CreateQuote(
    Guid projectId,
    PipePurpose pipePurpose,
    PipeLayer pipeLayer,
    IEnumerable<(Guid ProductId, decimal Quantity)> items)
{
    ArgumentNullException.ThrowIfNull(items);

    // بررسی وجود پروژه
    var project = _projects.GetById(projectId);

    if (project is null)
        throw new ArgumentException("پروژه موردنظر پیدا نشد.");

    // تبدیل اقلام به لیست
    var requestedItems = items.ToList();

    if (requestedItems.Count == 0)
        throw new ArgumentException("پیش‌فاکتور باید حداقل یک محصول داشته باشد.");

    // ساخت پیش‌فاکتور
    var quote = new Quote
    {
        ProjectId = projectId,
        PipePurpose = pipePurpose,
        PipeLayer = pipeLayer,
        CreatedAt = DateTime.UtcNow
    };

    // افزودن اقلام پیش‌فاکتور
    foreach (var requestedItem in requestedItems)
    {
        if (requestedItem.Quantity <= 0)
            throw new ArgumentException(  "تعداد یا مقدار محصول باید بیشتر از صفر باشد.");

        // دریافت محصول برای بررسی وجود و ثبت اطلاعات آن
        var product = _products.GetById(requestedItem.ProductId);

        if (product is null)
            throw new ArgumentException(  $"محصول با شناسه {requestedItem.ProductId} پیدا نشد.");

        var quoteItem = new QuoteItem
        {
            QuoteId = quote.Id,
            ProductId = product.Id,
            ProductName = product.Name,
            Quantity = requestedItem.Quantity,
            UnitPrice = product.UnitPrice,
           ProductSpecification = product.Specification ?? string.Empty,
        };
        quote.Items.Add(quoteItem);
    }

    // ذخیره پیش‌فاکتور و اقلام آن
    _quotes.Add(quote);

    return quote;
}
}
