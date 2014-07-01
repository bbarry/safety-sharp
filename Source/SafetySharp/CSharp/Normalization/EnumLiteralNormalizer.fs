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

namespace SafetySharp.Internal.CSharp.Normalization

open System
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax
open SafetySharp.Internal.CSharp.Roslyn
open SafetySharp.Modeling

/// Replaces uses of enum literals within a component by their underlying int value.
type internal EnumLiteralNormalizer () =
    inherit CSharpNormalizer (NormalizationScope.Components)

    override this.VisitMemberAccessExpression node =
        let symbolInfo = this.semanticModel.GetSymbolInfo node
        match symbolInfo.Symbol with
        | :? IFieldSymbol as fieldSymbol when fieldSymbol.ContainingType.TypeKind = TypeKind.Enum ->
            upcast (SyntaxFactory.ParseExpression (fieldSymbol.ConstantValue.ToString ()) |> Syntax.WithTriviaFromNode node)
        | _ -> 
            upcast node