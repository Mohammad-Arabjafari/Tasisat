using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Spectre.Console;
using Frosh.Core.Entities;

using Frosh.Core.Interface;
using Frosh.Core.Service;

using Frosh.Data.SqlServer.Context;
using Frosh.Data.SqlServer.Repository;
using Frosh.Data.InMemori.Import;

using Microsoft.EntityFrameworkCore;


// ==========================================
// 1. اتصال به SQL Server
// ==========================================

string connectionString =
    "Server=localhost;Database=FroshDb;Trusted_Connection=True;TrustServerCertificate=True;";

var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlServer(connectionString)
    .Options;

using var dbContext = new AppDbContext(options);


// ==========================================
// 2. ساخت Repositoryهای SQL
// ==========================================

IMemberRepository memberRepository = new MemberRepository(dbContext);

IProductRepository productRepository = new ProductRepository(dbContext);

IProjectRepository projectRepository = new ProjectRepository(dbContext);

IQuoteRepository quoteRepository = new QuoteRepository(dbContext);


// ==========================================
// 3. ساخت Service
// ==========================================

var froshService = new FroshService(
    memberRepository,
    productRepository,
    projectRepository,
    quoteRepository
);


// ==========================================
// 4. اجرای برنامه
// ==========================================

Console.OutputEncoding = System.Text.Encoding.UTF8;

ShowWelcome();

bool running = true;

while (running)
{
    var choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[bold cyan]منوی اصلی فروش تأسیسات[/]")
            .PageSize(10)
            .HighlightStyle(new Style(Color.Yellow))
            .AddChoices(
                "📦 نمایش محصولات",
                "➕ افزودن محصول",
                "👥 نمایش اعضا",
                "➕ افزودن عضو",
                "🧾 ثبت پیش‌فاکتور",
                "🏗️ ثبت پروژه جدید",
                "🏗️ نمایش پروژه‌ها",
                "📄 نمایش پیش‌فاکتورها",
                "📥 ورود محصولات از اکسل",
                "🚪 خروج"
            )
    );

    switch (choice)
    {
        case "📦 نمایش محصولات":
            ShowProducts();
            break;

        case "➕ افزودن محصول":
            AddProduct();
            break;

        case "👥 نمایش اعضا":
            ShowMembers();
            break;

        case "➕ افزودن عضو":
            AddMember();
            break;

        case "🧾 ثبت پیش‌فاکتور":
            CreateQuoteMenu(froshService);
            break;

        case "🏗️ ثبت پروژه جدید":
            AddProject();
            break;

        case "🏗️ نمایش پروژه‌ها":
            ShowProjects();
            break;

        case "📄 نمایش پیش‌فاکتورها":
            ShowQuotes(froshService);
            break;

        case "📥 ورود محصولات از اکسل":
            ImportProductsFromExcel(froshService);
            break;

        case "🚪 خروج":
            running = false;
            break;
    }
}

AnsiConsole.MarkupLine(
    "\n[bold green]برنامه با موفقیت بسته شد.[/]"
);


// ==========================================
// نمایش عنوان برنامه
// ==========================================

void ShowWelcome()
{
    var rule = new Rule("[bold cyan]FROSH TASISAT[/]")
    {
        Justification = Justify.Center,
        Style = Style.Parse("cyan")
    };

    AnsiConsole.Write(rule);

    AnsiConsole.MarkupLine(
        "[bold white]سیستم مدیریت فروش تأسیسات ساختمانی[/]"
    );

    AnsiConsole.MarkupLine(
        "[grey]مدیریت محصولات، مشتریان، پروژه‌ها و پیش‌فاکتورها[/]\n"
    );
}


// ==========================================
// نمایش محصولات
// ==========================================

