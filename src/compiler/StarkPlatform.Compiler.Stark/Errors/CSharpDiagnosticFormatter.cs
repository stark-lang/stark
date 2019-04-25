// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace StarkPlatform.Compiler.Stark
{
    public class CSharpDiagnosticFormatter : DiagnosticFormatter
    {
        internal CSharpDiagnosticFormatter()
        {
        }

        public new static CSharpDiagnosticFormatter Instance { get; } = new CSharpDiagnosticFormatter();
    }
}
