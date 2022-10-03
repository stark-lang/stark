// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Text;
using BenchmarkDotNet.Attributes;
using Microsoft.CodeAnalysis.CSharp;
using Stark.Compiler.Parsing;
using Varena;

namespace Stark.Compiler.BenchApp;

/// <summary>
/// Benchmark of Stark lexer vs Roslyn lexer.
/// 
/// The comparison is not completely fair for Stark, as it is parsing C# keywords, so it will generate a lot more identifiers than C#
/// But as the computation is relatively the same if it is a keyword or an identifier (expect it needs to store the identifier)
/// we keep it simple.
/// </summary>
public class BenchLexer
{
    private readonly Random _random;
    private readonly StringBuilder _builder;
    private readonly string _csharpText;
    private readonly List<(Microsoft.CodeAnalysis.CSharp.SyntaxKind, Microsoft.CodeAnalysis.Text.TextSpan, object?)> _roslynTokens;
    private readonly Stark.Compiler.Parsing.LexerInputOutput _lio;
    private readonly Lexer _lexer;
    private readonly MemoryStream _starkText;

    public BenchLexer()
    {
        _random = new Random(1);
        // 10MB
        _builder = new StringBuilder(10 << 20);
        _csharpText = PrepareCode(true);
        _roslynTokens = new List<(Microsoft.CodeAnalysis.CSharp.SyntaxKind, Microsoft.CodeAnalysis.Text.TextSpan, object?)>();
        var vm = new VirtualArenaManager();
        _lio = new LexerInputOutput(vm);
        _lexer = new Lexer(_lio);

        _starkText = new MemoryStream(Encoding.UTF8.GetBytes(_csharpText));
    }

    [Benchmark]
    public void Roslyn()
    {
        _roslynTokens.Clear();

        foreach (var token in Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ParseTokens(_csharpText))
        {
            _roslynTokens.Add((token.Kind(), token.Span, token.Value));

            if (token.HasLeadingTrivia)
            {
                foreach (var trivia in token.LeadingTrivia)
                {
                    _roslynTokens.Add((trivia.Kind(), trivia.Span, null));
                }
            }

            if (token.HasTrailingTrivia)
            {
                foreach (var trivia in token.TrailingTrivia)
                {
                    _roslynTokens.Add((trivia.Kind(), trivia.Span, null));
                }
            }
        }
        // Console.WriteLine($"Roslyn {_roslynTokens.Count} tokens");
    }

    [Benchmark]
    public void Stark()
    {
        _lio.Reset();
        _starkText.Position = 0;
        _lexer.Run(_starkText);
        // Console.WriteLine($"Stark {_lio.Tokens.Count} tokens");
    }

    private string PrepareCode(bool isCsharp)
    {
        var totalCount = SyntaxStats.Select(x => x.Item3).Sum();
        var tokenList = SyntaxStats.OrderByDescending(x => x.Item3).Select(x => (x.Item1, (double)x.Item2 / x.Item3, (double)x.Item3 / totalCount, 0.0)).ToList();
        for (var i = 0; i < tokenList.Count; i++)
        {
            var tokenPair = tokenList[i];
            for (var j = 0; j <= i; j++)
            {
                tokenPair.Item4 += tokenList[j].Item3;
            }
            tokenList[i] = tokenPair;
            //totalPercent += tokenPair.Item3;
            ////if (SyntaxFacts.GetText(tokenPair.Item1) != string.Empty) continue;
            //Console.WriteLine($"{tokenPair.Item1} {tokenPair.Item2} {tokenPair.Item4 * 100}%");
        }

        _builder.Length = 0;

        var percents = tokenList.Select(x => x.Item4).ToList();

        //Console.WriteLine($"{totalPercent * 100}%");

        bool isPreviousKeywordOrIdentifier = false;
        while (true)
        {
            var percentage = _random.NextDouble();
            var index = percents.BinarySearch(percentage);
            if (index < 0)
            {
                index = ~index;
            }
            var kind = tokenList[index].Item1;
            var text = GetText(kind, isCsharp);
            if (_builder.Length + text.Length + 1> _builder.Capacity)
            {
                break;
            }

            var nextIsKeywordOrIdentifier = IsKeywordOrIdentifier(kind);
            if (nextIsKeywordOrIdentifier && isPreviousKeywordOrIdentifier)
            {
                _builder.Append(' ');
            }
            isPreviousKeywordOrIdentifier = nextIsKeywordOrIdentifier;

            _builder.Append(text);
        }

        return _builder.ToString();
    }

