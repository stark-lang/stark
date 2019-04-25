namespace StarkPlatform.Compiler
{
    public interface IConstLiteralTypeSymbol : ITypeWithElementTypeSymbol
    {
        object Value { get; }
    }
}
