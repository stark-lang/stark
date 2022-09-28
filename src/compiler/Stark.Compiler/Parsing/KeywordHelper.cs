// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Text;
using Stark.Compiler.Syntax;

namespace Stark.Compiler.Parsing;

internal static class KeywordHelper
{
    private static readonly Dictionary<int, (byte[], TokenKind)> KeywordHashToTokenKind = new();


    public static TokenKind GetKeywordTokenKind(ReadOnlySpan<byte> identifier, int hash)
    {
        if (KeywordHashToTokenKind.TryGetValue(hash, out var pair))
        {
            var keyword = new ReadOnlySpan<byte>(pair.Item1);
            if (keyword.SequenceEqual(identifier))
            {
                return pair.Item2;
            }
        }

        return TokenKind.Identifier;
    }

    public static int HashKeyword(byte[] keyword)
    {
        var hashCode = new HashCode();
        hashCode.AddBytes(keyword);
        return hashCode.ToHashCode();
    }

    static KeywordHelper()
    {
        var keywords = new List<(string, TokenKind)>()
        {
            ("as", TokenKind.AsKeyword),
            ("async", TokenKind.AsyncKeyword),
            ("await", TokenKind.AwaitKeyword),
            ("break", TokenKind.BreakKeyword),
            ("catch", TokenKind.CatchKeyword),
            ("const", TokenKind.ConstKeyword),
            ("continue", TokenKind.ContinueKeyword),
            ("else", TokenKind.ElseKeyword),
            ("for", TokenKind.ForKeyword),
            ("func", TokenKind.FuncKeyword),
            ("if", TokenKind.IfKeyword),
            ("is", TokenKind.IsKeyword),
            ("let", TokenKind.LetKeyword),
            ("match", TokenKind.MatchKeyword),
            ("new", TokenKind.NewKeyword),
            ("not", TokenKind.NotKeyword),
            ("out", TokenKind.OutKeyword),
            ("ref", TokenKind.RefKeyword),
            ("return", TokenKind.ReturnKeyword),
            ("then", TokenKind.ThenKeyword),
            ("this", TokenKind.ThisKeyword),
            ("throw", TokenKind.ThrowKeyword),
            ("try", TokenKind.TryKeyword),
            ("unsafe", TokenKind.UnsafeKeyword),
            ("var", TokenKind.VarKeyword),
            ("while", TokenKind.WhileKeyword),
            ("alias", TokenKind.AliasKeyword),
            ("are", TokenKind.AreKeyword),
            ("attr", TokenKind.AttrKeyword),
            ("binary", TokenKind.BinaryKeyword),
            ("can", TokenKind.CanKeyword),
            ("case", TokenKind.CaseKeyword),
            ("constructor", TokenKind.ConstructorKeyword),
            ("enum", TokenKind.EnumKeyword),
            ("exclusive", TokenKind.ExclusiveKeyword),
            ("extends", TokenKind.ExtendsKeyword),
            ("extension", TokenKind.ExtensionKeyword),
            ("get", TokenKind.GetKeyword),
            ("has", TokenKind.HasKeyword),
            ("immutable", TokenKind.ImmutableKeyword),
            ("implements", TokenKind.ImplementsKeyword),
            ("import", TokenKind.ImportKeyword),
            ("in", TokenKind.InKeyword),
            ("indirect", TokenKind.IndirectKeyword),
            ("interface", TokenKind.InterfaceKeyword),
            ("isolated", TokenKind.IsolatedKeyword),
            ("kind", TokenKind.KindKeyword),
            ("lifetime", TokenKind.LifetimeKeyword),
            ("module", TokenKind.ModuleKeyword),
            ("macro", TokenKind.MacroKeyword),
            ("managed", TokenKind.ManagedKeyword),
            ("mutable", TokenKind.MutableKeyword),
            ("operator", TokenKind.OperatorKeyword),
            ("ownership", TokenKind.OwnershipKeyword),
            ("permission", TokenKind.PermissionKeyword),
            ("partial", TokenKind.PartialKeyword),
            ("public", TokenKind.PublicKeyword),
            ("readable", TokenKind.ReadableKeyword),
            ("requires", TokenKind.RequiresKeyword),
            ("rooted", TokenKind.RootedKeyword),
            ("set", TokenKind.SetKeyword),
            ("shared", TokenKind.SharedKeyword),
            ("static", TokenKind.StaticKeyword),
            ("struct", TokenKind.StructKeyword),
            ("transient", TokenKind.TransientKeyword),
            ("throws", TokenKind.ThrowsKeyword),
            ("type", TokenKind.TypeKeyword),
            ("unary", TokenKind.UnaryKeyword),
            ("union", TokenKind.UnionKeyword),
            ("unique", TokenKind.UniqueKeyword),
            ("unit", TokenKind.UnitKeyword),
            ("where", TokenKind.WhereKeyword),
            ("identifier", TokenKind.IdentifierKeyword),
            ("expression", TokenKind.ExpressionKeyword),
            ("statement", TokenKind.StatementKeyword),
            ("literal", TokenKind.LiteralKeyword),
            ("token", TokenKind.TokenKeyword),
            ("bool", TokenKind.BoolType),
            ("uint", TokenKind.UintType),
            ("u8", TokenKind.U8Type),
            ("u16", TokenKind.U16Type),
            ("u32", TokenKind.U32Type),
            ("u64", TokenKind.U64Type),
            ("int", TokenKind.IntType),
            ("i8", TokenKind.I8Type),
            ("i16", TokenKind.I16Type),
            ("i32", TokenKind.I32Type),
            ("i64", TokenKind.I64Type),
            ("f32", TokenKind.F32Type),
            ("f64", TokenKind.F64Type),
            ("v128", TokenKind.V128Type),
            ("v256", TokenKind.V256Type),
        };

        foreach (var pair in keywords)
        {
            var bytes = Encoding.UTF8.GetBytes(pair.Item1);
            var hash = HashKeyword(bytes);
            KeywordHashToTokenKind.Add(hash, (bytes, pair.Item2));
        }
    }
}