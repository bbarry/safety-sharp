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

namespace SafetySharp.CSharpCompiler.Roslyn.Symbols
{
	using System;
	using System.Linq;
	using Microsoft.CodeAnalysis;
	using Modeling;
	using Utilities;

	/// <summary>
	///     Provides extension methods for working with <see cref="ITypeSymbol" /> instances.
	/// </summary>
	public static class TypeSymbolExtensions
	{
		/// <summary>
		///     Checks whether <paramref name="typeSymbol" /> is directly or indirectly derived from the <paramref name="baseType" />
		///     interface or class.
		/// </summary>
		/// <param name="typeSymbol">The type symbol that should be checked.</param>
		/// <param name="baseType">The base type interface or class that <paramref name="typeSymbol" /> should be derived from.</param>
		[Pure]
		public static bool IsDerivedFrom([NotNull] this ITypeSymbol typeSymbol, [NotNull] ITypeSymbol baseType)
		{
			Requires.NotNull(typeSymbol, () => typeSymbol);
			Requires.NotNull(baseType, () => baseType);

			// Check the interfaces implemented by the type
			if (baseType.TypeKind == TypeKind.Interface && typeSymbol.AllInterfaces.Any(baseType.Equals))
				return true;

			// We've reached the top of the inheritance chain without finding baseType
			if (typeSymbol.BaseType == null)
				return false;

			// Check whether the base matches baseType
			if (baseType.TypeKind == TypeKind.Class && typeSymbol.BaseType.Equals(baseType))
				return true;

			// Recursively check the base
			return typeSymbol.BaseType.IsDerivedFrom(baseType);
		}

		/// <summary>
		///     Checks whether <paramref name="typeSymbol" /> is directly or indirectly derived from the <see cref="Component" />
		///     class within the context of the <paramref name="compilation" />.
		/// </summary>
		/// <param name="typeSymbol">The type symbol that should be checked.</param>
		/// <param name="compilation">
		///     The compilation that should be used to resolve the type symbol for the <see cref="Component" /> class.
		/// </param>
		[Pure]
		public static bool IsDerivedFromComponent([NotNull] this ITypeSymbol typeSymbol, [NotNull] Compilation compilation)
		{
			Requires.NotNull(typeSymbol, () => typeSymbol);
			Requires.NotNull(compilation, () => compilation);

			return typeSymbol.IsDerivedFrom(compilation.GetComponentClassSymbol());
		}

		/// <summary>
		///     Checks whether <paramref name="typeSymbol" /> directly or indirectly implements the <see cref="IComponent" />
		///     interface within the context of the <paramref name="compilation" />.
		/// </summary>
		/// <param name="typeSymbol">The type symbol that should be checked.</param>
		/// <param name="compilation">
		///     The compilation that should be used to resolve the type symbol for the <see cref="IComponent" /> interface.
		/// </param>
		[Pure]
		public static bool ImplementsIComponent([NotNull] this ITypeSymbol typeSymbol, [NotNull] Compilation compilation)
		{
			Requires.NotNull(typeSymbol, () => typeSymbol);
			Requires.NotNull(compilation, () => compilation);

			return typeSymbol.IsDerivedFrom(compilation.GetComponentInterfaceSymbol());
		}

		/// <summary>
		///     Checks whether <paramref name="typeSymbol" /> is directly or indirectly derived from the <see cref="Component" />
		///     class within the context of the <paramref name="semanticModel" />.
		/// </summary>
		/// <param name="typeSymbol">The type symbol that should be checked.</param>
		/// <param name="semanticModel">
		///     The semantic model that should be used to resolve the type symbol for the <see cref="Component" /> class.
		/// </param>
		[Pure]
		public static bool IsDerivedFromComponent([NotNull] this ITypeSymbol typeSymbol, [NotNull] SemanticModel semanticModel)
		{
			Requires.NotNull(typeSymbol, () => typeSymbol);
			Requires.NotNull(semanticModel, () => semanticModel);

			return typeSymbol.IsDerivedFrom(semanticModel.GetComponentClassSymbol());
		}

		/// <summary>
		///     Checks whether <paramref name="typeSymbol" /> directly or indirectly implements the <see cref="IComponent" />
		///     interface within the context of the <paramref name="semanticModel" />.
		/// </summary>
		/// <param name="typeSymbol">The type symbol that should be checked.</param>
		/// <param name="semanticModel">
		///     The semantic model that should be used to resolve the type symbol for the <see cref="IComponent" /> interface.
		/// </param>
		[Pure]
		public static bool ImplementsIComponent([NotNull] this ITypeSymbol typeSymbol, [NotNull] SemanticModel semanticModel)
		{
			Requires.NotNull(typeSymbol, () => typeSymbol);
			Requires.NotNull(semanticModel, () => semanticModel);

			return typeSymbol.IsDerivedFrom(semanticModel.GetComponentInterfaceSymbol());
		}

		/// <summary>
		///     Checks whether <paramref name="typeSymbol" /> represents the <see cref="int" />,
		///     <see cref="bool" />, or <see cref="decimal" /> types.
		/// </summary>
		/// <param name="typeSymbol">The type symbol that should be checked.</param>
		/// <param name="semanticModel">The semantic model that should be used to resolve symbol information.</param>
		[Pure]
		public static bool IsBuiltType([NotNull] this ITypeSymbol typeSymbol, [NotNull] SemanticModel semanticModel)
		{
			Requires.NotNull(typeSymbol, () => typeSymbol);
			Requires.NotNull(semanticModel, () => semanticModel);

			return typeSymbol.Equals(semanticModel.GetTypeSymbol<int>()) ||
				   typeSymbol.Equals(semanticModel.GetTypeSymbol<bool>()) ||
				   typeSymbol.Equals(semanticModel.GetTypeSymbol<decimal>());
		}
	}
}