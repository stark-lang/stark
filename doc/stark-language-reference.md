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

### Module declaration

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



