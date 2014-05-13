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
namespace Tests.Modelchecking.Promela
{
    using System;
    using System.Collections.Immutable;
    using System.Diagnostics;
    using System.IO;
    using FluentAssertions;
    using NUnit.Framework;
    using SafetySharp.Modelchecking.Promela;
    using SafetySharp.Modelchecking.Promela.Expressions;
    using SafetySharp.Modelchecking.Promela.Statements;

    [TestFixture]
    public class EnumTests
    {

        [Test]
        public void SpinFound()
        {
            Spin.ExecuteSpin("-V").Should().Be(Spin.SpinResult.Success);
        }

        [Test]
        public void Test()
        {
            var filename = "Modelchecking\\Promela\\test1.pml";

            var expr_false = new BooleanLiteral(false);
            var expr_true = new BooleanLiteral(false);

            var stmnt_declvarx = new DeclarationStatement(PromelaTypeName.Bool, "x", 0, expr_false);
            var ref_varX = new VariableReferenceExpression("x", null, null);
            var stmnt_assignTrueToVarx = new AssignmentStatement(ref_varX, expr_true);


            var code = ImmutableArray.Create<Statement>().Add(stmnt_declvarx).Add(stmnt_assignTrueToVarx);

            var testProcType = new Proctype(true,"TestProcType",code);

            var fileWriter = new PromelaModelWriter();
            fileWriter.Visit(testProcType);

            fileWriter.CodeWriter.WriteToFile(filename);

            Spin.ExecuteSpin("-a "+filename).Should().Be(Spin.SpinResult.Success);
        }

    }
}