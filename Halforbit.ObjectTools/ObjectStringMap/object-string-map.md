﻿
# String Map

Use a text map to turn simple objects into strings and back again. 

Good for moving structured data into and out of file paths and URLs.

## Usage

Create a class for your data. A flat POCO with simple types such as `int`, `string`, `Guid` and `DateTime`:
```cs
public class FileKey
{
    int AccountId { get; set; }
    
    DateTime? CreateTime { get; set; }
}
```
Create a new `StringMap` that describes how to turn your data to and from a string:
```cs
var map = new StringMap<FileKey>(
    "/accounts/{AccountId}/{CreateTime:yyyy/MM/dd}/data.json");
```
Use the `StringMap` to turn an object into a `string`:
```cs
var obj = new FileKey
{
    AccountId = 1234,
    
    CreateTime = new DateTime(2016, 7, 9)
};

var str = map.Map(obj);
```
Here `str` will be `/accounts/1234/2016/07/09/data.json`.

Use the `StringMap` to turn a `string` into an object:
```cs
var str = "/accounts/1234/2016/07/09/data.json";

var obj = map.Map(str);
```
Here `obj` will equal the `FileKey` defined above.

## Partial Mapping

Suppose in the above example that you have an `AccountId` but you do not know the `CreateTime`:
```cs
var fileKey = new FileKey
{
    AccountId = 123,
    
    CreateTime = null // or just omit this for the default of null.
};
```
You can create a partial string that will be as complete as possible from left to right, stopping once a needed value is null:
```cs
var str = map.Map(fileKey, allowPartialMap: true);
```
Here `str` will be `/accounts/1234/`.

## Simple Data Types

The data type does not need to be a class, and can instead be a simple data type, e.g. `int`, `Guid`, etc. In this case you can use `this` to refer to the data object itself in the map:
```cs
var map = new StringMap<int>("/accounts/{this}");

var str = map.Map(123);
```
Here `str` will be `/accounts/123`.