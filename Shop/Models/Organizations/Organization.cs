namespace Shop.Models.Organizations;

/// <summary>
/// Покупатель
/// </summary>
public class Organization
{
    /// <summary>
    /// Код 
    /// </summary>
    public int Code { get; set; }
    
    /// <summary>
    /// Наименование организации
    /// </summary>
    public string Name {get; set; }
    
    /// <summary>
    /// Контактное лицо (ФИО)
    /// </summary>
    public string ContactPerson { get; set; }
}