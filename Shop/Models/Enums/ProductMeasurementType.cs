using System.ComponentModel.DataAnnotations;

namespace Shop;

/// <summary>
/// Единица измерения продукта
/// </summary>
public enum ProductMeasurementType
{
    /// <summary>
    /// Литр 
    /// </summary>
    [Display(Name = "Литр")]
    Liter,
    
    /// <summary>
    /// Килограмм 
    /// </summary>
    [Display(Name = "Килограмм")]
    Kilogram,
    
    /// <summary>
    /// Штука
    /// </summary>
    [Display(Name = "Штука")]
    Item
}