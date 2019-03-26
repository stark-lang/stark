# Stark CLI (SKCLI)

This document is listing the changes to the ECMA-335 CLI required by stark

TODO:

- [ ] add changes to type specs
- [ ] add changes to type constraints

## In `II.23.1.11 Flags for methods [MethodImplAttributes]`

- Add FuncImplOptions.intrinsic attribute to identify "extern" methods that are actually implemented by the runtime as intrinsics

  In `Implementation info and interop`:  `Intrinsic = 0x2000`



