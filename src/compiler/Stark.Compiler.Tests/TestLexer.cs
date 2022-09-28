using System;
using System.Collections.Generic;
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

    private void Lexer(string text, List<(TokenKind, TokenSpan, object?)> tokens, List<(DiagnosticId, TextSpan)>? diagnosticIds = null)
    {
        _lio.Reset();
        _lexer.Run(text);

        if (tokens.Count != _lio.Tokens.Count)
        {
            Console.WriteLine($"Error while lexing: {text}");
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
                Console.WriteLine($"Error while lexing: {text}");
                DumpTokens();
                DumpExpectedTokens(tokens);
            }
            Assert.AreEqual(expected.Item1, _lio.Tokens[i], $"Invalid token kind at index {i}");

            // Check TokenSpan
            if (_lio.TokenSpans[i] != expected.Item2)
            {
                Console.WriteLine($"Error while lexing: {text}");
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
                    Console.WriteLine($"Error while lexing: {text} - No TokenValue string or identifier found while expecting `{str}`");
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
                        Console.WriteLine($"Error while lexing: {text}");
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
                    Console.WriteLine($"Error while lexing: {text}");
                    DumpTokens();
                    DumpExpectedTokens(tokens);
                }
                Assert.AreEqual(expectedValue, tokenValue, $"Invalid TokenValue at index {i}");
            }
        }

        if (!_lio.Diagnostics.Select(x => x.Message.Id).SequenceEqual(diagnosticIds is null ? new List<DiagnosticId>() : diagnosticIds.Select(x => x.Item1)) || 
            !_lio.Diagnostics.Select(GetSpanFromLocation).SequenceEqual(diagnosticIds is null ? new List<TextSpan>() : diagnosticIds.Select(x => x.Item2)))
        {
            Console.WriteLine($"Diagnostic not matching while lexing: {text}");
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

    private static TokenValue GetTokenValue(object? value)
    {
        return value is null ? new TokenValue(0) : value is double f64Value ? new TokenValue(f64Value) : new TokenValue(Convert.ToUInt64(value));
    }
}
