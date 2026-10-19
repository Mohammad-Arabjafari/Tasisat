
using ClosedXML.Excel;
using Frosh.Core.Entities;

namespace Frosh.Data.InMemori.Import;

public class ExcelProductImporter
{
    public List<Product> ReadProducts(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(
                "فایل اکسل پیدا نشد.",
                filePath);
        }

        using var workbook = new XLWorkbook(filePath);

        var worksheet = workbook.Worksheet(1);

        var products = new List<Product>();

        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        for (int row = 2; row <= lastRow; row++)
        {
            string name = worksheet.Cell(row, 1).GetString().Trim();

            string unit = worksheet.Cell(row, 2).GetString().Trim();

            string priceText = worksheet.Cell(row, 3).GetString().Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidDataException(
                    $"نام محصول در ردیف {row} خالی است.");
            }

            if (string.IsNullOrWhiteSpace(unit))
            {
                throw new InvalidDataException(
                    $"واحد محصول در ردیف {row} خالی است.");
            }

            if (!decimal.TryParse(
                    priceText,
                    System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out decimal unitPrice))
            {
                if (!decimal.TryParse(priceText, out unitPrice))
                {
                    throw new InvalidDataException(
                        $"قیمت محصول در ردیف {row} معتبر نیست.");
                }
            }

            if (unitPrice <= 0)
            {
                throw new InvalidDataException(
                    $"قیمت محصول در ردیف {row} باید بیشتر از صفر باشد.");
            }

            products.Add(new Product
            {
                Name = name,
                Unit = unit,
                UnitPrice = unitPrice
            });
        }

        return products;
    }
}

