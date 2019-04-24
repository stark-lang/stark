using System.Reflection;

namespace StarkPlatform.CodeAnalysis
{
    public static class TypeAttributesExt
    {
        public const TypeAttributes ClassSemanticsMask = (TypeAttributes)0x00000060;
        public const TypeAttributes Struct = (TypeAttributes)0x00000040;
    }

    public static class MethodAttributesExt
    {
        public const MethodAttributes ReadOnly= (MethodAttributes)0x0004000;
    }

    public static class GenericParameterAttributesExt
    {
        public const GenericParameterAttributes ConstConstraint = (GenericParameterAttributes)0x20;
    }
}
