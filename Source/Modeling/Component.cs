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
	/// 
	/// </summary>
	public abstract class Component : IComponent
	{
		/// <summary>
		/// 
		/// </summary>
		protected virtual void Update()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		///     Sets the initial values of a field of the component instance.
		/// </summary>
		/// <param name="field">[LiftExpression] A field of the component.</param>
		/// <param name="initialValues">The initial values of the field.</param>
		protected void SetInitialValues<T>([LiftExpression] T field, params T[] initialValues)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		///     Sets the initial values of a field of the component instance.
		/// </summary>
		/// <param name="field">An expression of the form <c>() => field</c> that referes to a field of the component.</param>
		/// <param name="initialValues">The initial values of the field.</param>
		protected void SetInitialValues<T>(Expression<Func<T>> field, params T[] initialValues)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		///     Allows access to a non-public member of the component.
		/// </summary>
		/// <typeparam name="T">The type of the accessed member.</typeparam>
		/// <param name="memberName">The name of the member that should be accessed.</param>
		/// <returns>Returns an <see cref="MemberAccess{T}" /> instance that can be used to access the non-public member.</returns>
		public MemberAccess<T> Access<T>(string memberName)
		{
			throw new NotSupportedException();
		}
	}
}