using Discord;
using Discord.Commands;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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
        [Alias("filmik", "link")]
        public async Task VideoAsync()
        {
            using (Context.Channel.EnterTypingState())
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
                    Uri imageUri;
                    try
                    {
                        imageUri = new Uri(imgNode.GetAttributeValue("data-thumb_url"));
                    }
                    catch (Exception)
                    {
                        imageUri = new Uri("https://i2.wp.com/www.scribblesandcrumbs.com/wp-content/plugins/penci-portfolio//images/no-thumbnail.jpg");
                    }
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
                builder.WithImageUrl(videos[itemNum].ThumbnailUri.AbsoluteUri);
                builder.AddField(new EmbedFieldBuilder()
                {
                    Name = $"Link: {videos[itemNum].VideoUri}",
                    Value = $"Obejrzyj go [TUTAJ]({videos[itemNum].VideoUri})"
                });
                builder.WithCurrentTimestamp();

                await ReplyAsync("", false, builder.Build());
            }
        }

        [Command("video")]
        [Summary("Daje losowy filmik pasujący do wyszukiwania ze strony Pornhub")]
        [Alias("filmik", "link")]
        public async Task VideoAsync(
            [Summary("Słowa kluczowe")][Remainder]string query)
        {
            using (Context.Channel.EnterTypingState())
            {
                var browser = new ScrapingBrowser();

                query = query.Replace(' ', '+');

                Uri searchUri = new Uri(website, $"/video/search?search={query}");

                WebPage page = browser.NavigateToPage(searchUri);

                var nodes = page.Html.CssSelect("#videoSearchResult").First();

                var vidNode = nodes.ChildNodes[3].FirstChild.NextSibling.FirstChild.NextSibling.ChildNodes[3].FirstChild.NextSibling;

                string title = WebUtility.HtmlDecode(vidNode.GetAttributeValue("title"));
                Uri fullVideoUrl = new Uri(website, vidNode.GetAttributeValue("href"));
                var imgNode = vidNode.ChildNodes.ToArray()[1];
                Uri imageUri;
                try
                {
                    imageUri = new Uri(imgNode.GetAttributeValue("data-thumb_url"));
                }
                catch (Exception)
                {
                    imageUri = new Uri("https://i2.wp.com/www.scribblesandcrumbs.com/wp-content/plugins/penci-portfolio//images/no-thumbnail.jpg");
                }
                var outVid = new PornVideo
                {
                    Title = title,
                    VideoUri = fullVideoUrl,
                    ThumbnailUri = imageUri
                };

                var builder = new EmbedBuilder()
                {
                    Color = new Color(0xFFA31A),
                    Title = "Tego szukałeś/aś?",
                    Description = outVid.Title
                };
                builder.WithImageUrl(outVid.ThumbnailUri.AbsoluteUri);
                builder.AddField(new EmbedFieldBuilder()
                {
                    Name = $"Link: {outVid.VideoUri}",
                    Value = $"Obejrzyj go [TUTAJ]({outVid.VideoUri})"
                })
                    .WithFooter(new EmbedFooterBuilder()
                    {
                        Text = "Jeśli nie tego szukałeś/aś, spróbuj wprowadzić inne zapytanie"
                    });

                builder.WithCurrentTimestamp();

                await ReplyAsync("", false, builder.Build());
            }
        }

        [Command("photo")]
        [Summary("Wysyła losowe zdjęcie ze strony Pornhub")]
        [Alias("zdjęcie", "img")]
        public async Task PhotoAsync()
        {
            using (Context.Channel.EnterTypingState())
            {
                var browser = new ScrapingBrowser();

                Uri albumsWebsite = new Uri(website, "/albums/female-misc-straight-transgender-uncategorized");

                WebPage page = browser.NavigateToPage(albumsWebsite);

                var albumNodes = page.Html.CssSelect("div.photoAlbumListBlock");

                List<PornImageAlbum> albums = new List<PornImageAlbum>(albumNodes.Count());

                foreach (var albumNode in albumNodes)
                {
                    string title = WebUtility.HtmlDecode(albumNode.GetAttributeValue("title"));
                    Uri pathUri = new Uri(website, albumNode.ChildNodes.First(x => x.Name == "a").GetAttributeValue("href"));
                    albums.Add(new PornImageAlbum
                    {
                        Name = title,
                        PathUri = pathUri
                    });
                }

                int albumNum = _rand.Next(0, albums.Count - 1);

                page = browser.NavigateToPage(albums[albumNum].PathUri);

                var imageNodes = page.Html.CssSelect("div.photoAlbumListBlock");

                List<Uri> imageUris = new List<Uri>(imageNodes.Count());

                imageNodes.ToList().ForEach(x => imageUris.Add(new Uri(x.GetAttributeValue("data-bkg"))));

                int imageNum = _rand.Next(0, imageUris.Count - 1);

                var builder = new EmbedBuilder()
                {
                    Color = new Color(0xFFA31A),
                    Title = "Zdjęcie dla ciebie",
                    Description = $"{albums[albumNum].Name} - zdjęcie nr {imageNum + 1}"
                };
                builder.WithImageUrl(imageUris[imageNum].AbsoluteUri);
                builder.AddField(new EmbedFieldBuilder()
                {
                    Name = $"Link: {albums[albumNum].PathUri}",
                    Value = $"Zobacz album [TUTAJ]({albums[albumNum].PathUri})"
                });
                builder.WithCurrentTimestamp();

                await ReplyAsync("", false, builder.Build());
            }
        }
    }

    internal class PornVideo
    {
        public string Title { get; set; }
        public Uri VideoUri { get; set; }
        public Uri ThumbnailUri { get; set; }
    }

    internal class PornImageAlbum
    {
        public string Name { get; set; }
        public Uri PathUri { get; set; }
    }
}
