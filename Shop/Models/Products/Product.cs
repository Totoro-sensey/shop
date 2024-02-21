namespace Shop.Models;

/// <summary>
/// Товар
/// </summary>
public class Product
{
    /// <summary>
    /// Код товара
    /// </summary>
    public int Code { get; set; }
    
    /// <summary>
    /// Наименование товара 
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Тип продукта
    /// </summary>
    public ProductMeasurementType Type { get; set; }
    
    /// <summary>
    /// Цена
    /// </summary>
    public decimal Price { get; set; }
}