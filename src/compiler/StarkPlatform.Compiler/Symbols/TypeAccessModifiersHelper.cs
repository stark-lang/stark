using System.Diagnostics;
using StarkPlatform.Reflection.Metadata;

namespace StarkPlatform.Compiler
{

    public static class TypeAccessModifiersHelper
    {
        public static bool IsNone(this TypeAccessModifiers modifiers) => modifiers == TypeAccessModifiers.None;

        public static bool IsReadOnly(this TypeAccessModifiers modifiers) => (modifiers & TypeAccessModifiers.ReadOnly) != 0;

        public static bool IsRef(this TypeAccessModifiers modifiers) => (modifiers & TypeAccessModifiers.Ref) != 0;

        public static bool IsTransient(this TypeAccessModifiers modifiers) => (modifiers & TypeAccessModifiers.Transient) != 0;

        public static TypeAccessModifiers GetRequiredAccessModifiers(this ITypeSymbol source, ITypeSymbol destination, out TypeAccessModifiers destModifiers)
        {
            Debug.Assert(source != null && destination != null);

            var sourceExtended = source as IExtendedTypeSymbol;
            var destinationExtended = destination as IExtendedTypeSymbol;

            var sourceModifiers = sourceExtended?.AccessModifiers ?? TypeAccessModifiers.None;
            destModifiers = destinationExtended?.AccessModifiers ?? TypeAccessModifiers.None;
            if (sourceModifiers == destModifiers)
            {
                return TypeAccessModifiers.None;
            }

            if ((destModifiers & TypeAccessModifiers.ReadOnly) != 0 && sourceModifiers == 0) sourceModifiers = TypeAccessModifiers.ReadOnly;


            var requiredModifiers = sourceModifiers ^ destModifiers;
            return requiredModifiers;
        }
    }
}