void ShowProducts()
{
    var products = froshService.GetAllProducts().ToList();

    if (products.Count == 0)
    {
        AnsiConsole.MarkupLine(
            "[yellow]هنوز محصولی در سیستم ثبت نشده است.[/]"
        );

        Pause();
        return;
    }

    var table = new Table();

    table.Border(TableBorder.Rounded);
    table.Title("[bold cyan]فهرست محصولات[/]");

    table.AddColumn("ردیف");
    table.AddColumn("نام محصول");
    table.AddColumn("کاربرد");
    table.AddColumn("نوع لوله");
    table.AddColumn("واحد");
    table.AddColumn("قیمت واحد");
    table.AddColumn("وضعیت");

    int row = 1;

    foreach (var product in products)
    {
        table.AddRow(
            row.ToString(),
            Markup.Escape(product.Name ?? ""),
            product.Purpose?.ToString() ?? "-",
            product.Layer?.ToString() ?? "-",
            Markup.Escape(product.Unit ?? ""),
            product.UnitPrice.ToString("N0"),
            product.IsActive ? "[green]فعال[/]" : "[red]غیرفعال[/]"
        );

        row++;
    }

    AnsiConsole.Write(table);

    AnsiConsole.MarkupLine(
        $"\n[bold green]تعداد محصولات: {products.Count}[/]"
    );

    Pause();
}


// ==========================================
// نمایش اعضا
// ==========================================

void ShowMembers()
{
    var members = froshService.GetAllMembers().ToList();

    if (members.Count == 0)
    {
        AnsiConsole.MarkupLine(
            "[yellow]هنوز عضوی ثبت نشده است.[/]"
        );

        Pause();
        return;
    }

    var table = new Table();

    table.Border(TableBorder.Rounded);
    table.Title("[bold cyan]فهرست اعضا[/]");

    table.AddColumn("ردیف");
    table.AddColumn("نام");
    table.AddColumn("شماره تماس");
    table.AddColumn("نوع عضو");
    table.AddColumn("شهر");

    int row = 1;

    foreach (var member in members)
    {
        table.AddRow(
            row.ToString(),
            Markup.Escape(member.Name ?? ""),
            Markup.Escape(member.phoneNumber ?? ""),
            Markup.Escape(member.MemberType ?? ""),
            Markup.Escape(member.City ?? "")
        );

        row++;
    }

    AnsiConsole.Write(table);

    Pause();
}


// ==========================================
// نمایش پروژه‌ها
// ==========================================

void ShowProjects()
{
    var projects = froshService.GetAllProjects().ToList();

    if (projects.Count == 0)
    {
        AnsiConsole.MarkupLine(
            "[yellow]هنوز پروژه‌ای ثبت نشده است.[/]"
        );

        Pause();
        return;
    }

    var table = new Table();

    table.Border(TableBorder.Rounded);
    table.Title("[bold cyan]فهرست پروژه‌ها[/]");

    table.AddColumn("ردیف");
    table.AddColumn("نام پروژه");
    table.AddColumn("شناسه عضو");
    table.AddColumn("آدرس");

    int row = 1;

    foreach (var project in projects)
    {
        table.AddRow(
            row.ToString(),
            Markup.Escape(project.Name ?? ""),
            project.MemberId.ToString(),
            Markup.Escape(project.Address ?? "")
        );

        row++;
    }

    AnsiConsole.Write(table);

    Pause();
}


// ==========================================
// نمایش پیش‌فاکتورها
// ==========================================

 void ShowQuotes(FroshService froshService)
{
    var quotes = froshService.GetAllQuotes().ToList();

    if (quotes.Count == 0)
    {
        AnsiConsole.MarkupLine("[yellow]هیچ پیش‌فاکتوری ثبت نشده است.[/]");
        Pause();
        return;
    }

    var table = new Table();

    table.Border(TableBorder.Rounded);

    table.AddColumn("شماره");
    table.AddColumn("شناسه پیش‌فاکتور");
    table.AddColumn("نام مشتری");
    table.AddColumn("نام پروژه");
    table.AddColumn("تاریخ ثبت");
    table.AddColumn("مبلغ کل");

    for (int i = 0; i < quotes.Count; i++)
    {
        var quote = quotes[i];

        table.AddRow(
            (i + 1).ToString(),
            quote.Id.ToString(),
            quote.Project?.Member?.Name ?? "نامشخص",
            quote.Project?.Name ?? "نامشخص",
            quote.CreatedAt.ToLocalTime().ToString("yyyy/MM/dd"),
            $"{quote.TotalPrice:N0} ریال"
        );
    }

    AnsiConsole.Write(table);

    // انتخاب پیش‌فاکتور برای مشاهده جزئیات
    var choices = quotes
        .Select((quote, index) =>
            $"{index + 1} - {quote.Project?.Name ?? "بدون نام"} - {quote.TotalPrice:N0} ریال")
        .ToList();

    choices.Add("بازگشت");

    var selectedQuote = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("پیش‌فاکتوری را برای مشاهده جزئیات انتخاب کنید:")
            .PageSize(15)
            .AddChoices(choices)
    );

    if (selectedQuote == "بازگشت")
        return;

    int selectedIndex = choices.IndexOf(selectedQuote);

    if (selectedIndex >= 0 && selectedIndex < quotes.Count)
    {
        ShowQuoteDetails(quotes[selectedIndex]);
    }
}


