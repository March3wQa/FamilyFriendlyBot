using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyFriendlyBot.Modules
{
    [Group("test")]
    [Summary("Moduł testowy")]
    public class TestModule : ModuleBase<SocketCommandContext>
    {
        [Command("echo")]
        [Summary("Powatrza to co chcesz")]
        [Alias("repeat", "say", "powtórz")]
        public async Task EchoAsync(
            [Summary("Co chcesz żeby bot powtórzył")]object input,
            [Summary("Ile razy ma powtórzyć")]int amount = 1,
            [Summary("`true` jeśli powtórzenia mają być oddzielone")]bool separated = true)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < amount; i++)
            {
                builder.Append(input);
                if (separated)
                    builder.Append(' ');
            }

            foreach (string msg in Utilities.Split(builder, 2000))
            {
                await ReplyAsync(msg);
            }
        }
    }
}