    /// <summary>
    /// <c>true</c> if the kind is a keyword or an identifier.
    /// </summary>
    private static bool IsKeywordOrIdentifier(SyntaxKind kind)
    {
        return SyntaxFacts.IsKeywordKind(kind) || kind == SyntaxKind.IdentifierToken;
    }

    /// <summary>
    /// Get the text associated to a specific SyntaxKind
    /// </summary>
    private static string GetText(SyntaxKind kind, bool isCsharp)
    {
        switch (kind)
        {
            case SyntaxKind.WhitespaceTrivia:
                return "     ";
            //  5.663806850820647  24.89375682562034%
            case SyntaxKind.IdentifierToken:
                return "abcDEFgZ_1";
            // 10.885354429437067 21.51937860751891 %
            case SyntaxKind.EndOfLineTrivia:
                return "\r\n";
            // 2 12.115911115803648 %
            case SyntaxKind.StringLiteralToken:
                return "\"01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789\"";
            // 99.28415660665608 1.5735030439692832 %
            case SyntaxKind.SingleLineCommentTrivia:
                return "// 012345678901234567890123456789012345678901234567890123456789\r\n";
            // 64.87759382725298 0.9787606129375346 %
            case SyntaxKind.NumericLiteralToken:
                return "59";
            // 2.0525428325492 0.8883035786671392 %
            case SyntaxKind.SingleLineDocumentationCommentTrivia:
                return "/// 01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789\r\n";
            // 228.0139344262295 0.09584088276680373 %
            case SyntaxKind.InterpolatedStringToken:
                return "\"01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789\"";
            // 198.84106483921983 0.01986996225230893 %
            case SyntaxKind.CharacterLiteralToken:
                return "'Z'";
            // 3.2856743926277576 0.018754437223383835 %
            case SyntaxKind.MultiLineCommentTrivia:
                return "/* 0123456789\r\n0123456789\r\n0123456789\r\n0123456789 */";
            // 41.819397993311036 0.003131849611723969 %
            default:
                return SyntaxFacts.GetText(kind);
        }
    }

