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
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.Diagnostics;
	using Modeling;
	using Roslyn;
	using Roslyn.Symbols;

	/// <summary>
	///     Ensures that a method or property marked with the <see cref="RequiredAttribute" /> is <c>extern</c>.
	/// </summary>
	[DiagnosticAnalyzer]
	public class NonExternRequiredPortAnalyzer : CSharpAnalyzer
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public NonExternRequiredPortAnalyzer()
		{
			Error(1003,
				String.Format("A method or property marked with '{0}' must be extern.", typeof(ProvidedAttribute).FullName),
				"Required port '{0}' must be extern.");
		}

		/// <summary>
		///     Called once at session start to register actions in the analysis context.
		/// </summary>
		/// <param name="context">The analysis context that should be used to register analysis actions.</param>
		public override void Initialize(AnalysisContext context)
		{
			context.RegisterSymbolAction(Analyze, SymbolKind.Method, SymbolKind.Property);
		}

		/// <summary>
		///     Performs the analysis.
		/// </summary>
		/// <param name="context">The context in which the analysis should be performed.</param>
		private void Analyze(SymbolAnalysisContext context)
		{
			var compilation = context.Compilation;
			var symbol = context.Symbol;

			if (!symbol.ContainingType.IsDerivedFromComponent(compilation))
				return;

			// Ignore getter and setter methods of properties
			var methodSymbol = symbol as IMethodSymbol;
			if (methodSymbol != null && methodSymbol.AssociatedSymbol is IPropertySymbol)
				return;

			// If the required port attribute is not applied, we've nothing to do here
			if (!symbol.HasAttribute<RequiredAttribute>(compilation))
				return;

			// If the method is also marked as a provided port, something is wrong; we'll let another 
			// analyzer handle this situation
			if (symbol.HasAttribute<ProvidedAttribute>(compilation))
				return;

			if (!symbol.IsExtern)
				EmitDiagnostic(context, symbol, symbol.ToDisplayString());
		}
	}
}