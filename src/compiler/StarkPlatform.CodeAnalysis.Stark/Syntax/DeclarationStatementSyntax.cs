// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace StarkPlatform.CodeAnalysis.Stark.Syntax
{
    public partial class LocalDeclarationStatementSyntax : StatementSyntax
    {
        public bool IsConst
        {
            get { return this.Declaration.VariableKeyword.Kind() == SyntaxKind.ConstKeyword; }
        }
        public bool IsLet
        {
            get { return this.Declaration.VariableKeyword.Kind() == SyntaxKind.LetKeyword; }
        }
    }
}
