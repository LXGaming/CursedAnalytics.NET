﻿using System.Text.Json.Serialization;

namespace LXGaming.CursedAnalytics.Configuration.Categories;

public class QuartzCategory {

    public const int DefaultMaxConcurrency = 5;

    [JsonPropertyName("maxConcurrency")]
    public int MaxConcurrency { get; set; } = DefaultMaxConcurrency;
}