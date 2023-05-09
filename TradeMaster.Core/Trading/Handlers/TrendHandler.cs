using TradeMaster.Core.Trading.Enums;

namespace TradeMaster.Core.Trading.Handlers;

/// <summary>
/// Класс определения текущего тренда
/// </summary>
internal class TrendHandler
{
    public Trend DefineTrend()
    {
        //разработать функционал по определению текущего тренда
        //пока временно возвращаем Bear
        return Trend.Bear;
    }
}
