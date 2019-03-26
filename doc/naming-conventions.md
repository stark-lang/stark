# Stark naming conventions

## General

Stark is using `UpperCamelCase` for "type-level" constructs (class, struct, enum, interface, union, extension)
and `snake_case` for "value-level" constructs

> note: this is derived from Rust naming convention: https://github.com/rust-lang/rfcs/blob/master/text/0430-finalizing-naming-conventions.md

| Item | Convention |
| ---- | ---------- |
| Package | `snake_case` (but prefer single word) |
| Namespace | `snake_case` (but prefer single word) |
| Module | `snake_case` (but prefer single word) |
| Types: class, struct, interface, enum, union | `UpperCamelCase` |
| Union cases | `UpperCamelCase` |
| Functions | `snake_case` |
| Local variables | `snake_case` |
| Static variables | `SCREAMING_SNAKE_CASE` |
| Constant variables | `SCREAMING_SNAKE_CASE` |
| Enum items | `SCREAMING_SNAKE_CASE` |
| Type parameters | concise `UpperCamelCase`, prefixed by single uppercase letter: `T` |
