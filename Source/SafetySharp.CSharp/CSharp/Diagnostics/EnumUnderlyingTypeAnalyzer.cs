﻿// The MIT License (MIT)
// 
// Copyright (c) 2014, Institute for Software & Systems Engineering
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

namespace SafetySharp.CSharp.Diagnostics
{
	using System;
	using System.Threading;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using Microsoft.CodeAnalysis.Diagnostics;

	/// <summary>
	///     Ensures that no enumerations explicitly declare an underlying type.
	/// </summary>
	[DiagnosticAnalyzer]
	[ExportDiagnosticAnalyzer(DiagnosticIdentifier, LanguageNames.CSharp)]
	internal class EnumUnderlyingTypeAnalyzer : SyntaxNodeAnalyzer<EnumDeclarationSyntax>
	{
		private const string DiagnosticIdentifier = Compiler.DiagnosticsPrefix + "1001";

		/// <summary>
		///     Initializes a new instance of the <see cref="EnumUnderlyingTypeAnalyzer" /> type.
		/// </summary>
		public EnumUnderlyingTypeAnalyzer()
		{
			Error(DiagnosticIdentifier,
				  "Enumeration declarations must not explicitly declare an underlying type.",
				  "Enum '{0}' must not declare an underlying type.");
		}

		/// <summary>
		///     Analyzes the <paramref name="syntaxNode"/>.
		/// </summary>
		/// <param name="syntaxNode">The syntax node that should be analyzed.</param>
		/// <param name="addDiagnostic">A delegate that should be used to emit diagnostics.</param>
		/// <param name="cancellationToken">A token that should be checked for cancelling the analysis.</param>
		protected override void Analyze(EnumDeclarationSyntax syntaxNode, DiagnosticCallback addDiagnostic, CancellationToken cancellationToken)
		{
			if (syntaxNode.BaseList != null)
				addDiagnostic(syntaxNode.BaseList.Types.First(), syntaxNode.Identifier.ValueText);
		}
	}
}