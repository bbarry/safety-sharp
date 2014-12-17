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

namespace SafetySharp.Tests.Modelchecking.NuXmv.NuXmvTransformMetamodelTests


open NUnit.Framework
open SafetySharp.Tests.Modelchecking
open SafetySharp
open SafetySharp.Internal.Modelchecking
open SafetySharp.Internal.Modelchecking.NuXmv

[<TestFixture>]
module TestCase1ToNuXmvTests =
    open TestCase1
    
    [<Test>]
    let ``transforms model without exception`` () =
        let modelTransformer = MetamodelToNuXmv (testCase1Configuration)
        let nuXmvCode = modelTransformer.transformConfiguration
        ()
        
    [<Test>]
    let ``write transformed model to string`` () =
        let modelTransformer = MetamodelToNuXmv (testCase1Configuration)        
        let nuXmvCode = modelTransformer.transformConfiguration
        let nuXmvWriter = ExportNuXmvAstToFile()
        let nuXmvCodeString = nuXmvWriter.ExportNuXmvProgram nuXmvCode
        ()

    [<Test>]
    let ``write transformed model to file`` () =
        let modelTransformer = MetamodelToNuXmv (testCase1Configuration)        
        let nuXmvCode = modelTransformer.transformConfiguration
        let nuXmvWriter = ExportNuXmvAstToFile()
        let nuXmvCodeString = nuXmvWriter.ExportNuXmvProgram nuXmvCode
        FileSystem.WriteToAsciiFile "Modelchecking/NuXmv/testcase1.smv" nuXmvCodeString
        ()