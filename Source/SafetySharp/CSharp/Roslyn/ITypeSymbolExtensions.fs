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

namespace SafetySharp.CSharp.Roslyn

open System.Linq
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax
open SafetySharp.Utilities

/// Provides extension methods for working with <see cref="ITypeSymbol" /> instances.
[<AutoOpen>]
module TypeSymbolExtensions =
    type ITypeSymbol with

        /// Checks whether the type symbol is directly or indirectly derived from the given base type interface or class.
        member this.IsDerivedFrom (baseType : ITypeSymbol) =
            nullArg this "this"
            nullArg baseType "baseType"

            // Check the interfaces implemented by the type
            if baseType.TypeKind = TypeKind.Interface && this.AllInterfaces |> Seq.exists ((=) (baseType :?> INamedTypeSymbol)) then
                true
            // We've reached the top of the inheritance chain without finding baseType
            elif this.BaseType = null then
                false
            // Check whether the base matches baseType
            elif baseType.TypeKind = TypeKind.Class && this.BaseType.Equals(baseType) then
                true
            // Recursively check the base
            else
                this.BaseType.IsDerivedFrom(baseType);

        /// Checks whether the type symbol is directly or indirectly derived from the <see cref="SafetySharp.Modeling.Component"/> class.
        member this.IsDerivedFromComponent (compilation : Compilation) =
            nullArg this "this"
            nullArg compilation "compilation"
            compilation.GetComponentClassSymbol () |> this.IsDerivedFrom

        /// Checks whether the type symbol directly or indirectly implements the <see cref="SafetySharp.Modeling.IComponent"/> interface.
        member this.ImplementsIComponent (compilation : Compilation) =
            nullArg this "this"
            nullArg compilation "compilation"
            compilation.GetComponentInterfaceSymbol () |> this.IsDerivedFrom

        /// Checks whether the type symbol is directly or indirectly derived from the <see cref="SafetySharp.Modeling.Component"/> class.
        member this.IsDerivedFromComponent (semanticModel : SemanticModel) =
            nullArg this "this"
            nullArg semanticModel "semanticModel"
            this.IsDerivedFromComponent semanticModel.Compilation

        /// Checks whether the type symbol directly or indirectly implements the <see cref="SafetySharp.Modeling.IComponent"/> interface.
        member this.ImplementsIComponent (semanticModel : SemanticModel) =
            nullArg this "this"
            nullArg semanticModel "semanticModel"
            this.ImplementsIComponent semanticModel.Compilation