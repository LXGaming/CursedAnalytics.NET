﻿using System.Text.Json.Serialization;

namespace LXGaming.CursedAnalytics.Configuration.Category;

public class QuartzCategory {

    public const int DefaultMaxConcurrency = 2;

    [JsonPropertyName("maxConcurrency")]
    public int MaxConcurrency { get; set; } = DefaultMaxConcurrency;
}