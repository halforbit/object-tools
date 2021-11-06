using Halforbit.ObjectTools.ObjectBuild.Implementation;
using Halforbit.ObjectTools.ObjectStringMap.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Halforbit.ObjectTools.ObjectStringMap.Implementation
{
    public class StringMap<TObject> :
        IStringMap<TObject>
    {
        readonly Lazy<ParseInfo> _parseInfo;

        public StringMap(string source)
        {
            Source = source;

            _parseInfo = new Lazy<ParseInfo>(() => ParseInfo.ResolveParseInfo(source));
        }

        public string Source { get; }

        public Regex Regex => _parseInfo.Value.Regex;

        public static implicit operator StringMap<TObject>(string source) => new StringMap<TObject>(source);

        public static implicit operator string(StringMap<TObject> stringMap) => stringMap?.Source;

        public override string ToString() => Source;

        public bool IsMatch(string str) => _parseInfo.Value.Regex.IsMatch(str);

        public TObject Map(string str)
        {
            var parseInfo = _parseInfo.Value;

            var match = parseInfo.Regex.Match(str);

            if (!match.Success) return default;

            var thisGroup = match.Groups[ObjectToStringConverter.ThisKeyword];

            string format;

            TObject obj;

            if (thisGroup.Success)
            {
                parseInfo.Formats.TryGetValue(ObjectToStringConverter.ThisKeyword, out format);

                var typedValue = StringToObjectConverter.ConvertToObject(
                    typeof(TObject),
                    thisGroup.Value,
                    format);

                if (typedValue == null)
                {
                    return default;
                }

                obj = (TObject)typedValue;
            }
            else
            {
                var builder = new Builder<TObject>();

                var typeInfo = typeof(TObject).GetTypeInfo();

                foreach (var name in parseInfo.Regex.GetGroupNames())
                {
                    if (int.TryParse(name, out var i)) continue;

                    var property = typeInfo.GetProperty(name);

                    var field = typeInfo.GetField(name);

                    if (property == null && field == null)
                    {
                        throw new ArgumentException(
                            $"Cannot map string to type `{typeInfo.Name}` " +
                            $"because it does not have a property or field named `{name}` " +
                            $"which is referenced by the map `{Source}`.");
                    }

                    var value = match.Groups[name].Value;

                    format = parseInfo.Formats.TryGetValue(name, out format) ? format : null;

                    var typedValue = StringToObjectConverter.ConvertToObject(
                        property?.PropertyType ?? field?.FieldType,
                        value,
                        format);

                    if (typedValue == null)
                    {
                        return default;
                    }

                    builder.Set(
                        property?.Name ?? field?.Name,
                        typedValue);
                }

                obj = builder.Build();
            }

            return obj;
        }

        public string Map(
            TObject obj,
            bool allowPartialMap = false)
        {
            return Map(
                (name, format) => ObjectToStringConverter.ResolveStringFromProperty(obj, name, format),
                allowPartialMap);
        }

        public string Map(
            IReadOnlyDictionary<string, object> memberValues,
            bool allowPartialMap = false)
        {
            return Map(
                (name, format) => ObjectToStringConverter.ResolveStringFromKeyValue(memberValues, name, format),
                allowPartialMap);
        }

        string Map(
            Func<string, string, string> resolveMemberString,
            bool allowPartialMap)
        {
            var output = new StringBuilder();

            var nodeMatches = ParseInfo.NodePattern.Matches(Source).Cast<Match>();

            var index = 0;

            foreach (var nodeMatch in nodeMatches)
            {
                if (nodeMatch.Index > index)
                {
                    output.Append(Source.Substring(index, nodeMatch.Index - index));
                }

                var name = nodeMatch.Groups[ParseInfo.NameGroupKey].Value;

                if (name.StartsWith("*"))
                {
                    name = name.Substring(1);
                }

                var format = nodeMatch.Groups[ParseInfo.FormatGroupKey].Value;

                var value = resolveMemberString(name, format);

                if (value == null)
                {
                    if (allowPartialMap)
                    {
                        return output.ToString();
                    }
                    else
                    {
                        throw new ArgumentNullException(
                            name,
                            $"A required value was null while mapping {typeof(TObject).Name} to a string.");
                    }
                }

                output.Append(value);

                index = nodeMatch.Index + nodeMatch.Length;
            }

            if (index < Source.Length)
            {
                output.Append(Source.Substring(index));
            }

            return output.ToString();
        }
    }
}