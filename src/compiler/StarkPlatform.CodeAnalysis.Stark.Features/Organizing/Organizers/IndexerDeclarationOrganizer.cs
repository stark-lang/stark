// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using System.Threading;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Organizing.Organizers;

namespace StarkPlatform.CodeAnalysis.Stark.Organizing.Organizers
{
    [ExportSyntaxNodeOrganizer(LanguageNames.Stark), Shared]
    internal class IndexerDeclarationOrganizer : AbstractSyntaxNodeOrganizer<IndexerDeclarationSyntax>
    {
        protected override IndexerDeclarationSyntax Organize(
            IndexerDeclarationSyntax syntax,
            CancellationToken cancellationToken)
        {
            return syntax.Update(
                attributeLists: syntax.AttributeLists,
                modifiers: ModifiersOrganizer.Organize(syntax.Modifiers),
                returnToken: syntax.ReturnToken,
                type: syntax.Type,
                explicitInterfaceSpecifier: syntax.ExplicitInterfaceSpecifier,
                funcKeyword: syntax.FuncKeyword,
                operatorKeyword: syntax.OperatorKeyword,
                parameterList: syntax.ParameterList,
                accessorList: syntax.AccessorList,
                expressionBody: syntax.ExpressionBody,
                contractClauses: syntax.ContractClauses,
                eosToken: syntax.EosToken);
        }
    }
}
