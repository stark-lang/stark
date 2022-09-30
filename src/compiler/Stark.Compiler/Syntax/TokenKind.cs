// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace Stark.Compiler.Syntax;

public enum TokenKind : byte
{
    Invalid = 0,
    InvalidUtf8,
    InvalidTab,

    WhiteSpace,
    NewLine,
    Identifier,
    Integer,
    Float,
    CommentSingleLine,
    CommentDocumentationSingleLine,
    CommentMultiLine,
    String,
    StringInterpolatedMacro,
    StringInterpolatedPart,
    StringInterpolatedBegin,
    StringInterpolatedEnd,

    // 1 byte symbols
    Exclamation,        // !
    DoubleQuote,        // "
    Number,             // #
    Dollar,             // $
    Percent,            // %
    Ampersand,          // &
    SingleQuote,        // '
    LeftParent,         // (
    RightParent,        // )
    Star,               // *
    Plus,               // +
    Comma,              // ,
    Minus,              // -
    Dot,                // .
    Slash,              // /
    Colon,              // :
    SemiColon,          // ;
    LessThan,           // <
    Equal,              // =
    GreaterThan,        // >
    Question,           // ?
    CommercialAt,       // @
    LeftBracket,        // [
    Backslash,          // \
    RightBracket,       // ]
    Circumflex,         // ^
    Underscore,         // _
    Backtick,           // `
    LeftBrace,          // {
    VerticalBar,        // |
    RightBrace,         // }
    Tilde,              // ~

    // 2-3 byte symbols
    SlashEqual,         // /=
    PercentEqual,       // %=
    AmpersandEqual,     // &=
    StarEqual,          // *=
    PlusEqual,          // +=
    MinusEqual,         // -=
    DoubleDot,          // ..
    DoubleDotLessThan,  // ..<
    DoubleColon,        // ::
    DoubleEqual,        // ==
    EqualGreaterThan,   // =>
    TripleEqual,        // ===
    CircumflexEqual,    // ^=
    VerticalBarEqual,   // |=
    VerticalBarGreaterThan,  // |>
    TildeEqual,         // ~=

    AsKeyword,
    AsyncKeyword,
    AwaitKeyword,
    BreakKeyword,
    CatchKeyword,
    ConstKeyword,
    ContinueKeyword,
    ElseKeyword,
    ForKeyword,
    FuncKeyword,
    IfKeyword,
    IsKeyword,
    LetKeyword,
    MatchKeyword,
    NewKeyword,
    NotKeyword,
    OutKeyword,
    RefKeyword,
    ReturnKeyword,
    ThenKeyword,
    ThisKeyword,
    ThrowKeyword,
    TryKeyword,
    UnsafeKeyword,
    VarKeyword,
    WhileKeyword,
    AliasKeyword,
    AreKeyword,
    AttrKeyword,
    BinaryKeyword,
    CanKeyword,
    CaseKeyword,
    ConstructorKeyword,
    EnumKeyword,
    ExclusiveKeyword,
    ExtendsKeyword,
    ExtensionKeyword,
    GetKeyword,
    HasKeyword,
    ImmutableKeyword,
    ImplementsKeyword,
    ImportKeyword,
    InKeyword,
    IndirectKeyword,
    InterfaceKeyword,
    IsolatedKeyword,
    KindKeyword,
    LifetimeKeyword,
    ModuleKeyword,
    MacroKeyword,
    ManagedKeyword,
    MutableKeyword,
    OperatorKeyword,
    OwnershipKeyword,
    PermissionKeyword,
    PartialKeyword,
    PubKeyword,
    ReadableKeyword,
    RequiresKeyword,
    RootedKeyword,
    SetKeyword,
    SharedKeyword,
    StaticKeyword,
    StructKeyword,
    TransientKeyword,
    ThrowsKeyword,
    TypeKeyword,
    UnaryKeyword,
    UnionKeyword,
    UniqueKeyword,
    UnitKeyword,
    WhereKeyword,
    IdentifierKeyword,
    ExpressionKeyword,
    StatementKeyword,
    LiteralKeyword,
    TokenKeyword,

    BoolType,
    UintType,
    U8Type,
    U16Type,
    U32Type,
    U64Type,
    IntType,
    I8Type,
    I16Type,
    I32Type,
    I64Type,
    F32Type,
    F64Type,
    V128Type,
    V256Type,
}