using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Stark.Compiler.Diagnostics;
using Stark.Compiler.Parsing;
using Stark.Compiler.Syntax;
using Varena;

namespace Stark.Compiler.Tests;

/// <summary>
/// Class for testing Stark <see cref="Lexer"/>.
/// </summary>
public class TestLexer
{
    private readonly VirtualArenaManager _manager;
    private readonly LexerInputOutput _lio;
    private readonly Lexer _lexer;


    [Test]
    public void TestSimpleString()
    {
        Lexer(@" """" ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.String, new TokenSpan(1, 2, 0, 1), null),
            (TokenKind.WhiteSpace, new TokenSpan(3, 1, 0, 3), null),
        });

        Lexer(@" ""hello world"" ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.String, new TokenSpan(1, 13, 0, 1), "hello world"),
            (TokenKind.WhiteSpace, new TokenSpan(14, 1, 0, 14), null),
        });


        Lexer(@" ""hello é world"" ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.String, new TokenSpan(1, 16, 0, 1), "hello é world"),
            (TokenKind.WhiteSpace, new TokenSpan(17, 1, 0, 16), null),
        });
    }

    [TestCase(@"\n", "\n")]
    [TestCase(@"\r", "\r")]
    [TestCase(@"\'", "\'")]
    [TestCase(@"\""", "\"")]
    [TestCase(@"\\", "\\")]
    [TestCase(@"\b", "\b")]
    [TestCase(@"\f", "\f")]
    [TestCase(@"\t", "\t")]
    [TestCase(@"\v", "\v")]
    [TestCase(@"\0", "\0")]
    [TestCase(@"\u0000", "\u0000")]
    [TestCase(@"\u01aF", "\u01aF")]
    [TestCase(@"\x0", "\x0")]
    [TestCase(@"\x12", "\x12")]
    [TestCase(@"\x12c", "\x12c")]
    [TestCase(@"\x12cF", "\x12cF")]
    [TestCase(@"\U00000000", "\U00000000")]
    [TestCase(@"\U000012cF", "\U000012CF")]
    public void TestStringEscapeSequences(string escaped, string real)
    {
        var length = 12 + escaped.Length;

        //  0123456789abcdef
        // ` "hello\nworld" `
        Lexer($" \"hello{escaped}world\" ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.String, new TokenSpan(1, (uint)length, 0, 1), $"hello{real}world"),
            (TokenKind.WhiteSpace, new TokenSpan((uint)length + 1, 1, 0, (uint)length + 1), null),
        });
    }
    
    [Test]
    public void TestStringEscapeErrors()
    {
        //  0123456789ab
        // ` "\Zhello" `
        Lexer($" \"\\Zhello\" ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.String, new TokenSpan(1, 9, 0, 1), "Zhello"),
            (TokenKind.WhiteSpace, new TokenSpan(10, 1, 0, 10), null),
        }, new()
        {
            (DiagnosticId.ERR_UnexpectedEscapeCharacter, new TextSpan(new TextLocation(3, 0, 3)))
        });

        //  0123456789ab
        // ` "\U1hello" `
        Lexer($" \"\\U1hello\" ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.String, new TokenSpan(1, 10, 0, 1), "hello"),
            (TokenKind.WhiteSpace, new TokenSpan(11, 1, 0, 11), null),
        },new() 
        {
            (DiagnosticId.ERR_InvalidHexNumberInString3, new TextSpan(new TextLocation(5, 0, 5)))
        });

        //  0123456789ab
        // ` "\xhello" `
        Lexer($" \"\\xhello\" ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.String, new TokenSpan(1, 9, 0, 1), "hello"),
            (TokenKind.WhiteSpace, new TokenSpan(10, 1, 0, 10), null),
        }, new()
        {
            (DiagnosticId.ERR_InvalidHexNumberInString2, new TextSpan(new TextLocation(4, 0, 4)))
        });

        //  0123456789ab
        // ` "\uhello" `
        Lexer($" \"\\uhello\" ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.String, new TokenSpan(1, 9, 0, 1), "hello"),
            (TokenKind.WhiteSpace, new TokenSpan(10, 1, 0, 10), null),
        }, new()
        {
            (DiagnosticId.ERR_InvalidHexNumberInString1, new TextSpan(new TextLocation(4, 0, 4)))
        });

        //  0123456789ab
        // ` "\u0hello" `
        Lexer($" \"\\u0hello\" ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.String, new TokenSpan(1, 10, 0, 1), "hello"),
            (TokenKind.WhiteSpace, new TokenSpan(11, 1, 0, 11), null),
        }, new()
        {
            (DiagnosticId.ERR_InvalidHexNumberInString1, new TextSpan(new TextLocation(5, 0, 5)))
        });

        //  0123456789ab
        // ` "\u00hello" `
        Lexer($" \"\\u00hello\" ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.String, new TokenSpan(1, 11, 0, 1), "hello"),
            (TokenKind.WhiteSpace, new TokenSpan(12, 1, 0, 12), null),
        }, new()
        {
            (DiagnosticId.ERR_InvalidHexNumberInString1, new TextSpan(new TextLocation(6, 0, 6)))
        });

        //  0123456789abcde
        // ` "\u000hello" `
        Lexer($" \"\\u000hello\" ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.String, new TokenSpan(1, 12, 0, 1), "hello"),
            (TokenKind.WhiteSpace, new TokenSpan(13, 1, 0, 13), null),
        }, new()
        {
            (DiagnosticId.ERR_InvalidHexNumberInString1, new TextSpan(new TextLocation(7, 0, 7)))
        });

        //  0123456789abcdef012
        // ` "\Uffffffffhello" `
        Lexer($" \"\\Uffffffffhello\" ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.String, new TokenSpan(1, 17, 0, 1), "hello"),
            (TokenKind.WhiteSpace, new TokenSpan(18, 1, 0, 18), null),
        }, new()
        {
            (DiagnosticId.ERR_InvalidUtf8InString, new TextSpan(new TextLocation(2, 0, 2)))
        });

        Lexer($" \"\n ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.String, new TokenSpan(1, 1, 0, 1), null),
            (TokenKind.NewLine, new TokenSpan(2, 1, 0, 2), null),
            (TokenKind.WhiteSpace, new TokenSpan(3, 1, 1, 0), null),
        }, new()
        {
            (DiagnosticId.ERR_UnexpectedEndOfString, new TextSpan(new TextLocation(2, 0, 2)))
        });

        Lexer($" \"", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.String, new TokenSpan(1, 1, 0, 1), null),
        }, new()
        {
            (DiagnosticId.ERR_UnexpectedEndOfString, new TextSpan(new TextLocation(2, 0, 2)))
        });
    }

    [Test]
    public void TestInvalidCharacters()
    {
        Lexer(" \t ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.InvalidTab, new TokenSpan(1, 1, 0, 1), null),
            (TokenKind.WhiteSpace, new TokenSpan(2, 1, 0, 2), null),
        });

        Lexer(" \x01 ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.Invalid, new TokenSpan(1, 1, 0, 1), null),
            (TokenKind.WhiteSpace, new TokenSpan(2, 1, 0, 2), null),
        });

        // \xC0 encoded as C3 80 in UTF8
        Lexer(" \xC0 ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.InvalidUtf8, new TokenSpan(1, 2, 0, 1), null),
            (TokenKind.WhiteSpace, new TokenSpan(3, 1, 0, 2), null),
        });

        // Encode an invalid character
        Lexer(new byte[] { (byte)' ', 0xc0, (byte)' ' }, new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.Invalid, new TokenSpan(1, 1, 0, 1), null),
            (TokenKind.WhiteSpace, new TokenSpan(2, 1, 0, 2), null),
        });
    }

    [Test]
    public void TestMultiLineComment()
    {
        Lexer(" /**/ ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.CommentMultiLine, new TokenSpan(1, 4, 0, 1), null),
            (TokenKind.WhiteSpace, new TokenSpan(5, 1, 0, 5), null),
        });

        Lexer(" /* */ ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.CommentMultiLine, new TokenSpan(1, 5, 0, 1), null),
            (TokenKind.WhiteSpace, new TokenSpan(6, 1, 0, 6), null),
        });

        Lexer(" /* a */ ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.CommentMultiLine, new TokenSpan(1, 7, 0, 1), null),
            (TokenKind.WhiteSpace, new TokenSpan(8, 1, 0, 8), null),
        });
        
        Lexer(" /*/**/*/ ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.CommentMultiLine, new TokenSpan(1, 8, 0, 1), null),
            (TokenKind.WhiteSpace, new TokenSpan(9, 1, 0, 9), null),
        });

        Lexer(" /*\r*/ ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.CommentMultiLine, new TokenSpan(1, 5, 0, 1), null),
            (TokenKind.WhiteSpace, new TokenSpan(6, 1, 1, 2), null),
        });
        
        Lexer(" /*\n*/ ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.CommentMultiLine, new TokenSpan(1, 5, 0, 1), null),
            (TokenKind.WhiteSpace, new TokenSpan(6, 1, 1, 2), null),
        });

        Lexer(" /*\r\n*/ ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.CommentMultiLine, new TokenSpan(1, 6, 0, 1), null),
            (TokenKind.WhiteSpace, new TokenSpan(7, 1, 1, 2), null),
        });

        // 🎉 is 4 bytes in UTF8, takes 2 columns
        Lexer(" /*🎉*/ ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.CommentMultiLine, new TokenSpan(1, 8, 0, 1), null),
            (TokenKind.WhiteSpace, new TokenSpan(9, 1, 0, 7), null),
        });

        // é is 2 bytes in UTF8, takes 1 column
        Lexer(" /*é*/ ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.CommentMultiLine, new TokenSpan(1, 6, 0, 1), null),
            (TokenKind.WhiteSpace, new TokenSpan(7, 1, 0, 6), null),
        });

        Lexer(" /*", new()
            {
                (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
                (TokenKind.CommentMultiLine, new TokenSpan(1, 2, 0, 1), null),
            }, new()
            {
                (DiagnosticId.ERR_UnexpectedEndOfFileForMultiLineComment, new TextSpan(new TextLocation(1, 0, 1), new TextLocation(2, 0, 2))),
            }
        );

        Lexer(" /*\n ", new()
            {
                (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
                (TokenKind.CommentMultiLine, new TokenSpan(1, 4, 0, 1), null),
            }, new()
            {
                (DiagnosticId.ERR_UnexpectedEndOfFileForMultiLineComment, new TextSpan(new TextLocation(1, 0, 1), new TextLocation(4, 1, 0))),
            }
        );
    }

    [Test]
    public void TestSingleLineComment()
    {
        Lexer(" //", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.CommentSingleLine, new TokenSpan(1, 2, 0, 1), null),
        });

        Lexer(" // ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.CommentSingleLine, new TokenSpan(1, 3, 0, 1), null),
        });

        Lexer(" //\r", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.CommentSingleLine, new TokenSpan(1, 2, 0, 1), null),
            (TokenKind.NewLine, new TokenSpan(3, 1, 0, 3), null),
        });

        Lexer(" //\n", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.CommentSingleLine, new TokenSpan(1, 2, 0, 1), null),
            (TokenKind.NewLine, new TokenSpan(3, 1, 0, 3), null),
        });

        Lexer(" //\r\n", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.CommentSingleLine, new TokenSpan(1, 2, 0, 1), null),
            (TokenKind.NewLine, new TokenSpan(3, 2, 0, 3), null),
        });

        Lexer(" // abc\n  // def", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.CommentSingleLine, new TokenSpan(1, 6, 0, 1), null),
            (TokenKind.NewLine, new TokenSpan(7, 1, 0, 7), null),
            (TokenKind.WhiteSpace, new TokenSpan(8, 2, 1, 0), null),
            (TokenKind.CommentSingleLine, new TokenSpan(10, 6, 1, 2), null),
        });
 
        // 🎉 is 4 bytes in UTF8, takes 2 columns
        Lexer(" // 🎉\n", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.CommentSingleLine, new TokenSpan(1, 7, 0, 1), null),
            (TokenKind.NewLine, new TokenSpan(8, 1, 0, 6), null),
        });

        // é is 2 bytes in UTF8, takes 1 column
        Lexer(" // é\n", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.CommentSingleLine, new TokenSpan(1, 5, 0, 1), null),
            (TokenKind.NewLine, new TokenSpan(6, 1, 0, 5), null),
        });
    }

    [TestCase(TokenKind.Exclamation, "!")]
    [TestCase(TokenKind.Number, "#")]
    [TestCase(TokenKind.Dollar, "$")]
    [TestCase(TokenKind.Percent, "%")]
    [TestCase(TokenKind.Ampersand, "&")]
    [TestCase(TokenKind.LeftParent, "(")]
    [TestCase(TokenKind.RightParent, ")")]
    [TestCase(TokenKind.Star, "*")]
    [TestCase(TokenKind.Plus, "+")]
    [TestCase(TokenKind.Comma, ",")]
    [TestCase(TokenKind.Minus, "-")]
    [TestCase(TokenKind.Dot, ".")]
    [TestCase(TokenKind.Slash, "/")]
    [TestCase(TokenKind.Colon, ":")]
    [TestCase(TokenKind.SemiColon, ";")]
    [TestCase(TokenKind.LessThan, "<")]
    [TestCase(TokenKind.Equal, "=")]
    [TestCase(TokenKind.GreaterThan, ">")]
    [TestCase(TokenKind.Question, "?")]
    [TestCase(TokenKind.CommercialAt, "@")]
    [TestCase(TokenKind.LeftBracket, "[")]
    [TestCase(TokenKind.Backslash, "\\")]
    [TestCase(TokenKind.RightBracket, "]")]
    [TestCase(TokenKind.Circumflex, "^")]
    [TestCase(TokenKind.Underscore, "_")]
    [TestCase(TokenKind.Backtick, "`")]
    [TestCase(TokenKind.LeftBrace, "{")]
    [TestCase(TokenKind.VerticalBar, "|")]
    [TestCase(TokenKind.RightBrace, "}")]
    [TestCase(TokenKind.Tilde, "~")]
    [TestCase(TokenKind.SlashEqual, "/=")]
    [TestCase(TokenKind.PercentEqual, "%=")]
    [TestCase(TokenKind.AmpersandEqual, "&=")]
    [TestCase(TokenKind.StarEqual, "*=")]
    [TestCase(TokenKind.PlusEqual, "+=")]
    [TestCase(TokenKind.MinusEqual, "-=")]
    [TestCase(TokenKind.DoubleDot, "..")]
    [TestCase(TokenKind.DoubleDotLessThan, "..<")]
    [TestCase(TokenKind.DoubleColon, "::")]
    [TestCase(TokenKind.DoubleEqual, "==")]
    [TestCase(TokenKind.EqualGreaterThan, "=>")]
    [TestCase(TokenKind.TripleEqual, "===")]
    [TestCase(TokenKind.CircumflexEqual, "^=")]
    [TestCase(TokenKind.VerticalBarEqual, "|=")]
    [TestCase(TokenKind.VerticalBarGreaterThan, "|>")]
    [TestCase(TokenKind.TildeEqual, "~=")]
    public void TestSymbols(TokenKind kind, string expected)
    {
        var column = 1 + (uint)expected.Length;
        Lexer($" {expected} ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (kind, new TokenSpan(1, (uint)expected.Length, 0, 1), null),
            (TokenKind.WhiteSpace, new TokenSpan(column, 1, 0, column), null),
        });
    }

    [Test]
    public void TestIdentifier()
    {
        Lexer(" hello ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.Identifier, new TokenSpan(1, 5, 0, 1), "hello"),
            (TokenKind.WhiteSpace, new TokenSpan(6, 1, 0, 6), null),
        });

        Lexer(" h ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.Identifier, new TokenSpan(1, 1, 0, 1), "h"),
            (TokenKind.WhiteSpace, new TokenSpan(2, 1, 0, 2), null),
        });

        Lexer(" _h ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.Identifier, new TokenSpan(1, 2, 0, 1), "_h"),
            (TokenKind.WhiteSpace, new TokenSpan(3, 1, 0, 3), null),
        });

        Lexer(" h_ ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.Identifier, new TokenSpan(1, 2, 0, 1), "h_"),
            (TokenKind.WhiteSpace, new TokenSpan(3, 1, 0, 3), null),
        });

        Lexer(" h1234567890 ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.Identifier, new TokenSpan(1, 11, 0, 1), "h1234567890"),
            (TokenKind.WhiteSpace, new TokenSpan(12, 1, 0, 12), null),
        });

        Lexer(" abcdefghjiklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_0123456789 ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.Identifier, new TokenSpan(1, 63, 0, 1), "abcdefghjiklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_0123456789"),
            (TokenKind.WhiteSpace, new TokenSpan(64, 1, 0, 64), null),
        });

        Lexer(" _ ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.Underscore, new TokenSpan(1, 1, 0, 1), null),
            (TokenKind.WhiteSpace, new TokenSpan(2, 1, 0, 2), null),
        });

        Lexer(" __ ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.Underscore, new TokenSpan(1, 2, 0, 1), null),
            (TokenKind.WhiteSpace, new TokenSpan(3, 1, 0, 3), null),
        });
    }

    [Test]
    public void TestKeywords()
    {
        Lexer(" pub ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.PubKeyword, new TokenSpan(1, 3, 0, 1), null),
            (TokenKind.WhiteSpace, new TokenSpan(4, 1, 0, 4), null),
        });

        Lexer(" as ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.AsKeyword, new TokenSpan(1, 2, 0, 1), null),
            (TokenKind.WhiteSpace, new TokenSpan(3, 1, 0, 3), null),
        });

        Lexer(" async ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.AsyncKeyword, new TokenSpan(1, 5, 0, 1), null),
            (TokenKind.WhiteSpace, new TokenSpan(6, 1, 0, 6), null),
        });
    }

    [Test]
    public void TestNewLine()
    {
        Lexer(" \n", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.NewLine, new TokenSpan(1, 1, 0, 1), null),
        });

        Lexer("\n ", new()
        {
            (TokenKind.NewLine, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.WhiteSpace, new TokenSpan(1, 1, 1, 0), null),
        });

        Lexer("\r\n ", new()
        {
            (TokenKind.NewLine, new TokenSpan(0, 2, 0, 0), null),
            (TokenKind.WhiteSpace, new TokenSpan(2, 1, 1, 0), null),
        });

        Lexer(" \r\n", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.NewLine, new TokenSpan(1, 2, 0, 1), null),
        });

        Lexer(" \r", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.NewLine, new TokenSpan(1, 1, 0, 1), null),
        });

        Lexer("\r ", new()
        {
            (TokenKind.NewLine, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.WhiteSpace, new TokenSpan(1, 1, 1, 0), null),
        });
    }

    [Test]
    public void TestWhitespace()
    {
        Lexer(" ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
        });

        Lexer("  ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 2, 0, 0), null),
        });

        Lexer("         ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 9, 0, 0), null),
        });

        Lexer("0         ", new()
        {
            (TokenKind.Integer, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.WhiteSpace, new TokenSpan(1, 9, 0, 1), null),
        });
    }

    [Test]
    public void TestInteger()
    {
        Lexer("1234567890", new()
        {
            (TokenKind.Integer, new TokenSpan(0, 10, 0, 0), 1234567890),
        });

        Lexer("41", new()
        {
            (TokenKind.Integer, new TokenSpan(0, 2, 0, 0), 41),
        });

        Lexer("9876543210", new()
        {
            (TokenKind.Integer, new TokenSpan(0, 10, 0, 0), 9876543210),
        });

        Lexer("-125", new ()
        {
            (TokenKind.Minus, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.Integer, new TokenSpan(1, 3, 0, 1), 125),
        });

        Lexer("0", new()
        {
            (TokenKind.Integer, new TokenSpan(0, 1, 0, 0), 0),
        });

        Lexer("12_1", new()
        {
            (TokenKind.Integer, new TokenSpan(0, 4, 0, 0), 121),
        });

        Lexer("12__1", new()
        {
            (TokenKind.Integer, new TokenSpan(0, 5, 0, 0), 121),
        });

        // Errors

        Lexer("1_", new()
            {
                (TokenKind.Integer, new TokenSpan(0, 2, 0, 0), 1),
            }, new()
            {
                (DiagnosticId.ERR_UnexpectedUnderscoreAfterDigit, new TextSpan(new TextLocation(1, 0, 1))),
            }
        );

        Lexer(" 1__ ", new()
            {
                (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
                (TokenKind.Integer, new TokenSpan(1, 3, 0, 1), 1),
                (TokenKind.WhiteSpace, new TokenSpan(4, 1, 0, 4), null),
            }, new()
            {
                (DiagnosticId.ERR_UnexpectedUnderscoreAfterDigit, new TextSpan(new TextLocation(2, 0, 2), new TextLocation(3, 0, 3))),
            }
        );
        
        // 1 << 64 will overflow
        Lexer(" 18_446_744_073_709_551_616 ", new()
            {
                (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
                (TokenKind.Integer, new TokenSpan(1, 26, 0, 1), 0),
                (TokenKind.WhiteSpace, new TokenSpan(27, 1, 0, 27), null),
            }, new()
            {
                (DiagnosticId.ERR_NumberOverflow, new TextSpan(new TextLocation(1, 0, 1), new TextLocation(26, 0, 26))),
            }
        );
    }

    [Test]
    public void TestIntegerHex()
    {
        Lexer(" 0x12345678 ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.Integer, new TokenSpan(1, 10, 0, 1), 0x12345678),
            (TokenKind.WhiteSpace, new TokenSpan(11, 1, 0, 11), null),
        });

        for (int i = 0; i < 9; i++)
        {
            Lexer($" 0x{i} ", new()
            {
                (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
                (TokenKind.Integer, new TokenSpan(1, 3, 0, 1), i),
                (TokenKind.WhiteSpace, new TokenSpan(4, 1, 0, 4), null),
            });
        }

        Lexer(" 0xABCDEF01 ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.Integer, new TokenSpan(1, 10, 0, 1), 0xABCDEF01),
            (TokenKind.WhiteSpace, new TokenSpan(11, 1, 0, 11), null),
        });

        Lexer(" 0xabcdef01 ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.Integer, new TokenSpan(1, 10, 0, 1), 0xABCDEF01),
            (TokenKind.WhiteSpace, new TokenSpan(11, 1, 0, 11), null),
        });

        Lexer(" 0x1234_abcd ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.Integer, new TokenSpan(1, 11, 0, 1), 0x1234ABCD),
            (TokenKind.WhiteSpace, new TokenSpan(12, 1, 0, 12), null),
        });

        Lexer(" 0x1234_abcd_5678_9abc ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.Integer, new TokenSpan(1, 21, 0, 1), 0x1234_abcd_5678_9abc),
            (TokenKind.WhiteSpace, new TokenSpan(22, 1, 0, 22), null),
        });

        // Errors
        Lexer(" 0x ", new()
            {
                (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
                (TokenKind.Integer, new TokenSpan(1, 2, 0, 1), 0),
                (TokenKind.WhiteSpace, new TokenSpan(3, 1, 0, 3), null),
            }, new()
            {
                (DiagnosticId.ERR_InvalidHexNumberExpectingDigit, new TextSpan(new TextLocation(1, 0, 1), new TextLocation(2, 0, 2))),
            }
        );

        Lexer(" 0x1_ ", new()
            {
                (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
                (TokenKind.Integer, new TokenSpan(1, 4, 0, 1), 1),
                (TokenKind.WhiteSpace, new TokenSpan(5, 1, 0, 5), null),
            }, new()
            {
                (DiagnosticId.ERR_UnexpectedUnderscoreAfterDigit, new TextSpan(new TextLocation(4, 0, 4))),
            }
        );

        // Overflow
        Lexer(" 0x1_0000_0000_0000_0000 ", new()
            {
                (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
                (TokenKind.Integer, new TokenSpan(1, 23, 0, 1), 0),
                (TokenKind.WhiteSpace, new TokenSpan(24, 1, 0, 24), null),
            }, new()
            {
                (DiagnosticId.ERR_NumberOverflow,  new TextSpan(new TextLocation(1, 0, 1), new TextLocation(23, 0, 23))),
            }
        );
    }

    [Test]
    public void TestIntegerOctal()
    {
        Lexer(" 0o01234567 ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.Integer, new TokenSpan(1, 10, 0, 1), 0b_000_001_010_011_100_101_110_111),
            (TokenKind.WhiteSpace, new TokenSpan(11, 1, 0, 11), null),
        });

        // Errors
        Lexer(" 0o ", new()
            {
                (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
                (TokenKind.Integer, new TokenSpan(1, 2, 0, 1), 0),
                (TokenKind.WhiteSpace, new TokenSpan(3, 1, 0, 3), null),
            }, new()
            {
                (DiagnosticId.ERR_InvalidOctalNumberExpectingDigit, new TextSpan(new TextLocation(1, 0, 1), new TextLocation(2, 0, 2))),
            }
        );

        Lexer(" 0o17_ ", new()
            {
                (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
                (TokenKind.Integer, new TokenSpan(1, 5, 0, 1), 0b_001_111),
                (TokenKind.WhiteSpace, new TokenSpan(6, 1, 0, 6), null),
            }, new()
            {
                (DiagnosticId.ERR_UnexpectedUnderscoreAfterDigit, new TextSpan(new TextLocation(5, 0, 5))),
            }
        );

        // Overflow
        Lexer(" 0o_7_00000000000_00000000000 ", new()
            {
                (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
                (TokenKind.Integer, new TokenSpan(1, 28, 0, 1), 0),
                (TokenKind.WhiteSpace, new TokenSpan(29, 1, 0, 29), null),
            }, new()
            {
                (DiagnosticId.ERR_NumberOverflow,  new TextSpan(new TextLocation(1, 0, 1), new TextLocation(28, 0, 28))),
            }
        );
    }

    [Test]
    public void TestIntegerBinary()
    {
        Lexer(" 0b1011_1101 ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.Integer, new TokenSpan(1, 11, 0, 1), 0b1011_1101),
            (TokenKind.WhiteSpace, new TokenSpan(12, 1, 0, 12), null),
        });

        Lexer(" 0b1111_1010_0101_0011_0111_1000_1001_1100_0011_1111_1010_0101_0011_0111_1000_1001 ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.Integer, new TokenSpan(1, 81, 0, 1), 0b1111_1010_0101_0011_0111_1000_1001_1100_0011_1111_1010_0101_0011_0111_1000_1001),
            (TokenKind.WhiteSpace, new TokenSpan(82, 1, 0, 82), null),
        });

        // Errors
        Lexer(" 0b ", new()
            {
                (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
                (TokenKind.Integer, new TokenSpan(1, 2, 0, 1), 0),
                (TokenKind.WhiteSpace, new TokenSpan(3, 1, 0, 3), null),
            }, new()
            {
                (DiagnosticId.ERR_InvalidHexNumberExpectingDigit, new TextSpan(new TextLocation(1, 0, 1), new TextLocation(2, 0, 2))),
            }
        );

        Lexer(" 0b10_ ", new()
            {
                (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
                (TokenKind.Integer, new TokenSpan(1, 5, 0, 1), 2),
                (TokenKind.WhiteSpace, new TokenSpan(6, 1, 0, 6), null),
            }, new()
            {
                (DiagnosticId.ERR_UnexpectedUnderscoreAfterDigit, new TextSpan(new TextLocation(5, 0, 5))),
            }
        );

        // Overflow
        Lexer(" 0b1_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000 ", new()
            {
                (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
                (TokenKind.Integer, new TokenSpan(1, 83, 0, 1), 0),
                (TokenKind.WhiteSpace, new TokenSpan(84, 1, 0, 84), null),
            }, new()
            {
                (DiagnosticId.ERR_NumberOverflow,  new TextSpan(new TextLocation(1, 0, 1), new TextLocation(83, 0, 83))),
            }
        );
    }

    [Test]
    public void TestFloat()
    {
        Lexer(" 1.5 ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.Float, new TokenSpan(1, 3, 0, 1), 1.5),
            (TokenKind.WhiteSpace, new TokenSpan(4, 1, 0, 4), null),
        });

        Lexer(" 0.5 ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.Float, new TokenSpan(1, 3, 0, 1), 0.5),
            (TokenKind.WhiteSpace, new TokenSpan(4, 1, 0, 4), null),
        });

        Lexer(" 1_1.2_5 ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.Float, new TokenSpan(1, 7, 0, 1), 1_1.2_5),
            (TokenKind.WhiteSpace, new TokenSpan(8, 1, 0, 8), null),
        });

        Lexer(" 2.1e10 ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.Float, new TokenSpan(1, 6, 0, 1), 2.1e10),
            (TokenKind.WhiteSpace, new TokenSpan(7, 1, 0, 7), null),
        });

        Lexer(" 2.1e+10 ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.Float, new TokenSpan(1, 7, 0, 1), 2.1e+10),
            (TokenKind.WhiteSpace, new TokenSpan(8, 1, 0, 8), null),
        });

        Lexer(" 2.1e-10 ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.Float, new TokenSpan(1, 7, 0, 1), 2.1e-10),
            (TokenKind.WhiteSpace, new TokenSpan(8, 1, 0, 8), null),
        });

        Lexer(" 3e5 ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.Float, new TokenSpan(1, 3, 0, 1), 3e5),
            (TokenKind.WhiteSpace, new TokenSpan(4, 1, 0, 4), null),
        });

        Lexer(" 3E5 ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.Float, new TokenSpan(1, 3, 0, 1), 3E5),
            (TokenKind.WhiteSpace, new TokenSpan(4, 1, 0, 4), null),
        });
        
        Lexer(" 1. ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.Integer, new TokenSpan(1, 1, 0, 1), 1),
            (TokenKind.Dot, new TokenSpan(2, 1, 0, 2), null),
            (TokenKind.WhiteSpace, new TokenSpan(3, 1, 0, 3), null),
        });

        Lexer(" 1e2. ", new()
        {
            (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
            (TokenKind.Float, new TokenSpan(1, 3, 0, 1), 1e2),
            (TokenKind.Dot, new TokenSpan(4, 1, 0, 4), null),
            (TokenKind.WhiteSpace, new TokenSpan(5, 1, 0, 5), null),
        });

        Lexer(" 1e1000 ", new()
            {
                (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
                (TokenKind.Float, new TokenSpan(1, 6, 0, 1), double.PositiveInfinity),
                (TokenKind.WhiteSpace, new TokenSpan(7, 1, 0, 7), null),
            }
        );

        // Check errors

        Lexer(" 1_.5 ", new()
            {
                (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
                (TokenKind.Float, new TokenSpan(1, 4, 0, 1), 1.5),
                (TokenKind.WhiteSpace, new TokenSpan(5, 1, 0, 5), null),
            }, new()
            {
                (DiagnosticId.ERR_UnexpectedUnderscoreAfterDigit, new TextSpan(new TextLocation(2, 0, 2)))
            }
        );

        Lexer(" 1.5_ ", new()
            {
                (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
                (TokenKind.Float, new TokenSpan(1, 4, 0, 1), 1.5),
                (TokenKind.WhiteSpace, new TokenSpan(5, 1, 0, 5), null),
            }, new()
            {
                (DiagnosticId.ERR_UnexpectedUnderscoreAfterDigit, new TextSpan(new TextLocation(4, 0, 4)))
            }
        );

        Lexer(" 1ex ", new()
            {
                (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
                (TokenKind.Float, new TokenSpan(1, 2, 0, 1), 1.0),
                (TokenKind.Identifier, new TokenSpan(3, 1, 0, 3), "x"),
                (TokenKind.WhiteSpace, new TokenSpan(4, 1, 0, 4), null),
            }, new()
            {
                (DiagnosticId.ERR_UnexpectedCharacterForExponent, new TextSpan(new TextLocation(3, 0, 3)))
            }
        );
        
        Lexer(" 1e2_ ", new()
            {
                (TokenKind.WhiteSpace, new TokenSpan(0, 1, 0, 0), null),
                (TokenKind.Float, new TokenSpan(1, 4, 0, 1), 1e2),
                (TokenKind.WhiteSpace, new TokenSpan(5, 1, 0, 5), null),
            }, new()
            {
                (DiagnosticId.ERR_UnexpectedUnderscoreAfterDigit, new TextSpan(new TextLocation(4, 0, 4)))
            }
        );
    }

    // Internals

    public TestLexer()
    {
        _manager = new VirtualArenaManager();
        _lio = new LexerInputOutput(_manager);
        _lexer = new Lexer(_lio);
    }


    [SetUp]
    public void BeforeTest()
    {
        _lio.Reset();
    }

    [OneTimeTearDown]
    public void Teardown()
    {
        _manager.Dispose();
    }

    private void Lexer(object input, List<(TokenKind, TokenSpan, object?)> tokens, List<(DiagnosticId, TextSpan)>? diagnosticIds = null)
    {
        _lio.Reset();
        if (input is string inputAsString)
        {
            _lexer.Run(inputAsString);
        }
        else if (input is byte[] inputAsBytes)
        {
            _lexer.Run(new MemoryStream(inputAsBytes));
        }
        else
        {
            throw new ArgumentException($"Invalid input type {input.GetType().FullName}. Expecting a string or bytes", nameof(input));
        }


        if (tokens.Count != _lio.Tokens.Count)
        {
            Console.WriteLine($"Error while lexing: {input}");
            DumpTokens();
            DumpExpectedTokens(tokens);
        }
        Assert.AreEqual(tokens.Count, _lio.Tokens.Count, "Invalid number of tokens");

        for (int i = 0; i < _lio.Tokens.Count; i++)
        {
            var expected = tokens[i];

            // Check TokenKind
            if (_lio.Tokens[i] != expected.Item1)
            {
                Console.WriteLine($"Error while lexing: {input}");
                DumpTokens();
                DumpExpectedTokens(tokens);
            }
            Assert.AreEqual(expected.Item1, _lio.Tokens[i], $"Invalid token kind at index {i}");

            // Check TokenSpan
            if (_lio.TokenSpans[i] != expected.Item2)
            {
                Console.WriteLine($"Error while lexing: {input}");
                DumpTokens();
                DumpExpectedTokens(tokens);
            }
            Assert.AreEqual(expected.Item2, _lio.TokenSpans[i], $"Invalid token span at index {i}");

            // Check TokenKind
            var tokenValue = _lio.TokenValues[i];
            if (expected.Item3 is string str)
            {
                if (tokenValue.Data == 0)
                {
                    Console.WriteLine($"Error while lexing: {input} - No TokenValue string or identifier found while expecting `{str}`");
                    DumpTokens();
                    DumpExpectedTokens(tokens);
                }
                else
                {
                    var expectedUtf8 = Encoding.UTF8.GetBytes(str);
                    var utf8HandleFound = tokenValue.AsStringHandle();
                    var utf8Found = _lio.GetString(utf8HandleFound);
                    var resultStr = utf8Found.ToString();
                    if (string.CompareOrdinal(str, resultStr) != 0)
                    {
                        Console.WriteLine($"Error while lexing: {input}");
                        DumpTokens();
                        DumpExpectedTokens(tokens);
                        Assert.AreEqual(str, resultStr, $"Invalid TokenValue string or identifier found at index {i}");
                    }
                }
            }
            else
            {
                var expectedValue = GetTokenValue(expected.Item3);
                if (tokenValue != expectedValue)
                {
                    Console.WriteLine($"Error while lexing: {input}");
                    DumpTokens();
                    DumpExpectedTokens(tokens);
                }
                Assert.AreEqual(expectedValue, tokenValue, $"Invalid TokenValue at index {i}");
            }
        }

        if (!_lio.Diagnostics.Select(x => x.Message.Id).SequenceEqual(diagnosticIds is null ? new List<DiagnosticId>() : diagnosticIds.Select(x => x.Item1)) || 
            !_lio.Diagnostics.Select(GetSpanFromLocation).SequenceEqual(diagnosticIds is null ? new List<TextSpan>() : diagnosticIds.Select(x => x.Item2)))
        {
            Console.WriteLine($"Diagnostic not matching while lexing: {input}");
            Console.WriteLine("Result:");
            Console.WriteLine("----------------------------------");
            foreach (var diag in _lio.Diagnostics)
            {
                Console.WriteLine($"{diag.Message.Id} {GetSpanFromLocation(diag)} => {diag.Message.Text}");
            }

            Console.WriteLine("Expected:");
            Console.WriteLine("----------------------------------");
            if (diagnosticIds is not null)
            {
                foreach (var diag in diagnosticIds)
                {
                    Console.WriteLine($"{diag.Item1} {diag.Item2}");
                }
            }

            Assert.Fail("Diagnostics are not matching");
        }
    }

    private TextSpan GetSpanFromLocation(Diagnostic diag)
    {
        if (diag.Location is DiagnosticSourceFileLocation fileLocation)
        {
            return fileLocation.Span;
        }
        return default;
    }

    private TextSpan GetSpanFromTokenSpan(TokenSpan span)
    {
        var startLocation = new TextLocation(span.Offset, span.Line, span.Column);
        var endLocation = new TextLocation(span.Offset + span.Length - 1, span.Line, span.Column + span.Length - 1);
        return new TextSpan(startLocation, endLocation);
    }

    private void DumpTokens()
    {
        Console.WriteLine("Result:");
        Console.WriteLine("----------------------------------");

        for (int i = 0; i < _lio.Tokens.Count; i++)
        {
            Console.WriteLine($"[{i,00}] {_lio.Tokens[i],-20}, {_lio.TokenSpans[i], -30}, {_lio.TokenValues[i], -20}");
        }
        Console.WriteLine();
    }

    private void DumpExpectedTokens(List<(TokenKind, TokenSpan, object?)> tokens)
    {
        Console.WriteLine("Expected:");
        Console.WriteLine("----------------------------------");
        int i = 0;
        foreach (var tokenGroup in tokens)
        {
            Console.WriteLine($"[{i,00}] {tokenGroup.Item1,-20}, {tokenGroup.Item2,-30}, {GetTokenValue(tokenGroup.Item3),-20}");
            i++;
        }
        Console.WriteLine();
    }

    private TokenValue GetTokenValue(object? value)
    {
        if (value is null)
        {
            return new TokenValue(0);
        }
        else if (value is string)
        {
            return new TokenValue(ulong.MaxValue);
        }
        else
        {
            return value is double f64Value ? new TokenValue(f64Value) : new TokenValue(Convert.ToUInt64(value));
        }
    }
}
