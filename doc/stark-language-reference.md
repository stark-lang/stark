# Stark Language Reference

- [Modules](#modules)
  - [Module declaration](#module-declaration)
  - [Import statement](#import-statement)
- [Concepts](#concepts)
  - [Lifetime](#lifetime)
  - [Ownership](#ownership)
  - [Permission](#permission)
  - [Capability](#capability)
- [Types](#types)
  - [Primitive types](#primitive-types)
  - [Enum types](#enum-types)
  - [Union types](#union-types)
  - [Struct types](#struct-types)
  - [Interface types](#interface-types)
  - [Optional types](#optional-types)
  - [Tuple types](#tuple-types)
  - [Unit types](#unit-types)
  - [Alias and indirect types](#alias-and-indirect-types)
  - [Type extensions](#type-extensions)
  - [Function types](#function-types)
  - [Generic type parameterization](#generic-type-parameterization)
- [Functions](#functions)
  - [Static functions](#static-functions)
  - [This functions](#this-functions)
  - [Property functions](#property-functions)
  - [Indexer functions](#indexer-functions)
  - [Operator functions](#operator-functions)
  - [Function contracts](#function-contracts)
- [Global values](#global-values)
  - [Const declaration](#const-declaration)
  - [Static declaration](#static-declaration)
- [Statements](#statements)
- [Expressions](#expressions)
- [Exceptions](#exceptions)
- [Attributes](#attributes)
- [Macros](#macros)
- [Packages](#packages)
- [Libraries](#libraries)
- [Naming conventions](#naming-conventions)
- [Lexicals](#lexicals)
  - [`identifier` (lexer)](#identifier-lexer)

## Modules

A module allows to group together types and functions. It can be private (the default) or public.
You can have multiple modules in a [library](#libraries)
### Module declaration

> **Rule 1001**: The name of a module is composed of a list of `snake_case` [identifiers](#identifier-lexer) separated by `::`.

A module name is separated by `::`. For example the module `core::io::console` declares:
- a module `core`
- the sub-module `io` of `core`
- the sub-module `console` of `core::io`.

```stark
module core::io::console
```

> **Rule 1002**: You can declare **only one module in a file** and it must come first **at the beginning of the file** before any other declarations or imports.

> **Rule 1003**: By default, a module can only be declared from one file, if you use it from different file, you need to prefix it with `partial`

```stark
// File: types.sk
partial module core::io::console
...
// File: funcs.sk
partial module core::io::console
...
```

> **Rule 1004**: By default, a module is private. You need to declare it public to be visible from outside a library

```stark
// The module will be visible outside of the library
public module core::io::console
```

> **Rule 1005**: All private modules and types are visible/accessible from a same library.

It is recommended to organize the code on the filesystem like this:
- If a module doesn't have nested modules
  - It can be stored in a single file and in that case the file should be the name of the module. (e.g `console.sk` above)
  - It can be stored in a folder if the module requires multiple files and in that case the directory should be the name of the module. (e.g `console/` above)

> **Rule 1006**: A file declared without a module lands into the top level anonymous module. 

Usually, the top-level anonymous module is used by executables with a main entrypoint function.

It is not recommended to use an anonymous module within a library.

> **Rule 1007**: A module is closed from a library. It cannot be extended outside of this library except that sub-modules can be created. An alias that doesn't collide needs to be created in the build configuration file.

TBD link to how to declare this in a configuration file.

> **Rule 1008**: Multiple modules declared in a library must share the same root name.

For example, `core::io` and `core::collections` share the same `core` root name.

### Import statement

## Concepts

### Lifetime

### Ownership

### Permission

### Capability

## Types

### Primitive types

### Enum types

### Union types

### Struct types

### Interface types

### Optional types

### Tuple types

### Unit types

### Alias and indirect types

### Type extensions

### Function types

### Generic type parameterization

## Functions

### Static functions

### This functions

### Property functions

### Indexer functions

### Operator functions

### Function contracts

## Global values

### Const declaration

### Static declaration

## Statements

## Expressions

## Exceptions

## Attributes

## Macros

## Packages

## Libraries
## Naming conventions

Stark is using `UpperCamelCase` for "type-level" constructs (struct, enum, interface, union, extension)
and `snake_case` for "value-level" constructs

> note: this was inspired from [Rust naming convention](https://github.com/rust-lang/rfcs/blob/master/text/0430-finalizing-naming-conventions.md)

| Item | Convention |
| ---- | ---------- |
| Package | `snake_case` (but prefer single word)
| Library | `snake_case` (but prefer single word)
| Module | `snake_case`
| Primitive types | concise `snake_case`, but should be mainly with no underscore `_`
| Types: struct, interface, enum, union, extension, type | `UpperCamelCase`
| Union cases | `UpperCamelCase`
| Functions | `snake_case`
| Methods   | `snake_case`
| Named Constructors | `snake_case`
| Local variables | `snake_case`
| Static variables | `SCREAMING_SNAKE_CASE`
| Constant variables | `SCREAMING_SNAKE_CASE`
| Enum items | `SCREAMING_SNAKE_CASE`
| Type parameters | concise `UpperCamelCase`, prefixed by single letter, uppercase `T` for types or lowercase `t` for literals
| Type arguments | `` `snake_case`` or `` `UpperCamelCase``, prefixed by a backstick `` ` ``
| Lifetime | `#snake_case`, prefixed by a `#`, concise for type parameters `#l`
| Attribute | `@snake_case`, prefixed by a `@`
| Macros | `$snake_case`, prefixed by a `$`
| Unit | `'snake_case`, prefixed by a `'`


## Lexicals

### `identifier` (lexer)



