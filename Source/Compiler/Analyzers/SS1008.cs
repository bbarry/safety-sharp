// The MIT License (MIT)
// 
// Copyright (c) 2014-2015, Institute for Software & Systems Engineering
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

namespace SafetySharp.CSharpCompiler.Analyzers
{
	using System;
	using System.Linq;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using Microsoft.CodeAnalysis.Diagnostics;
	using Roslyn;
	using Roslyn.Syntax;

	/// <summary>
	///     Ensures that no type, type member, variable, etc. uses a name reserved for synthesized variables.
	/// </summary>
	[DiagnosticAnalyzer]
	public class SS1008 : CSharpAnalyzer
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public SS1008()
		{
			Error(1008,
				"The identifier name is reserved for internal use.",
				"Identifier name '{0}' is reserved for internal use.");
		}

		/// <summary>
		///     Called once at session start to register actions in the analysis context.
		/// </summary>
		/// <param name="context">The analysis context that should be used to register analysis actions.</param>
		public override void Initialize(AnalysisContext context)
		{
			context.RegisterSyntaxTreeAction(Analyze);
		}

		/// <summary>
		///     Performs the analysis.
		/// </summary>
		/// <param name="context">The context in which the analysis should be performed.</param>
		private void Analyze(SyntaxTreeAnalysisContext context)
		{
			var syntaxTree = context.Tree;

			CheckIdentifiers<BaseTypeDeclarationSyntax>(context, syntaxTree, t => t.Identifier);
			CheckIdentifiers<EnumMemberDeclarationSyntax>(context, syntaxTree, e => e.Identifier);
			CheckIdentifiers<ParameterSyntax>(context, syntaxTree, p => p.Identifier);
			CheckIdentifiers<VariableDeclaratorSyntax>(context, syntaxTree, v => v.Identifier);
			CheckIdentifiers<MethodDeclarationSyntax>(context, syntaxTree, m => m.Identifier);
			CheckIdentifiers<EventDeclarationSyntax>(context, syntaxTree, e => e.Identifier);
			CheckIdentifiers<PropertyDeclarationSyntax>(context, syntaxTree, p => p.Identifier);

			var invalidNamespaces = syntaxTree
				.Descendants<NamespaceDeclarationSyntax>()
				.SelectMany(n => n.Descendants<IdentifierNameSyntax>())
				.Where(identifierName => IdentifierNameSynthesizer.IsSynthesized(identifierName.Identifier));

			foreach (var invalidNamespace in invalidNamespaces)
				EmitDiagnostic(context, invalidNamespace.Identifier, invalidNamespace.Identifier.ValueText);
		}

		/// <summary>
		///     Checks the identifier of all nodes of type <typeparamref name="T" /> within <paramref name="syntaxTree" />.
		///     The <paramref name="getIdentifier" /> function is used to extract the identifier
		///     <see cref="SyntaxToken" /> from the <see cref="SyntaxNode" />.
		/// </summary>
		/// <typeparam name="T">The type of the <see cref="SyntaxNode" />s that should be checked.</typeparam>
		/// <param name="context">The context in which the analysis should be performed.</param>
		/// <param name="syntaxTree">The <see cref="SyntaxTree" /> that should be checked.</param>
		/// <param name="getIdentifier">
		///     A function that gets the <see cref="SyntaxToken" /> representing the identifier of a
		///     <see cref="SyntaxNode" /> of type <typeparamref name="T" />.
		/// </param>
		private void CheckIdentifiers<T>(SyntaxTreeAnalysisContext context, SyntaxTree syntaxTree, Func<T, SyntaxToken> getIdentifier)
			where T : CSharpSyntaxNode
		{
			var invalidIdentifiers = syntaxTree
				.Descendants<T>()
				.Select(getIdentifier)
				.Where(IdentifierNameSynthesizer.IsSynthesized);

			foreach (var identifier in invalidIdentifiers)
				EmitDiagnostic(context, identifier, identifier.ValueText);
		}
	}
}