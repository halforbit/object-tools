using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Halforbit.ObjectTools.Naming
{
    [JsonConverter(typeof(NameJsonConverter))]
    public class Name : IComparable<Name>
    {
        static Regex rxPascalCaseMatcher = new Regex(
            "((?<=[a-z0-9])(?<character>[A-Z]))|((?<![0-9.])(?<character>[0-9]))",
            RegexOptions.Compiled);

        protected readonly Type _nodeType;

        protected Name(Type nodeType)
        {
            _nodeType = nodeType;
        }

        protected static Type GetRootType(Type nodeType)
        {
            var parentType = nodeType;

            while (parentType.DeclaringType != null)
            {
                parentType = parentType.DeclaringType;
            }

            return parentType;
        }

        public int CompareTo(Name other)
        {
            return string.Compare(ToString(), other.ToString());
        }

        public override string ToString()
        {
            // 1. Get just the nested type name chain
            // 2. Replace + sign with .
            // 3. Add a hyphen at word boundaries
            // 4. Force to lower case

            return rxPascalCaseMatcher
                .Replace(
                    _nodeType.FullName
                        .Substring(_nodeType.FullName.IndexOf("+") + 1)
                        .Replace("+", "."),
                    match => "-" + match.Groups["character"].Value)
                .ToLower();
        }
    }

    [JsonConverter(typeof(NameJsonConverter))]
    public class Name<TRoot> : Name
    {
        Name(Type nodeType) : base(nodeType) { }

        static Regex rxRouteSegmentMatcher = new Regex(
            "(^|-|(?<=[.]))(?<character>[A-Za-z0-9])",
            RegexOptions.Compiled);

        public static bool operator ==(Name<TRoot> a, Name<TRoot> b)
        {
            if (ReferenceEquals(b, null))
            {
                return ReferenceEquals(a, null);
            }
            else if (ReferenceEquals(a, null))
            {
                return ReferenceEquals(b, null);
            }

            return a._nodeType.Equals(b._nodeType);
        }

        public static bool operator !=(Name<TRoot> a, Name<TRoot> b)
        {
            if (ReferenceEquals(b, null))
            {
                return !ReferenceEquals(a, null);
            }
            else if (ReferenceEquals(a, null))
            {
                return !ReferenceEquals(b, null);
            }

            return !a._nodeType.Equals(b._nodeType);
        }

        public static implicit operator string(Name<TRoot> name)
        {
            if (name == null) return null;

            return name.ToString();
        }

        public static Name<TRoot> Parse(string name)
        {
            if (name == null) return null;

            var treenumType = typeof(TRoot);

            var typeName = string.Format(
                "{0}+{1}",
                treenumType.FullName,
                rxRouteSegmentMatcher
                    .Replace(
                        name.ToLower(),
                        match => match.Groups["character"].Value.ToUpper())
                    .Replace(".", "+"));

            var nodeType = treenumType.Assembly.GetType(typeName);

            if (nodeType == null) return null;

            if (!GetRootType(nodeType).Equals(typeof(TRoot)))
            {
                return null;
            }

            return new Name<TRoot>(nodeType);
        }

        public static implicit operator Name<TRoot>(string name)
        {
            return Parse(name);
        }

        public static implicit operator Name<TRoot>(Type nodeType)
        {
            if (nodeType == null) return null;

            if (!GetRootType(nodeType).Equals(typeof(TRoot)))
            {
                return null;
            }

            return new Name<TRoot>(nodeType);
        }

        public static Name<TRoot> GetNode<TNode>()
        {
            if (!GetRootType(typeof(TNode)).Equals(typeof(TRoot)))
            {
                return null;
            }

            return new Name<TRoot>(typeof(TNode));
        }

        public TAttribute Get<TAttribute>() where TAttribute : Attribute
        {
            return _nodeType
                .GetCustomAttributes(typeof(TAttribute), true)
                .Cast<TAttribute>()
                .FirstOrDefault();
        }

        public override bool Equals(object obj)
        {
            var other = obj as Name<TRoot>;

            if (other == null) return false;

            return _nodeType.Equals(other._nodeType);
        }

        public override int GetHashCode()
        {
            return _nodeType.GetHashCode();
        }
    }
}
