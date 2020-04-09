using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FamilyFriendlyBot.Modules
{
    [Summary("Moduł moderacji")]
    [RequireUserPermission(Discord.GuildPermission.Administrator, Group = "Permission")]
    [RequireOwner(Group = "Permission")]
    [RequireUserPermission(Discord.GuildPermission.ManageChannels, Group = "Permission")]
    public class ModerationModule : ModuleBase<SocketCommandContext>
    {
        private readonly Utilities _utilities;

        public ModerationModule(Utilities utilities)
        {
            _utilities = utilities;
        }

        [Command("prefix")]
        [Summary("Ustawia nowy prefix")]
        [Alias()]
        public async Task PrefixAsync([Summary("Nowy prefix")]string prefix)
        {
            string guildId = Context.Guild.Id.ToString();

            await _utilities.SetPrefixes(guildId, prefix);

            await ReplyAsync($"Nowy prefix: **{prefix}**");
        }
    }
}
