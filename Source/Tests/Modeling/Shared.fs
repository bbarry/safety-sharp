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

namespace SafetySharp.Tests.Modeling

open System
open System.Linq
open System.Linq.Expressions
open System.Reflection
open NUnit.Framework
open Swensen.Unquote
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp.Syntax
open SafetySharp.CSharp
open SafetySharp.Metamodel
open SafetySharp.Modeling
open SafetySharp.Tests.CSharp

[<AutoOpen>]
module private ModelingShared =
    // This code depends on the F# compiler creating an internal field called 'fieldName@' for a 'val fieldName' expression
    let fieldName field = field + "@"

    let createExpression<'T> (component' : Component) field = 
        let fieldInfo = component'.GetType().GetField(fieldName field, BindingFlags.NonPublic ||| BindingFlags.Instance)
        if fieldInfo = null then
            sprintf "Unable to find field '%s' in '%s'." field (component'.GetType().FullName) |> invalidOp
        Expression.Lambda<Func<'T>>(Expression.MakeMemberAccess(Expression.Constant(component'), fieldInfo))

    type EmptyComponent () =
        inherit Component ()

    type FieldComponent<'T> =
        inherit Component 
    
        val _field : 'T

        new () = { _field = Unchecked.defaultof<'T> }

        new (value : 'T) as this = { _field = Unchecked.defaultof<'T> } then
            this.SetInitialValues (createExpression<'T> this "_field", value)

        new (value1 : 'T, value2 : 'T) as this = { _field = Unchecked.defaultof<'T> } then
            this.SetInitialValues (createExpression<'T> this "_field", value1, value2)

    type FieldComponent<'T1, 'T2> =
        inherit Component 
    
        val _field1 : 'T1
        val _field2 : 'T2

        new () = { _field1 = Unchecked.defaultof<'T1>; _field2 = Unchecked.defaultof<'T2> }

        new (value1 : 'T1, value2 : 'T2) as this = 
            { _field1 = Unchecked.defaultof<'T1>; _field2 = Unchecked.defaultof<'T2> } then
            this.SetInitialValues (createExpression<'T1> this "_field1", value1)
            this.SetInitialValues (createExpression<'T2> this "_field2", value2)

        new (value1a : 'T1, value1b : 'T1, value2a : 'T2, value2b : 'T2) as this = 
            { _field1 = Unchecked.defaultof<'T1>; _field2 = Unchecked.defaultof<'T2> } then
            this.SetInitialValues (createExpression<'T1> this "_field1", value1a, value1b)
            this.SetInitialValues (createExpression<'T2> this "_field2", value2a, value2b)
        
    type OneSubcomponent =
        inherit Component

        val _component : Component

        new () = { _component = Unchecked.defaultof<Component> }
        new component' = { _component = component' }

    type TwoSubcomponents =
        inherit Component

        val _component1 : Component
        val _component2 : Component

        new (component1, component2) = { _component1 = component1; _component2 = component2 }

    type ComplexComponent =
        inherit Component

        val _nested1 : Component
        val _nested2 : Component
        val _other : obj

        new (nested1, nested2, other) =
            { _nested1 = nested1; _nested2 = nested2; _other = other }

    type EmptyModel () =
        inherit Model ()

    type TestModel ([<ParamArray>] components : Component array) as this =
        inherit Model ()

        do this.SetPartitions components