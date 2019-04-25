namespace StarkPlatform.Compiler
{
    public interface ITypeWithElementTypeSymbol : ITypeSymbol
    {
        ITypeSymbol ElementType { get; }
    }
}
