// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace Stark.Compiler.Syntax;

/// <summary>
/// The kind of a token.
/// </summary>
public enum TokenKind : byte
{
    /// <summary>
    /// An invalid character.
    /// </summary>
    Invalid = 0,

    /// <summary>
    /// An invalid UTF8 character.
    /// </summary>
    InvalidUtf8,

    /// <summary>
    /// An invalid TAB character.
    /// </summary>
    InvalidTab,

    Eof,

    WhiteSpace,
    NewLine,
    Identifier,
    Integer,
    Float,
    CommentSingleLine,
    CommentDocumentationSingleLine,
    CommentMultiLine,
    Rune,
    String,
    StringInterpolatedMacro,
    StringInterpolatedPart,
    StringInterpolatedBegin,
    StringInterpolatedEnd,
    MultiLineStringBegin,
    MultiLineStringPart,
    MultiLineStringInterpolatedPart,
    MultiLineStringEnd,

    // 1 byte symbols
    Exclamation,        // !
    // DoubleQuote,        // " not used alone, only used through string
    Number,             // #
    Dollar,             // $
    Percent,            // %
    Ampersand,          // &
    SingleQuote,        // ' used for unit
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