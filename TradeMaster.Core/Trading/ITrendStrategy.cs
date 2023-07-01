using TradeMaster.Core.Trading.Models;
using TradeMaster.Core.Trading.Strategies;

namespace TradeMaster.Core.Trading;

public interface ITrendStrategy
{
    Task<TradeOperationResult> Run(TradeParameter strategyParameter);
}
