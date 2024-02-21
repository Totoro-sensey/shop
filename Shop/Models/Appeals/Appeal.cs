namespace Shop.Models.Appeals;

/// <summary>
/// Заявка
/// </summary>
public class Appeal
{
    /// <summary>
    /// Код
    /// </summary>
    public int Code { get; set; }
    
    /// <summary>
    /// Код товара
    /// </summary>
    public int ProductCode { get; set; }
    
    /// <summary>
    /// Код организации
    /// </summary>
    public int OrganizationCode { get; set; }
    
    /// <summary>
    /// Номер 
    /// </summary>
    public int Number { get; set; } 
    
    /// <summary>
    /// Требуемое количество 
    /// </summary>
    public int Count { get; set; }
    
    /// <summary>
    /// Дата размещения
    /// </summary>
    public DateOnly CreateDate { get; set; }
}