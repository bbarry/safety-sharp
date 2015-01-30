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

namespace SafetySharp.CSharp.Analyzers
{
	using System;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.Diagnostics;
	using Roslyn;
	using Utilities;

	/// <summary>
	///     Ensures that bindings resolve to a unique pair of bound ports.
	/// </summary>
	[DiagnosticAnalyzer, UsedImplicitly]
	public class BindingAnalyzer : CSharpAnalyzer
	{
		/// <summary>
		///     The error diagnostic emitted by the analyzer if a binding failed.
		/// </summary>
		private static readonly DiagnosticInfo BindingFailure = DiagnosticInfo.Error(
			DiagnosticIdentifier.BindingFailure,
			"There are no accessible signature-compatible ports that could be bound.",
			"Port binding failure: There are no accessible signature-compatible ports with the given names that could be bound. " +
			"\nOn the left-hand side, could be:\n{0}\nOn the right-hand side, could be:\n{1}");

		/// <summary>
		///     The error diagnostic emitted by the analyzer if a binding is ambiguous.
		/// </summary>
		private static readonly DiagnosticInfo AmbiguousBinding = DiagnosticInfo.Error(
			DiagnosticIdentifier.AmbiguousBinding,
			"There are multiple signature-compatible ports that could be bound.",
			"Port binding is ambiguous: There are multiple accessible and signature-compatible ports with the given " +
			"names that could be bound.\nOn the left-hand side, could be:\n{0}\nOn the right-hand side, could be:\n{1}");

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public BindingAnalyzer()
			: base(BindingFailure, AmbiguousBinding)
		{
		}

		/// <summary>
		///     Called once at session start to register actions in the analysis context.
		/// </summary>
		/// <param name="context" />
		public override void Initialize(AnalysisContext context)
		{
			context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.SimpleMemberAccessExpression);
		}

		/// <summary>
		///     Performs the analysis on the given binding.
		/// </summary>
		/// <param name="context">The context in which the analysis should be performed.</param>
		private static void Analyze(SyntaxNodeAnalysisContext context)
		{
		}
	}
}