// ==========================================
// افزودن محصول
// ==========================================

void AddProduct()
{
    AnsiConsole.MarkupLine(
        "\n[bold cyan]ثبت محصول جدید[/]\n"
    );

    string name = AnsiConsole.Ask<string>(
        "نام محصول:"
    );

    decimal price = AnsiConsole.Ask<decimal>(
        "قیمت واحد:"
    );

    string unit = AnsiConsole.Ask<string>(
        "واحد محصول:"
    );

    var purpose = AnsiConsole.Prompt(
        new SelectionPrompt<PipePurpose>()
            .Title("کاربرد محصول:")
            .AddChoices(Enum.GetValues<PipePurpose>())
    );

    var layer = AnsiConsole.Prompt(
        new SelectionPrompt<PipeLayer>()
            .Title("نوع لوله:")
            .AddChoices(Enum.GetValues<PipeLayer>())
    );

    var product = new Product
    {
        Name = name,
        UnitPrice = price,
        Unit = unit,
        Purpose = purpose,
        Layer = layer,
        IsActive = true
    };

    try
    {
        froshService.AddProduct(product);

        AnsiConsole.MarkupLine(
            "\n[bold green]محصول با موفقیت ثبت شد.[/]"
        );
    }
    catch (Exception ex)
    {
        AnsiConsole.MarkupLine(
            $"[red]خطا: {Markup.Escape(ex.Message)}[/]"
        );
    }

    Pause();
}


// ==========================================
// افزودن عضو
// ==========================================

void AddMember()
{
    AnsiConsole.MarkupLine(
        "\n[bold cyan]ثبت عضو جدید[/]\n"
    );

    string name = AnsiConsole.Ask<string>(
        "نام و نام خانوادگی:"
    );

    string phone = AnsiConsole.Ask<string>(
        "شماره تماس:"
    );

    string city = AnsiConsole.Ask<string>(
        "شهر:"
    );

    var memberType = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("نوع عضو را انتخاب کنید:")
            .AddChoices(
                "مجری",
                "پیمانکار",
                "کارفرما"
            )
    );

    if (phone.Length != 11 ||
        !phone.StartsWith("09") ||
        !phone.All(char.IsDigit))
    {
        AnsiConsole.MarkupLine(
            "[red]شماره تماس معتبر نیست.[/]"
        );

        Pause();
        return;
    }

    var member = new Member
    {
        Name = name,
        phoneNumber = phone,
        City = city,
        MemberType = memberType
    };

    try
    {
        froshService.AddMember(member);

        AnsiConsole.MarkupLine(
            "\n[bold green]عضو با موفقیت ثبت شد.[/]"
        );
    }
    catch (Exception ex)
    {
        AnsiConsole.MarkupLine(
            $"[red]خطا: {Markup.Escape(ex.Message)}[/]"
        );
    }

    Pause();
}


// ==========================================
// ثبت پروژه جدید
// ==========================================

