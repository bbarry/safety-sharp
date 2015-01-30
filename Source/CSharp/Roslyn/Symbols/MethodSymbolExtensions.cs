﻿// The MIT License (MIT)
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

namespace SafetySharp.CSharp.Roslyn.Symbols
{
	using System;
	using System.Linq;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using Modeling;
	using Utilities;

	/// <summary>
	///     Provides extension methods for working with <see cref="IMethodSymbol" /> instances.
	/// </summary>
	public static class MethodSymbolExtensions
	{
		/// <summary>
		///     Checks whether <paramref name="methodSymbol" /> overrides <paramref name="overriddenMethod" />.
		/// </summary>
		/// <param name="methodSymbol">The symbol of the method that should be checked.</param>
		/// <param name="overriddenMethod">The symbol of the method that should be overridden.</param>
		[Pure]
		public static bool Overrides([NotNull] this IMethodSymbol methodSymbol, [NotNull] IMethodSymbol overriddenMethod)
		{
			Requires.NotNull(methodSymbol, () => methodSymbol);
			Requires.NotNull(overriddenMethod, () => overriddenMethod);

			if (methodSymbol.Equals(overriddenMethod))
				return true;

			if (!methodSymbol.IsOverride)
				return false;

			if (methodSymbol.OverriddenMethod.Equals(overriddenMethod))
				return true;

			return methodSymbol.OverriddenMethod.Overrides(overriddenMethod);
		}

		/// <summary>
		///     Checks whether <paramref name="methodSymbol" /> overrides the <see cref="Component.Update()" /> method within the
		///     context of the <paramref name="compilation" />.
		/// </summary>
		/// <param name="methodSymbol">The method symbol that should be checked.</param>
		/// <param name="compilation">The compilation that should be used to resolve symbol information.</param>
		[Pure]
		public static bool IsUpdateMethod([NotNull] this IMethodSymbol methodSymbol, [NotNull] Compilation compilation)
		{
			Requires.NotNull(methodSymbol, () => methodSymbol);
			Requires.NotNull(compilation, () => compilation);

			return methodSymbol.Overrides(compilation.GetUpdateMethodSymbol());
		}

		/// <summary>
		///     Checks whether <paramref name="methodSymbol" /> overrides the <see cref="Component.Update()" /> method within the
		///     context of the <paramref name="semanticModel" />.
		/// </summary>
		/// <param name="methodSymbol">The method symbol that should be checked.</param>
		/// <param name="semanticModel">The semantic model that should be used to resolve symbol information.</param>
		[Pure]
		public static bool IsUpdateMethod([NotNull] this IMethodSymbol methodSymbol, [NotNull] SemanticModel semanticModel)
		{
			Requires.NotNull(methodSymbol, () => methodSymbol);
			Requires.NotNull(semanticModel, () => semanticModel);

			return methodSymbol.Overrides(semanticModel.GetUpdateMethodSymbol());
		}

		/// <summary>
		///     Checks whether <paramref name="methodSymbol" /> represents a built-in operator of the <see cref="int" />,
		///     <see cref="bool" />, or <see cref="decimal" /> types.
		/// </summary>
		/// <param name="methodSymbol">The method symbol that should be checked.</param>
		/// <param name="semanticModel">The semantic model that should be used to resolve symbol information.</param>
		[Pure]
		public static bool IsBuiltInOperator([NotNull] this IMethodSymbol methodSymbol, [NotNull] SemanticModel semanticModel)
		{
			Requires.NotNull(methodSymbol, () => methodSymbol);
			Requires.NotNull(semanticModel, () => semanticModel);

			return methodSymbol.ContainingType.Equals(semanticModel.GetTypeSymbol<int>()) ||
				   methodSymbol.ContainingType.Equals(semanticModel.GetTypeSymbol<bool>()) ||
				   methodSymbol.ContainingType.Equals(semanticModel.GetTypeSymbol<decimal>());
		}

