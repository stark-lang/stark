# Stark Language Reference

- [Modules](#modules)
  - [Module declaration](#module-declaration)
  - [Import statement](#import-statement)
- [Concepts](#concepts)
  - [Smart references](#smart-references)
    - [Lifetime](#lifetime)
    - [Ownership](#ownership)
    - [Permission](#permission)
  - [Capabilities](#capabilities)
  - [Asynchronous programming](#asynchronous-programming)
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

[:top:](#stark-language-reference)

## Modules

A `module` allows to group together types and functions. It can be private (the default) or public.
You can have multiple modules in a [library](#libraries)
### Module declaration

> **Rule-1001**: The name of a module is composed of a list of `snake_case` [identifiers](#identifier-lexer) separated by `::`.

A module name is separated by `::`. For example the module `core::io::console` declares:
- a module `core`
- the sub-module `io` of `core`
- the sub-module `console` of `core::io`.

```stark
module core::io::console
```

> **Rule-1002**: You can declare **only one module in a file** and it must come first **at the beginning of the file** before any other declarations or imports.

> **Rule-1003**: By default, a module can only be declared from one file, if you use it from different file, you need to prefix it with `partial`

```stark
// File: types.sk
partial module core::io::console
...
// File: funcs.sk
partial module core::io::console
...
```

> **Rule-1004**: By default, a module is private. You need to declare it public to be visible from outside a library

```stark
// The module will be visible outside of the library
public module core::io::console
```

> **Rule-1005**: All private modules and types are visible/accessible from a same library.

It is recommended to organize the code on the filesystem like this:
- If a module doesn't have nested modules
  - It can be stored in a single file and in that case the file should be the name of the module. (e.g `console.sk` above)
  - It can be stored in a folder if the module requires multiple files and in that case the directory should be the name of the module. (e.g `console/` above)

> **Rule-1006**: A file declared without a module lands into the top level anonymous module. 

Usually, the top-level anonymous module is used by executables with a main entrypoint function.

It is not recommended to use an anonymous module within a library.

> **Rule-1007**: A module is closed from a library. It cannot be extended outside of this library except that sub-modules can be created. An alias that doesn't collide needs to be created in the build configuration file.

TBD link to how to declare this in a configuration file.

> **Rule-1008**: Multiple modules declared in a library must share the same root name.

For example, `core::io` and `core::collections` share the same `core` root name.

[:top:](#stark-language-reference)

### Import statement

The `import` statement allows to import declarations from another module into the current scope.

> **Rule-1101**: All declarations from parent modules are accessible to the current module.

For declarations that are in a different module path that doesn't share the same module path, you need to import the module.

```stark
partial module core::io
// has access to all declarations from `core::io` and from `core`

// Imports all declarations from the sub-module core::io::internal
import core::io::internal::*
```

> **Rule-1102**: The import statement provides 3 kinds of import. 
> 1. Wildcard: `*` that will import all declarations from a module.
>    ```stark
>    import core::io::internal::*
>    ```
> 2. Single: Will import a single declarations from a module.
>    ```stark
>    import core::io::internal::MyInternalType
>    ```
> 3. Grouped: Will import a list of declarations from a module.
>    ```stark
>    import core::io::internal::{ MyInternalType, my_function2 }
>    ```

The import statement can also be used by interface, struct and extension declarations within their extra-declarations. (TBD: add a link to extra-declaration).

[:top:](#stark-language-reference)

## Concepts

Before diving into the other parts of this document we need to explain a few core concepts:
- [Smart references](#smart-references)
- [Capabilities](#capabilities)
- [Asynchronous programming](#asynchronous-programming)

### Smart references

When a reference is made to a value in memory, it is composed of a type of the value referenced and 3 kind of qualifiers:

- A [lifetime](#lifetime).
- A [ownership](#ownership).
- A [permission](#permission).

```stark
// A reference to a unique mutable MyObject with the #heap lifetime
let obj: ref #heap`unique`mutable MyObject
```
#### Lifetime

Stark is using the concept of lifetime to identify the kind of region of memory that is referenced. Lifetimes  have also a hierarchy of precedence.

> **Rule-1201**: A lifetime is composed of an [identifier](#identifier-lexer) prefixed by `#`.

For example, the lifetime `#heap` denotes a general lifetime allocated on the heap.

> **Rule-1202**: A lifetime is defined relatively to a parent lifetime. The parent lifetime has a longer scope than the lifetime being defined.
> 
> **Rule-1203**: A global lifetime can only be declared at the module level.

```stark
lifetime #my_special_sub_heap < #heap
```

> **Rule-1204**: A lifetime can have multiple child lifetime but an object of one lifetime can only reference objects with a lifetime that are directly a child of their lifetime.

```stark
lifetime #my_special_sub_heap < #heap
lifetime #my_special_sub_heap_1 < #my_special_sub_heap
lifetime #my_special_sub_heap_2 < #my_special_sub_heap
```

In the example above the lifetime `#my_special_sub_heap` can only be referenced by an object on the heap `#heap`. The lifetime `#my_special_sub_heap1` and `2` can only be referenced by an object on the `#my_special_sub_heap`.

It is recommended to define lifetimes at the application level and to use lifetime parameterization for all the object containers.

> **Rule-1205**: The special lifetime `#stack` is the parent of any lifetime deriving from the `#heap`. This lifetime cannot be used directly by a `ref` as it is inferred from the code and it is scoped to the liveness of the value the reference is pointing to.

Conceptually, it's like defining the following lifetime:

```stark
// Stack is a the only existing root lifetime
lifetime #stack
// Heap is deriving from the #stack lifetime
lifetime #heap < #stack
```

An object with the `#heap` cannot reference an object with the `#stack` lifetime.

> **Rule-1206**: The special lifetime `#temp` is a child of the `#heap` lifetime, but can only be used from within the `#stack` or the `#temp` lifetime.

Conceptually, it's like defining the following lifetime:

```stark
// Temp is deriving from the #stack lifetime
lifetime #temp < #stack
```

An object from the `#heap` cannot reference an object from `#temp` lifetime.

> **Rule-1207**: The special lifetime `#this` is derived from the lifetime of the enclosing declaration object. This lifetime can only be used for type parameterization of input/output of a struct.

Reserved lifetime identifiers:

- `#stack` denotes a lifetime on the stack.
- `#heap` denotes a lifetime on the heap.
- `#temp` denotes a lifetime on the temp heap.
- `#this` denotes a lifetime of the parent declaration.

We will see in the [generic type parameterization](#generic-type-parameterization) how lifetime can be parameterized.

#### Ownership

The ownership defines the copy-ability of a reference when it is passed and used around in a program.

> **Rule-1220**: `transient` is the implicit default ownership for which a reference can only be used from the stack and cannot be stored outside of it. This reference can be copied around as long as it stays on the stack.

> **Rule-1221**: `shared` is a reference that can be copied around. Multiple objects can reference the same object with this kind of reference.

> **Rule-1222**: `unique` is a unique reference to an object. When it is copied around, it invalidates the previous reference.

> **Rule-1223**: `rooted` is a unique reference to a root object that can have a sub-object graph. No external references can be made to its internal objects. When it is copied around, it invalidates the previous reference.

> **Rule-1224**: A `shared` ref can be casted to a `transient` ref.

> **Rule-1225**: A `rooted` ref can be casted to a `shared` ref. It can be temporarily casted to it and reverted back to `rooted` as long as the sub-object graph is known to respect the `rooted` ref subgraph rules (rule-1223 above). The previous `rooted` ref cannot be used if the cast to a `shared` ref is definitive.

#### Permission

The permission defines which interactions are allowed with an object.

> **Rule-1240**: `readable` is the implicit default permission for functions and ref. The pointed object can be read from, it won't mutate the object. The object might mutate in the future.

> **Rule-1241**: `immutable` is the implicit default permission for struct declaration or when creating objects. The referenced object can be read from and will never mutate.

> **Rule-1242**: `mutable` is the permission tha allows to mutate the value referenced.

### Capabilities

Stark is an [object-capability](https://en.wikipedia.org/wiki/Object-capability_model) based programming language.

A library or a program to interact with the operating system has to explicitly request objects with the right capability.

For example, a console program will need to request to a e.g `IConsoleService` capability in order to print to the console. Same to get an access to the network/socket layer, or to the filesystem.

```stark
// This program is a console program asking for the `IConsoleService` capability
async func main(console: ref #heap`shared`mutable IConsoleService) = 
    await console.println("Hello World!")
```

### Asynchronous programming

All interactions requiring an access to an I/O OS layer must be done through non-blocking asynchronous API.

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

Stark is following a strict naming convention meaning that the compiler will emit a compiler warning if the convention is not respected. By default, warning are treated as errors by the compiler.

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