void AddProject()
{
    var members = froshService.GetAllMembers().ToList();

    if (members.Count == 0)
    {
        AnsiConsole.MarkupLine(
            "[yellow]ابتدا باید یک عضو ثبت کنید.[/]"
        );

        Pause();
        return;
    }

    AnsiConsole.MarkupLine(
        "\n[bold cyan]ثبت پروژه جدید[/]\n"
    );

    var selectedMember = AnsiConsole.Prompt(
        new SelectionPrompt<Member>()
            .Title("انتخاب صاحب پروژه:")
            .UseConverter(m => $"{m.Name} - {m.phoneNumber}")
            .AddChoices(members)
    );

    string projectName = AnsiConsole.Ask<string>(
        "نام پروژه:"
    );

    string address = AnsiConsole.Ask<string>(
        "آدرس پروژه:"
    );

    var project = new Project
    {
        Name = projectName,
        Address = address,
        MemberId = selectedMember.Id
    };

    try
    {
        froshService.AddProject(project);

        AnsiConsole.MarkupLine(
            "\n[bold green]پروژه با موفقیت ثبت شد.[/]"
        );
    }
    catch (Exception ex)
    {
        AnsiConsole.MarkupLine(
            $"[red]خطا: {Markup.Escape(ex.Message)}[/]"
        );
    }

    Pause();
}


// ==========================================
// ثبت پیش‌فاکتور
// ==========================================

 void CreateQuoteMenu(FroshService froshService)
{
    // 1. انتخاب پروژه
    var projects = froshService.GetAllProjects().ToList();

    if (!projects.Any())
    {
        AnsiConsole.MarkupLine("[red]هیچ پروژه‌ای ثبت نشده است.[/]");
        Pause();
        return;
    }

    var selectedProject = AnsiConsole.Prompt(
        new SelectionPrompt<Project>()
            .Title("انتخاب پروژه:")
            .PageSize(10)
            .UseConverter(p => p.Name)
            .AddChoices(projects)
    );

    // 2. انتخاب نوع لوله
    var purpose = AnsiConsole.Prompt(
        new SelectionPrompt<PipePurpose>()
            .Title("انتخاب نوع لوله:")
            .AddChoices(PipePurpose.Water, PipePurpose.Sewage)
    );

    // 3. انتخاب لایه (فقط برای آب)
    PipeLayer layer = PipeLayer.SingleLayer;

    if (purpose == PipePurpose.Water)
    {
        layer = AnsiConsole.Prompt(
            new SelectionPrompt<PipeLayer>()
                .Title("انتخاب نوع لایه:")
                .AddChoices(PipeLayer.SingleLayer, PipeLayer.FiveLayer)
        );
    }

    // 4. دریافت محصولات فعال
    var products = froshService.GetAllProducts()
        .Where(p => p.IsActive)
        .ToList();

    if (!products.Any())
    {
        AnsiConsole.MarkupLine("[red]هیچ محصول فعالی ثبت نشده است.[/]");
        Pause();
        return;
    }

    // 5. لیست اقلام پیش‌فاکتور
    var selectedItems = new List<(Guid ProductId, decimal Quantity)>();

    while (true)
    {
        // گروه‌بندی محصولات بر اساس نام
        var productNames = products
            .Select(p => p.Name)
            .Distinct()
            .OrderBy(name => name)
            .ToList();

        const string finishOption = "🟢 پایان انتخاب و ثبت پیش‌فاکتور";

        // گزینه پایان همیشه آخرین گزینه است
        productNames.Add(finishOption);

        // ساخت منوی انتخاب گروه محصول
        var prompt = new SelectionPrompt<string>();
        prompt.Title("انتخاب گروه محصول:");
        prompt.PageSize(15);
        prompt.HighlightStyle(new Style(Color.Green));
        prompt.AddChoices(productNames);

        string selectedName = AnsiConsole.Prompt(prompt);

        // بررسی انتخاب گزینه پایان
        if (selectedName == finishOption)
        {
            break;
        }

        // 6. نمایش سایزهای گروه انتخاب‌شده
        var productSizes = products
            .Where(p => p.Name == selectedName)
            .ToList();

        if (!productSizes.Any())
        {
            continue;
        }

        var selectedProduct = AnsiConsole.Prompt(
            new SelectionPrompt<Product>()
                .Title($"انتخاب سایز {selectedName}:")
                .PageSize(15)
                .UseConverter(p =>
                    $"{p.Specification ?? "بدون مشخصات"} | {p.UnitPrice:N0} ریال")
                .AddChoices(productSizes)
        );

        // 7. دریافت تعداد
        decimal quantity = AnsiConsole.Prompt(
            new TextPrompt<decimal>("تعداد موردنظر را وارد کنید:")
                .Validate(q =>
                    q > 0
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]تعداد باید بیشتر از صفر باشد.[/]"))
        );

        // 8. اضافه کردن محصول به پیش‌فاکتور
        selectedItems.Add((selectedProduct.Id, quantity));

        AnsiConsole.MarkupLine(
            $"[green]محصول {selectedProduct.Name} با موفقیت اضافه شد.[/]");

        AnsiConsole.WriteLine();
    }

    // 9. بررسی اینکه حداقل یک محصول انتخاب شده باشد
    if (!selectedItems.Any())
    {
        AnsiConsole.MarkupLine("[yellow]هیچ محصولی انتخاب نشد.[/]");
        Pause();
        return;
    }

    // 10. ثبت پیش‌فاکتور
    froshService.CreateQuote(
        selectedProject.Id,
        purpose,
        layer,
        selectedItems
    );

    AnsiConsole.MarkupLine(
        "[bold green]پیش‌فاکتور با موفقیت ثبت شد![/]");

    Pause();
}


