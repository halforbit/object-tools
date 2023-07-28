# Introducing `Name<T>`

`Name<T>` is intended as a drop-in replacement for `enum` types with the following improvements:

- Members can be nested in a hierarchy.
- Members are identified by their C# type instead of an integer.
- Members map to/from `string`s that are `kebab-case` and so are url/file path friendly.
- Members JSON serialize as their `string` equivalent instead of e.g. an unsemantic integer value like `123`.

## Examples

Suppose we have the following 'treenum' of names:

```cs
public class Animal
{
    public class Mammal
    {
        public class Cat { }

        public class Dog { }
    }

    public class Insect
    {
        public class LightningBug { }
    }
}
```

### Create a `Name<T>` using `typeof`

`Name<T>` implicitly typecasts from `Type`, where the type is a nested type in `T`. 

The `typeof` keyword can be used to produce a `Type` for the nested type:

```cs
Name<Animal> cat = typeof(Animal.Mammal.Cat);
```

### Get the `string` value of a `Name<T>`

`Name<T>` implicitly typecasts to `string`. The string is `kebab-case` with `.` characters separating the levels of nesting. The string does not include the name of the root type `T`:

```cs
Name<Animal> lightningBug = typeof(Animal.Insect.LightningBug);

string name = lightningBug; // == "insect.lightning-bug"
```

### Get a `Name<T>` from its `string` value

`Name<T>` implicitly typecasts from `string`. If no matching name exists for the string, the value will be `null`.

```cs
string name = "insect.lightning-bug";

Name<Animal> lightningBug = name;
```

### Compare two `Name<T>`s

`Name<T>` implements (in)equality checks with other `Name<T>`s, and can be compared with `==` and `!=`.

```cs
Name<Animal> animalA = "insect.lightning-bug";

Name<Animal> animalB = typeof(Animal.Insect.LightningBug);

bool areEqual = animalA == animalB; // == true
```

### Compare a `Name<T>` to a `Type`

`Name<T>` implements (in)equality checks with `Type`, and can be compared with `==` and `!=`.

```cs
bool areEqual = (Name<Animal>)"cat" == typeof(Animal.Mammal.Cat); // == true
```

## JSON Serialization

`Name<T>` is suitable for use as e.g. a property type of a Newtonsoft.Json-serialized type. The string value of the `Name<T>` will be stored.

## Unit Tests

If you are so inclined, you can review the unit tests defined around `Name<T>` to gain a better insight of how it works, located at https://github.com/halforbit/object-tools/blob/master/Halforbit.ObjectTools.Tests/Naming/NameTests.cs

