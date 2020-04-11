using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FamilyFriendlyBot.Services
{
    public class StartupService
    {
        private readonly IServiceProvider _provider;
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly Utilities _utilities;

        // DiscordSocketClient, CommandService, and IConfigurationRoot are injected automatically
        // from the IServiceProvider
        public StartupService(
            IServiceProvider provider,
            DiscordSocketClient discord,
            CommandService commands,
            Utilities utilities)
        {
            _provider = provider;
            _discord = discord;
            _commands = commands;
            _utilities = utilities;
        }

        public async Task StartAsync()
        {
            await _utilities.DownloadPrefixes();

            string discordToken = Environment.GetEnvironmentVariable("DISCORD_TOKEN");     // Get the discord token from the config file
            if (string.IsNullOrWhiteSpace(discordToken))
                throw new Exception("No provided token.");

            await _discord.LoginAsync(TokenType.Bot, discordToken);     // Login to discord
            await _discord.StartAsync();                                // Connect to the websocket

            await _commands.AddModulesAsync(Assembly.GetExecutingAssembly(), _provider);     // Load commands and modules into the command service
        }
    }
}
