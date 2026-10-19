using Spectre.Console;
using Frosh.Data.InMemori.Import;
using Frosh.Core.Entities;
using Frosh.Core.Interface;
using Frosh.Core.Service;
using Frosh.Data.InMemori.Repository;


// ==========================================
// 1. ساخت Repositoryها
// ==========================================

IMemberRepository memberRepository = new MemberRepository();

IProductRepository productRepository = new ProductRepository();

IProjectRepository projectRepository = new ProjectRepository();

IQuoteRepository quoteRepository = new QuoteRepository();

IQuoteItemRepository quoteItemRepository = new QuoteItemRepository();

// ==========================================
// 2. ساخت Service
// ==========================================

var froshService = new FroshService(
    memberRepository,
    productRepository,
    projectRepository,
    quoteRepository,
    quoteItemRepository
);


// ==========================================
// 3. اجرای برنامه
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
// متد نمایش عنوان برنامه
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

    table.AddColumn("[bold yellow]ردیف[/]");
    table.AddColumn("[bold yellow]نام محصول[/]");
    table.AddColumn("[bold yellow]کاربرد[/]");
    table.AddColumn("[bold yellow]نوع لوله[/]");
    table.AddColumn("[bold yellow]واحد[/]");
    table.AddColumn("[bold green]قیمت واحد[/]");
    table.AddColumn("[bold yellow]وضعیت[/]");

    int row = 1;

    foreach (var product in products)
    {
        table.AddRow(
            row.ToString(),
            Markup.Escape(product.Name ?? ""),
            Markup.Escape(product.Purpose.ToString()),
            Markup.Escape(product.Layer.ToString()),
            Markup.Escape(product.Unit ?? ""),
            product.UnitPrice.ToString("N0"),
            product.IsActive
                ? "[green]فعال[/]"
                : "[red]غیرفعال[/]"
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

    table.AddColumn("[yellow]نام[/]");
    table.AddColumn("[yellow]شماره تماس[/]");
    table.AddColumn("[yellow]نوع عضو[/]");
    table.AddColumn("[yellow]شهر[/]");

    foreach (var member in members)
    {
        table.AddRow(
            Markup.Escape(member.Name ?? ""),
            member.phoneNumber.ToString(),
            Markup.Escape(member.MemberType ?? ""),
            Markup.Escape(member.City ?? "")
        );
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

    table.AddColumn("[yellow]نام پروژه[/]");
    table.AddColumn("[yellow]شناسه عضو[/]");
    table.AddColumn("[yellow]آدرس[/]");

    foreach (var project in projects)
    {
        table.AddRow(
            Markup.Escape(project.Name ?? ""),
            project.MemberId.ToString(),
            Markup.Escape(project.Address ?? "")
        );
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

    table.AddColumn("[yellow]شناسه[/]");
    table.AddColumn("[yellow]شناسه پروژه[/]");
    table.AddColumn("[yellow]کاربرد لوله[/]");
    table.AddColumn("[yellow]نوع لوله[/]");
    table.AddColumn("[yellow]تاریخ ایجاد[/]");

    foreach (var quote in quotes)
    {
        table.AddRow(
            quote.Id.ToString()[..8],
            quote.ProjectId.ToString()[..8],
            quote.PipePurpose.ToString(),
            quote.PipeLayer.ToString(),
            quote.CreatedAt.ToLocalTime().ToString("yyyy/MM/dd")
        );
    }

    AnsiConsole.Write(table);

    Pause();
}


// ==========================================
// توقف تا فشردن کلید
// ==========================================

void Pause()
{
    AnsiConsole.MarkupLine(
        "\n[grey]برای بازگشت به منو، یک کلید فشار بدهید...[/]"
    );

    Console.ReadKey(true);
}

void AddProduct()
{
    AnsiConsole.Clear();

    AnsiConsole.Write(
        new Rule("[bold cyan]افزودن محصول جدید[/]")
        {
            Justification = Justify.Center
        });

    string name = AnsiConsole.Ask<string>(
        "[yellow]نام محصول را وارد کنید:[/]");

    string unit = AnsiConsole.Ask<string>(
        "[yellow]واحد فروش (عدد، متر، شاخه و ...):[/]");

    decimal unitPrice = AnsiConsole.Prompt(
        new TextPrompt<decimal>(
            "[yellow]قیمت واحد محصول را وارد کنید:[/]")
            .Validate(price =>
                price > 0
                    ? ValidationResult.Success()
                    : ValidationResult.Error(
                        "[red]قیمت باید بیشتر از صفر باشد.[/]")));

    try
    {
        var product = new Product
        {
            Name = name,
            Unit = unit,
            UnitPrice = unitPrice
        };

        froshService.AddProduct(product);

        AnsiConsole.MarkupLine(
            "\n[bold green]محصول با موفقیت ثبت شد.[/]");
    }
    catch (ArgumentException ex)
    {
        AnsiConsole.MarkupLine(
            $"[red]خطا: {Markup.Escape(ex.Message)}[/]");
    }
   catch (Exception ex)
{
    AnsiConsole.MarkupLine(
        $"[red]نوع خطا: {Markup.Escape(ex.GetType().FullName ?? "Unknown")}[/]");

    AnsiConsole.MarkupLine(
        $"[red]پیام خطا: {Markup.Escape(ex.Message)}[/]");

    AnsiConsole.MarkupLine(
        $"[grey]جزئیات خطا: {Markup.Escape(ex.StackTrace ?? "موجود نیست")}[/]");
}
    Pause();
}




void AddMember()
{
    AnsiConsole.Clear();

    AnsiConsole.Write(
        new Rule("[bold cyan]افزودن عضو جدید[/]")
        {
            Justification = Justify.Center
        });

    string name = AnsiConsole.Ask<string>(
        "[yellow]نام و نام خانوادگی عضو را وارد کنید:[/]");

    int phoneNumber = AnsiConsole.Prompt(
        new TextPrompt<int>(
            "[yellow]شماره تماس را وارد کنید:[/]")
            .Validate(number =>
                number > 0
                    ? ValidationResult.Success()
                    : ValidationResult.Error(
                        "[red]شماره تماس معتبر نیست.[/]")));

    string city = AnsiConsole.Ask<string>(
        "[yellow]شهر محل فعالیت را وارد کنید:[/]");

    string memberType = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[yellow]نوع عضو را انتخاب کنید:[/]")
            .AddChoices(
                "مجری",
                "پیمانکار",
                "کارفرما"
            ));

    try
    {
        var member = new Member
        {
            Name = name,
            phoneNumber = phoneNumber,
            City = city,
            MemberType = memberType
        };

        froshService.AddMember(member);

        AnsiConsole.MarkupLine(
            "\n[bold green]عضو با موفقیت ثبت شد.[/]");
    }
    catch (ArgumentException ex)
    {
        AnsiConsole.MarkupLine(
            $"[red]خطا: {Markup.Escape(ex.Message)}[/]");
    }
    catch (Exception ex)
    {
        AnsiConsole.MarkupLine(
            $"[red]خطای غیرمنتظره: {Markup.Escape(ex.Message)}[/]");
    }

    Pause();
    }

    void ImportProductsFromExcel()
    {
    AnsiConsole.Clear();

    string filePath = AnsiConsole.Ask<string>(
        "[yellow]مسیر کامل فایل اکسل را وارد کنید:[/]");

    try
    {
        var importer = new ExcelProductImporter();

        var products = importer.ReadProducts(filePath);

        if (products.Count == 0)
        {
            AnsiConsole.MarkupLine(
                "[yellow]محصولی برای ورود پیدا نشد.[/]");

            Pause();
            return;
        }

        if (!AnsiConsole.Confirm(
                $"تعداد {products.Count} محصول شناسایی شد. آیا ذخیره شوند؟"))
        {
            AnsiConsole.MarkupLine(
                "[yellow]عملیات ورود لغو شد.[/]");

            Pause();
            return;
        }

        foreach (var product in products)
        {
            froshService.AddProduct(product);
        }

        AnsiConsole.MarkupLine(
            $"[bold green]{products.Count} محصول با موفقیت وارد شدند.[/]");
    }
    catch (Exception ex)
    {
        AnsiConsole.MarkupLine(
            $"[red]خطا در ورود اکسل: {Markup.Escape(ex.Message)}[/]");
    }

    Pause();
    }
        void CreateQuoteMenu()
    {
        AnsiConsole.Clear();

        var projects = froshService.GetAllProjects().ToList();
        var products = froshService.GetAllProducts().ToList();

        if (projects.Count == 0)
        {
            AnsiConsole.MarkupLine(
                "[yellow]ابتدا باید یک پروژه ثبت کنید.[/]");

            Pause();
            return;
        }

        if (products.Count == 0)
        {
            AnsiConsole.MarkupLine(
                "[yellow]ابتدا باید محصول ثبت کنید.[/]");

            Pause();
            return;
        }

        var selectedProject = AnsiConsole.Prompt(
            new SelectionPrompt<Project>()
                .Title("[cyan]پروژه موردنظر را انتخاب کنید:[/]")
                .UseConverter(project => project.Name)
                .AddChoices(projects));

        var selectedPurpose = AnsiConsole.Prompt(
            new SelectionPrompt<PipePurpose>()
                .Title("[cyan]کاربرد لوله را انتخاب کنید:[/]")
                .AddChoices(Enum.GetValues<PipePurpose>()));

        var selectedLayer = AnsiConsole.Prompt(
            new SelectionPrompt<PipeLayer>()
                .Title("[cyan]نوع لوله را انتخاب کنید:[/]")
                .AddChoices(Enum.GetValues<PipeLayer>()));

        var selectedItems =
            new List<(Guid ProductId, decimal Quantity)>();

        bool addingItems = true;

        while (addingItems)
        {
            var selectedProduct = AnsiConsole.Prompt(
                new SelectionPrompt<Product>()
                    .Title("[cyan]محصول موردنظر را انتخاب کنید:[/]")
                    .UseConverter(product =>
                        $"{product.Name} | {product.UnitPrice:N0} تومان")
                    .AddChoices(products));

            decimal quantity = AnsiConsole.Prompt(
                new TextPrompt<decimal>(
                    "[yellow]تعداد یا مقدار محصول را وارد کنید:[/]")
                    .Validate(value =>
                        value > 0
                            ? ValidationResult.Success()
                            : ValidationResult.Error(
                                "[red]مقدار باید بیشتر از صفر باشد.[/]")));

            selectedItems.Add((selectedProduct.Id, quantity));

            addingItems = AnsiConsole.Confirm(
                "آیا می‌خواهید محصول دیگری اضافه کنید؟");
        }

        try
        {
            var quote = froshService.CreateQuote(
                selectedProject.Id,
                selectedPurpose,
                selectedLayer,
                selectedItems);

            AnsiConsole.MarkupLine(
                $"\n[bold green]پیش‌فاکتور با شناسه {quote.Id} ثبت شد.[/]");

            AnsiConsole.MarkupLine(
                $"[cyan]تعداد ردیف‌ها: {quote.Items.Count}[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine(
                $"[red]ثبت پیش‌فاکتور ناموفق بود: {Markup.Escape(ex.Message)}[/]");
        }

        Pause();
    }


 void AddProject()
    {
    AnsiConsole.Clear();

    AnsiConsole.Write(
        new Rule("[bold cyan]ثبت پروژه جدید[/]")
        {
            Justification = Justify.Center
        });

    try
    {
        string name = AnsiConsole.Prompt(
            new TextPrompt<string>("نام پروژه را وارد کنید:")
                .Validate(value =>
                    !string.IsNullOrWhiteSpace(value)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("نام پروژه الزامی است.")));

        string address = AnsiConsole.Prompt(
            new TextPrompt<string>("آدرس پروژه را وارد کنید:")
                .Validate(value =>
                    !string.IsNullOrWhiteSpace(value)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("آدرس پروژه الزامی است.")));

        var members = froshService.GetAllMembers().ToList();

        if (members.Count == 0)
        {
            AnsiConsole.MarkupLine(
                "[yellow]ابتدا باید یک عضو ثبت کنید تا پروژه به او اختصاص داده شود.[/]");

            Pause();
            return;
        }

        var selectedMember = AnsiConsole.Prompt(
            new SelectionPrompt<Member>()
                .Title("[cyan]عضو مرتبط با پروژه را انتخاب کنید:[/]")
                .UseConverter(member => $"{member.Name} - {member.City}")
                .AddChoices(members));

        var project = new Project
        {
            Name = name,
            Address = address,
            MemberId = selectedMember.Id
        };

        froshService.AddProject(project);

        AnsiConsole.MarkupLine(
            "\n[bold green]پروژه با موفقیت ثبت شد.[/]");

        AnsiConsole.MarkupLine(
            $"[cyan]نام پروژه: {Markup.Escape(project.Name)}[/]");

        AnsiConsole.MarkupLine(
            $"[cyan]عضو مرتبط: {Markup.Escape(selectedMember.Name)}[/]");
    }
    catch (Exception ex)
    {
        AnsiConsole.MarkupLine(
            $"[red]خطا در ثبت پروژه: {Markup.Escape(ex.Message)}[/]");
    }

    Pause();
    }



