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
        const string NameGroupKey = "Name";

        const string FormatGroupKey = "Format";

        const string ThisKeyword = "this";

        static readonly Regex NodePattern = new Regex(
            @"\{(?<Name>[^}]+?)(:(?<Format>[^}]+)){0,1}}",
            RegexOptions.None);// RegexOptions.Compiled);

        readonly Lazy<ParseInfo> _parseInfo;

        public StringMap(string source)
        {
            Source = source;

            _parseInfo = new Lazy<ParseInfo>(() => ResolveParseInfo(source));
        }

        public static implicit operator StringMap<TObject>(string source)
        {
            return new StringMap<TObject>(source);
        }

        public static implicit operator string(StringMap<TObject> stringMap)
        {
            return stringMap?.Source;
        }

        public string Source { get; }

        public override string ToString()
        {
            return Source;
        }

        public TObject Map(string str)
        {
            var parseInfo = _parseInfo.Value;

            var match = parseInfo.Regex.Match(str);

            if (!match.Success)
            {
                return default(TObject);
            }

            var thisGroup = match.Groups[ThisKeyword];

            var format = default(string);

            var obj = default(TObject);

            if (thisGroup.Success)
            {
                parseInfo.Formats.TryGetValue(ThisKeyword, out format);

                var typedValue = ConvertStringToType(
                    typeof(TObject),
                    thisGroup.Value,
                    format);

                if (typedValue == null)
                {
                    return default(TObject);
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

                    var value = match.Groups[name].Value;

                    format = parseInfo.Formats.TryGetValue(name, out format) ? format : null;

                    var typedValue = ConvertStringToType(
                        property?.PropertyType ?? field?.FieldType,
                        value,
                        format);

                    if (typedValue == null)
                    {
                        return default(TObject);
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
                (name, format) => ResolveValue(obj, name, format),
                allowPartialMap);
        }

        public string Map(
            IReadOnlyDictionary<string, object> memberValues,
            bool allowPartialMap = false)
        {
            return Map(
                (name, format) => ResolveValue(memberValues, name, format),
                allowPartialMap);
        }

        string Map(
            Func<string, string, string> resolveValue,
            bool allowPartialMap)
        {
            var output = new StringBuilder();

            var nodeMatches = NodePattern.Matches(Source).Cast<Match>();

            var index = 0;

            foreach (var nodeMatch in nodeMatches)
            {
                if (nodeMatch.Index > index)
                {
                    output.Append(Source.Substring(index, nodeMatch.Index - index));
                }

                var name = nodeMatch.Groups[NameGroupKey].Value;

                if (name.StartsWith("*"))
                {
                    name = name.Substring(1);
                }

                var format = nodeMatch.Groups[FormatGroupKey].Value;

                var value = resolveValue(name, format);

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

        public bool IsMatch(string str)
        {
            return _parseInfo.Value.Regex.IsMatch(str);
        }

        public Regex Regex => _parseInfo.Value.Regex;

        static string ResolveValue(
            IReadOnlyDictionary<string, object> objMembers,
            string name,
            string format)
        {
            var key = objMembers.Keys
                .FirstOrDefault(k => string.Equals(k, name, StringComparison.InvariantCultureIgnoreCase));

            if (key == null)
            {
                return null;
            }

            return FormatValue(name, format, objMembers[key]);
        }

        static string ResolveValue(
            TObject obj,
            string name,
            string format)
        {
            var value = default(object);

            if (name == ThisKeyword)
            {
                value = obj;
            }
            else
            {
                var typeInfo = typeof(TObject).GetTypeInfo();

                value = typeInfo
                    .GetProperties()
                    .SingleOrDefault(p => p.Name == name)?
                    .GetValue(obj) ?? typeInfo
                        .GetFields()
                        .SingleOrDefault(p => p.Name == name)
                        .GetValue(obj);
            }

            return FormatValue(name, format, value);
        }

        static string FormatValue(
            string name,
            string format,
            object value)
        {
            if (value == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(format))
            {
                var formattable = value as IFormattable;

                if (formattable == null)
                {
                    throw new ArgumentException($"{name} has a format but is not IFormattable.");
                }

                return formattable.ToString(format, null);
            }

            if (value is Guid)
            {
                return ((Guid)value).ToString("N");
            }

            return value.ToString();
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

        static object ConvertStringToType(
            Type type,
            string stringValue,
            string format)
        {
            var typeInfo = type.GetTypeInfo();

            try
            {
                if (type.Equals(typeof(Guid)) || type.Equals(typeof(Guid?)))
                {
                    return Guid.Parse(stringValue);
                }
                else if ((type.Equals(typeof(DateTime)) || type.Equals(typeof(DateTime?))) &&
                    !string.IsNullOrWhiteSpace(format))
                {
                    return DateTime.ParseExact(stringValue, format, null);
                }
                else
                {
                    if (typeInfo.IsGenericType &&
                        type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                    {
                        type = typeInfo.GenericTypeArguments.Single();
                    }

                    return Convert.ChangeType(stringValue, type);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        static ParseInfo ResolveParseInfo(string source)
        {
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
                RegexOptions.None);// RegexOptions.Compiled);

            return new ParseInfo(formats, regex);
        }

        class ParseInfo
        {
            public ParseInfo(
                Dictionary<string, string> formats,
                Regex regex)
            {
                Formats = formats;

                Regex = regex;
            }

            public Dictionary<string, string> Formats { get; }

            public Regex Regex { get; }
        }
    }
}
