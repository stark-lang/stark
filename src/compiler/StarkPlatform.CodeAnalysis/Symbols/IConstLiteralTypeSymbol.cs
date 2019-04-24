namespace StarkPlatform.CodeAnalysis
{
    public interface IConstLiteralTypeSymbol : ITypeWithElementTypeSymbol
    {
        object Value { get; }
    }
}
