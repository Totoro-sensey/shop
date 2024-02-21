namespace Shop.Helpers;

public static class ProductMeasurementTypeHelper
{
    public static ProductMeasurementType GetProductType(this string value)
    {
        if (value.Trim().ToLower().Contains("штука"))
            return ProductMeasurementType.Item;
        if (value.Trim().ToLower().Contains("килограмм"))
            return ProductMeasurementType.Kilogram;  
        if (value.Trim().ToLower().Contains("литр"))
            return ProductMeasurementType.Liter;
        
        throw new InvalidDataException("Ошибка получения типа единицы измерения продукта!");
    }
}