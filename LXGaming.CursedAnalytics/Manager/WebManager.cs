﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LXGaming.CursedAnalytics.Configuration.Category;
using LXGaming.CursedAnalytics.Util;
using LXGaming.CursedAnalytics.Util.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace LXGaming.CursedAnalytics.Manager {

    public static class WebManager {

        public static WebCategory WebCategory => CursedAnalytics.Instance.Config?.WebCategory;
        public static HttpClient HttpClient { get; private set; }

        public static void Prepare() {
            if (WebCategory.Timeout <= 0) {
                Log.Warning("Timeout is out of bounds. Resetting to {Value}", WebCategory.DefaultTimeout);
                WebCategory.Timeout = WebCategory.DefaultTimeout;
            }

            HttpClient = new HttpClient(new LoggingHandler(new HttpClientHandler())) {
                Timeout = TimeSpan.FromMilliseconds(WebCategory.Timeout)
            };
            HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", CursedAnalytics.UserAgent);
        }

        public static void Shutdown() {
            HttpClient?.Dispose();
        }

        public static async Task<List<JObject>> GetAddonsAsync(params long[] ids) {
            using var request = new HttpRequestMessage(HttpMethod.Post, "https://addons-ecs.forgesvc.net/api/v2/addon") {
                Content = new StringContent(JsonConvert.SerializeObject(ids), Encoding.UTF8, "application/json")
            };
            using var response = await WebManager.HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            await using var stream = await response.Content.ReadAsStreamAsync();
            using var streamReader = new StreamReader(stream);
            using var jsonTextReader = new JsonTextReader(streamReader);
            return Toolbox.JsonSerializer.Deserialize<List<JObject>>(jsonTextReader);
        }
    }
}