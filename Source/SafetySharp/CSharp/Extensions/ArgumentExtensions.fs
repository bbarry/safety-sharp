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

open System
open System.Linq
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax
open SafetySharp.Utilities

/// Provides extension methods for working with <see cref="ArgumentSyntax" /> instances.
[<AutoOpen>]
module ArgumentExtensions =
    type ArgumentSyntax with

        /// Gets the <see cref="IMethodSymbol" /> of the method call expression that contains the argument.
        member this.GetMethodSymbol (semanticModel : SemanticModel) =
            nullArg this "this"
            nullArg semanticModel "semanticModel"

            let expression = this.GetMethodCallExpression ()
            match semanticModel.GetSymbolInfo(expression :> ExpressionSyntax).Symbol with
            | :? IMethodSymbol as methodSymbol -> methodSymbol
            | _ -> invalidOp "Unable to determine symbol of method call '%A'." expression

        ///  Checks whether the <see cref="IParameterSymbol" /> corresponding to the <paramref name="argument" /> of a
        ///  method call has the attribute of type <typeparamref name="T" /> applied.
        member this.ParameterHasAttribute<'T when 'T :> Attribute> (semanticModel : SemanticModel) =
            nullArg this "this"
            nullArg semanticModel "semanticModel"

            let attributeSymbol = semanticModel.GetTypeSymbol<'T> ()
            this.GetParameterSymbol(semanticModel).GetAttributes()
            |> Seq.exists (fun attribute -> attribute.AttributeClass.Equals attributeSymbol)

        /// Gets the <see cref="InvocationExpressionSyntax" /> or the <see cref="ObjectCreationExpressionSyntax" /> 
        /// that contains the argument.
        member private this.GetMethodCallExpression () =
            nullArg this "this"

            let rec getMethodCallExpression (node : SyntaxNode) =
                match node with
                | :? InvocationExpressionSyntax as parent -> parent :> ExpressionSyntax |> Some
                | :? ObjectCreationExpressionSyntax as parent -> parent :> ExpressionSyntax |> Some
                | null -> None
                | _ -> getMethodCallExpression node.Parent

            match getMethodCallExpression this.Parent with
            | None -> invalidOp "Unable to find the method call expression containing argument '%A'." this
            | Some parent -> parent

        /// Gets the <see cref="IParameterSymbol" /> corresponding to <paramref name="argument" />.
        /// TODO: There might be an official Roslyn API one day that should be used to replace this method.
        /// (see also https://roslyn.codeplex.com/discussions/541303)
        member private this.GetParameterSymbol (semanticModel : SemanticModel) : IParameterSymbol =
            nullArg this "this"
            nullArg semanticModel "semanticModel"

            let arguments =
                match this.GetMethodCallExpression () with
                | :? InvocationExpressionSyntax as invocation -> invocation.ArgumentList.Arguments
                | :? ObjectCreationExpressionSyntax as objectCreation -> objectCreation.ArgumentList.Arguments
                | _ -> invalidOp "Unknown method call."

            let methodSymbol = this.GetMethodSymbol semanticModel

            if this.NameColon <> null then
                methodSymbol.Parameters |> Seq.find (fun parameter -> parameter.Name = this.NameColon.Name.Identifier.ValueText)
            else
                let rec findParameter index =
                    if index >= methodSymbol.Parameters.Length then
                        let lastParameter = methodSymbol.Parameters.[methodSymbol.Parameters.Count() - 1];
                        if lastParameter.IsParams then
                            lastParameter
                        else
                            invalidOp "There are more arguments than parameters."
                    else if arguments.[index].Equals this then
                        methodSymbol.Parameters.[index]
                    else if index >= arguments.Count then
                        invalidOp "Unable to determine parameter symbol for argument '%A'." this
                    else
                        findParameter <| index + 1
                    
                findParameter 0
