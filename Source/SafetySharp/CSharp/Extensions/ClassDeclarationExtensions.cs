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

namespace SafetySharp.CSharp.Extensions
{
	using System;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using Modeling;
	using Utilities;

	/// <summary>
	///     Provides extension methods for working with class declarations.
	/// </summary>
	internal static class ClassDeclarationExtensions
	{
		/// <summary>
		///     Checks whether <paramref name="classDeclaration" /> is a component declaration by recursively searching for the
		///     <see cref="SafetySharp.Modeling.Component" /> base type.
		/// </summary>
		/// <param name="classDeclaration">The class declaration that should be checked.</param>
		/// <param name="semanticModel">The semantic model that should be to determine the base types.</param>
		internal static bool IsComponentDeclaration(this ClassDeclarationSyntax classDeclaration, SemanticModel semanticModel)
		{
			return classDeclaration.IsDerivedFrom(semanticModel, typeof(Component).FullName);
		}

		/// <summary>
		///     Checks whether <paramref name="classDeclaration" /> is a transitively derived from a class with the given
		///     <paramref name="baseTypeName" />.
		/// </summary>
		/// <param name="classDeclaration">The class declaration that should be checked.</param>
		/// <param name="semanticModel">The semantic model that should be to determine the base types.</param>
		/// <param name="baseTypeName">
		///     The name of the base type <paramref name="classDeclaration" /> should be derived from. The class
		///     name must be prefixed by all of the namespaces the class is defined in, as in 'System.Collections.ArrayList'.
		/// </param>
		internal static bool IsDerivedFrom(this ClassDeclarationSyntax classDeclaration, SemanticModel semanticModel, string baseTypeName)
		{
			Argument.NotNull(classDeclaration, () => classDeclaration);
			Argument.NotNull(semanticModel, () => semanticModel);

			var symbol = semanticModel.GetDeclaredSymbol(classDeclaration) as ITypeSymbol;

			Assert.NotNull(symbol);
			Assert.That(symbol.TypeKind == TypeKind.Class, "Unexpected symbol kind.");

			return IsDerivedFrom(symbol, baseTypeName);
		}

		/// <summary>
		///     Gets the full name of <paramref name="classDeclaration" /> in the form of 'Namespace1.Namespace2.ClassName+InnerClass'.
		/// </summary>
		/// <param name="classDeclaration">The class declaration the full name should be returned for.</param>
		/// <param name="semanticModel">The semantic model that should be used to determine the full name.</param>
		internal static string GetFullName(this ClassDeclarationSyntax classDeclaration, SemanticModel semanticModel)
		{
			Argument.NotNull(classDeclaration, () => classDeclaration);
			return semanticModel.GetDeclaredSymbol(classDeclaration).GetFullName();
		}

		/// <summary>
		///     Recursively checks whether <paramref name="typeSymbol" /> is derived from <paramref name="baseTypeName." />
		/// </summary>
		/// <param name="typeSymbol">The type symbol that should be checked.</param>
		/// <param name="baseTypeName">The name of the base type <paramref name="typeSymbol" /> should be derived from.</param>
		private static bool IsDerivedFrom(ITypeSymbol typeSymbol, string baseTypeName)
		{
			// We've reached the top of the inheritance chain (namely, System.Object) without finding the base type we've searched for
			if (typeSymbol.BaseType == null)
				return false;

			// Use a type name comparison to determine whether the type symbol's base type is the searched for type
			if (typeSymbol.BaseType.GetFullName() == baseTypeName)
				return true;

			return IsDerivedFrom(typeSymbol.BaseType, baseTypeName);
		}
	}
}