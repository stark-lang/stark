// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.Stark.Symbols;

namespace StarkPlatform.CodeAnalysis.Stark
{
    internal sealed class SynthesizedClosureEnvironmentConstructor : SynthesizedInstanceConstructor, ISynthesizedMethodBodyImplementationSymbol
    {
        internal SynthesizedClosureEnvironmentConstructor(SynthesizedClosureEnvironment frame)
            : base(frame)
        {
        }

        IMethodSymbol ISynthesizedMethodBodyImplementationSymbol.Method
        {
            get { return ((ISynthesizedMethodBodyImplementationSymbol)this.ContainingSymbol).Method; }
        }

        bool ISynthesizedMethodBodyImplementationSymbol.HasMethodBodyDependency
        {
            get { return false; }
        }
    }
}
