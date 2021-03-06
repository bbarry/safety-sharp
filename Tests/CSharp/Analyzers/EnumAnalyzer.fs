﻿// The MIT License (MIT)
// 
// Copyright (c) 2014-2015, Institute for Software & Systems Engineering
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

namespace Analyzers

open System
open System.Linq
open NUnit.Framework
open SafetySharp.Modeling
open SafetySharp.CSharp.Analyzers
open SafetySharp.CSharp.Roslyn.Syntax
open SafetySharp.CSharp.Roslyn.Symbols

[<TestFixture>]
module ``Enumerations`` =
    let getDiagnostic = TestCompilation.GetDiagnostic (EnumAnalyzer ())

    let underlyingType location length =
        errorDiagnostic DiagnosticIdentifier.ExplicitEnumType (1, location) (1, location + length)
            "Enum 'E' must not explicitly declare an underlying type."

    let memberValue location memberName =
        errorDiagnostic DiagnosticIdentifier.ExplicitEnumMemberValue (1, location) (1, location + 1)
            "Value of enum member 'E.%s' cannot be declared explicitly." memberName

    [<Test>]
    let ``implicit underlying type is valid`` () =
        getDiagnostic "enum E { A }" =? None

    [<Test>]
    let ``byte as underlying type is invalid`` () =
        getDiagnostic "enum E : byte { A }" =? underlyingType 9 4

    [<Test>]
    let ``int as underlying type is invalid`` () =
        getDiagnostic "enum E : int { A }" =? underlyingType 9 3

    [<Test>]
    let ``long as underlying type is invalid`` () =
        getDiagnostic "enum E : long { A }" =? underlyingType 9 4

    [<Test>]
    let ``sbyte as underlying type is invalid`` () =
        getDiagnostic "enum E : sbyte { A }" =? underlyingType 9 5

    [<Test>]
    let ``short as underlying type is invalid`` () =
        getDiagnostic "enum E : short { A }" =? underlyingType 9 5

    [<Test>]
    let ``uint as underlying type is invalid`` () =
        getDiagnostic "enum E : uint { A }" =? underlyingType 9 4

    [<Test>]
    let ``ulong as underlying type is invalid`` () =
        getDiagnostic "enum E : ulong { A }" =? underlyingType 9 5

    [<Test>]
    let ``ushort as underlying type is invalid`` () =
        getDiagnostic "enum E : ushort { A }" =? underlyingType 9 6

    [<Test>]
    let ``enum declaration without explicit member values is valid`` () =
        getDiagnostic "enum E { A, B, C }" =? None

    [<Test>]
    let ``enum declaration with explicit value on first member is invalid`` () =
        getDiagnostic "enum E { A = 1, B, C }" =? memberValue 13 "A"

    [<Test>]
    let ``enum declaration with explicit value on second member is invalid`` () =
        getDiagnostic "enum E { A, B = 1, C }" =? memberValue 16 "B"

    [<Test>]
    let ``enum declaration with explicit value on third member is invalid`` () =
        getDiagnostic "enum E { A, B, C = 3 }" =? memberValue 19 "C"