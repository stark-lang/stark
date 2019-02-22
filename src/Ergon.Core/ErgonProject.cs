using System;
using System.IO;
using System.Text;
using Tomlyn;
using Tomlyn.Syntax;

namespace Ergon.Core
{
    public sealed class ErgonProject
    {
        public const string DefaultFileName = "ergon.tonl";
        
        private ErgonProject()
        {
            Diagnostics = new DiagnosticsBag();
            Package = new ErgonPackageDeclaration();
        }

        public DiagnosticsBag Diagnostics { get; }

        public bool HasErrors => Diagnostics.HasErrors;
        
        public ErgonPackageDeclaration Package { get; }


        public static ErgonProject FromFile(string filePath)
        {
            if (filePath == null) throw new ArgumentNullException(nameof(filePath));
            var fullFilePath = Path.Combine(Environment.CurrentDirectory, filePath);
            var ergonProject = new ErgonProject();

            if (!File.Exists(fullFilePath))
            {
                ergonProject.Diagnostics.Error(new SourceSpan(filePath, new TextPosition(), new TextPosition()), $"Unable to find ergon.toml file `{fullFilePath}");
                return ergonProject;
            }

            var content = File.ReadAllText(fullFilePath, Encoding.UTF8);
            ergonProject.Load(content, filePath);
            return ergonProject;
        }

        public static ErgonProject FromString(string content, string filePath = null)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            var ergonProject = new ErgonProject();
            ergonProject.Load(content, filePath);
            return ergonProject;
        }

        private void Load(string content, string filePath = null)
        {
            var doc = Toml.Parse(content, filePath);

            if (doc.Diagnostics.Count > 0)
            {
                foreach (var docDiagnostic in doc.Diagnostics)
                {
                    Diagnostics.Add(docDiagnostic);
                }
            }

            if (HasErrors)
            {
                return;
            }

            // Load package information
            Package.Load(doc, filePath, Diagnostics);
        }
    }
}