		/// <summary>
		///     Gets a unique name for the given method symbol that disambiguates method overloads.
		/// </summary>
		/// <param name="methodSymbol">The method symbol the unique name should be generated for.</param>
		[Pure]
		public static string GetUniqueName([NotNull] this IMethodSymbol methodSymbol)
		{
			Requires.NotNull(methodSymbol, () => methodSymbol);

			var parameters = String.Join("__", methodSymbol.Parameters.Select(parameter =>
			{
				var prefix = String.Empty;
				if (parameter.IsParams)
					prefix = "params_";

				switch (parameter.RefKind)
				{
					case RefKind.None:
						break;
					case RefKind.Ref:
						prefix = "ref_";
						break;
					case RefKind.Out:
						prefix = "out_";
						break;
					default:
						throw new InvalidOperationException("Unsupported ref kind.");
				}

				var name = prefix + parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
				return name.Replace(".", "_").Replace("[]", "_a").Replace("<", "___").Replace(">", "___").Replace("global::", "");
			}));

			return String.Format("{0}__{1}__", methodSymbol.Name.Replace(".", "_"), parameters);
		}

		/// <summary>
		///     Returns a <see cref="DelegateDeclarationSyntax" /> for a delegate that can be used to invoke
		///     <paramref name="methodSymbol" />.
		/// </summary>
		/// <param name="methodSymbol">The method the delegate should be synthesized for.</param>
		[Pure]
		public static DelegateDeclarationSyntax GetSynthesizedDelegateDeclaration([NotNull] this IMethodSymbol methodSymbol)
		{
			Requires.NotNull(methodSymbol, () => methodSymbol);

			var returnType = SyntaxFactory.ParseTypeName(methodSymbol.ReturnType.ToDisplayString());
			var name = IdentifierNameSynthesizer.ToSynthesizedName(methodSymbol.GetUniqueName() + "Delegate");

			var parameters = methodSymbol.Parameters.Select(parameter =>
			{
				var identifier = SyntaxFactory.Identifier(parameter.Name);
				var type = SyntaxFactory.ParseTypeName(parameter.Type.ToDisplayString());

				SyntaxKind? keyword = null;
				if (parameter.IsParams)
					keyword = SyntaxKind.ParamsKeyword;

				switch (parameter.RefKind)
				{
					case RefKind.None:
						break;
					case RefKind.Ref:
						keyword = SyntaxKind.RefKeyword;
						break;
					case RefKind.Out:
						keyword = SyntaxKind.OutKeyword;
						break;
					default:
						throw new InvalidOperationException("Unsupported ref kind.");
				}

				var declaration = SyntaxFactory.Parameter(identifier).WithType(type);
				if (keyword != null)
					declaration = declaration.WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(keyword.Value)));

				return declaration;
			});

			return SyntaxFactory
				.DelegateDeclaration(returnType, name)
				.WithModifiers(SyntaxTokenList.Create(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
				.WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(parameters)))
				.NormalizeWhitespace();
		}

		/// <summary>
		///     Returns a <see cref="CastExpressionSyntax" /> for a delegate that can be used to invoke
		///     <paramref name="methodSymbol" />.
		/// </summary>
		/// <param name="methodSymbol">The method the delegate is synthesized from.</param>
		/// <param name="expression">The expression that should be cast to the synthesized delegate type.</param>
		[Pure]
		public static CastExpressionSyntax CastToSynthesizedDelegate([NotNull] this IMethodSymbol methodSymbol,
																	 [NotNull] ExpressionSyntax expression)
		{
			Requires.NotNull(methodSymbol, () => methodSymbol);
			Requires.NotNull(expression, () => expression);

			var delegateName = IdentifierNameSynthesizer.ToSynthesizedName(methodSymbol.GetUniqueName() + "Delegate");
			var type = SyntaxFactory.ParseTypeName(String.Format("{0}.{1}", methodSymbol.ContainingType.ToDisplayString(), delegateName));
			return SyntaxFactory.CastExpression(type, SyntaxFactory.ParenthesizedExpression(expression)).NormalizeWhitespace();
		}
	}
}