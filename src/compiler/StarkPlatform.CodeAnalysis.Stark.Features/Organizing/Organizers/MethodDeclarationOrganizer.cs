// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using System.Threading;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Organizing.Organizers;

namespace StarkPlatform.CodeAnalysis.Stark.Organizing.Organizers
{
    [ExportSyntaxNodeOrganizer(LanguageNames.Stark), Shared]
    internal class MethodDeclarationOrganizer : AbstractSyntaxNodeOrganizer<MethodDeclarationSyntax>
    {
        protected override MethodDeclarationSyntax Organize(
            MethodDeclarationSyntax syntax,
            CancellationToken cancellationToken)
        {
            return syntax.Update(
                attributeLists: syntax.AttributeLists,
                modifiers: ModifiersOrganizer.Organize(syntax.Modifiers),
                funcKeyword: syntax.FuncKeyword,
                returnToken: syntax.ReturnToken,
                returnType: syntax.ReturnType,
                explicitInterfaceSpecifier: syntax.ExplicitInterfaceSpecifier,
                identifier: syntax.Identifier,
                typeParameterList: syntax.TypeParameterList,
                parameterList: syntax.ParameterList,
                constraintClauses: syntax.ConstraintClauses,
                contractClauses: syntax.ContractClauses,
                throwsList: syntax.ThrowsList,
                body: syntax.Body,
                expressionBody: syntax.ExpressionBody,
                eosToken: syntax.EosToken);
        }
    }
}
