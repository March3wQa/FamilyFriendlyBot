using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyFriendlyBot.Modules
{
    [Summary("Pomoc")]
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _service;

        public HelpModule(CommandService service)
        {
            _service = service;
        }

        [Command("help")]
        [Summary("Pokazuje dostępne komendy")]
        [Alias("commands", "komendy")]
        public async Task HelpAsync()
        {
            string prefix = "-";
            var builder = new EmbedBuilder()
            {
                Color = Color.Green,
                Title = "POMOC",
                Description = $"Więcej informacji o komendzie: __{prefix}help **komenda**__"
            };

            foreach (var module in _service.Modules)
            {
                string description = "";
                foreach (var cmd in module.Commands)
                {
                    description += $"{prefix}{cmd.Aliases.First()}";
                    foreach (var param in cmd.Parameters)
                    {
                        string paramDesc = "";
                        if (param.IsOptional)
                            paramDesc += "[";
                        else
                            paramDesc += "{";
                        if (param.IsMultiple)
                            paramDesc += "*";
                        if (param.IsRemainder)
                            paramDesc += "^";
                        paramDesc += param.Name;
                        if (param.DefaultValue != null)
                            paramDesc += $"={param.DefaultValue}";
                        if (param.IsOptional)
                            paramDesc += "]";
                        else
                            paramDesc += "}";
                        description += " " + paramDesc;
                    }
                    description += "\n";
                }

                if (!string.IsNullOrWhiteSpace(description))
                {
                    builder.AddField(x =>
                    {
                        x.Name = module.Summary;
                        x.Value = description;
                        x.IsInline = false;
                    });
                }

                var footerBuilder = new EmbedFooterBuilder()
                    .WithText("*-Kilka wartości\n^-Bez zakończenia\n[OPCJONALNE]\n{WYMAGANE}");

                builder.WithFooter(footerBuilder);
                builder.WithCurrentTimestamp();
            }

            await ReplyAsync("", false, builder.Build());
        }

        [Command("help")]
        [Summary("Detale o komendzie")]
        [Alias("komenda", "command")]
        public async Task HelpAsync([Summary("Komenda o której chcesz wiedzieć więcej")][Remainder]string command)
        {
            var result = _service.Search(Context, command);

            if (!result.IsSuccess)
            {
                await ReplyAsync($"Sorry, nie mam żadnej komendy **{command}**.");
                return;
            }
            var builder = new EmbedBuilder()
            {
                Color = Color.Green,
                Description = $"Tutaj masz komendy pasujące do **{command}**"
            };

            foreach (var match in result.Commands)
            {
                var cmd = match.Command;

                string paramString = "";

                foreach (var param in cmd.Parameters)
                {
                    string paramDesc = "\t";
                    if (param.IsOptional)
                        paramDesc += "[";
                    else
                        paramDesc += "{";
                    if (param.IsMultiple)
                        paramDesc += "*";
                    if (param.IsRemainder)
                        paramDesc += "^";
                    paramDesc += param.Name;
                    if (param.DefaultValue != null)
                        paramDesc += $"={param.DefaultValue}";
                    if (param.IsOptional)
                        paramDesc += "]";
                    else
                        paramDesc += "}";
                    paramDesc += $" - {param.Summary ?? "Brak opisu"}";
                    paramString += paramDesc + "\n";
                }

                string fullDesc = $"**Opis**: {cmd.Summary}";

                if (paramString == "")
                {
                    fullDesc += "\n**Brak argumentów**";
                }
                else
                {
                    fullDesc += $"\n**Argumenty**:\n" + paramString;
                }

                string names = "";

                foreach (var name in cmd.Aliases)
                {
                    names += $"{name}/";
                }

                names = names.Remove(names.Length - 1);

                builder.AddField(x =>
                {
                    x.Name = names;
                    x.Value = fullDesc;
                    x.IsInline = false;
                });
            }

            var footerBuilder = new EmbedFooterBuilder()
                    .WithText("*-Kilka wartości\n^-Bez zakończenia\n[OPCJONALNE]\n{WYMAGANE}");

            builder.WithFooter(footerBuilder);
            builder.WithCurrentTimestamp();

            await ReplyAsync("", false, builder.Build());
        }
    }
}
