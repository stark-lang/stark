// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace StarkPlatform.CodeAnalysis.Stark
{
    internal partial class BoundConditionalOperator : IBoundConditional
    {
        BoundNode IBoundConditional.AlternativeOpt => this.Alternative;

        BoundNode IBoundConditional.Condition => this.Condition;

        BoundNode IBoundConditional.Consequence => this.Consequence;
    }
}
