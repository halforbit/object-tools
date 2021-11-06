using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Halforbit.ObjectTools.ObjectStringMap.Implementation
{
    class ParseInfo
    {
        public const string NameGroupKey = "Name";

        public const string FormatGroupKey = "Format";

        public static readonly Regex NodePattern = new Regex(
            @"\{(?<Name>[^}]+?)(:(?<Format>[^}]+)){0,1}}",
            RegexOptions.None);// RegexOptions.Compiled);

        public ParseInfo(
            Dictionary<string, string> formats,
            Regex regex)
        {
            Formats = formats;

            Regex = regex;
        }

        public Dictionary<string, string> Formats { get; }

        public Regex Regex { get; }

        public static ParseInfo ResolveParseInfo(string source)
        {
            ValidateSource(source);

            var pattern = new StringBuilder();

            var nodeMatches = NodePattern.Matches(source).Cast<Match>();

            var index = 0;

            var formats = new Dictionary<string, string>();

            foreach (var nodeMatch in nodeMatches)
            {
                if (nodeMatch.Index > index)
                {
                    pattern.Append(Regex.Escape(source.Substring(index, nodeMatch.Index - index)));
                }

                var nodeName = nodeMatch.Groups[NameGroupKey].Value;

                var nodeFormat = nodeMatch.Groups[FormatGroupKey].Value;

                if (!string.IsNullOrWhiteSpace(nodeFormat))
                {
                    formats.Add(nodeName, nodeFormat);
                }

                var slashCount = string.IsNullOrWhiteSpace(nodeFormat) ?
                    0 :
                    nodeFormat.Count(ch => ch == '/');

                var nodePattern = ResolvePattern(nodeName, slashCount);

                pattern.Append(nodePattern);

                index = nodeMatch.Index + nodeMatch.Length;
            }

            if (index < source.Length)
            {
                pattern.Append(Regex.Escape(source.Substring(index)));
            }

            var regex = new Regex(
                $"^{pattern}$",
                RegexOptions.Compiled);

            return new ParseInfo(formats, regex);
        }

        static void ValidateSource(string source)
        {
            int nesting = 0;

            for (var i = 0; i < source.Length; i++)
            {
                var c = source[i];

                switch (c)
                {
                    case '{': 
                        
                        nesting++; 
                        
                        if (nesting > 1)
                        {
                            throw new ArgumentException($"Unexpected extra `{{` in map `{source}`.");
                        }

                        break;

                    case '}': 
                        
                        nesting--;

                        if (nesting < 0)
                        {
                            throw new ArgumentException($"Unexpected `}}` at position {i} of map `{source}`.");
                        }

                        break;
                }
            }

            if (nesting > 0)
            {
                throw new ArgumentException($"Opening `{{` is missing a closing `}}` in map `{source}`.");
            }
        }

        static string ResolvePattern(string name, int slashCount)
        {
            if (name.StartsWith("*"))
            {
                return $"(?<{name.Substring(1)}>.*)";
            }

            if (slashCount == 0)
            {
                return $"(?<{name}>[^/]*)";
            }

            var segments = string.Join(
                "/",
                Enumerable
                    .Range(0, slashCount + 1)
                    .Select(i => "[^/]*"));

            return $"(?<{name}>{segments})";
        }
    }
}
