using System.Collections.Generic;
using Tomlyn.Syntax;

namespace Ergon.Core
{
    public class ErgonPackageDeclaration
    {
        public ErgonPackageDeclaration()
        {
            Authors = new List<string>();
        }

        public string Name { get; set; }
        
        public string Version { get; set; }

        public List<string> Authors { get; }
        
        internal void Load(DocumentSyntax doc, string filePath, DiagnosticsBag diagnostics)
        {
            var table = doc.Tables.FindByName("package");
            if (table == null)
            {
                diagnostics.Error(new SourceSpan(filePath, new TextPosition(), new TextPosition()), $"Unable to find [package] section");
                return;
            }

            if (table.Kind != SyntaxKind.Table)
            {
                diagnostics.Error(new SourceSpan(filePath, new TextPosition(), new TextPosition()), $"Unable to find [package] section must be a table and not a table array [[package]]");
                return;
            }

            foreach (var item in table.Items)
            {
                var key = item.Key.AsText();
                switch (key)
                {
                    case "name":
                        Name = item.Value.AsObject()?.ToString();
                        break;
                    case "version":
                        Name = item.Value.AsObject()?.ToString();
                        break;
                    case "authors":
                        break;
                    default:
                        // log error
                        break;
                }
            }
        }
    }
}