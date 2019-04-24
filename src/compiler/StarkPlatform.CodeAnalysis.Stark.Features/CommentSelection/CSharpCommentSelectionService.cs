// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.CodeAnalysis.CommentSelection;
using StarkPlatform.CodeAnalysis.Host.Mef;

namespace StarkPlatform.CodeAnalysis.Editor.CSharp.CommentSelection
{
    [ExportLanguageService(typeof(ICommentSelectionService), LanguageNames.Stark), Shared]
    internal class CSharpCommentSelectionService : AbstractCommentSelectionService
    {
        public override string SingleLineCommentString => "//";
        public override bool SupportsBlockComment => true;
        public override string BlockCommentStartString => "/*";
        public override string BlockCommentEndString => "*/";
    }
}
