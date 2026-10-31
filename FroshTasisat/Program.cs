using Spectre.Console;
using Frosh.Core.Entities;
using Frosh.Core.Interface;
using Frosh.Core.Service;
using Frosh.Data.SqlServer.Context;
using Frosh.Data.SqlServer.Repository;
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
            CreateQuoteMenu();
            break;

        case "🏗️ ثبت پروژه جدید":
            AddProject();
            break;

        case "🏗️ نمایش پروژه‌ها":
            ShowProjects();
            break;

        case "📄 نمایش پیش‌فاکتورها":
            ShowQuotes();
            break;

        case "📥 ورود محصولات از اکسل":
            ImportProductsFromExcel();
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
            product.Purpose.ToString(),
            product.Layer.ToString(),
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

void ShowQuotes()
{
    var quotes = froshService.GetAllQuotes().ToList();

    if (quotes.Count == 0)
    {
        AnsiConsole.MarkupLine(
            "[yellow]هنوز پیش‌فاکتوری ثبت نشده است.[/]"
        );

        Pause();
        return;
    }

    var table = new Table();

    table.Border(TableBorder.Rounded);
    table.Title("[bold cyan]فهرست پیش‌فاکتورها[/]");

    table.AddColumn("ردیف");
    table.AddColumn("شناسه پیش‌فاکتور");
    table.AddColumn("شناسه پروژه");
    table.AddColumn("کاربرد");
    table.AddColumn("نوع لوله");
    table.AddColumn("تاریخ");

    int row = 1;

    foreach (var quote in quotes)
    {
        table.AddRow(
            row.ToString(),
            quote.Id.ToString(),
            quote.ProjectId.ToString(),
            quote.PipePurpose.ToString(),
            quote.PipeLayer.ToString(),
            quote.CreatedAt.ToLocalTime().ToString("yyyy/MM/dd")
        );

        row++;
    }

    AnsiConsole.Write(table);

    Pause();
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

void CreateQuoteMenu()
{
    var projects = froshService.GetAllProjects().ToList();

    if (projects.Count == 0)
    {
        AnsiConsole.MarkupLine(
            "[yellow]ابتدا باید یک پروژه ثبت کنید.[/]"
        );

        Pause();
        return;
    }

    var selectedProject = AnsiConsole.Prompt(
        new SelectionPrompt<Project>()
            .Title("انتخاب پروژه:")
            .UseConverter(p => p.Name)
            .AddChoices(projects)
    );

    var purpose = AnsiConsole.Prompt(
        new SelectionPrompt<PipePurpose>()
            .Title("انتخاب کاربرد لوله:")
            .AddChoices(Enum.GetValues<PipePurpose>())
    );

    var layer = AnsiConsole.Prompt(
        new SelectionPrompt<PipeLayer>()
            .Title("انتخاب نوع لوله:")
            .AddChoices(Enum.GetValues<PipeLayer>())
    );

    var products = froshService.GetAllProducts()
        .Where(p =>
            p.IsActive &&
            p.Purpose == purpose &&
            p.Layer == layer)
        .ToList();

    if (products.Count == 0)
    {
        AnsiConsole.MarkupLine(
            "[yellow]محصولی با این مشخصات پیدا نشد.[/]"
        );

        Pause();
        return;
    }

    var selectedItems = new List<(Guid ProductId, decimal Quantity)>();

    while (true)
    {
        var choices = products
            .Select(p => p.Name)
            .ToList();

        choices.Add("پایان انتخاب محصولات");

        var selectedName = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("انتخاب محصول:")
                .AddChoices(choices)
        );

        if (selectedName == "پایان انتخاب محصولات")
            break;

        var selectedProduct = products.First(
            p => p.Name == selectedName
        );

        decimal quantity = AnsiConsole.Ask<decimal>(
            $"مقدار {selectedProduct.Name}:"
        );

        if (quantity <= 0)
        {
            AnsiConsole.MarkupLine(
                "[red]مقدار باید بیشتر از صفر باشد.[/]"
            );

            continue;
        }

        selectedItems.Add(
            (selectedProduct.Id, quantity)
        );

        products.Remove(selectedProduct);

        if (products.Count == 0)
            break;
    }

    if (selectedItems.Count == 0)
    {
        AnsiConsole.MarkupLine(
            "[yellow]هیچ محصولی انتخاب نشده است.[/]"
        );

        Pause();
        return;
    }

    try
    {
        var quote = froshService.CreateQuote(
            selectedProject.Id,
            purpose,
            layer,
            selectedItems
        );

        AnsiConsole.MarkupLine(
            $"\n[bold green]پیش‌فاکتور با موفقیت ثبت شد.[/]"
        );

        AnsiConsole.MarkupLine(
            $"شناسه پیش‌فاکتور: {quote.Id}"
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
// ورود محصولات از اکسل
// ==========================================

void ImportProductsFromExcel()
{
    AnsiConsole.MarkupLine(
        "[yellow]بخش ورود محصولات از اکسل هنوز به Repositoryهای SQL متصل نشده است.[/]"
    );

    AnsiConsole.MarkupLine(
        "[grey]این قسمت را پس از تست اتصال دیتابیس تکمیل می‌کنیم.[/]"
    );

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