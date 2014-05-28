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

// ------------------------------------------------------------------------------
// <auto-generated>
//     Generated by the Safety Sharp Code Generator.
//     Wednesday, May 28, 2014, 15:01:16
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------

namespace SafetySharp.Formulas
{
    using System;

    internal abstract partial class Formula
    {
        /// <summary>
        ///     Accepts <paramref name="visitor" />, calling the type-specific visit method.
        /// </summary>
        /// <param name="visitor">The visitor the type-specific visit method should be invoked on.</param>
        public abstract void Accept(FormulaVisitor visitor);

        /// <summary>
        ///     Accepts <paramref name="visitor" />, calling the type-specific visit method.
        /// </summary>
        /// <typeparam name="TResult">The type of the value returned by <paramref name="visitor" />.</typeparam>
        /// <param name="visitor">The visitor the type-specific visit method should be invoked on.</param>
        public abstract TResult Accept<TResult>(FormulaVisitor<TResult> visitor);

        /// <summary>
        ///     Determines whether <paramref name="other" /> is equal to the current instance.
        /// </summary>
        /// <param name="other">The <see cref="Formula" /> to compare with the current instance.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="other" /> is equal to the current instance; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool Equals(Formula other);
    }
}

namespace SafetySharp.Formulas
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using SafetySharp.Utilities;

    internal sealed partial class ExpressionFormula : Formula
    {
        /// <summary>
        ///     Gets the metamodel expression used as a non-temporal Boolean formula.
        /// </summary>
        public SafetySharp.Metamodel.Expressions.Expression Expression { get; private set; }

        /// <summary>
        ///     Gets the associated component is the scope in which the expression is evaluated.
        /// </summary>
        public SafetySharp.Metamodel.Configurations.ComponentConfiguration AssociatedComponent { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExpressionFormula" /> class.
        /// </summary>
        /// <param name="expression">The metamodel expression used as a non-temporal Boolean formula.</param>
        /// <param name="associatedComponent">The associated component is the scope in which the expression is evaluated.</param>
        public ExpressionFormula(SafetySharp.Metamodel.Expressions.Expression expression, SafetySharp.Metamodel.Configurations.ComponentConfiguration associatedComponent)
            : base()
        {
            Argument.NotNull(expression, () => expression);
            Argument.NotNull(associatedComponent, () => associatedComponent);

            Expression = expression;
            AssociatedComponent = associatedComponent;

            Validate();
        }

        /// <summary>
        ///     Validates all of the property values.
        /// </summary>
        partial void Validate();

        /// <summary>
        ///     Creates a copy of the <see cref="ExpressionFormula" /> instance, changing only the value of the
        ///     <see cref="ExpressionFormula.Expression" /> property; if the property value has not changed, 
        ///     no copy is made and the original object is returned.
        /// </summary>
        /// <param name="expression">The metamodel expression used as a non-temporal Boolean formula.</param>
        public ExpressionFormula WithExpression(SafetySharp.Metamodel.Expressions.Expression expression)
        {
            return Update(expression, AssociatedComponent);
        }

        /// <summary>
        ///     Creates a copy of the <see cref="ExpressionFormula" /> instance, changing only the value of the
        ///     <see cref="ExpressionFormula.AssociatedComponent" /> property; if the property value has not changed, 
        ///     no copy is made and the original object is returned.
        /// </summary>
        /// <param name="associatedComponent">The associated component is the scope in which the expression is evaluated.</param>
        public ExpressionFormula WithAssociatedComponent(SafetySharp.Metamodel.Configurations.ComponentConfiguration associatedComponent)
        {
            return Update(Expression, associatedComponent);
        }

        /// <summary>
        ///     Creates a new instance of the <see cref="ExpressionFormula" /> class if any of the property values
        ///     have changed; if none have changed, no copy is made and the original instance is returned.
        /// </summary>
        /// <param name="expression">The metamodel expression used as a non-temporal Boolean formula.</param>
        /// <param name="associatedComponent">The associated component is the scope in which the expression is evaluated.</param>
        public ExpressionFormula Update(SafetySharp.Metamodel.Expressions.Expression expression, SafetySharp.Metamodel.Configurations.ComponentConfiguration associatedComponent)
        {
            if (Expression != expression || AssociatedComponent != associatedComponent)
                return new ExpressionFormula(expression, associatedComponent);

            return this;
        }

        /// <summary>
        ///     Implements the visitor pattern, calling <paramref name="visitor" />'s
        ///     <see cref="FormulaVisitor.VisitExpressionFormula(ExpressionFormula)" /> method.
        /// </summary>
        /// <param name="visitor">The visitor the type-specific visit method should be invoked on.</param>
        public override void Accept(FormulaVisitor visitor)
        {
            Argument.NotNull(visitor, () => visitor);
            visitor.VisitExpressionFormula(this);
        }

        /// <summary>
        ///     Implements the visitor pattern, calling <paramref name="visitor" />'s
        ///     <see cref="FormulaVisitor{TResult}.VisitExpressionFormula(ExpressionFormula)" /> method.
        /// </summary>
        /// <typeparam name="TResult">The type of the value returned by <paramref name="visitor" />.</typeparam>
        /// <param name="visitor">The visitor the type-specific visit method should be invoked on.</param>
        public override TResult Accept<TResult>(FormulaVisitor<TResult> visitor)
        {
            Argument.NotNull(visitor, () => visitor);
            return visitor.VisitExpressionFormula(this);
        }

        /// <summary>
        ///     Determines whether <paramref name="obj" /> is equal to the current instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="obj" /> is equal to the current instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                	return true;

            if (obj.GetType() != GetType())
                	return false;

            return Equals((ExpressionFormula)obj);
        }

        /// <summary>
        ///     Determines whether <paramref name="other" /> is equal to the current instance.
        /// </summary>
        /// <param name="other">The <see cref="ExpressionFormula" /> to compare with the current instance.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="other" /> is equal to the current instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(Formula other)
        {
            Argument.NotNull(other, () => other);

            var element = other as ExpressionFormula;
            if (element == null)
                return false;

            return Expression.Equals(element.Expression) && AssociatedComponent.Equals(element.AssociatedComponent);
        }

        /// <summary>
        ///     Gets the hash code for the current instance.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = (int)2166136261;
                hash = hash * 16777619 ^ Expression.GetHashCode();
                hash = hash * 16777619 ^ AssociatedComponent.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        ///     Checks whether <paramref name="left" /> and <paramref name="right" /> are equal.
        /// </summary>
        /// <param name="left">The element on the left hand side of the equality operator.</param>
        /// <param name="right">The element on the right hand side of the equality operator.</param>
        public static bool operator ==(ExpressionFormula left, ExpressionFormula right)
        {
            return Equals(left, right);
        }

        /// <summary>
        ///     Checks whether <paramref name="left" /> and <paramref name="right" /> are not equal.
        /// </summary>
        /// <param name="left">The element on the left hand side of the inequality operator.</param>
        /// <param name="right">The element on the right hand side of the inequality operator.</param>
        public static bool operator !=(ExpressionFormula left, ExpressionFormula right)
        {
            return !Equals(left, right);
        }
    }

    internal sealed partial class BinaryFormula : Formula
    {
        /// <summary>
        ///     Gets the formula on the left-hand side of the binary operator.
        /// </summary>
        public Formula Left { get; private set; }

        /// <summary>
        ///     Gets the operator of the binary formula.
        /// </summary>
        public BinaryTemporalOperator Operator { get; private set; }

        /// <summary>
        ///     Gets the path quantifier of the binary formula.
        /// </summary>
        public PathQuantifier PathQuantifier { get; private set; }

        /// <summary>
        ///     Gets the formula on the right-hand side of the binary operator.
        /// </summary>
        public Formula Right { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BinaryFormula" /> class.
        /// </summary>
        /// <param name="left">The formula on the left-hand side of the binary operator.</param>
        /// <param name="operator">The operator of the binary formula.</param>
        /// <param name="pathQuantifier">The path quantifier of the binary formula.</param>
        /// <param name="right">The formula on the right-hand side of the binary operator.</param>
        public BinaryFormula(Formula left, BinaryTemporalOperator @operator, PathQuantifier pathQuantifier, Formula right)
            : base()
        {
            Argument.NotNull(left, () => left);
            Argument.InRange(@operator, () => @operator);
            Argument.InRange(pathQuantifier, () => pathQuantifier);
            Argument.NotNull(right, () => right);

            Left = left;
            Operator = @operator;
            PathQuantifier = pathQuantifier;
            Right = right;

            Validate();
        }

        /// <summary>
        ///     Validates all of the property values.
        /// </summary>
        partial void Validate();

        /// <summary>
        ///     Creates a copy of the <see cref="BinaryFormula" /> instance, changing only the value of the
        ///     <see cref="BinaryFormula.Left" /> property; if the property value has not changed, 
        ///     no copy is made and the original object is returned.
        /// </summary>
        /// <param name="left">The formula on the left-hand side of the binary operator.</param>
        public BinaryFormula WithLeft(Formula left)
        {
            return Update(left, Operator, PathQuantifier, Right);
        }

        /// <summary>
        ///     Creates a copy of the <see cref="BinaryFormula" /> instance, changing only the value of the
        ///     <see cref="BinaryFormula.Operator" /> property; if the property value has not changed, 
        ///     no copy is made and the original object is returned.
        /// </summary>
        /// <param name="operator">The operator of the binary formula.</param>
        public BinaryFormula WithOperator(BinaryTemporalOperator @operator)
        {
            return Update(Left, @operator, PathQuantifier, Right);
        }

        /// <summary>
        ///     Creates a copy of the <see cref="BinaryFormula" /> instance, changing only the value of the
        ///     <see cref="BinaryFormula.PathQuantifier" /> property; if the property value has not changed, 
        ///     no copy is made and the original object is returned.
        /// </summary>
        /// <param name="pathQuantifier">The path quantifier of the binary formula.</param>
        public BinaryFormula WithPathQuantifier(PathQuantifier pathQuantifier)
        {
            return Update(Left, Operator, pathQuantifier, Right);
        }

        /// <summary>
        ///     Creates a copy of the <see cref="BinaryFormula" /> instance, changing only the value of the
        ///     <see cref="BinaryFormula.Right" /> property; if the property value has not changed, 
        ///     no copy is made and the original object is returned.
        /// </summary>
        /// <param name="right">The formula on the right-hand side of the binary operator.</param>
        public BinaryFormula WithRight(Formula right)
        {
            return Update(Left, Operator, PathQuantifier, right);
        }

        /// <summary>
        ///     Creates a new instance of the <see cref="BinaryFormula" /> class if any of the property values
        ///     have changed; if none have changed, no copy is made and the original instance is returned.
        /// </summary>
        /// <param name="left">The formula on the left-hand side of the binary operator.</param>
        /// <param name="operator">The operator of the binary formula.</param>
        /// <param name="pathQuantifier">The path quantifier of the binary formula.</param>
        /// <param name="right">The formula on the right-hand side of the binary operator.</param>
        public BinaryFormula Update(Formula left, BinaryTemporalOperator @operator, PathQuantifier pathQuantifier, Formula right)
        {
            if (Left != left || Operator != @operator || PathQuantifier != pathQuantifier || Right != right)
                return new BinaryFormula(left, @operator, pathQuantifier, right);

            return this;
        }

        /// <summary>
        ///     Implements the visitor pattern, calling <paramref name="visitor" />'s
        ///     <see cref="FormulaVisitor.VisitBinaryFormula(BinaryFormula)" /> method.
        /// </summary>
        /// <param name="visitor">The visitor the type-specific visit method should be invoked on.</param>
        public override void Accept(FormulaVisitor visitor)
        {
            Argument.NotNull(visitor, () => visitor);
            visitor.VisitBinaryFormula(this);
        }

        /// <summary>
        ///     Implements the visitor pattern, calling <paramref name="visitor" />'s
        ///     <see cref="FormulaVisitor{TResult}.VisitBinaryFormula(BinaryFormula)" /> method.
        /// </summary>
        /// <typeparam name="TResult">The type of the value returned by <paramref name="visitor" />.</typeparam>
        /// <param name="visitor">The visitor the type-specific visit method should be invoked on.</param>
        public override TResult Accept<TResult>(FormulaVisitor<TResult> visitor)
        {
            Argument.NotNull(visitor, () => visitor);
            return visitor.VisitBinaryFormula(this);
        }

        /// <summary>
        ///     Determines whether <paramref name="obj" /> is equal to the current instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="obj" /> is equal to the current instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                	return true;

            if (obj.GetType() != GetType())
                	return false;

            return Equals((BinaryFormula)obj);
        }

        /// <summary>
        ///     Determines whether <paramref name="other" /> is equal to the current instance.
        /// </summary>
        /// <param name="other">The <see cref="BinaryFormula" /> to compare with the current instance.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="other" /> is equal to the current instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(Formula other)
        {
            Argument.NotNull(other, () => other);

            var element = other as BinaryFormula;
            if (element == null)
                return false;

            return Left.Equals(element.Left) && Operator.Equals(element.Operator) && PathQuantifier.Equals(element.PathQuantifier) && Right.Equals(element.Right);
        }

        /// <summary>
        ///     Gets the hash code for the current instance.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = (int)2166136261;
                hash = hash * 16777619 ^ Left.GetHashCode();
                hash = hash * 16777619 ^ Operator.GetHashCode();
                hash = hash * 16777619 ^ PathQuantifier.GetHashCode();
                hash = hash * 16777619 ^ Right.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        ///     Checks whether <paramref name="left" /> and <paramref name="right" /> are equal.
        /// </summary>
        /// <param name="left">The element on the left hand side of the equality operator.</param>
        /// <param name="right">The element on the right hand side of the equality operator.</param>
        public static bool operator ==(BinaryFormula left, BinaryFormula right)
        {
            return Equals(left, right);
        }

        /// <summary>
        ///     Checks whether <paramref name="left" /> and <paramref name="right" /> are not equal.
        /// </summary>
        /// <param name="left">The element on the left hand side of the inequality operator.</param>
        /// <param name="right">The element on the right hand side of the inequality operator.</param>
        public static bool operator !=(BinaryFormula left, BinaryFormula right)
        {
            return !Equals(left, right);
        }
    }

    internal sealed partial class UnaryFormula : Formula
    {
        /// <summary>
        ///     Gets the operand of the unary formula.
        /// </summary>
        public Formula Operand { get; private set; }

        /// <summary>
        ///     Gets the operator of the unary formula.
        /// </summary>
        public UnaryTemporalOperator Operator { get; private set; }

        /// <summary>
        ///     Gets the path quantifier of the unary formula.
        /// </summary>
        public PathQuantifier PathQuantifier { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UnaryFormula" /> class.
        /// </summary>
        /// <param name="operand">The operand of the unary formula.</param>
        /// <param name="operator">The operator of the unary formula.</param>
        /// <param name="pathQuantifier">The path quantifier of the unary formula.</param>
        public UnaryFormula(Formula operand, UnaryTemporalOperator @operator, PathQuantifier pathQuantifier)
            : base()
        {
            Argument.NotNull(operand, () => operand);
            Argument.InRange(@operator, () => @operator);
            Argument.InRange(pathQuantifier, () => pathQuantifier);

            Operand = operand;
            Operator = @operator;
            PathQuantifier = pathQuantifier;

            Validate();
        }

        /// <summary>
        ///     Validates all of the property values.
        /// </summary>
        partial void Validate();

        /// <summary>
        ///     Creates a copy of the <see cref="UnaryFormula" /> instance, changing only the value of the
        ///     <see cref="UnaryFormula.Operand" /> property; if the property value has not changed, 
        ///     no copy is made and the original object is returned.
        /// </summary>
        /// <param name="operand">The operand of the unary formula.</param>
        public UnaryFormula WithOperand(Formula operand)
        {
            return Update(operand, Operator, PathQuantifier);
        }

        /// <summary>
        ///     Creates a copy of the <see cref="UnaryFormula" /> instance, changing only the value of the
        ///     <see cref="UnaryFormula.Operator" /> property; if the property value has not changed, 
        ///     no copy is made and the original object is returned.
        /// </summary>
        /// <param name="operator">The operator of the unary formula.</param>
        public UnaryFormula WithOperator(UnaryTemporalOperator @operator)
        {
            return Update(Operand, @operator, PathQuantifier);
        }

        /// <summary>
        ///     Creates a copy of the <see cref="UnaryFormula" /> instance, changing only the value of the
        ///     <see cref="UnaryFormula.PathQuantifier" /> property; if the property value has not changed, 
        ///     no copy is made and the original object is returned.
        /// </summary>
        /// <param name="pathQuantifier">The path quantifier of the unary formula.</param>
        public UnaryFormula WithPathQuantifier(PathQuantifier pathQuantifier)
        {
            return Update(Operand, Operator, pathQuantifier);
        }

        /// <summary>
        ///     Creates a new instance of the <see cref="UnaryFormula" /> class if any of the property values
        ///     have changed; if none have changed, no copy is made and the original instance is returned.
        /// </summary>
        /// <param name="operand">The operand of the unary formula.</param>
        /// <param name="operator">The operator of the unary formula.</param>
        /// <param name="pathQuantifier">The path quantifier of the unary formula.</param>
        public UnaryFormula Update(Formula operand, UnaryTemporalOperator @operator, PathQuantifier pathQuantifier)
        {
            if (Operand != operand || Operator != @operator || PathQuantifier != pathQuantifier)
                return new UnaryFormula(operand, @operator, pathQuantifier);

            return this;
        }

        /// <summary>
        ///     Implements the visitor pattern, calling <paramref name="visitor" />'s
        ///     <see cref="FormulaVisitor.VisitUnaryFormula(UnaryFormula)" /> method.
        /// </summary>
        /// <param name="visitor">The visitor the type-specific visit method should be invoked on.</param>
        public override void Accept(FormulaVisitor visitor)
        {
            Argument.NotNull(visitor, () => visitor);
            visitor.VisitUnaryFormula(this);
        }

        /// <summary>
        ///     Implements the visitor pattern, calling <paramref name="visitor" />'s
        ///     <see cref="FormulaVisitor{TResult}.VisitUnaryFormula(UnaryFormula)" /> method.
        /// </summary>
        /// <typeparam name="TResult">The type of the value returned by <paramref name="visitor" />.</typeparam>
        /// <param name="visitor">The visitor the type-specific visit method should be invoked on.</param>
        public override TResult Accept<TResult>(FormulaVisitor<TResult> visitor)
        {
            Argument.NotNull(visitor, () => visitor);
            return visitor.VisitUnaryFormula(this);
        }

        /// <summary>
        ///     Determines whether <paramref name="obj" /> is equal to the current instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="obj" /> is equal to the current instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                	return true;

            if (obj.GetType() != GetType())
                	return false;

            return Equals((UnaryFormula)obj);
        }

        /// <summary>
        ///     Determines whether <paramref name="other" /> is equal to the current instance.
        /// </summary>
        /// <param name="other">The <see cref="UnaryFormula" /> to compare with the current instance.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="other" /> is equal to the current instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(Formula other)
        {
            Argument.NotNull(other, () => other);

            var element = other as UnaryFormula;
            if (element == null)
                return false;

            return Operand.Equals(element.Operand) && Operator.Equals(element.Operator) && PathQuantifier.Equals(element.PathQuantifier);
        }

        /// <summary>
        ///     Gets the hash code for the current instance.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = (int)2166136261;
                hash = hash * 16777619 ^ Operand.GetHashCode();
                hash = hash * 16777619 ^ Operator.GetHashCode();
                hash = hash * 16777619 ^ PathQuantifier.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        ///     Checks whether <paramref name="left" /> and <paramref name="right" /> are equal.
        /// </summary>
        /// <param name="left">The element on the left hand side of the equality operator.</param>
        /// <param name="right">The element on the right hand side of the equality operator.</param>
        public static bool operator ==(UnaryFormula left, UnaryFormula right)
        {
            return Equals(left, right);
        }

        /// <summary>
        ///     Checks whether <paramref name="left" /> and <paramref name="right" /> are not equal.
        /// </summary>
        /// <param name="left">The element on the left hand side of the inequality operator.</param>
        /// <param name="right">The element on the right hand side of the inequality operator.</param>
        public static bool operator !=(UnaryFormula left, UnaryFormula right)
        {
            return !Equals(left, right);
        }
    }
}

