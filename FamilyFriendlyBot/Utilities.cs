using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FamilyFriendlyBot
{
    public class Utilities
    {
        private readonly HttpClient _client;
        private readonly string GitHubApiToken = Environment.GetEnvironmentVariable("GITHUB_API_TOKEN");
        private Dictionary<string, string> Prefixes = new Dictionary<string, string>();

        public Utilities(HttpClient client)
        {
            _client = client;
        }

        public IEnumerable<string> Split(string str, int maxChunkSize)
        {
            for (int i = 0; i < str.Length; i += maxChunkSize)
            {
                yield return str.Substring(i, Math.Min(maxChunkSize, str.Length - i));
            }
        }

        public IEnumerable<string> Split(StringBuilder builder, int maxChunkSize)
        {
            for (int i = 0; i < builder.Length; i += maxChunkSize)
            {
                yield return builder.ToString(i, Math.Min(maxChunkSize, builder.Length - i));
            }
        }

        public async Task RefreshPrefixes()
        {
            var content = await _client.GetStringAsync("https://gist.githubusercontent.com/March3wQa/16e4be2a48f911cd592a626d4bd28a3d/raw/88cec452f4656e556f57330486d733db367ae669/prefixes.json");
            Prefixes = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
        }

        public async Task<string> GetPrefixes(string key)
        {
            Prefixes.TryGetValue(key, out string prefix);

            if (String.IsNullOrEmpty(prefix))
            {
                var content = await _client.GetStringAsync("https://gist.githubusercontent.com/March3wQa/16e4be2a48f911cd592a626d4bd28a3d/raw/88cec452f4656e556f57330486d733db367ae669/prefixes.json");
                Prefixes = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
                Prefixes.TryGetValue(key, out prefix);
                if (String.IsNullOrEmpty(prefix))
                {
                    Prefixes[key] = "-";
                    return "-";
                }
                return prefix;
            }
            return prefix;
        }

        public async Task SetPrefixes(string key, string value)
        {
            Prefixes[key] = value;

            string prefixesEscaped = "{";

            foreach (var pair in Prefixes)
            {
                prefixesEscaped += "\\\"" + pair.Key + "\\\"" + ":" + "\\\"" + pair.Value + "\\\"" + ",";
            }

            prefixesEscaped = prefixesEscaped.Remove(prefixesEscaped.Length - 1);
            prefixesEscaped += "}";

            var json = $"{{\"description\":\"Servers bot prefixes\",\"files\":{{\"bit_prefixes.json\":{{\"content\":\"{prefixesEscaped}\",\"filename\":\"prefixes.json\"}}}}}}";
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Patch, "https://api.github.com/gists/16e4be2a48f911cd592a626d4bd28a3d")
            {
                Content = data
            };
            requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("token", $"{GitHubApiToken}");
            requestMessage.Headers.UserAgent.ParseAdd("March3wQa/FamilyFriendlyBot");

            (await _client.SendAsync(requestMessage)).EnsureSuccessStatusCode();
        }
    }
}
