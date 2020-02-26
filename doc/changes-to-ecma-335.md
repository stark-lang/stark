# Stark CLI (SKCLI)

This document is listing the changes to the ECMA-335 CLI required by stark

TODO:

- [ ] add changes to type specs
- [ ] add changes for contracts (e.g requires)
- [ ] add changes to generic literal type constraints

## In `II.23.1.15 Flags for types [TypeAttributes]`

In the table, at the section `Class semantics attributes`:

| Flag |  Value | Description |
|-|-|-|
|`ClassSemanticsMask` | `0x00000060` (changed) | Use this mask to retrieve class semantics information. This bit contains one of the following values:
|`Class` | `0x00000000` | Type is a class
|`Interface` | `0x00000020` | Type is an interface
|`Struct` | `0x00000040` (new) | Type is a struct

## In `II.23.1.10 Flags for methods [MethodAttributes]`

- `HasSecurity` is transformed to `IsReadOnly` `0x4000`: when `this` is `readonly`, only a method instance with `IsReadOnly` can be called. Only valid for method instance (doesn't work with static modifier)
- `RequireSecObject` is transformed to `RetainThis` `0x8000`: when `this` is `transient`, only a method instance without `RetainThis` can be called. Only valid for method instance (doesn't work with static modifier)

## In `II.23.1.11 Flags for methods [MethodImplAttributes]`

- Add FuncImplOptions.intrinsic attribute to identify "extern" methods that are actually implemented by the runtime as intrinsics

  In `Implementation info and interop`:  `Intrinsic = 0x2000`

## In `II.22.8 ClassLayout : 0x0F`

Add Alignment (a 2 byte-constant) to the ClassLayout

The ClassLayout table has the following columns:
- PackingSize (a 2-byte constant)
- Add Alignment (a 2 byte-constant)
- ClassSize (a 4-byte constant)
- Parent (an index into the TypeDef table)

## References

Challenges around CustomModifiers and byref in the current spec:
https://github.com/dotnet/corefx/blob/master/src/System.Reflection.Metadata/specs/Ecma-335-Issues.md

