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

namespace Tests.CSharp
{
	using System;

	namespace ModelingCompilationTests
	{
		using FluentAssertions;
		using NUnit.Framework;
		using SafetySharp.Modeling;

		[TestFixture]
		internal class GetClassDeclarationMethod
		{
			private class TestComponent : Component
			{
			}

			private static void GetClassDeclaration(string csharpCode, Component component = null)
			{
				var compilation = new TestCompilation(String.Format("namespace Tests.CSharp {{ {0} }}", csharpCode));

				if (component == null)
				{
					var assembly = compilation.Compile();
					component = (Component)Activator.CreateInstance(assembly.GetType("Tests.CSharp.TestComponent"));
				}

				var actual = compilation.ModelingCompilation.GetClassDeclaration(component);
				var expected = compilation.FindClassDeclaration(component.GetType().FullName);

				actual.Should().Be(expected);
			}

			[Test]
			public void ReturnsDeclarationForComponent()
			{
				GetClassDeclaration("class TestComponent : Component {}");
				GetClassDeclaration("namespace Nested { class TestComponent : Component {} } class TestComponent : Component {}");
			}

			[Test]
			public void ThrowsIfComponentIsUnknown()
			{
				Action action = () => GetClassDeclaration("namespace Nested { class TestComponent : Component {} }", new TestComponent());
				action.ShouldThrow<InvalidOperationException>();

				action = () => GetClassDeclaration("class Test : Component {}", new TestComponent());
				action.ShouldThrow<InvalidOperationException>();
			}
		}
	}
}