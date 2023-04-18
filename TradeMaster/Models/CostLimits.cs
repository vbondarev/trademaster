namespace TradeMaster.Models;

/// <summary>
/// Границы стоимости монеты
/// </summary>
public class CostLimits
{
    public int IntervalNumber { get; set; }
    public DateTimeOffset StartDateTime { get; set; }
    public DateTimeOffset EndDateTime { get; set; }
    
    /// <summary>
    /// Верхняя граница стоимости
    /// </summary>
    public decimal UpperCostBound { get; set; }
    
    /// <summary>
    /// Нижняя граница стоимости
    /// </summary>
    public decimal LowerCostBound { get; set; }
    
    // в этой модели уже должен быть тип коэффициента, который необходимо рассчитать при формировании этой модели
    // по схеме если верхняя цена в текущем интервале больше нижней, тип коэфициента будет негативный,
    // если верхняя цена меньше нижней, тип коэфиициента будет позитивный,
    // если верхняя цена равна нижней, тип коэффициента будет нейтральный
    public RateType RateType { get; set; } 
    
    //в этой модели уже должен быть процентный коэффициент, который необходимо рассчитать при формировании этой модели
    public double Rate { get; set; }
}
