namespace StarkPlatform.CodeAnalysis
{
    public interface ITypeWithElementTypeSymbol : ITypeSymbol
    {
        ITypeSymbol ElementType { get; }
    }
}
