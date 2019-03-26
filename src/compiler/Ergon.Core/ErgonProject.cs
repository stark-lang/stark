using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using Tomlyn;
using Tomlyn.Syntax;

namespace Ergon.Core
{
    public sealed class ErgonProjectCollection
    {
        private readonly ImmutableDictionary<string, string> _globalProperties;

        public ErgonProjectCollection(ImmutableDictionary<string, string> globalProperties)
        {
            _globalProperties = globalProperties;
            Projects = new List<ErgonProject>();
        }


        public void Add(ErgonProject project)
        {
            Projects.Add(project);
        }

        public List<ErgonProject> Projects { get; }
        

        public List<ErgonProject> GetLoadedProjects(string path)
        {
            var projects = new List<ErgonProject>();
            foreach (var project in Projects)
            {
                if (project.FullPath == path)
                {
                    projects.Add(project);
                }
            }
            return projects;
        }

        public void UnloadAllProjects()
        {
            
        }
    }

    public sealed class ErgonProject
    {
        public const string DefaultFileName = "ergon.toml";
        
        private ErgonProject(string path)
        {
            Diagnostics = new DiagnosticsBag();
            Package = new ErgonPackageDeclaration();
            FullPath = path;
        }

        public DiagnosticsBag Diagnostics { get; }

        public bool HasErrors => Diagnostics.HasErrors;
        
        public ErgonPackageDeclaration Package { get; }


        public string FullPath { get; }

        public string DirectoryPath => Path.GetDirectoryName(FullPath);

        public static ErgonProject FromFile(string filePath)
        {
            if (filePath == null) throw new ArgumentNullException(nameof(filePath));
            var fullFilePath = Path.Combine(Environment.CurrentDirectory, filePath);
            var ergonProject = new ErgonProject(fullFilePath);

            if (!File.Exists(fullFilePath))
            {
                ergonProject.Diagnostics.Error(new SourceSpan(filePath, new TextPosition(), new TextPosition()), $"Unable to find {DefaultFileName} file `{fullFilePath}");
                return ergonProject;
            }

            var content = File.ReadAllText(fullFilePath, Encoding.UTF8);
            ergonProject.Load(content, filePath);
            return ergonProject;
        }

        public static ErgonProject FromString(string content, string filePath = null)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            var ergonProject = new ErgonProject(filePath);
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

        public IEnumerable<ErgonProjectItem> GetItems(string analyzer)
        {
            throw new NotImplementedException();
        }

        public void RemoveItem(ErgonProjectItem item)
        {
            throw new NotImplementedException();
        }

        public void AddItem(string type, string relativePath, Dictionary<string, string> metadatas = null)
        {
            throw new NotImplementedException();
        }

        public string GetPropertyValue(string targetFramework)
        {
            throw new NotImplementedException();
        }
        public void Save()
        {
            throw new NotImplementedException();
        }

        public ErgonProjectInstance CreateProjectInstance()
        {
            throw new NotImplementedException();
        }
    }



    public class ErgonProjectItem
    {



        public Dictionary<string, string> Metadata { get; }
        
        public string EvaluatedInclude { get; }

        public string GetMetadataValue(string name)
        {
            throw new NotImplementedException();
        }
    }

    public interface ITaskItem
    {
        string ItemSpec { get; set; }

        string EvaluatedInclude { get; set; }

        string GetMetadata(string link);
    }
    
    public class ErgonProjectInstance
    {

        public string FullPath { get; }

        public Dictionary<string, string> Targets { get; set; }


        public IEnumerable<ErgonProjectItemInstance> GetItems(string additionalFiles)
        {
            throw new NotImplementedException();
        }

        public ErgonProjectPropertyInstance GetProperty(string propertyName)
        {
            throw new NotImplementedException();
        }
    }

    public class ErgonProjectItemInstance : ITaskItem
    {
        public string ItemSpec { get; set; }

        public string EvaluatedInclude { get; set; }

        public string GetMetadata(string link)
        {
            throw new NotImplementedException();
        }

        public ImmutableArray<string> GetAliases()
        {
            throw new NotImplementedException();
        }
    }

    public class ErgonProjectPropertyInstance
    {
        public string EvaluatedValue { get; set; }
    }

    public class ErgonBuildResult
    {

    }
}