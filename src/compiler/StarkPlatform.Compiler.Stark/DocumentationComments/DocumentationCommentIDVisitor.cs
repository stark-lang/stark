// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Text;
using StarkPlatform.Compiler.Stark.Symbols;

namespace StarkPlatform.Compiler.Stark
{
    internal sealed partial class DocumentationCommentIDVisitor : CSharpSymbolVisitor<StringBuilder, object>
    {
        public static readonly DocumentationCommentIDVisitor Instance = new DocumentationCommentIDVisitor();

        private DocumentationCommentIDVisitor()
        {
        }

        public override object DefaultVisit(Symbol symbol, StringBuilder builder)
        {
            // We need to return something to API users, but this should never happen within Roslyn.
            return null;
        }

        public override object VisitNamespace(NamespaceSymbol symbol, StringBuilder builder)
        {
            if (!symbol.IsGlobalNamespace)
            {
                builder.Append("N:");
                PartVisitor.Instance.Visit(symbol, builder);
            }

            return null;
        }

        public override object VisitMethod(MethodSymbol symbol, StringBuilder builder)
        {
            builder.Append("M:");
            PartVisitor.Instance.Visit(symbol, builder);

            return null;
        }

        public override object VisitField(FieldSymbol symbol, StringBuilder builder)
        {
            builder.Append("F:");
            PartVisitor.Instance.Visit(symbol, builder);

            return null;
        }

        public override object VisitEvent(EventSymbol symbol, StringBuilder builder)
        {
            builder.Append("E:");
            PartVisitor.Instance.Visit(symbol, builder);

            return null;
        }

        public override object VisitProperty(PropertySymbol symbol, StringBuilder builder)
        {
            builder.Append("P:");
            PartVisitor.Instance.Visit(symbol, builder);

            return null;
        }

        public override object VisitNamedType(NamedTypeSymbol symbol, StringBuilder builder)
        {
            builder.Append("T:");
            PartVisitor.Instance.Visit(symbol, builder);

            return null;
        }

        public override object VisitErrorType(ErrorTypeSymbol symbol, StringBuilder builder)
        {
            builder.Append("!:");
            PartVisitor.Instance.Visit(symbol, builder);

            return null;
        }

        public override object VisitTypeParameter(TypeParameterSymbol symbol, StringBuilder builder)
        {
            builder.Append("!:");
            builder.Append(symbol.Name);

            return null;
        }
    }
}
