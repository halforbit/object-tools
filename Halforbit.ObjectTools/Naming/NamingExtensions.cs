using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Halforbit.ObjectTools
{
    public static class NamingExtensions
    {
        static Regex _wordMatcher = new Regex(
            @"([A-Z]+(?![a-z]))|([0-9]+)|([A-Z][a-z]+)|([a-z]+)|\.", 
            RegexOptions.Compiled);

        public static string TrainToPascalCase(
            this string source, 
            bool retainDots = true)
        {
            if (string.IsNullOrWhiteSpace(source)) return source;

            return string.Join(
                string.Empty,
                GetWords(source, retainDots)
                    .Select(w => $"{char.ToUpper(w[0])}{(w.Length > 1 ? w.Substring(1).ToLower() : string.Empty)}"));
        }

        public static string PascalToTrainCase(
            this string source,
            bool retainDots = true)
        {
            if (string.IsNullOrWhiteSpace(source)) return source;

            var sb = new StringBuilder();

            var first = true;

            var prevWord = default(string);

            foreach(var word in GetWords(source, retainDots))
            {
                if (first)
                {
                    first = false;
                }
                else if(word != "." && prevWord != ".")
                {
                    sb.Append("-");
                }

                sb.Append(word.ToLower());

                prevWord = word;
            }

            return sb.ToString();
        }

        public static string TrainToCamelCase(
            this string source,
            bool retainDots = true)
        {
            if (string.IsNullOrWhiteSpace(source)) return source;

            var sb = new StringBuilder();

            var first = true;

            foreach (var word in GetWords(source, retainDots))
            {
                if (first)
                {
                    sb.Append(word.ToLower());

                    first = false;
                }
                else if (word == ".")
                {
                    sb.Append(word);

                    first = true;
                }
                else
                {
                    sb.Append($"{char.ToUpper(word[0])}{(word.Length > 1 ? word.Substring(1).ToLower() : string.Empty)}");
                }
            }

            return sb.ToString();
        }

        public static string CamelToTrainCase(
            this string source,
            bool retainDots = true)
        {
            if (string.IsNullOrWhiteSpace(source)) return source;

            var sb = new StringBuilder();

            var first = true;

            var prevWord = default(string);

            foreach (var word in GetWords(source, retainDots))
            {
                if (first)
                {
                    first = false;
                }
                else if (word != "." && prevWord != ".")
                {
                    sb.Append("-");
                }

                sb.Append(word.ToLower());

                prevWord = word;
            }

            return sb.ToString();
        }

        public static string PascalToCamelCase(
            this string source,
            bool retainDots = true)
        {
            if (string.IsNullOrWhiteSpace(source)) return source;

            var sb = new StringBuilder();

            var first = true;

            foreach (var word in GetWords(source, retainDots))
            {
                if (first)
                {
                    sb.Append(word.ToLower());

                    first = false;
                }
                else if (word == ".")
                {
                    sb.Append(word);

                    first = true;
                }
                else
                {
                    sb.Append($"{char.ToUpper(word[0])}{(word.Length > 1 ? word.Substring(1).ToLower() : string.Empty)}");
                }
            }

            return sb.ToString();
        }

        public static string CamelToPascalCase(
            this string source,
            bool retainDots = true)
        {
            if (string.IsNullOrWhiteSpace(source)) return source;
            
            var sb = new StringBuilder();

            var first = true;

            return string.Join(
                string.Empty, 
                GetWords(source, retainDots)
                    .Select(w => $"{char.ToUpper(w[0])}{(w.Length > 1 ? w.Substring(1).ToLower() : string.Empty)}"));
        }

        static IEnumerable<string> GetWords(
            string source, 
            bool retainDots)
        {
            var matches = _wordMatcher
                .Matches(source)
                .Cast<Match>()
                .Select(m => m.Value)
                .Where(w => retainDots || w != ".")
                .ToList();

            return matches;
        }
    }
}
