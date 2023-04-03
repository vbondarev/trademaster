namespace TradeMaster.Models;

/// <summary>
/// Модель для истории изменений цены в определенном интервале
/// </summary>
public class HistoryPriceModel
{
    public Interval Interval { get; set; }
    public int IntervalCount { get; set; }
    public List<CostLimits> CostLimits { get; set; }
}

/// <summary>
/// Возможные временные интервалы
/// </summary>
public enum Interval
{
    Year,
    Month,
    Week,
    Day,
    FourHour,
    TwoHour,
    Hour,
    HalfHour,
    QuarterHour,
    Minute
}

/// <summary>
/// Границы стоимости монеты
/// </summary>
public class CostLimits
{
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public decimal UpperCostBound { get; set; }
    public decimal LowerCostBound { get; set; }
    
    // в этой модели уже должен быть тип коэффициента, который необходимо рассчитать при формировании этой модели
    // по схеме если верхняя цена в текущем интервале больше нижней, тип коэфициента будет негативный,
    // если верхняя цена меньше нижней, тип коэфиициента будет позитивный,
    // если верхняя цена равна нижней, тип коэффициента будет нейтральный
    public RateTypes RateType { get; set; } 
    
    //в этой модели уже должен быть процентный коэффициент, который необходимо рассчитать при формировании этой модели
    public decimal Rate { get; set; } = 100; //- LowerCostBound;
}

public enum RateTypes
{
    Positive,
    Negative,
    Neutral
}
