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

namespace SafetySharp.Modeling
{
	using System;
	using Utilities;

	/// <summary>
	///     Provides metadata about a reference of a Safety Sharp modeling assembly to another Safety Sharp modeling assembly.
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
	[UsedImplicitly]
	public class ModelingAssemblyReferenceAttribute : Attribute
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="ModelingAssemblyReferenceAttribute" /> type.
		/// </summary>
		/// <param name="assemblyName">The fully qualified name of the referenced modeling assembly.</param>
		public ModelingAssemblyReferenceAttribute(string assemblyName)
		{
			Requires.NotNull(assemblyName, () => assemblyName);
			AssemblyName = assemblyName;
		}

		/// <summary>
		///     Gets the fully qualified name of the referenced modeling assembly.
		/// </summary>
		internal string AssemblyName { get; private set; }
	}
}