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
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.CodeAnalysis;
	using Utilities;

	/// <summary>
	///     Provides extension methods for working with <see cref="SyntaxNode" /> instances.
	/// </summary>
	internal static class SyntaxNodeExtensions
	{
		/// <summary>
		///     Gets a list of descendant syntax nodes of type <typeparamref name="T" /> in prefix document order.
		/// </summary>
		/// <typeparam name="T">The type of the syntax nodes that should be returned.</typeparam>
		/// <param name="syntaxNode">The syntax node whose descendents should be returned.</param>
		internal static IEnumerable<T> DescendantNodes<T>(this SyntaxNode syntaxNode)
			where T : SyntaxNode
		{
			Requires.NotNull(syntaxNode, () => syntaxNode);
			return syntaxNode.DescendantNodes().OfType<T>();
		}

		/// <summary>
		///     Gets a list of descendant syntax nodes (including <paramref name="syntaxNode" />) of type <typeparamref name="T" /> in
		///     prefix document order.
		/// </summary>
		/// <typeparam name="T">The type of the syntax nodes that should be returned.</typeparam>
		/// <param name="syntaxNode">The syntax node whose descendents should be returned.</param>
		internal static IEnumerable<T> DescendantNodesAndSelf<T>(this SyntaxNode syntaxNode)
			where T : SyntaxNode
		{
			Requires.NotNull(syntaxNode, () => syntaxNode);
			return syntaxNode.DescendantNodesAndSelf().OfType<T>();
		}
	}
}