using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FamilyFriendlyBot.TypeReaders
{
    internal class ObjectTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            object result = input;
            return Task.FromResult(TypeReaderResult.FromSuccess(result));
        }
    }
}
