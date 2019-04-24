
// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("ApiDesign", "RS0026:Do not add multiple public overloads with optional parameters", Justification = "<Pending>", Scope = "member", Target = "~M:StarkPlatform.CodeAnalysis.Compilation.CreateTupleTypeSymbol(System.Collections.Immutable.ImmutableArray{StarkPlatform.CodeAnalysis.ITypeSymbol},System.Collections.Immutable.ImmutableArray{System.String})~StarkPlatform.CodeAnalysis.INamedTypeSymbol")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("ApiDesign", "RS0026:Do not add multiple public overloads with optional parameters", Justification = "<Pending>", Scope = "member", Target = "~M:StarkPlatform.CodeAnalysis.Compilation.CreateTupleTypeSymbol(StarkPlatform.CodeAnalysis.INamedTypeSymbol,System.Collections.Immutable.ImmutableArray{System.String})~StarkPlatform.CodeAnalysis.INamedTypeSymbol")]

// These arise from modifications to existing API. The analysis seems too strict as the first
// required parameter type is different for all overloads and so there is no ambiguity. 
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("ApiDesign", "RS0026:Do not add multiple public overloads with optional parameters", Justification = "<Pending>", Scope = "member", Target = "~M:StarkPlatform.CodeAnalysis.Text.SourceText.From(System.IO.Stream,System.Text.Encoding,StarkPlatform.CodeAnalysis.Text.SourceHashAlgorithm,System.Boolean,System.Boolean)~StarkPlatform.CodeAnalysis.Text.SourceText")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("ApiDesign", "RS0026:Do not add multiple public overloads with optional parameters", Justification = "<Pending>", Scope = "member", Target = "~M:StarkPlatform.CodeAnalysis.Text.SourceText.From(System.Byte[],System.Int32,System.Text.Encoding,StarkPlatform.CodeAnalysis.Text.SourceHashAlgorithm,System.Boolean,System.Boolean)~StarkPlatform.CodeAnalysis.Text.SourceText")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("ApiDesign", "RS0026:Do not add multiple public overloads with optional parameters", Justification = "<Pending>", Scope = "member", Target = "~M:StarkPlatform.CodeAnalysis.Text.SourceText.From(System.IO.TextReader,System.Int32,System.Text.Encoding,StarkPlatform.CodeAnalysis.Text.SourceHashAlgorithm)~StarkPlatform.CodeAnalysis.Text.SourceText")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("ApiDesign", "RS0027:Public API with optional parameter(s) should have the most parameters amongst its public overloads.", Justification = "<Pending>", Scope = "member", Target = "~M:StarkPlatform.CodeAnalysis.Text.SourceText.From(System.String,System.Text.Encoding,StarkPlatform.CodeAnalysis.Text.SourceHashAlgorithm)~StarkPlatform.CodeAnalysis.Text.SourceText")]

