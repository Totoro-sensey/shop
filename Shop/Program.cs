 using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Shop.Models;
using Shop.Models.Appeals;
using Shop.Models.Organizations;
 using Shop.Models.Products;
 using System.Globalization;
 using Shop.Helpers;

 namespace Shop;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Введите путь до файла");
        var path = Console.ReadLine();
        if (path.Trim() != null) 
            Console.WriteLine("Неверно указан путь!");
        
        var (products, organizations, appeals) = ReadExcel(path);
        
        Console.WriteLine("Выберите услугу: " +
                          "\n 1 - Вывести информацию о клиенте " +
                          "\n 2 - Обновить информацию о клиенте" +
                          "\n 3 - Узнать 'Золотого' клиента");
        
        int.TryParse(Console.ReadLine(), out var service);
        switch (service)
        {
            case 1:
                ShowOrganizationInformation(products, organizations, appeals);
                break;
            case 2:
                ChangeInformation(organizations);
                break;
            case 3:
                ShowGoldOrganization(organizations, appeals);
                break;
        }
    }

    private static void ShowGoldOrganization(List<Organization> organizations, List<Appeal> appeals)
    {
        Console.WriteLine("Введите месяц в формате 01.2024");
        DateOnly.TryParse(Console.ReadLine(), out var month);

        var goldOrganization = appeals.Where(i => i.CreateDate.Month == month.Month && i.CreateDate.Year == month.Year)
            .GroupBy(i => i.OrganizationCode)
            .MaxBy(o => o.Count());
        
        Console.WriteLine($"Золотой клиент месяца - {organizations.FirstOrDefault(i => i.Code == goldOrganization.Key).Name}");
    }

    private static void ChangeInformation(List<Organization> organizations)
    {
        Console.WriteLine("Введите текущее наименование");
        var oldName = Console.ReadLine();
        
        Console.WriteLine("Введите новое наименование");
        var newName = Console.ReadLine();
        
        Console.WriteLine("Введите новое контактное лицо");
        var contactPerson = Console.ReadLine();

        var organization = organizations.FirstOrDefault(i => i.Name.Trim().ToLower() == oldName.Trim().ToLower());

        organization.Name = newName;
        var oldPerson = organization.ContactPerson;
        organization.ContactPerson = contactPerson;
        
        Console.WriteLine($"Данные изменены: {oldName} -> {newName}, {oldPerson} -> {contactPerson}");
    }

    private static void ShowOrganizationInformation(List<Product> products, List<Organization> organizations,
        List<Appeal> appeals)
    {
        Console.WriteLine("Введите наименование товара");
        var productName = Console.ReadLine();

         var product = products.FirstOrDefault(i => string.Equals(i.Name, productName.Trim()));
         var appeal = appeals.Where(i => i.ProductCode == product.Code).ToList();

        Console.WriteLine("Заявки: \n");
        appeal.ForEach(i => Console.WriteLine($"Название организации : {organizations.FirstOrDefault(j => j.Code == i.OrganizationCode).Name} \n" +
                                              $"количество товара: {i.Count}\n" +
                                              $"цена: {product.Price * i.Count}\n" +
                                              $"дата: {i.CreateDate.ToString(CultureInfo.CurrentCulture)}"));
    }

    private static string GetCellValue(CellType cell, OpenXmlPartContainer? workbookPart)
    {
        var cellValue = string.Empty;
        if (cell is null)
            return cellValue;

        if (cell.DataType != null && cell.DataType == CellValues.SharedString)
        {
            var sharedStringPart = workbookPart?.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
            if (sharedStringPart != null)
            {
                cellValue = sharedStringPart.SharedStringTable.ElementAt(int.Parse(cell.InnerText)).InnerText;
            }
        }
        else
        {
            cellValue = cell.InnerText;
        }

        return cellValue;
    }

    private static WorksheetPart GetWorksheetPartByName(SpreadsheetDocument document, string sheetName)
    {
        var sheets = document.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>().Where(s => s.Name == sheetName);

        if (!sheets.Any())
            return null;
        

        var relationshipId = sheets.First().Id.Value;
        var worksheetPart = (WorksheetPart)document.WorkbookPart.GetPartById(relationshipId);

        return worksheetPart;
    }

    private static (List<Product> products, List<Organization> organizations, List<Appeal> appeals) ReadExcel(string filePath)
    {
        var dict = new Dictionary<int, string>
        {
            [1] = "Товары", 
            [2] = "Клиенты", 
            [3] = "Заявки"
        };

        var products = new List<Product>();
        var organizations = new List<Organization>();
        var appeals = new List<Appeal>();
        
        using var spreadsheetDocument = SpreadsheetDocument.Open(filePath, false);
        var workbookPart = spreadsheetDocument.WorkbookPart;
        foreach (var entity in dict)
        {
            var worksheet = GetWorksheetPartByName(spreadsheetDocument, entity.Value);
            var sheetData = worksheet?.Worksheet.Elements<SheetData>().First();
            switch (entity.Key)
            {
                case 1: GetProducts(sheetData, workbookPart, products);
                    break;
                case 2: GetOrganizations(sheetData, workbookPart, organizations);
                    break;
                case 3: GetAppeals(sheetData, workbookPart, appeals);
                    break;
            }
        }
        return (products, organizations, appeals);
    }

    private static void GetAppeals(SheetData sheetData, WorkbookPart? workbookPart, List<Appeal> appeals)
    {
        foreach (var row in sheetData.Elements<Row>().Skip(1))
        {
            if (string.IsNullOrEmpty(row.First().InnerText))
                break;
            
            appeals.Add(new Appeal
            {
                Code = int.Parse(GetCellValue((Cell)row.ElementAt(AppealColumns.Code), workbookPart)),
                Number = int.Parse(GetCellValue((Cell)row.ElementAt(AppealColumns.Number), workbookPart)),
                CreateDate = new DateOnly(), // DateOnly.Parse(GetCellValue((Cell)row.ElementAt(AppealColumns.CreateDate), workbookPart)),
                OrganizationCode = int.Parse(GetCellValue((Cell)row.ElementAt(AppealColumns.OrganizationCode), workbookPart)),
                Count = int.Parse(GetCellValue((Cell)row.ElementAt(AppealColumns.Count), workbookPart)),
                ProductCode = int.Parse(GetCellValue((Cell)row.ElementAt(AppealColumns.ProductCode), workbookPart))
            });
        }
    }

    private static void GetOrganizations(SheetData sheetData, WorkbookPart? workbookPart,
        List<Organization> organizations)
    {
        foreach (var row in sheetData.Elements<Row>().Skip(1))
        {
            if (string.IsNullOrEmpty(row.First().InnerText))
                break;
            
            organizations.Add(new Organization
            {
                Code = int.Parse(GetCellValue((Cell)row.ElementAt(OrganizationColumn.Code), workbookPart)), 
                Name = GetCellValue((Cell)row.ElementAt(OrganizationColumn.Name), workbookPart), 
                ContactPerson = GetCellValue((Cell)row.ElementAt(OrganizationColumn.ContactPerson), workbookPart), 
            });
        }
    }

    private static void GetProducts(SheetData sheetData, WorkbookPart? workbookPart, List<Product> products)
    {
        foreach (var row in sheetData.Elements<Row>().Skip(1))
        {
            if (string.IsNullOrEmpty(row.First().InnerText))
                break;
            
            products.Add(new Product
            {
                Code = int.Parse(GetCellValue((Cell)row.ElementAt(ProductColumn.Code), workbookPart)), 
                Name = GetCellValue((Cell)row.ElementAt(ProductColumn.Name), workbookPart), 
                Price = decimal.Parse(GetCellValue((Cell)row.ElementAt(ProductColumn.Price), workbookPart)), 
                Type = GetCellValue((Cell)row.ElementAt(ProductColumn.Type), workbookPart).GetProductType()
            });
        }
    }

   
}