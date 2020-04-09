using Discord.Commands;
using Discord.WebSocket;
using FamilyFriendlyBot.TypeReaders;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FamilyFriendlyBot.Services
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly IServiceProvider _provider;
        private readonly Utilities _utilities;

        // DiscordSocketClient, CommandService, IConfigurationRoot, and IServiceProvider are
        // injected automatically from the IServiceProvider
        public CommandHandler(
            DiscordSocketClient discord,
            CommandService commands,
            IServiceProvider provider,
            Utilities utilities)
        {
            _discord = discord;
            _commands = commands;
            _provider = provider;
            _utilities = utilities;

            _discord.MessageReceived += OnMessageReceivedAsync;

            _commands.AddTypeReader(typeof(object), new ObjectTypeReader());
        }

        private async Task OnMessageReceivedAsync(SocketMessage s)
        {
            // Ensure the message is from a user/bot
            if (!(s is SocketUserMessage msg)) return;
            if (msg.Author.Id == _discord.CurrentUser.Id) return;     // Ignore self when checking commands

            var context = new SocketCommandContext(_discord, msg);     // Create the command context

            var guildId = context.Guild.Id.ToString();

            string prefix = await _utilities.GetPrefixes(guildId);

            int argPos = 0;     // Check if the message has a valid command prefix
            if (msg.HasStringPrefix(prefix, ref argPos) || msg.HasMentionPrefix(_discord.CurrentUser, ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _provider);     // Execute the command

                if (!result.IsSuccess)
                {
                    switch (result.Error)
                    {
                        case CommandError.UnknownCommand:
                            break;

                        case CommandError.BadArgCount:
                            await context.Channel.SendMessageAsync("Zła ilość argumentów.");
                            break;

                        case CommandError.UnmetPrecondition:
                            await context.Channel.SendMessageAsync(result.ErrorReason);
                            break;

                        default:
                            await context.Channel.SendMessageAsync("Miałem problem z tą komendą. Sorka!");
                            break;
                    }
                }
            }
        }
    }
}
