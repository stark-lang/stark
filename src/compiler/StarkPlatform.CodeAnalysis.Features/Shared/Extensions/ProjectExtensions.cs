namespace StarkPlatform.CodeAnalysis.Shared.Extensions
{
    internal static class ProjectExtensions
    {
        public static Glyph GetGlyph(this Project project)
        {
            // TODO: Get the glyph from the hierarchy
            return project.Language == LanguageNames.Stark ? Glyph.StarkProject : Glyph.Assembly;
        }
    }
}
