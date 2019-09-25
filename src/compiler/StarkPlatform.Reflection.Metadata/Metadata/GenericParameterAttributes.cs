using System;

namespace StarkPlatform.Reflection.Metadata
{
    /// <summary>Describes the constraints on a generic type parameter of a generic type or method.</summary>
    [Flags]
    public enum GenericParameterAttributes
    {
        /// <summary>The generic type parameter is contravariant. A contravariant type parameter can appear as a parameter type in method signatures.</summary>
        /// <returns></returns>
        Contravariant = 2,

        /// <summary>The generic type parameter is covariant. A covariant type parameter can appear as the result type of a method, the type of a read-only field, a declared base type, or an implemented interface.</summary>
        /// <returns></returns>
        Covariant = 1,

        /// <summary>A type can be substituted for the generic type parameter only if it has a parameterless constructor.</summary>
        /// <returns></returns>
        DefaultConstructorConstraint = 16, // 0x00000010

        ConstConstraint = 0x20,

        /// <summary>There are no special flags.</summary>
        /// <returns></returns>
        None = 0,

        /// <summary>A type can be substituted for the generic type parameter only if it is a value type and is not nullable.</summary>
        /// <returns></returns>
        NotNullableValueTypeConstraint = 8,

        /// <summary>A type can be substituted for the generic type parameter only if it is a reference type.</summary>
        /// <returns></returns>
        ReferenceTypeConstraint = 4,

        /// <summary>Selects the combination of all special constraint flags. This value is the result of using logical OR to combine the following flags: <see cref="GenericParameterAttributes.DefaultConstructorConstraint"></see>, <see cref="GenericParameterAttributes.ReferenceTypeConstraint"></see>, and <see cref="GenericParameterAttributes.NotNullableValueTypeConstraint"></see>.</summary>
        /// <returns></returns>
        SpecialConstraintMask = ReferenceTypeConstraint | NotNullableValueTypeConstraint | DefaultConstructorConstraint, // 0x0000001C

        /// <summary>Selects the combination of all variance flags. This value is the result of using logical OR to combine the following flags: <see cref="GenericParameterAttributes.Contravariant"></see> and <see cref="GenericParameterAttributes.Covariant"></see>.</summary>
        /// <returns></returns>
        VarianceMask = Covariant | Contravariant, // 0x00000003

    }
}
