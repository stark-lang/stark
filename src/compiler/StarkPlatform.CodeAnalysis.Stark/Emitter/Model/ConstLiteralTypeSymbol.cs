using System.Reflection.Stark.Metadata;
using StarkPlatform.Cci;
using StarkPlatform.CodeAnalysis.Emit;
using StarkPlatform.CodeAnalysis.Stark.Emit;
using PrimitiveTypeCode = StarkPlatform.Cci.PrimitiveTypeCode;

namespace StarkPlatform.CodeAnalysis.Stark.Symbols
{
    internal sealed partial class ConstLiteralTypeSymbol : IConstLiteralTypeSymbol, IConstLiteralTypeReference
    {
        public ITypeSymbol ElementType => _elementType.TypeSymbol;

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

        bool Cci.ITypeReference.IsEnum => false;

        bool Cci.ITypeReference.IsValueType => false;


        TypeDefinitionHandle Cci.ITypeReference.TypeDef => default(TypeDefinitionHandle);
        Cci.PrimitiveTypeCode Cci.ITypeReference.TypeCode => Cci.PrimitiveTypeCode.NotPrimitive;

        Cci.ITypeDefinition Cci.ITypeReference.GetResolvedType(EmitContext context) => null;
        Cci.IGenericMethodParameterReference Cci.ITypeReference.AsGenericMethodParameterReference => null;
        Cci.IGenericTypeInstanceReference Cci.ITypeReference.AsGenericTypeInstanceReference => null;
        Cci.IGenericTypeParameterReference Cci.ITypeReference.AsGenericTypeParameterReference => null;
        Cci.INamespaceTypeDefinition Cci.ITypeReference.AsNamespaceTypeDefinition(EmitContext context) => null;
        Cci.INamespaceTypeReference Cci.ITypeReference.AsNamespaceTypeReference => null;
        Cci.INestedTypeDefinition Cci.ITypeReference.AsNestedTypeDefinition(EmitContext context) => null;
        Cci.INestedTypeReference Cci.ITypeReference.AsNestedTypeReference => null;
        Cci.ISpecializedNestedTypeReference Cci.ITypeReference.AsSpecializedNestedTypeReference => null;
        Cci.ITypeDefinition Cci.ITypeReference.AsTypeDefinition(EmitContext context) => null;
        Cci.IDefinition Cci.IReference.AsDefinition(EmitContext context) => null;
    }
}
