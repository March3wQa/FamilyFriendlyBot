using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using FamilyFriendlyBot.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace FamilyFriendlyBot
{
    public class Startup
    {
        public static async Task RunAsync()
        {
            var services = new ServiceCollection();             // Create a new instance of a service collection
            ConfigureServices(services);

            var provider = services.BuildServiceProvider();     // Build the service provider
            provider.GetRequiredService<LoggingService>();      // Start the logging service
            provider.GetRequiredService<CommandHandler>(); 		// Start the command handler service

            await provider.GetRequiredService<StartupService>().StartAsync();       // Start the startup service
            await Task.Delay(-1);                               // Keep the program alive
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
            {                                       // Add discord to the collection
                LogLevel = LogSeverity.Verbose,
                MessageCacheSize = 1000             // Cache 1,000 messages per channel
            }))
            .AddSingleton(new CommandService(new CommandServiceConfig
            {                                       // Add the command service to the collection
                LogLevel = LogSeverity.Verbose,
                DefaultRunMode = RunMode.Async,     // Force all commands to run async by default
                CaseSensitiveCommands = false
            }))
            .AddSingleton<CommandHandler>()         // Add the command handler to the collection
            .AddSingleton<StartupService>()         // Add startupservice to the collection
            .AddSingleton<LoggingService>()         // Add loggingservice to the collection
            .AddSingleton<Random>()                 // Add random to the collection
            .AddSingleton<HttpClient>();
        }
    }
}
