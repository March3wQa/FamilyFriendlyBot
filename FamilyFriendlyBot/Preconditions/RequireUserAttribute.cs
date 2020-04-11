using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FamilyFriendlyBot.Preconditions
{
    internal class RequireUserAttribute : PreconditionAttribute
    {
        private readonly ulong _userId;

        public RequireUserAttribute(ulong userId) => _userId = userId;

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (context.User is SocketGuildUser gUser)
            {
                if (gUser.Id == _userId)
                    return Task.FromResult(PreconditionResult.FromSuccess());
                else
                    return Task.FromResult(PreconditionResult.FromError($"Nie możesz korzystać z tej komendy"));
            }
            else
                return Task.FromResult(PreconditionResult.FromError("Musisz być na serwerze aby korzystać z tej komendy"));
        }
    }
}
