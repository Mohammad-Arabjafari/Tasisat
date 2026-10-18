using Spectre.Console;


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


// ==========================================
// 2. ساخت Service
// ==========================================

var froshService = new FroshService(
    memberRepository,
    productRepository,
    projectRepository,
    quoteRepository
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
        case "🏗️ نمایش پروژه‌ها":
            ShowProjects();
            break;

        case "📄 نمایش پیش‌فاکتورها":
            ShowQuotes();
            break;

        case "📥 ورود محصولات از اکسل":
            AnsiConsole.MarkupLine(
                "[yellow]بخش ورود از اکسل را در مرحله بعد تکمیل می‌کنیم.[/]"
            );
            Pause();
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
AnsiConsole.MarkupLine(
"[yellow]قابلیت افزودن محصول هنوز پیاده‌سازی نشده است.[/]"
);


Pause();

}

void AddMember()
{
AnsiConsole.MarkupLine(
"[yellow]قابلیت افزودن عضو هنوز پیاده‌سازی نشده است.[/]"
);


Pause();


}
