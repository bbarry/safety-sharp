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

open System
open System.Linq.Expressions
open SafetySharp.Modeling.CompilerServices
open SafetySharp.Internal.Utilities
open SafetySharp.Internal.Metamodel

/// Represents a computation tree logic formula provided by a C# model.
[<AllowNullLiteral; Sealed>]
type CtlFormula internal (formula : CSharpFormula) = 
    /// Gets the wrapped C# formula.
    member internal this.Formula = formula

    /// Applies the implication operator to this instance and the given formula.
    member this.Implies (formula : CtlFormula) = 
        CtlFormula (CSharpBinaryFormula(this.Formula, BinaryFormulaOperator.Implication, formula.Formula))

/// Describes a path quantifier of a computation tree logic formula.
type internal PathQuantifier =
    | AllPaths
    | ExistsPath

[<Sealed>]
type CtlOperatorFactory internal (quantifier) =
    
    /// <summary>
    ///     Returns a <see cref="CtlFormula" /> that applies the 'next' operator to <paramref name="operand" />.
    /// </summary>
    /// <param name="operand">The operand the 'next' operator should be applied to.</param>
    member this.Next (operand : CtlFormula) =
        nullArg operand "operand"
        let operator = 
            match quantifier with
            | AllPaths -> UnaryFormulaOperator.AllPathsNext
            | ExistsPath -> UnaryFormulaOperator.ExistsPathNext
        CtlFormula (CSharpUnaryFormula(operand.Formula, operator))

    /// <summary>
    ///     Returns a <see cref="CtlFormula" /> that applies the 'next' operator to <paramref name="operand" />.
    /// </summary>
    /// <param name="operand">The operand the 'next' operator should be applied to.</param>
    member this.Next (operand : Expression<Func<bool>>) =
        nullArg operand "operand"
        let operator = 
            match quantifier with
            | AllPaths -> UnaryFormulaOperator.AllPathsNext
            | ExistsPath -> UnaryFormulaOperator.ExistsPathNext
        CtlFormula (CSharpUnaryFormula(CSharpStateFormula operand, operator))

    /// <summary>
    ///     Returns a <see cref="CtlFormula" /> that applies the 'next' operator to <paramref name="operand" />.
    /// </summary>
    /// <param name="operand">[LiftExpression] The operand the 'next' operator should be applied to.</param>
    member this.Next ([<LiftExpression>] operand : bool) : CtlFormula =
        invalidUnliftedCall ()
        null

    /// <summary>
    ///     Returns a <see cref="CtlFormula" /> that applies the 'finally' operator to <paramref name="operand" />.
    /// </summary>
    /// <param name="operand">The operand the 'finally' operator should be applied to.</param>
    member this.Finally (operand : CtlFormula) =
        nullArg operand "operand"
        let operator = 
            match quantifier with
            | AllPaths -> UnaryFormulaOperator.AllPathsFinally
            | ExistsPath -> UnaryFormulaOperator.ExistsPathFinally
        CtlFormula (CSharpUnaryFormula(operand.Formula, operator))

    /// <summary>
    ///     Returns a <see cref="CtlFormula" /> that applies the 'finally' operator to <paramref name="operand" />.
    /// </summary>
    /// <param name="operand">The operand the 'finally' operator should be applied to.</param>
    member this.Finally (operand : Expression<Func<bool>>) =
        nullArg operand "operand"
        let operator = 
            match quantifier with
            | AllPaths -> UnaryFormulaOperator.AllPathsFinally
            | ExistsPath -> UnaryFormulaOperator.ExistsPathFinally
        CtlFormula (CSharpUnaryFormula(CSharpStateFormula operand, operator))

    /// <summary>
    ///     Returns a <see cref="CtlFormula" /> that applies the 'finally' operator to <paramref name="operand" />.
    /// </summary>
    /// <param name="operand">[LiftExpression] The operand the 'finally' operator should be applied to.</param>
    member this.Finally ([<LiftExpression>] operand : bool) : CtlFormula =
        invalidUnliftedCall ()
        null

    /// <summary>
    ///     Returns a <see cref="CtlFormula" /> that applies the 'globally' operator to <paramref name="operand" />.
    /// </summary>
    /// <param name="operand">The operand the 'globally' operator should be applied to.</param>
    member this.Globally (operand : CtlFormula) =
        nullArg operand "operand"
        let operator = 
            match quantifier with
            | AllPaths -> UnaryFormulaOperator.AllPathsGlobally
            | ExistsPath -> UnaryFormulaOperator.ExistsPathGlobally
        CtlFormula (CSharpUnaryFormula(operand.Formula, operator))

    /// <summary>
    ///     Returns a <see cref="CtlFormula" /> that applies the 'globally' operator to <paramref name="operand" />.
    /// </summary>
    /// <param name="operand">The operand the 'globally' operator should be applied to.</param>
    member this.Globally (operand : Expression<Func<bool>>) =
        nullArg operand "operand"
        let operator = 
            match quantifier with
            | AllPaths -> UnaryFormulaOperator.AllPathsGlobally
            | ExistsPath -> UnaryFormulaOperator.ExistsPathGlobally
        CtlFormula (CSharpUnaryFormula(CSharpStateFormula operand, operator))

    /// <summary>
    ///     Returns a <see cref="CtlFormula" /> that applies the 'globally' operator to <paramref name="operand" />.
    /// </summary>
    /// <param name="operand">[LiftExpression] The operand the 'globally' operator should be applied to.</param>
    member this.Globally ([<LiftExpression>] operand : bool) : CtlFormula =
        invalidUnliftedCall ()
        null

    /// <summary>
    ///     Returns a <see cref="CtlFormula" /> that applies the 'until' operator to <paramref name="leftOperand" /> and
    ///     <paramref name="rightOperand" />.
    /// </summary>
    /// <param name="leftOperand">The operand on the left-hand side of the 'until' operator.</param>
    /// <param name="rightOperand">The operand on the right-hand side of the 'until' operator.</param>
    member this.Until (leftOperand : CtlFormula, rightOperand : CtlFormula) =
        nullArg leftOperand "leftOperand"
        nullArg rightOperand "rightOperand"
        let operator = 
            match quantifier with
            | AllPaths -> BinaryFormulaOperator.AllPathsUntil
            | ExistsPath -> BinaryFormulaOperator.ExistsPathUntil
        CtlFormula (CSharpBinaryFormula(leftOperand.Formula, operator, rightOperand.Formula))

    /// <summary>
    ///     Returns a <see cref="CtlFormula" /> that applies the 'until' operator to <paramref name="leftOperand" /> and
    ///     <paramref name="rightOperand" />.
    /// </summary>
    /// <param name="leftOperand">The operand on the left-hand side of the 'until' operator.</param>
    /// <param name="rightOperand">The operand on the right-hand side of the 'until' operator.</param>
    member this.Until (leftOperand : Expression<Func<bool>>, rightOperand : Expression<Func<bool>>) =
        nullArg leftOperand "leftOperand"
        nullArg rightOperand "rightOperand"
        let operator = 
            match quantifier with
            | AllPaths -> BinaryFormulaOperator.AllPathsUntil
            | ExistsPath -> BinaryFormulaOperator.ExistsPathUntil
        CtlFormula (CSharpBinaryFormula(CSharpStateFormula leftOperand, operator, CSharpStateFormula rightOperand))

    /// <summary>
    ///     Returns a <see cref="CtlFormula" /> that applies the 'until' operator to <paramref name="leftOperand" /> and
    ///     <paramref name="rightOperand" />.
    /// </summary>
    /// <param name="leftOperand">The operand on the left-hand side of the 'until' operator.</param>
    /// <param name="rightOperand">The operand on the right-hand side of the 'until' operator.</param>
    member this.Until (leftOperand : CtlFormula, rightOperand : Expression<Func<bool>>) =
        nullArg leftOperand "leftOperand"
        nullArg rightOperand "rightOperand"
        let operator = 
            match quantifier with
            | AllPaths -> BinaryFormulaOperator.AllPathsUntil
            | ExistsPath -> BinaryFormulaOperator.ExistsPathUntil
        CtlFormula (CSharpBinaryFormula(leftOperand.Formula, operator, CSharpStateFormula rightOperand))

    /// <summary>
    ///     Returns a <see cref="CtlFormula" /> that applies the 'until' operator to <paramref name="leftOperand" /> and
    ///     <paramref name="rightOperand" />.
    /// </summary>
    /// <param name="leftOperand">The operand on the left-hand side of the 'until' operator.</param>
    /// <param name="rightOperand">The operand on the right-hand side of the 'until' operator.</param>
    member this.Until (leftOperand : Expression<Func<bool>>, rightOperand : CtlFormula) =
        nullArg leftOperand "leftOperand"
        nullArg rightOperand "rightOperand"
        let operator = 
            match quantifier with
            | AllPaths -> BinaryFormulaOperator.AllPathsUntil
            | ExistsPath -> BinaryFormulaOperator.ExistsPathUntil
        CtlFormula (CSharpBinaryFormula(CSharpStateFormula leftOperand, operator, rightOperand.Formula))

    /// <summary>
    ///     Returns a <see cref="CtlFormula" /> that applies the 'until' operator to <paramref name="leftOperand" /> and
    ///     <paramref name="rightOperand" />.
    /// </summary>
    /// <param name="leftOperand">[LiftExpression] The operand on the left-hand side of the 'until' operator.</param>
    /// <param name="rightOperand">[LiftExpression] The operand on the right-hand side of the 'until' operator.</param>
    member this.Until ([<LiftExpression>] leftOperand : bool, [<LiftExpression>] rightOperand : bool) =
        invalidUnliftedCall ()
        null

    /// <summary>
    ///     Returns a <see cref="CtlFormula" /> that applies the 'until' operator to <paramref name="leftOperand" /> and
    ///     <paramref name="rightOperand" />.
    /// </summary>
    /// <param name="leftOperand">[LiftExpression] The operand on the left-hand side of the 'until' operator.</param>
    /// <param name="rightOperand">The operand on the right-hand side of the 'until' operator.</param>
    member this.Until ([<LiftExpression>] leftOperand : bool, rightOperand : CtlFormula) : CtlFormula =
        invalidUnliftedCall ()
        null

    /// <summary>
    ///     Returns a <see cref="CtlFormula" /> that applies the 'until' operator to <paramref name="leftOperand" /> and
    ///     <paramref name="rightOperand" />.
    /// </summary>
    /// <param name="leftOperand">The operand on the left-hand side of the 'until' operator.</param>
    /// <param name="rightOperand">[LiftExpression] The operand on the right-hand side of the 'until' operator.</param>
    member this.Until (leftOperand : CtlFormula, [<LiftExpression>] rightOperand : bool) : CtlFormula =
        invalidUnliftedCall ()
        null

    /// <summary>
    ///     Returns a <see cref="CtlFormula" /> that applies the 'until' operator to <paramref name="leftOperand" /> and
    ///     <paramref name="rightOperand" />.
    /// </summary>
    /// <param name="leftOperand">[LiftExpression] The operand on the left-hand side of the 'until' operator.</param>
    /// <param name="rightOperand">The operand on the right-hand side of the 'until' operator.</param>
    member this.Until ([<LiftExpression>] leftOperand : bool, rightOperand : Expression<Func<bool>>) : CtlFormula =
        invalidUnliftedCall ()
        null

    /// <summary>
    ///     Returns a <see cref="CtlFormula" /> that applies the 'until' operator to <paramref name="leftOperand" /> and
    ///     <paramref name="rightOperand" />.
    /// </summary>
    /// <param name="leftOperand">The operand on the left-hand side of the 'until' operator.</param>
    /// <param name="rightOperand">[LiftExpression] The operand on the right-hand side of the 'until' operator.</param>
    member this.Until (leftOperand : Expression<Func<bool>>, [<LiftExpression>] rightOperand : bool) : CtlFormula =
        invalidUnliftedCall ()
        null

