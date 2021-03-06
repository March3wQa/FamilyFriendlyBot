﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FamilyFriendlyBot
{
    public class Utilities
    {
        private readonly HttpClient _client;
        private readonly string _gitHubApiToken = Environment.GetEnvironmentVariable("GITHUB_API_TOKEN");
        private readonly Uri _gistUri = new Uri("https://api.github.com/gists/16e4be2a48f911cd592a626d4bd28a3d");
        private Dictionary<string, string> _prefixes = new Dictionary<string, string>();

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

        public async Task DownloadPrefixes()
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, _gistUri);
            requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("token", _gitHubApiToken);
            requestMessage.Headers.UserAgent.ParseAdd("March3wQa/FamilyFriendlyBot");

            var response = await _client.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            _prefixes = JsonConvert.DeserializeObject<Dictionary<string, string>>(Regex.Unescape(JsonConvert.DeserializeObject<GitHubGetObject>(content).Files["prefixes.json"].Content));
        }

        public async Task<string> GetPrefixes(string key)
        {
            _prefixes.TryGetValue(key, out string prefix);

            if (String.IsNullOrEmpty(prefix))
            {
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, _gistUri);
                requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("token", _gitHubApiToken);
                requestMessage.Headers.UserAgent.ParseAdd("March3wQa/FamilyFriendlyBot");

                var response = await _client.SendAsync(requestMessage);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                _prefixes = JsonConvert.DeserializeObject<Dictionary<string, string>>(Regex.Unescape(JsonConvert.DeserializeObject<GitHubGetObject>(content).Files["prefixes.json"].Content));
                _prefixes.TryGetValue(key, out prefix);
                if (String.IsNullOrEmpty(prefix))
                {
                    _prefixes[key] = "-";
                    return "-";
                }
                return prefix;
            }
            return prefix;
        }

        public async Task SetPrefixes(string key, string value)
        {
            if (!new Regex(@"[^a-zA-Z\d\s:]+").IsMatch(value[^1].ToString()))
                value += " ";

            _prefixes[key] = value;

            string prefixesJson = JsonConvert.SerializeObject(_prefixes, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            var fileObject = new GitHubPatchObject.FileObject
            {
                Content = prefixesJson
            };

            var patchObject = new GitHubPatchObject
            {
                Files = new Dictionary<string, GitHubPatchObject.FileObject>
                {
                    { "bot_prefixes.json", fileObject }
                }
            };

            var json = JsonConvert.SerializeObject(patchObject, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            var data = new StringContent(json, Encoding.UTF8, "application/json");

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Patch, "https://api.github.com/gists/16e4be2a48f911cd592a626d4bd28a3d")
            {
                Content = data
            };
            requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("token", _gitHubApiToken);
            requestMessage.Headers.UserAgent.ParseAdd("March3wQa/FamilyFriendlyBot");

            var response = await _client.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();
        }
    }

    internal struct GitHubPatchObject
    {
        internal struct FileObject
        {
            public string Content { get; set; }
            public string Filename { get => "prefixes.json"; }
        }

        public string Description { get => "Servers' bot prefixes"; }
        public IDictionary<string, FileObject> Files { get; set; }
    }

    internal struct GitHubGetObject
    {
        internal struct FileObject
        {
            public string Content { get; set; }
        }

        public IDictionary<string, FileObject> Files { get; set; }
    }
}