namespace SafetySharp.Formulas
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using SafetySharp.Utilities;

    internal abstract partial class FormulaVisitor
    {
        /// <summary>
        ///     Visits an element of type <see cref="Formula" />.
        /// </summary>
        /// <param name="element">The <see cref="Formula" /> instance that should be visited.</param>
        public virtual void Visit(Formula element)
        {
            Argument.NotNull(element, () => element);
            element.Accept(this);
        }

        /// <summary>
        ///     Visits an element of type <see cref="ExpressionFormula" />.
        /// </summary>
        /// <param name="expressionFormula">The <see cref="ExpressionFormula" /> instance that should be visited.</param>
        public virtual void VisitExpressionFormula(ExpressionFormula expressionFormula)
        {
            Argument.NotNull(expressionFormula, () => expressionFormula);
        }

        /// <summary>
        ///     Visits an element of type <see cref="BinaryFormula" />.
        /// </summary>
        /// <param name="binaryFormula">The <see cref="BinaryFormula" /> instance that should be visited.</param>
        public virtual void VisitBinaryFormula(BinaryFormula binaryFormula)
        {
            Argument.NotNull(binaryFormula, () => binaryFormula);
        }

        /// <summary>
        ///     Visits an element of type <see cref="UnaryFormula" />.
        /// </summary>
        /// <param name="unaryFormula">The <see cref="UnaryFormula" /> instance that should be visited.</param>
        public virtual void VisitUnaryFormula(UnaryFormula unaryFormula)
        {
            Argument.NotNull(unaryFormula, () => unaryFormula);
        }
    }

    internal abstract partial class FormulaVisitor<TResult>
    {
        /// <summary>
        ///     Visits an element of type <see cref="Formula" />.
        /// </summary>
        /// <param name="element">The <see cref="Formula" /> instance that should be visited.</param>
        public virtual TResult Visit(Formula element)
        {
            Argument.NotNull(element, () => element);
            return element.Accept(this);
        }

        /// <summary>
        ///     Visits an element of type <see cref="ExpressionFormula" />.
        /// </summary>
        /// <param name="expressionFormula">The <see cref="ExpressionFormula" /> instance that should be visited.</param>
        public virtual TResult VisitExpressionFormula(ExpressionFormula expressionFormula)
        {
            Argument.NotNull(expressionFormula, () => expressionFormula);
            return default(TResult);
        }

        /// <summary>
        ///     Visits an element of type <see cref="BinaryFormula" />.
        /// </summary>
        /// <param name="binaryFormula">The <see cref="BinaryFormula" /> instance that should be visited.</param>
        public virtual TResult VisitBinaryFormula(BinaryFormula binaryFormula)
        {
            Argument.NotNull(binaryFormula, () => binaryFormula);
            return default(TResult);
        }

        /// <summary>
        ///     Visits an element of type <see cref="UnaryFormula" />.
        /// </summary>
        /// <param name="unaryFormula">The <see cref="UnaryFormula" /> instance that should be visited.</param>
        public virtual TResult VisitUnaryFormula(UnaryFormula unaryFormula)
        {
            Argument.NotNull(unaryFormula, () => unaryFormula);
            return default(TResult);
        }
    }

    internal abstract partial class FormulaRewriter : FormulaVisitor<Formula>
    {
        /// <summary>
        ///     Visits elements of type <typeparamref name="TElement" /> stored in an <see cref="ImmutableArray{TElement}" />.
        /// </summary>
        /// <typeparam name="TElement">The types of the elements in the array that should be visited.</typeparam>
        /// <param name="elements">The <see cref="ImmutableArray{TElement}" /> instance that should be visited.</param>
        public virtual ImmutableArray<TElement> Visit<TElement>(ImmutableArray<TElement> elements)
        	where TElement : Formula
        {
            return elements.Aggregate(ImmutableArray<TElement>.Empty, (current, element) => current.Add((TElement)element.Accept(this)));
        }

        /// <summary>
        ///     Visits elements of type <typeparamref name="TElement" /> stored in a <see cref="ImmutableList{TElement}" />.
        /// </summary>
        /// <typeparam name="TElement">The types of the elements in the list that should be visited.</typeparam>
        /// <param name="elements">The <see cref="ImmutableList{TElement}" /> instance that should be visited.</param>
        public virtual ImmutableList<TElement> Visit<TElement>(ImmutableList<TElement> elements)
        	where TElement : Formula
        {
            Argument.NotNull(elements, () => elements);
            return elements.Aggregate(ImmutableList<TElement>.Empty, (current, element) => current.Add((TElement)element.Accept(this)));
        }

        /// <summary>
        ///     Rewrites an element of type <see cref="ExpressionFormula" />.
        /// </summary>
        /// <param name="expressionFormula">The <see cref="ExpressionFormula" /> instance that should be rewritten.</param>
        public override Formula VisitExpressionFormula(ExpressionFormula expressionFormula)
        {
            Argument.NotNull(expressionFormula, () => expressionFormula);

            var expression = expressionFormula.Expression;
            var associatedComponent = expressionFormula.AssociatedComponent;
            return expressionFormula.Update(expression, associatedComponent);
        }

        /// <summary>
        ///     Rewrites an element of type <see cref="BinaryFormula" />.
        /// </summary>
        /// <param name="binaryFormula">The <see cref="BinaryFormula" /> instance that should be rewritten.</param>
        public override Formula VisitBinaryFormula(BinaryFormula binaryFormula)
        {
            Argument.NotNull(binaryFormula, () => binaryFormula);

            var left = binaryFormula.Left;
            var @operator = binaryFormula.Operator;
            var pathQuantifier = binaryFormula.PathQuantifier;
            var right = binaryFormula.Right;
            return binaryFormula.Update(left, @operator, pathQuantifier, right);
        }

        /// <summary>
        ///     Rewrites an element of type <see cref="UnaryFormula" />.
        /// </summary>
        /// <param name="unaryFormula">The <see cref="UnaryFormula" /> instance that should be rewritten.</param>
        public override Formula VisitUnaryFormula(UnaryFormula unaryFormula)
        {
            Argument.NotNull(unaryFormula, () => unaryFormula);

            var operand = unaryFormula.Operand;
            var @operator = unaryFormula.Operator;
            var pathQuantifier = unaryFormula.PathQuantifier;
            return unaryFormula.Update(operand, @operator, pathQuantifier);
        }
    }
}
