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
	using System.Linq.Expressions;

	/// <summary>
	///     Provides factory methods for the construction of linear temporal logic formulas.
	/// </summary>
	public static class Ltl
	{
		#region StateExpression

		/// <summary>
		///     Returns a <see cref="LtlFormula" /> that evaluates <paramref name="expression"/> within a system state.
		/// </summary>
		/// <param name="expression">The expression that should be evaluated.</param>
		public static LtlFormula StateExpression(Expression<Func<bool>> expression)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		///     Returns a <see cref="LtlFormula" /> that evaluates <paramref name="expression"/> within a system state.
		/// </summary>
		/// <param name="expression">[LiftExpression] The expression that should be evaluated.</param>
		public static LtlFormula StateExpression([LiftExpression] bool expression)
		{
			throw new NotSupportedException();
		}

		#endregion

		#region Next

		/// <summary>
		///     Returns a <see cref="LtlFormula" /> that applies the 'next' operator to <paramref name="operand" />.
		/// </summary>
		/// <param name="operand">The operand the 'next' operator should be applied to.</param>
		public static LtlFormula Next(LtlFormula operand)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		///     Returns a <see cref="LtlFormula" /> that applies the 'next' operator to <paramref name="operand" />.
		/// </summary>
		/// <param name="operand">The operand the 'next' operator should be applied to.</param>
		public static LtlFormula Next(Expression<Func<bool>> operand)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		///     Returns a <see cref="LtlFormula" /> that applies the 'next' operator to <paramref name="operand" />.
		/// </summary>
		/// <param name="operand">[LiftExpression] The operand the 'next' operator should be applied to.</param>
		public static LtlFormula Next([LiftExpression] bool operand)
		{
			throw new NotSupportedException();
		}

		#endregion

		#region Finally

		/// <summary>
		///     Returns a <see cref="LtlFormula" /> that applies the 'finally' operator to <paramref name="operand" />.
		/// </summary>
		/// <param name="operand">The operand the 'finally' operator should be applied to.</param>
		public static LtlFormula Finally(LtlFormula operand)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		///     Returns a <see cref="LtlFormula" /> that applies the 'finally' operator to <paramref name="operand" />.
		/// </summary>
		/// <param name="operand">[LiftExpression] The operand the 'finally' operator should be applied to.</param>
		public static LtlFormula Finally([LiftExpression] bool operand)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		///     Returns a <see cref="LtlFormula" /> that applies the 'finally' operator to <paramref name="operand" />.
		/// </summary>
		/// <param name="operand">The operand the 'finally' operator should be applied to.</param>
		public static LtlFormula Finally(Expression<Func<bool>> operand)
		{
			throw new NotSupportedException();
		}

		#endregion

		#region Globally

		/// <summary>
		///     Returns a <see cref="LtlFormula" /> that applies the 'globally' operator to <paramref name="operand" />.
		/// </summary>
		/// <param name="operand">The operand the 'globally' operator should be applied to.</param>
		public static LtlFormula Globally(LtlFormula operand)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		///     Returns a <see cref="LtlFormula" /> that applies the 'globally' operator to <paramref name="operand" />.
		/// </summary>
		/// <param name="operand">[LiftExpression] The operand the 'globally' operator should be applied to.</param>
		public static LtlFormula Globally([LiftExpression] bool operand)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		///     Returns a <see cref="LtlFormula" /> that applies the 'globally' operator to <paramref name="operand" />.
		/// </summary>
		/// <param name="operand">The operand the 'globally' operator should be applied to.</param>
		public static LtlFormula Globally(Expression<Func<bool>> operand)
		{
			throw new NotSupportedException();
		}

		#endregion

		#region Until

		/// <summary>
		///     Returns a <see cref="LtlFormula" /> that applies the 'until' operator to <paramref name="leftOperand" /> and
		///     <paramref name="rightOperand" />.
		/// </summary>
		/// <param name="leftOperand">[LiftExpression] The operand on the left-hand side of the 'until' operator.</param>
		/// <param name="rightOperand">[LiftExpression] The operand on the right-hand side of the 'until' operator.</param>
		public static LtlFormula Until([LiftExpression] bool leftOperand, [LiftExpression] bool rightOperand)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		///     Returns a <see cref="LtlFormula" /> that applies the 'until' operator to <paramref name="leftOperand" /> and
		///     <paramref name="rightOperand" />.
		/// </summary>
		/// <param name="leftOperand">The operand on the left-hand side of the 'until' operator.</param>
		/// <param name="rightOperand">The operand on the right-hand side of the 'until' operator.</param>
		public static LtlFormula Until(Expression<Func<bool>> leftOperand, Expression<Func<bool>> rightOperand)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		///     Returns a <see cref="LtlFormula" /> that applies the 'until' operator to <paramref name="leftOperand" /> and
		///     <paramref name="rightOperand" />.
		/// </summary>
		/// <param name="leftOperand">The operand on the left-hand side of the 'until' operator.</param>
		/// <param name="rightOperand">The operand on the right-hand side of the 'until' operator.</param>
		public static LtlFormula Until(LtlFormula leftOperand, LtlFormula rightOperand)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		///     Returns a <see cref="LtlFormula" /> that applies the 'until' operator to <paramref name="leftOperand" /> and
		///     <paramref name="rightOperand" />.
		/// </summary>
		/// <param name="leftOperand">The operand on the left-hand side of the 'until' operator.</param>
		/// <param name="rightOperand">The operand on the right-hand side of the 'until' operator.</param>
		public static LtlFormula Until(Expression<Func<bool>> leftOperand, LtlFormula rightOperand)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		///     Returns a <see cref="LtlFormula" /> that applies the 'until' operator to <paramref name="leftOperand" /> and
		///     <paramref name="rightOperand" />.
		/// </summary>
		/// <param name="leftOperand">The operand on the left-hand side of the 'until' operator.</param>
		/// <param name="rightOperand">The operand on the right-hand side of the 'until' operator.</param>
		public static LtlFormula Until(LtlFormula leftOperand, Expression<Func<bool>> rightOperand)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		///     Returns a <see cref="LtlFormula" /> that applies the 'until' operator to <paramref name="leftOperand" /> and
		///     <paramref name="rightOperand" />.
		/// </summary>
		/// <param name="leftOperand">[LiftExpression] The operand on the left-hand side of the 'until' operator.</param>
		/// <param name="rightOperand">The operand on the right-hand side of the 'until' operator.</param>
		public static LtlFormula Until([LiftExpression] bool leftOperand, LtlFormula rightOperand)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		///     Returns a <see cref="LtlFormula" /> that applies the 'until' operator to <paramref name="leftOperand" /> and
		///     <paramref name="rightOperand" />.
		/// </summary>
		/// <param name="leftOperand">The operand on the left-hand side of the 'until' operator.</param>
		/// <param name="rightOperand">[LiftExpression] The operand on the right-hand side of the 'until' operator.</param>
		public static LtlFormula Until(LtlFormula leftOperand, [LiftExpression] bool rightOperand)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		///     Returns a <see cref="LtlFormula" /> that applies the 'until' operator to <paramref name="leftOperand" /> and
		///     <paramref name="rightOperand" />.
		/// </summary>
		/// <param name="leftOperand">[LiftExpression] The operand on the left-hand side of the 'until' operator.</param>
		/// <param name="rightOperand">The operand on the right-hand side of the 'until' operator.</param>
		public static LtlFormula Until([LiftExpression] bool leftOperand, Expression<Func<bool>> rightOperand)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		///     Returns a <see cref="LtlFormula" /> that applies the 'until' operator to <paramref name="leftOperand" /> and
		///     <paramref name="rightOperand" />.
		/// </summary>
		/// <param name="leftOperand">The operand on the left-hand side of the 'until' operator.</param>
		/// <param name="rightOperand">[LiftExpression] The operand on the right-hand side of the 'until' operator.</param>
		public static LtlFormula Until(Expression<Func<bool>> leftOperand, [LiftExpression] bool rightOperand)
		{
			throw new NotSupportedException();
		}

		#endregion
	}
}