/// Provides factory methods for the construction of computation tree logic formulas.
[<AbstractClass; Sealed>]
type Ctl =
    /// Gets a <see cref="CtlOperatorFactory" /> instance that provides methods for the application of CTL operators that
    /// operate on all paths of a subtree of a computation tree.
    static member AllPaths = CtlOperatorFactory PathQuantifier.AllPaths    

    /// Gets a <see cref="CtlOperatorFactory" /> instance that provides methods for the application of CTL operators that
    /// require the existence of a certain path within the subtree of a computation tree.
    static member ExistsPath = CtlOperatorFactory PathQuantifier.ExistsPath

    /// <summary>
    ///     Returns a <see cref="CtlFormula" /> that evaluates <paramref name="expression" /> within a system state.
    /// </summary>
    /// <param name="expression">The expression that should be evaluated.</param>
    static member StateExpression (expression : Expression<Func<bool>>) =
        nullArg expression "expression"
        CtlFormula (CSharpStateFormula expression)

    /// <summary>
    ///     Returns a <see cref="CtlFormula" /> that evaluates <paramref name="expression" /> within a system state.
    /// </summary>
    /// <param name="expression">[LiftExpression] The expression that should be evaluated.</param>
    static member StateExpression ([<LiftExpression>] expression : bool) =
        invalidUnliftedCall ()