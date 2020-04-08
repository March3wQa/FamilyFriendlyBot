using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FamilyFriendlyBot
{
    public static class Utilities
    {
        public static IEnumerable<string> Split(string str, int maxChunkSize)
        {
            for (int i = 0; i < str.Length; i += maxChunkSize)
            {
                yield return str.Substring(i, Math.Min(maxChunkSize, str.Length - i));
            }
        }

        public static IEnumerable<string> Split(StringBuilder builder, int maxChunkSize)
        {
            for (int i = 0; i < builder.Length; i += maxChunkSize)
            {
                yield return builder.ToString(i, Math.Min(maxChunkSize, builder.Length - i));
            }
        }
    }
}