    /// <summary>
    /// Statistics extracted from Roslyn codebase in order to be closer to reality
    /// (SyntaxKind, TotalLength, TotalCount)
    /// </summary>
    private static readonly (SyntaxKind, int, int)[] SyntaxStats = new[]
    {
        (SyntaxKind.IdentifierToken, 44727290, 4108942),
        (SyntaxKind.StringLiteralToken, 29829627, 300447),
        (SyntaxKind.WhitespaceTrivia, 26946405, 4757649),
        (SyntaxKind.SingleLineCommentTrivia, 12124714, 186886),
        (SyntaxKind.EndOfLineTrivia, 4626860, 2313430),
        (SyntaxKind.SingleLineDocumentationCommentTrivia, 4172655, 18300),
        (SyntaxKind.DotToken, 1125009, 1125009),
        (SyntaxKind.OpenParenToken, 1039686, 1039686),
        (SyntaxKind.CloseParenToken, 1039686, 1039686),
        //(SyntaxKind.DisabledTextTrivia, 923442, 986),
        (SyntaxKind.PublicKeyword, 768546, 128091),
        (SyntaxKind.InterpolatedStringToken, 754403, 3794),
        (SyntaxKind.CommaToken, 740607, 740607),
        (SyntaxKind.SemicolonToken, 693748, 693748),
        (SyntaxKind.ReturnKeyword, 441630, 73605),
        (SyntaxKind.NumericLiteralToken, 348140, 169614),
        (SyntaxKind.UsingKeyword, 291825, 58365),
        (SyntaxKind.OpenBraceToken, 286184, 286184),
        (SyntaxKind.CloseBraceToken, 286184, 286184),
        (SyntaxKind.EqualsToken, 285734, 285734),
        (SyntaxKind.StringKeyword, 270936, 45156),
        (SyntaxKind.PrivateKeyword, 238378, 34054),
        (SyntaxKind.VoidKeyword, 217380, 54345),
        (SyntaxKind.InternalKeyword, 208024, 26003),
        (SyntaxKind.NewKeyword, 206979, 68993),
        (SyntaxKind.ThisKeyword, 198024, 49506),
        (SyntaxKind.OverrideKeyword, 178144, 22268),
        (SyntaxKind.NullKeyword, 168940, 42235),
        (SyntaxKind.StaticKeyword, 144066, 24011),
        (SyntaxKind.OpenBracketToken, 136945, 136945),
        (SyntaxKind.CloseBracketToken, 136945, 136945),
        (SyntaxKind.ClassKeyword, 116505, 23301),
        (SyntaxKind.ColonToken, 115938, 115938),
        (SyntaxKind.ReadOnlyKeyword, 112248, 14031),
        (SyntaxKind.FalseKeyword, 110795, 22159),
        (SyntaxKind.BoolKeyword, 107752, 26938),
        (SyntaxKind.IfKeyword, 106706, 53353),
        (SyntaxKind.NamespaceKeyword, 84753, 9417),
        (SyntaxKind.CaseKeyword, 83264, 20816),
        (SyntaxKind.LessThanToken, 81387, 81387),
        (SyntaxKind.GreaterThanToken, 81330, 81330),
        (SyntaxKind.ProtectedKeyword, 79965, 8885),
        (SyntaxKind.IntKeyword, 79572, 26524),
        (SyntaxKind.EqualsGreaterThanToken, 78710, 39355),
        (SyntaxKind.TrueKeyword, 76492, 19123),
        (SyntaxKind.DefaultKeyword, 65268, 9324),
        (SyntaxKind.ObjectKeyword, 56886, 9481),
        (SyntaxKind.EqualsEqualsToken, 55776, 27888),
        (SyntaxKind.ForEachKeyword, 48727, 6961),
        (SyntaxKind.AbstractKeyword, 47200, 5900),
        //(SyntaxKind.RegionDirectiveTrivia, 44800, 1461),
        (SyntaxKind.ExclamationEqualsToken, 41962, 20981),
        (SyntaxKind.OutKeyword, 40203, 13401),
        (SyntaxKind.ElseKeyword, 34940, 8735),
        (SyntaxKind.ByteKeyword, 32008, 8002),
        (SyntaxKind.ThrowKeyword, 31320, 6264),
        //(SyntaxKind.NullableDirectiveTrivia, 29796, 1808),
        (SyntaxKind.TypeOfKeyword, 28188, 4698),
        (SyntaxKind.QuestionToken, 27079, 27079),
        (SyntaxKind.AmpersandAmpersandToken, 26492, 13246),
        (SyntaxKind.SealedKeyword, 26226, 4371),
        (SyntaxKind.BreakKeyword, 26165, 5233),
        //(SyntaxKind.PragmaWarningDirectiveTrivia, 25443, 354),
        (SyntaxKind.MultiLineCommentTrivia, 25008, 598),
        (SyntaxKind.BaseKeyword, 22664, 5666),
        (SyntaxKind.ConstKeyword, 22515, 4503),
        (SyntaxKind.SwitchKeyword, 21972, 3662),
        (SyntaxKind.BarBarToken, 21320, 10660),
        (SyntaxKind.RefKeyword, 20508, 6836),
        (SyntaxKind.VirtualKeyword, 20146, 2878),
        //(SyntaxKind.EndRegionDirectiveTrivia, 18452, 1461),
        (SyntaxKind.ExclamationToken, 15785, 15785),
        (SyntaxKind.InKeyword, 15712, 7856),
        (SyntaxKind.IsKeyword, 13168, 6584),
        (SyntaxKind.ContinueKeyword, 13000, 1625),
        //(SyntaxKind.IfDirectiveTrivia, 12873, 1018),
        (SyntaxKind.CharacterLiteralToken, 11766, 3581),
        (SyntaxKind.UIntKeyword, 8548, 2137),
        (SyntaxKind.PlusToken, 8471, 8471),
        (SyntaxKind.InterfaceKeyword, 7902, 878),
        (SyntaxKind.ParamsKeyword, 7572, 1262),
        (SyntaxKind.ForKeyword, 7515, 2505),
        (SyntaxKind.WhileKeyword, 6955, 1391),
        (SyntaxKind.StructKeyword, 6414, 1069),
        (SyntaxKind.PlusPlusToken, 6380, 3190),
        //(SyntaxKind.EndIfDirectiveTrivia, 6306, 1018),
        (SyntaxKind.AsKeyword, 5942, 2971),
        (SyntaxKind.CatchKeyword, 5625, 1125),
        (SyntaxKind.CharKeyword, 5164, 1291),
        (SyntaxKind.QuestionQuestionToken, 4628, 2314),
        (SyntaxKind.MinusToken, 4290, 4290),
        (SyntaxKind.TryKeyword, 4212, 1404),
        (SyntaxKind.BarToken, 3986, 3986),
        (SyntaxKind.GreaterThanEqualsToken, 3378, 1689),
        (SyntaxKind.UShortKeyword, 3000, 500),
        (SyntaxKind.FinallyKeyword, 2954, 422),
        (SyntaxKind.LongKeyword, 2424, 606),
        (SyntaxKind.DoubleKeyword, 2400, 400),
        (SyntaxKind.LockKeyword, 2172, 543),
        (SyntaxKind.EnumKeyword, 2120, 530),
        (SyntaxKind.UncheckedKeyword, 2061, 229),
        (SyntaxKind.PlusEqualsToken, 1946, 973),
        (SyntaxKind.ULongKeyword, 1785, 357),
        (SyntaxKind.OperatorKeyword, 1752, 219),
        (SyntaxKind.LessThanEqualsToken, 1662, 831),
        (SyntaxKind.ShortKeyword, 1520, 304),
        (SyntaxKind.AmpersandToken, 1456, 1456),
        (SyntaxKind.DelegateKeyword, 1432, 179),
        (SyntaxKind.GotoKeyword, 1372, 343),
        (SyntaxKind.BarEqualsToken, 1356, 678),
        (SyntaxKind.LessThanLessThanToken, 1272, 636),
        (SyntaxKind.EventKeyword, 1235, 247),
        (SyntaxKind.DecimalKeyword, 1071, 153),
        (SyntaxKind.AsteriskToken, 1009, 1009),
        (SyntaxKind.UnsafeKeyword, 966, 161),
        (SyntaxKind.SByteKeyword, 920, 184),
        (SyntaxKind.FloatKeyword, 800, 160),
        (SyntaxKind.MinusMinusToken, 698, 349),
        (SyntaxKind.MinusEqualsToken, 682, 341),
        (SyntaxKind.QuestionQuestionEqualsToken, 663, 221),
        (SyntaxKind.ExternKeyword, 636, 106),
        //(SyntaxKind.ElseDirectiveTrivia, 575, 115),
        (SyntaxKind.SizeOfKeyword, 498, 83),
        (SyntaxKind.ImplicitKeyword, 424, 53),
        //(SyntaxKind.ElifDirectiveTrivia, 310, 17),
        (SyntaxKind.AmpersandEqualsToken, 280, 140),
        (SyntaxKind.DoKeyword, 264, 132),
        (SyntaxKind.TildeToken, 250, 250),
        //(SyntaxKind.ErrorDirectiveTrivia, 224, 7),
        (SyntaxKind.FixedKeyword, 210, 42),
        (SyntaxKind.CheckedKeyword, 210, 30),
        (SyntaxKind.SlashToken, 182, 182),
        //(SyntaxKind.DefineDirectiveTrivia, 161, 6),
        (SyntaxKind.ColonColonToken, 136, 68),
        (SyntaxKind.ExplicitKeyword, 136, 17),
        (SyntaxKind.PercentToken, 100, 100),
        (SyntaxKind.CaretToken, 90, 90),
        (SyntaxKind.VolatileKeyword, 88, 11),
        //(SyntaxKind.LineDirectiveTrivia, 70, 4),
        (SyntaxKind.AsteriskEqualsToken, 40, 20),
        (SyntaxKind.DotDotToken, 28, 14),
        (SyntaxKind.MinusGreaterThanToken, 22, 11),
        (SyntaxKind.LessThanLessThanEqualsToken, 18, 6),
        (SyntaxKind.CaretEqualsToken, 12, 6),
        (SyntaxKind.SlashEqualsToken, 8, 4),
        //(SyntaxKind.BadToken, 6, 6),
        (SyntaxKind.PercentEqualsToken, 4, 2),
        //(SyntaxKind.EndOfFileToken, 0, 9407),
    };
}