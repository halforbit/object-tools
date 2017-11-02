using Halforbit.ObjectTools.Naming;
using System;
using System.Linq;
using Xunit;

namespace PureCars.ObjectTools.Tests
{
    public class NameTests
    {
        [Fact, Trait("Type", "Unit")]
        public void GetNode_Method_Works()
        {
            var a = Name<Fruit>.GetNode<Fruit.Apples.McIntosh>();

            var b = Name<Fruit>.GetNode<GuidAttribute>();

            Assert.NotNull(a);

            Assert.True(a.GetType().Equals(typeof(Name<Fruit>)));

            Assert.Null(b);
        }

        [Fact, Trait("Type", "Unit")]
        public void Equals_Method_Works()
        {
            var a = Name<Fruit>.GetNode<Fruit.Oranges.Mandarin>();

            var b = Name<Fruit>.GetNode<Fruit.Oranges.Mandarin>();

            var c = Name<Fruit>.GetNode<Fruit.Oranges.Tangelo>();

            var n = null as Name<Fruit>;

            Assert.True(a.Equals(b));

            Assert.False(a.Equals(c));

            Assert.False(a.Equals(n));
        }

        [Fact, Trait("Type", "Unit")]
        public void Equality_Operator_Works()
        {
            var a = Name<Fruit>.GetNode<Fruit.Oranges.Mandarin>();

            var b = Name<Fruit>.GetNode<Fruit.Oranges.Mandarin>();

            var c = Name<Fruit>.GetNode<Fruit.Oranges.Tangelo>();

            var n1 = null as Name<Fruit>;

            var n2 = null as Name<Fruit>;

            Assert.True(a == b);

            Assert.False(a == c);

            Assert.True(n1 == n2);

            Assert.False(n1 == a);

            Assert.False(a == n1);
        }

        [Fact, Trait("Type", "Unit")]
        public void Inequality_Operator_Works()
        {
            var a = Name<Fruit>.GetNode<Fruit.Oranges.Mandarin>();

            var b = Name<Fruit>.GetNode<Fruit.Oranges.Mandarin>();

            var c = Name<Fruit>.GetNode<Fruit.Oranges.Tangelo>();

            var n1 = null as Name<Fruit>;

            var n2 = null as Name<Fruit>;

            Assert.False(a != b);

            Assert.True(a != c);

            Assert.False(n1 != n2);

            Assert.True(n1 != a);

            Assert.True(a != n1);
        }

        [Fact, Trait("Type", "Unit")]
        public void ToString_Method_Works()
        {
            var validName = Name<Fruit>.GetNode<Fruit.Apples.McIntosh>();

            var nullName = null as Name<Fruit>;

            var validString = validName.ToString();

            var nullString = "" + nullName;

            Assert.Equal(
                "apples.mc-intosh",
                validString);

            Assert.Equal(
                "",
                nullString);
        }

        [Fact, Trait("Type", "Unit")]
        public void Parse_Method_Works()
        {
            var validName = Name<Fruit>.Parse("apples.mc-intosh");

            var nullName = Name<Fruit>.Parse(null);

            var invalidName = Name<Fruit>.Parse("i-am.not.a-doctor");

            Assert.Equal(
                Name<Fruit>.GetNode<Fruit.Apples.McIntosh>(),
                validName);

            Assert.Null(nullName);

            Assert.Null(invalidName);
        }

        [Fact, Trait("Type", "Unit")]
        public void Implicitly_Converts_To_String()
        {
            var validName = Name<Fruit>.GetNode<Fruit.Apples.McIntosh>();

            var validString = (string)validName;

            var nullName = null as Name<Fruit>;

            var nullString = (string)nullName;

            Assert.Equal(
                "apples.mc-intosh",
                validString);

            Assert.Null(nullString);
        }

        [Fact, Trait("Type", "Unit")]
        public void Implicitly_Converts_From_String()
        {
            var validString = "apples.mc-intosh";

            var nullString = null as string;

            var invalidString = "i-am.not.a-doctor";

            var validName = (Name<Fruit>)validString;

            var nullName = (Name<Fruit>)nullString;

            var invalidName = (Name<Fruit>)invalidString;

            Assert.Equal(
                Name<Fruit>.GetNode<Fruit.Apples.McIntosh>(),
                validName);

            Assert.Null(nullName);

            Assert.Null(invalidName);
        }

        [Fact, Trait("Type", "Unit")]
        public void Get_Attribute_Method_Works()
        {
            var name = Name<Fruit>.GetNode<Fruit.Apples.Delicious>();

            var validAttribute = name.Get<GuidAttribute>();

            var invalidAttribute = name.Get<ConnectionEntityAttribute>();

            Assert.NotNull(validAttribute);

            Assert.Equal(
                Fruit.TestGuidString,
                validAttribute.Guid.ToString("N"));

            Assert.Null(invalidAttribute);
        }

        [Fact, Trait("Type", "Unit")]
        public void Implicitly_Converts_From_Type()
        {
            // TODO
        }
    }

    public class Fruit
    {
        public const string TestGuidString = "44e0890854554fa5a193389855a7852d";

        public class Apples
        {
            [Guid(TestGuidString)]
            public class Delicious
            {
                public class Red { }

                public class Gold { }
            }

            public class McIntosh { }
        }

        public class Oranges
        {
            public class Tangelo { }

            public class Mandarin { }
        }
    }

    public class Vehicle
    {
        public class Coup { }

        public class Sedan { }

        public class Truck { }
    }

    public class GuidAttribute : Attribute
    {
        public GuidAttribute(string guid)
        {
            Guid = new Guid(guid);
        }

        public Guid Guid { get; protected set; }

        public static Guid GuidFromEnumValue<TEnum>(TEnum value)
        {
            var memberInfo = typeof(TEnum)
                .GetMember(value.ToString())
                .FirstOrDefault();

            if (memberInfo != null)
            {
                if (memberInfo
                    .GetCustomAttributes(typeof(GuidAttribute), true)
                    .FirstOrDefault() is GuidAttribute attribute)
                {
                    return attribute.Guid;
                }
            }

            throw new MissingMemberException(
                "A GuidAttribute could not be found for the enum value " + value);
        }

        public static TEnum EnumValueFromGuid<TEnum>(Guid guid)
        {
            return Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .First(value => GuidAttribute.GuidFromEnumValue(value) == guid);
        }

        public static Guid GetGuidOf<TType>()
        {
            var guidAttribute = typeof(TType)
                .GetCustomAttributes(typeof(GuidAttribute), true)
                .FirstOrDefault() as GuidAttribute;

            if (guidAttribute == null)
            {
                throw new Exception($"Type {typeof(TType)} does not have a Guid attribute.");
            }

            return guidAttribute.Guid;
        }
    }

    public class ConnectionEntityAttribute : Attribute
    {
    }
}
