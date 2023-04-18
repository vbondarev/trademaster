﻿using JetBrains.Annotations;

namespace TradeMaster.Options;

public record BinanceOptions
{
    public string BaseUri { get; [UsedImplicitly]init; } = null!;
    
    public string ApiKey { get; [UsedImplicitly]init; } = null!;
    
    public string SecretKey { get; [UsedImplicitly]init; } = null!;
}
