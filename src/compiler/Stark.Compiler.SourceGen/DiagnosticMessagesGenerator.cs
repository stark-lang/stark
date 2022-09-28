using Microsoft.CodeAnalysis;
using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;

namespace Stark.Compiler.SourceGen;

/// <summary>
/// This generator generates the DiagnosticMessages class from the DiagnosticId enum.
/// </summary>
[Generator]
public class DiagnosticMessagesGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // We make sure to not depend on CompilationProvider to avoid having this generator being called on every key stroke for any files in a project.
        var enumDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(predicate: static (s, _) => s is EnumDeclarationSyntax enumDecl && enumDecl.Identifier.Text == "DiagnosticId",
                transform: static (context, _) => (EnumDeclarationSyntax)context.Node);

        context.RegisterSourceOutput(enumDeclarations, static (spc, source) => Execute(source, spc));
    }

    private static void Execute(EnumDeclarationSyntax diagnosticIdEnum, SourceProductionContext context)
    {
        var enumsToGenerate = new List<DiagnosticEnumToGenerate>();
        EnumMemberDeclarationSyntax? currentMember = null;

        foreach (var member1 in diagnosticIdEnum.Members.GetWithSeparators())
        {
            if (member1.IsToken && currentMember != null)
            {
                Log($"Token: {member1.AsToken().ToFullString()}");
                var comment = member1.AsToken().TrailingTrivia.FirstOrDefault(x => x.IsKind(SyntaxKind.SingleLineCommentTrivia));
                if (comment != null)
                {
                    var commentAsText = comment.ToFullString();
                    if (commentAsText.StartsWith("// "))
                    {
                        enumsToGenerate.Add(new DiagnosticEnumToGenerate(currentMember.Identifier.Text, commentAsText));
                    }
                }
            }
            else if (member1.IsNode)
            {
                var node = member1.AsNode()!;
                 Log($"Syntax ({node.Kind()}: {node.ToFullString()}");
                currentMember = node as EnumMemberDeclarationSyntax;
            }
        }

        // If there were errors in the EnumDeclarationSyntax, we won't create an
        // EnumToGenerate for it, so make sure we have something to generate
        if (enumsToGenerate.Count > 0)
        {
            // generate the source code and add it to the output
            string result = GenerateExtensionClass(enumsToGenerate);
            Log($"We have some stuff to generate: {result}");

            context.AddSource("DiagnosticMessages.g.cs", SourceText.From(result, Encoding.UTF8));
        }
    }

    private static string GenerateExtensionClass(List<DiagnosticEnumToGenerate> enumsToGenerate)
    {
        var sb = new StringBuilder();
        sb.Append(@"// This file is auto-generated from DiagnosticId
namespace Stark.Compiler.Diagnostics;

public static partial class DiagnosticMessages
{");
        foreach (var enumToGenerate in enumsToGenerate)
        {
            // public static DiagnosticMessage ERR_InvalidCharacter(string c) =>
                // new(DiagnosticId.ERR_InvalidCharacter, $"The character `{c}` is invalid in a string");
                var arguments = enumToGenerate.Description.Substring("// ".Length);
            string description = arguments;
            if (arguments.StartsWith("("))
            {
                var index = arguments.IndexOf(")");
                if (index > 0)
                {
                    description = arguments.Substring(index + 1);
                    description = description.Trim();
                    arguments = arguments.Substring(0, index + 1);
                    if (description.StartsWith("=>"))
                    {
                        description = description.Substring("=>".Length).Trim();
                    }
                }
            }
            else
            {
                arguments = "()";
            }

            sb.Append($@"
    public static DiagnosticMessage {enumToGenerate.Name}{arguments} =>
        new(DiagnosticId.{enumToGenerate.Name}, $""{description}"");
");
        }

        sb.Append(@"
}
");

        // Normalize new lines
        return sb.ToString().Replace("\r\n", "\n").Replace("\n", Environment.NewLine);
    }

    [Conditional("DEBUG")]
    private static void Log(string message)
    {
        // Uncomment this line to get some log
        //File.AppendAllText(@"C:\code\stark\stark\src\Stark.Compiler\Stark.Compiler\roslyn.log", $"{DateTime.Now} {message}{Environment.NewLine}");
    }

    public readonly struct DiagnosticEnumToGenerate
    {
        public readonly string Name;
        public readonly string Description;

        public DiagnosticEnumToGenerate(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
