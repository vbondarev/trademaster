﻿using TradeMaster.Core.Integrations.Binance.Requests;
using TradeMaster.Core.Integrations.Binance.Responses;
using TradeMaster.Core.Trading.Enums;

namespace TradeMaster.Core.Integrations.Binance;

public interface IBinanceConnector
{
    /// <summary>
    /// Запрос статуса системы
    /// </summary>
    /// <returns></returns>
    Task<SystemStatusResponse> GetSystemStatus();

    /// <summary>
    /// Создание ордера на покупку
    /// </summary>
    Task<BuyOrderResponse> CreateBuyOrder(BuyOrderRequest request);
    
    /// <summary>
    /// Создание ЛИМИТНОГО ордера на продажу
    /// </summary>
    Task<SellOrderResponse> CreateSellLimitOrder(SellLimitOrderRequest request);
    
    /// <summary>
    /// Создание СТОП ЛИМИТНОГО ордера на продажу
    /// </summary>
    Task<SellOrderResponse> CreateSellStopLossLimitOrder(SellStopLossLimitOrderRequest request);
    
    /// <summary>
    /// Запрос ордера
    /// </summary>
    Task<QueryOrderResponse> QueryOrder(QueryOrderRequest request);
    
    /// <summary>
    /// Отмена ордера
    /// </summary>
    Task<CancelOrderResponse> CancelOrder(CancelOrderRequest request);

    /// <summary>
    /// Запрос списка всех торговых операций
    /// </summary>
    Task<IEnumerable<TradeListResponse>> GetAccountTradeList(TradeListRequest request);

    /// <summary>
    /// Запрос свечей за определенный интервал времени 
    /// </summary>
    /// <param name="request"><see cref="CandlestickDataRequest"/></param>
    /// <returns>Возвращает свечи в виде массива, где элементом массива является свеча  </returns>
    /// <example>
    /// <code language="json">
    /// <![CDATA[
    /// [
    ///     [
    ///         1499040000000,      // Kline open time
    ///         "0.01634790",       // Open price
    ///         "0.80000000",       // High price
    ///         "0.01575800",       // Low price
    ///         "0.01577100",       // Close price
    ///         "148976.11427815",  // Volume
    ///         1499644799999,      // Kline Close time
    ///         "2434.19055334",    // Quote asset volume
    ///         308,                // Number of trades
    ///         "1756.87402397",    // Taker buy base asset volume
    ///         "28.46694368",      // Taker buy quote asset volume
    ///         "0"                 // Unused field, ignore.
    ///     ]
    ///]
    /// ]]>
    /// </code>
    /// </example>
    Task<string[][]> GetCandlestickData(CandlestickDataRequest request);

    /// <summary>
    /// Запрос актуальной стоимости монеты
    /// </summary>
    Task<SymbolPriceTickerResponse> GetSymbolPriceTicker(LastPriceRequest request);

    /// <summary>
    /// Запрос общей сумму монет
    /// </summary>
    /// <returns></returns>
    Task<AccountInformationResponse> GetAccountInformation();

    Task<IEnumerable<CancelOrderResponse>> CancelAllOpenOrders(CancelAllOpenOrdersRequest request);
    
    Task<IEnumerable<OpenOrderResponse>> GetOpenOrders(OpenOrdersRequest request);
}
