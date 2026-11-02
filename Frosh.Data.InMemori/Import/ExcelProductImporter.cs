using ClosedXML.Excel;
using Frosh.Core.Entities;
using System.Globalization;

namespace Frosh.Data.InMemori.Import;

public class ExcelProductImporter
{
    public List<Product> ReadProducts(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("فایل اکسل پیدا نشد.", filePath);

        using var workbook = new XLWorkbook(filePath);

        var worksheet = workbook.Worksheet(1);
        var products = new List<Product>();

        int lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        for (int row = 2; row <= lastRow; row++)
        {
            string productCode = worksheet.Cell(row, 1)
                .GetFormattedString().Trim();

            string name = worksheet.Cell(row, 2)
                .GetString().Trim();

            string specification = worksheet.Cell(row, 3)
                .GetString().Trim();

            string unit = worksheet.Cell(row, 5)
                .GetString().Trim();

            string packagingNote = worksheet.Cell(row, 8)
                .GetString().Trim();

            if (string.IsNullOrWhiteSpace(productCode))
                throw new InvalidDataException( $"کد محصول در ردیف {row} خالی است.");

            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidDataException( $"نام محصول در ردیف {row} خالی است.");

            decimal price = ReadDecimal(worksheet.Cell(row, 4), row, "قیمت");

            if (price <= 0)
                throw new InvalidDataException( $"قیمت محصول در ردیف {row} معتبر نیست.");

            decimal? quantityPerBox =
                ReadOptionalDecimal(worksheet.Cell(row, 6), row);

            decimal? quantityPerCarton =
                ReadOptionalDecimal(worksheet.Cell(row, 7), row);

            var product = new Product
            {
                ProductCode = productCode,
                Name = name,
                Specification = specification,
                UnitPrice = price,
                Unit = unit,
                QuantityPerBox = quantityPerBox,
                QuantityPerCarton = quantityPerCarton,
                PackagingNote = packagingNote
            };

            products.Add(product);
        }

        return products;
    }

    private static decimal ReadDecimal(
        IXLCell cell, int row, string fieldName)
    {
        if (cell.TryGetValue<decimal>(out decimal value))
            return value;

        string text = NormalizeNumber(cell.GetString());

        if (decimal.TryParse(
            text,
            NumberStyles.Number,
            CultureInfo.InvariantCulture,
            out value))
            return value;

        throw new InvalidDataException(
            $"{fieldName} در ردیف {row} معتبر نیست.");
    }

private static decimal? ReadOptionalDecimal(IXLCell cell, int row)
{
    if (cell.IsEmpty())
        return null;

    string text = cell.GetFormattedString().Trim();

    // مقادیر خالی یا خط تیره به معنی نامشخص هستند
    if (string.IsNullOrWhiteSpace(text) ||
        text == "-" ||
        text == "–" ||
        text == "—")
    {
        return null;
    }

    text = NormalizeNumber(text);

    if (decimal.TryParse(
        text,
        NumberStyles.Number,
        CultureInfo.InvariantCulture,
        out decimal value))
    {
        return value;
    }

    throw new InvalidDataException(
        $"تعداد بسته‌بندی در ردیف {row} معتبر نیست. مقدار ثبت‌شده: {text}");
}

    private static string NormalizeNumber(string text)
    {
        return text
            .Replace("۰", "0")
            .Replace("۱", "1")
            .Replace("۲", "2")
            .Replace("۳", "3")
            .Replace("۴", "4")
            .Replace("۵", "5")
            .Replace("۶", "6")
            .Replace("۷", "7")
            .Replace("۸", "8")
            .Replace("۹", "9")
            .Replace("٠", "0")
            .Replace("١", "1")
            .Replace("٢", "2")
            .Replace("٣", "3")
            .Replace("٤", "4")
            .Replace("٥", "5")
            .Replace("٦", "6")
            .Replace("٧", "7")
            .Replace("٨", "8")
            .Replace("٩", "9")
            .Replace(",", "")
            .Replace("٬", "")
            .Trim();
    }
}