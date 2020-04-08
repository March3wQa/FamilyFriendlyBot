using Discord;
using Discord.Commands;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FamilyFriendlyBot.Modules
{
    [Group("nk")]
    [Summary("Grupa komend 'niech ktoś'")]
    [RequireNsfw]
    public class NkModule : ModuleBase<SocketCommandContext>
    {
        private readonly Uri website = new Uri("https://pornhub.com/");
        private readonly Random _rand;

        public NkModule(Random random)
        {
            _rand = random;
        }

        [Command("video")]
        [Summary("Daje losowy filmik ze strony Pornhub")]
        [Alias("filmik")]
        public async Task VideoAsync()
        {
            var browser = new ScrapingBrowser();

            WebPage page = browser.NavigateToPage(website);

            var nodes = page.Html.CssSelect("a.linkVideoThumb");

            List<PornVideo> videos = new List<PornVideo>();

            foreach (var video in nodes)
            {
                string title = WebUtility.HtmlDecode(video.GetAttributeValue("title"));
                Uri fullVideoUrl = new Uri(website, video.GetAttributeValue("href"));
                var imgNode = video.ChildNodes.ToArray()[1];
                Uri imageUri = new Uri(imgNode.GetAttributeValue("data-thumb_url"));
                videos.Add(new PornVideo
                {
                    Title = title,
                    VideoUri = fullVideoUrl,
                    ThumbnailUri = imageUri
                });
            }

            int itemNum = _rand.Next(0, videos.Count - 1);

            var builder = new EmbedBuilder()
            {
                Color = new Color(0xFFA31A),
                Title = "Filmik dla ciebie",
                Description = videos[itemNum].Title
            };
            builder.WithImageUrl(videos[itemNum].ThumbnailUri.ToString());
            builder.AddField(new EmbedFieldBuilder()
            {
                Name = $"Link: {videos[itemNum].VideoUri}",
                Value = $"Obejrzyj go [TUTAJ]({videos[itemNum].VideoUri})"
            });
            builder.WithCurrentTimestamp();

            await ReplyAsync("", false, builder.Build());
        }

        [Command("video")]
        [Summary("Daje losowy filmik pasujący do wyszukiwania ze strony Pornhub")]
        [Alias("filmik")]
        public async Task VideoAsync(
            [Summary("Rzeczy do wyszukania")]string query,
            [Summary("Liczba stron do przeszukania")]int pages = 1)
        {
            if (pages > 10 || pages < 1)
                await ReplyAsync($"Argument **pages** nie może być równy {pages}, musi się zawierać między 1 a 10 włącznie.");

            var browser = new ScrapingBrowser();

            query = query.Replace(' ', '+');

            List<PornVideo> videos = new List<PornVideo>();

            for (int i = 1; i <= pages; i++)
            {
                Uri searchUri = new Uri(website, $"/video/search?search={query}&page={i}");

                WebPage page = browser.NavigateToPage(searchUri);

                var nodes = page.Html.CssSelect("a.linkVideoThumb");

                foreach (var video in nodes)
                {
                    string title = WebUtility.HtmlDecode(video.GetAttributeValue("title"));
                    Uri fullVideoUrl = new Uri(website, video.GetAttributeValue("href"));
                    var imgNode = video.ChildNodes.ToArray()[1];
                    Uri imageUri = new Uri(imgNode.GetAttributeValue("data-thumb_url"));
                    videos.Add(new PornVideo
                    {
                        Title = title,
                        VideoUri = fullVideoUrl,
                        ThumbnailUri = imageUri
                    });
                }
            }

            int itemNum = _rand.Next(0, videos.Count - 1);

            var builder = new EmbedBuilder()
            {
                Color = new Color(0xFFA31A),
                Title = "Filmik dla ciebie",
                Description = videos[itemNum].Title
            };
            builder.WithImageUrl(videos[itemNum].ThumbnailUri.ToString());
            builder.AddField(new EmbedFieldBuilder()
            {
                Name = $"Link: {videos[itemNum].VideoUri}",
                Value = $"Obejrzyj go [TUTAJ]({videos[itemNum].VideoUri})"
            });
            builder.WithCurrentTimestamp();

            await ReplyAsync("", false, builder.Build());
        }
    }

    internal class PornVideo
    {
        public string Title { get; set; }
        public Uri VideoUri { get; set; }
        public Uri ThumbnailUri { get; set; }
    }
}
