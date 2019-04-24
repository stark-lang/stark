using StarkPlatform.Cci;
using StarkPlatform.CodeAnalysis.Stark.Emit;
using StarkPlatform.CodeAnalysis.Emit;

namespace StarkPlatform.CodeAnalysis.Stark.Symbols
{
    internal sealed partial class ExtendedNamedTypeSymbol : IExtendedTypeReference, IExtendedTypeSymbol
    {
        public ITypeSymbol ElementType => _underlyingType;

        public ITypeReference GetElementType(EmitContext context)
        {
            var type = ((PEModuleBuilder)context.Module).Translate(this._elementType.TypeSymbol, syntaxNodeOpt: (CSharpSyntaxNode)context.SyntaxNodeOpt, diagnostics: context.Diagnostics);

            if (this._elementType.CustomModifiers.Length == 0)
            {
                return type;
            }
            else
            {
                return new Cci.ModifiedTypeReference(type, this._elementType.CustomModifiers.As<Cci.ICustomModifier>());
            }
        }

        public bool IsTransient => (_accessModifiers & TypeAccessModifiers.Transient) != 0 || _underlyingType.IsRefLikeType;

        TypeAccessModifiers IExtendedTypeReference.AccessModifiers => _accessModifiers;

    }
}
