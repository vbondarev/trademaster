# Описание

**TradeMaster** - система для автоматизации торговых операций на криптовалютных рынках.
На текущий момент торговля доступна только на спотовом рынке [Binance](https://www.binance.com/). 

## Адреса public API для торговых площадок

| Spot API URL                         | Spot Test Network URL                   |
|--------------------------------------|-----------------------------------------|
| https://api.binance.com	            | https://testnet.binance.vision          |
| wss://ws-api.binance.com/ws-api/v3	| wss://testnet.binance.vision/ws-api/v3  |
| wss://stream.binance.com:9443/ws	    | wss://testnet.binance.vision/ws         |
| wss://stream.binance.com:9443/stream | wss://testnet.binance.vision/stream     |

## Авторизация
* Для использования тестовой площадки нужно получить API_KEY и SECRET_KEY [здесь](https://testnet.binance.vision)
* Для использования реальной площадки нужно получить API_KEY и SECRET_KEY [здесь](https://www.binance.com/en/my/settings/api-management)

Все настройки хранятся в конфигурационном файле. 


