# Object Tools

## Release Notes

### 2023-07-27

#### 1.1.27

- Added more detail to `Name<T>` documentation

#### 1.1.26

- Add documentation for `Name<T>`.

### 2023-02-02

#### 1.1.25

- Added implicit conversion to `StringMap<TObject>` from an `Expression<Func<TObject, string>>`.

### 2022-09-12

#### 1.1.22

- Update to Newtonsoft.Json 13.0.1 in response to a security patch.

#### 1.1.20

- Added `StringExpressionConverter` which allows defining string maps using a lambda expression over a string interpolation in the form `k => $"forecasts/{k.PostalCode}/{k.Date:yyyy/MM/dd}"`.

### 2020-05-15

#### 1.1.10

- Added `ArgumentItem<TItem>` method to `ConstructableExtensions` methods, which allows argument values to be accumulated in a list of items (instead of a single value) across multiple calls.

### 2020-05-04

#### 1.1.7

- Allow optional parameters to be omitted in a `Constructable` specification.