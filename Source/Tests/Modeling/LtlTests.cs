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

namespace Tests.Modeling
{
	using System;

	namespace LtlTests
	{
		using System.Collections.Immutable;
		using FluentAssertions;
		using NUnit.Framework;
		using SafetySharp.Formulas;
		using SafetySharp.Modeling;

		internal class LtlTests
		{
			protected readonly Formula Operand = new Formula(new UntransformedStateFormula(String.Empty, ImmutableArray<object>.Empty));
		}

		[TestFixture]
		internal class NextMethod : LtlTests
		{
			[Test]
			public void ReturnsUnaryNextFormula()
			{
				var formula = Ltl.Next(Operand);
				formula.WrappedFormula.Should().Be(new UnaryFormula(Operand.WrappedFormula, UnaryTemporalOperator.Next, PathQuantifier.None));
			}

			[Test]
			public void ThrowsWhenNullFormulaIsPassed()
			{
				Action action = () => Ltl.Next(null);
				action.ShouldThrow<ArgumentNullException>();
			}
		}

		[TestFixture]
		internal class FinallyMethod : LtlTests
		{
			[Test]
			public void ReturnsUnaryFinallyFormula()
			{
				var formula = Ltl.Finally(Operand);
				formula.WrappedFormula.Should().Be(new UnaryFormula(Operand.WrappedFormula, UnaryTemporalOperator.Finally, PathQuantifier.None));
			}

			[Test]
			public void ThrowsWhenNullFormulaIsPassed()
			{
				Action action = () => Ltl.Finally(null);
				action.ShouldThrow<ArgumentNullException>();
			}
		}

		[TestFixture]
		internal class GloballyMethod : LtlTests
		{
			[Test]
			public void ReturnsUnaryGloballyFormula()
			{
				var formula = Ltl.Globally(Operand);
				formula.WrappedFormula.Should().Be(new UnaryFormula(Operand.WrappedFormula, UnaryTemporalOperator.Globally, PathQuantifier.None));
			}

			[Test]
			public void ThrowsWhenNullFormulaIsPassed()
			{
				Action action = () => Ltl.Globally(null);
				action.ShouldThrow<ArgumentNullException>();
			}
		}

		[TestFixture]
		internal class UntilMethod : LtlTests
		{
			[Test]
			public void ReturnsBinaryUntilFormula()
			{
				var formula = Ltl.Until(Operand, Operand);

				var operand = Operand.WrappedFormula;
				formula.WrappedFormula.Should().Be(new BinaryFormula(operand, BinaryTemporalOperator.Until, PathQuantifier.None, operand));
			}

			[Test]
			public void ThrowsWhenNullFormulaIsPassed()
			{
				Action action = () => Ltl.Until(null, Operand);
				action.ShouldThrow<ArgumentNullException>();

				action = () => Ltl.Until(Operand, null);
				action.ShouldThrow<ArgumentNullException>();

				action = () => Ltl.Until(null, null);
				action.ShouldThrow<ArgumentNullException>();
			}
		}

		[TestFixture]
		internal class StateFormulaMethod : LtlTests
		{
			[Test]
			public void ReturnsUntrasformedStateFormula()
			{
				const string expression = "abc";
				var values = new object[] { true, 1, "test" };

				var formula = Ltl.StateFormula(expression, values);
				formula.WrappedFormula.Should().Be(new UntransformedStateFormula(expression, values.ToImmutableArray()));
			}

			[Test]
			public void ThrowsWhenEmptyExpressionIsPassed()
			{
				Action action = () => Ltl.StateFormula("   ");
				action.ShouldThrow<ArgumentException>();
			}

			[Test]
			public void ThrowsWhenNullExpressionIsPassed()
			{
				Action action = () => Ltl.StateFormula(null);
				action.ShouldThrow<ArgumentNullException>();
			}
		}
	}
}