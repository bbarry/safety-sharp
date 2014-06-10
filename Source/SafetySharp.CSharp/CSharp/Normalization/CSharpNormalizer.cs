// The MIT License (MIT)
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

namespace SafetySharp.CSharp.Normalization
{
	using System;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;

	/// <summary>
	///     A base class for C# normalizers that normalize certain C# features to equivalent ones that are easier to transform to
	///     the metamodel.
	/// </summary>
	internal abstract class CSharpNormalizer : CSharpSyntaxRewriter
	{
		/// <summary>
		///     The semantic model that can be used for semantic analysis during normalization.
		/// </summary>
		protected SemanticModel SemanticModel { get; private set; }

		/// <summary>
		///     Normalizes the C# code contained in <paramref name="compilation." />
		/// </summary>
		/// <param name="compilation">The C# compilation that should be normalized.</param>
		public Compilation Normalize(Compilation compilation)
		{
			foreach (var syntaxTree in compilation.SyntaxTrees)
			{
				SemanticModel = compilation.GetSemanticModel(syntaxTree);

				var root = syntaxTree.GetRoot();
				var normalizedRoot = Visit(root);

				if (root == normalizedRoot)
					continue;

				var newTree = syntaxTree.WithChangedText(normalizedRoot.GetText());
				compilation = compilation.ReplaceSyntaxTree(syntaxTree, newTree);
			}

			return compilation;
		}
	}
}