// ==========================================
// ورود محصولات از اکسل
// ==========================================

static void ImportProductsFromExcel(FroshService froshService)
{
    try
    {
        var importer = new ExcelProductImporter();

        string filePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "لیست_قیمت_نیوپایپ_تبدیل_به_اکسل.xlsx"
        );

        var products = importer.ReadProducts(filePath);

        froshService.ImportProductsFromExcel(products);

        AnsiConsole.MarkupLine(
            $"[green]عملیات ورود {products.Count} محصول با موفقیت انجام شد.[/]"
        );
    }
    catch (Exception ex)
    {
        AnsiConsole.MarkupLine(
            $"[red]خطا: {Markup.Escape(ex.ToString())}[/]"
        );
    }

    AnsiConsole.MarkupLine(
        "\nبرای ادامه یک کلید بزنید..."
    );

    Console.ReadKey(true);
}
 void ShowQuoteDetails(Quote quote)
{
    AnsiConsole.Clear();

    AnsiConsole.Write(
        new Rule("[bold green]جزئیات پیش‌فاکتور[/]")
            .RuleStyle("green")
    );

    AnsiConsole.MarkupLine($"[bold]شناسه:[/] {quote.Id}");
    AnsiConsole.MarkupLine($"[bold]نام پروژه:[/] {quote.Project?.Name ?? "نامشخص"}");
    AnsiConsole.MarkupLine($"[bold]نام مشتری:[/] {quote.Project?.Member?.Name ?? "نامشخص"}");
    AnsiConsole.MarkupLine($"[bold]تاریخ ثبت:[/] {quote.CreatedAt.ToLocalTime():yyyy/MM/dd}");

    var table = new Table();

    table.Border(TableBorder.Rounded);

    table.AddColumn("ردیف");
    table.AddColumn("نام محصول و سایز");
    table.AddColumn("تعداد");
    table.AddColumn("قیمت واحد");
    table.AddColumn("مبلغ کل");

    for (int i = 0; i < quote.Items.Count; i++)
    {
        var item = quote.Items[i];

        table.AddRow(
            (i + 1).ToString(),
            $"{item.ProductName} - {item.ProductSpecification}",
            item.Quantity.ToString("N2"),
            $"{item.UnitPrice:N0} ریال",
            $"{item.TotalPrice:N0} ریال"
        );
    }

    AnsiConsole.Write(table);

    AnsiConsole.MarkupLine(
        $"[bold green]مبلغ نهایی: {quote.TotalPrice:N0} ریال[/]");

    Pause();
}

// ==========================================
// توقف برنامه
// ==========================================

void Pause()
{
    AnsiConsole.MarkupLine(
        "\n[grey]برای ادامه یک کلید بزنید...[/]"
    );

    Console.ReadKey(true);
}