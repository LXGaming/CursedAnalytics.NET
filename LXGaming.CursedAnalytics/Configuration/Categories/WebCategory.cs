﻿using System.Text.Json.Serialization;

namespace LXGaming.CursedAnalytics.Configuration.Categories;

public class WebCategory {

    public const int DefaultTimeout = 100000; // 100 Seconds

    [JsonPropertyName("timeout")]
    public int Timeout { get; set; } = DefaultTimeout;
}