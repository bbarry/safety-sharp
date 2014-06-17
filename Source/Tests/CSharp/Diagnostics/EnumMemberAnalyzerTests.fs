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

namespace SafetySharp.Tests.CSharp.Diagnostics

open System.Linq
open System.Threading
open NUnit.Framework
open Swensen.Unquote
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp.Syntax
open Microsoft.CodeAnalysis.Diagnostics
open SafetySharp.CSharp
open SafetySharp.Tests
open SafetySharp.CSharp.Diagnostics
open SafetySharp.CSharp.Extensions

[<TestFixture>]
module EnumMemberAnalyzerTests =

    let validate csharpCode = 
        let compilation = TestCompilation csharpCode
        compilation.HasDiagnostics<EnumMemberAnalyzer> () |> not

    [<Test>]
    let ``enum declaration without explicit member values is valid`` () =
        validate "enum E { A, B, C }" =? true

    [<Test>]
    let ``enum declaration with explicit value on first member is invalid`` () =
        validate "enum E { A = 1, B, C }" =? false

    [<Test>]
    let ``enum declaration with explicit value on second member is invalid`` () =
        validate "enum E { A, B = 1, C }" =? false

    [<Test>]
    let ``enum declaration with explicit value on third member is invalid`` () =
        validate "enum E { A, B, C = 3 }" =? false

    [<Test>]
    let ``enum declaration with explicit values on all members is invalid`` () =
        validate "enum E { A = 4, B = 1, C = 3 }" =? false
