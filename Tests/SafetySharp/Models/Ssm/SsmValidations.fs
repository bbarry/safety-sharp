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

namespace Models.Ssm.Validation

open System
open System.Linq
open NUnit.Framework
open Mono.Cecil
open SafetySharp.Modeling
open SafetySharp.Models
open SafetySharp.Models.Ssm

[<TestFixture>]
module ``Invalid bindings`` =

    let private transform csharpCode =
        let model = TestCompilation.CreateModel csharpCode
        model.FinalizeMetadata ()
        (model, CilToSsm.transformModel model)

    let private check kinds binders sourceComponents targetComponents sourceMethods targetMethods csharpCode =
        let (model, ssm) = transform csharpCode
        let e = raisesWith<InvalidBindingsException> (fun () -> SsmValidation.validate model ssm)
        e.InvalidBindings |> List.ofArray |> List.map (fun binding -> binding.Kind) =? kinds
        e.InvalidBindings |> List.ofArray |> List.map (fun binding -> binding.BinderName) =? binders
        e.InvalidBindings |> List.ofArray |> List.map (fun binding -> (binding.SourcePort.Component :?> Component).UnmangledName) =? sourceComponents
        e.InvalidBindings |> List.ofArray |> List.map (fun binding -> (binding.TargetPort.Component :?> Component).UnmangledName) =? targetComponents
        e.InvalidBindings |> List.ofArray |> List.map (fun binding -> binding.SourcePort.Method.Name) =? sourceMethods
        e.InvalidBindings |> List.ofArray |> List.map (fun binding -> binding.TargetPort.Method.Name) =? targetMethods

    [<Test>]
    let ``binding at same level of hierarchy is valid`` () =
        let (model, ssm) = 
            transform
              "class X : Component { extern void M(); void N() {} public X() { Bind(RequiredPorts.M = ProvidedPorts.N).Delayed(); } }
               class TestModel : Model { public TestModel() { SetRootComponents(new X()); } }"

        nothrow (fun () -> SsmValidation.validate model ssm)

        let (model, ssm) = 
            transform
              "class X : Component { public extern void M(); public void N() {} }
               class Y : Component { X x = new X(); public Y() { Bind(x.RequiredPorts.M = x.ProvidedPorts.N).Delayed(); } }
               class TestModel : Model { public TestModel() { SetRootComponents(new Y()); } }"

        nothrow (fun () -> SsmValidation.validate model ssm)

    [<Test>]
    let ``binding at spanning one level of the hierarchy is valid`` () =
        let (model, ssm) = 
            transform
              "class X : Component { public extern void M(); }
               class Y : Component { void N() {} X x = new X(); public Y() { Bind(x.RequiredPorts.M = ProvidedPorts.N).Delayed(); } }
               class TestModel : Model { public TestModel() { SetRootComponents(new Y()); } }"

        nothrow (fun () -> SsmValidation.validate model ssm)

        let (model, ssm) = 
            transform
              "class X : Component { public void N() {}  }
               class Y : Component { extern void M(); X x = new X(); public Y() { Bind(RequiredPorts.M = x.ProvidedPorts.N).Delayed(); } }
               class TestModel : Model { public TestModel() { SetRootComponents(new Y()); } }"

        nothrow (fun () -> SsmValidation.validate model ssm)

    [<Test>]
    let ``binding with subsubcomponent is invalid`` () =
        check [Delayed] ["Root0"] ["Root0.x.z"] ["Root0"] ["N"] ["M"]
          "class Z : Component { public void N() {} }
           class X : Component { Z z; public X(Z z) { this.z = z; } }
           class Y : Component { extern void M(); X x; public Y(X x, Z z) { this.x = x; Bind(RequiredPorts.M = z.ProvidedPorts.N).Delayed(); } }
           class TestModel : Model { public TestModel() { var z = new Z(); SetRootComponents(new Y(new X(z), z)); } }"

        check [Delayed] ["Root0"] ["Root0"] ["Root0.x.z"] ["M"] ["N"]
          "class Z : Component { public extern void N(); }
           class X : Component { Z z; public X(Z z) { this.z = z; } }
           class Y : Component { void M() {} X x; public Y(X x, Z z) { this.x = x; Bind(z.RequiredPorts.N = ProvidedPorts.M).Delayed(); } }
           class TestModel : Model { public TestModel() { var z = new Z(); SetRootComponents(new Y(new X(z), z)); } }"

    [<Test>]
    let ``binding with non-subcomponent is invalid`` () =
        check [Delayed] ["Root1"] ["Root0.z"] ["Root1"] ["N"] ["M"]
          "class Z : Component { public void N() {} }
           class X : Component { Z z; public X(Z z) { this.z = z; } }
           class Y : Component { extern void M(); public Y(Z z) { Bind(RequiredPorts.M = z.ProvidedPorts.N).Delayed(); } }
           class TestModel : Model { public TestModel() { var z = new Z(); SetRootComponents(new X(z), new Y(z)); } }"

        check [Delayed] ["Root1"] ["Root1"] ["Root0.z"] ["M"] ["N"]
          "class Z : Component { public extern void N(); }
           class X : Component { Z z; public X(Z z) { this.z = z; } }
           class Y : Component { void M() {} public Y(Z z) { Bind(z.RequiredPorts.N = ProvidedPorts.M).Delayed(); } }
           class TestModel : Model { public TestModel() { var z = new Z(); SetRootComponents(new X(z), new Y(z)); } }"

[<TestFixture>]
module ``Unbound required ports`` =

    let private transform csharpCode =
        let model = TestCompilation.CreateModel csharpCode
        model.FinalizeMetadata ()
        (model, CilToSsm.transformModel model)

    let private check componentNames portNames csharpCode =
        let (model, ssm) = transform csharpCode
        let e = raisesWith<UnboundRequiredPortsException> (fun () -> SsmValidation.validate model ssm)
        e.UnboundPorts |> List.ofArray |> List.map (fun p -> (p.Component :?> Component).UnmangledName) =? componentNames
        e.UnboundPorts |> List.ofArray |> List.map (fun p -> p.Method.Name) =? portNames

    [<Test>]
    let ``valid when all ports are bound`` () =
        let (model, ssm) = 
            transform
              "class X : Component { extern void M(); void N() {} public X() { Bind(RequiredPorts.M = ProvidedPorts.N).Delayed(); } }
               class TestModel : Model { public TestModel() { SetRootComponents(new X()); } }"

        nothrow (fun () -> SsmValidation.validate model ssm)

    [<Test>]
    let ``invalid when single port is unbound`` () =
        check ["Root0"] ["M"] 
          "class X : Component { extern void M(); }
           class TestModel : Model { public TestModel() { SetRootComponents(new X()); } }"

    [<Test>]
    let ``invalid when subcomponent port is unbound`` () =
        check ["Root0.x"] ["N"] 
          "class X : Component { public extern void M(); public extern void N(); }
           class Y : Component { void N() {} X x = new X(); public Y() { Bind(x.RequiredPorts.M = ProvidedPorts.N); } }
           class TestModel : Model { public TestModel() { SetRootComponents(new Y()); } }"

[<TestFixture>]
module ``Ambiguous required port bindings`` =

    let private transform csharpCode =
        let model = TestCompilation.CreateModel csharpCode
        model.FinalizeMetadata ()
        (model, CilToSsm.transformModel model)

    let private check bindings kinds binders csharpCode =
        let (model, ssm) = transform csharpCode
        let e = raisesWith<AmbiguousRequiredPortBindingsException> (fun () -> SsmValidation.validate model ssm)
        e.AmbiguousBindings |> List.ofArray |> List.map (fun p -> 
            p |> List.ofArray |> List.map (fun b -> 
                ((b.TargetPort.Component :?> Component).UnmangledName,
                 b.TargetPort.Method.Name,
                 (b.SourcePort.Component :?> Component).UnmangledName,
                 b.SourcePort.Method.Name)
            )
        ) =? bindings
        let bindings = e.AmbiguousBindings |> Array.collect id
        bindings |> List.ofArray |> List.map (fun binding -> binding.Kind) =? kinds
        bindings |> List.ofArray |> List.map (fun binding -> binding.BinderName) =? binders

    [<Test>]
    let ``valid when all ports are bound`` () =
        let (model, ssm) = 
            transform
              "class X : Component { extern void M(); void N() {} public X() { Bind(RequiredPorts.M = ProvidedPorts.N).Delayed(); } }
               class TestModel : Model { public TestModel() { SetRootComponents(new X()); } }"

        nothrow (fun () -> SsmValidation.validate model ssm)

    [<Test>]
    let ``invalid when single port is bound ambiguously by the same component`` () =
        check [[("Root0", "M", "Root0", "N"); ("Root0", "M", "Root0", "R")]] [Instantaneous; Delayed] ["Root0"; "Root0"]
          "class X : Component { extern void M(); void N() {} void R() {} public X() { Bind(RequiredPorts.M = ProvidedPorts.N); Bind(RequiredPorts.M = ProvidedPorts.R).Delayed(); } }
           class TestModel : Model { public TestModel() { SetRootComponents(new X()); } }"

    [<Test>]
    let ``invalid when single port is bound ambiguously by different components`` () =
        check [["Root0.x", "M", "Root0", "N"; "Root0.x", "M", "Root0.x", "N"]] [Instantaneous; Delayed] ["Root0"; "Root0.x"]
          "class X : Component { public extern void M(); void N() {} public X() { Bind(RequiredPorts.M = ProvidedPorts.N).Delayed(); } }
           class Y : Component { X x = new X(); void N() {} public Y() { Bind(x.RequiredPorts.M = ProvidedPorts.N); } }
           class TestModel : Model { public TestModel() { SetRootComponents(new Y()); } }"

    [<Test>]
    let ``invalid when multiple ports are bound ambiguously`` () =
        check [[("Root0", "N", "Root0", "A"); ("Root0", "N", "Root0", "B")]; [("Root0", "M", "Root0", "A"); ("Root0", "M", "Root0", "B")]] 
          [Delayed; Instantaneous; Delayed; Instantaneous] ["Root0"; "Root0"; "Root0"; "Root0"]
          "class X : Component { 
                extern void N();
                extern void M();
                void A() {}
                void B() {}
                public X() {
                    Bind(RequiredPorts.N = ProvidedPorts.A).Delayed();
                    Bind(RequiredPorts.N = ProvidedPorts.B);
                    Bind(RequiredPorts.M = ProvidedPorts.A).Delayed();
                    Bind(RequiredPorts.M = ProvidedPorts.B);
                }
           }
           class TestModel : Model { public TestModel() { SetRootComponents(new X()); } }"

[<TestFixture>]
module ``Cyclic control flow`` =
    let private transform csharpCode =
        let model = TestCompilation.CreateModel csharpCode
        model.FinalizeMetadata ()
        (model, CilToSsm.transformModel model)

    let private check componentNames portNames csharpCode =
        let (model, ssm) = transform csharpCode
        let e = raisesWith<CyclicControlFlowException> (fun () -> SsmValidation.validate model ssm)
        e.ControlFlow |> List.ofArray |> List.map (fun (c, _) -> c.UnmangledName) =? componentNames
        e.ControlFlow |> List.ofArray |> List.map (fun (_, m) -> m.Name) =? portNames

    [<Test>]
    let ``model without any cycles is valid`` () =
        let (model, ssm) = 
            transform
              "class X : Component { 
                extern void M(); 
                void N() { }
                public X() { Bind(RequiredPorts.M = ProvidedPorts.N); }
           }
           class TestModel : Model { public TestModel() { SetRootComponents(new X()); } }"

        nothrow (fun () -> SsmValidation.validate model ssm)

    [<Test>]
    let ``model with simple delayed binding cycle is valid`` () =
        let (model, ssm) = 
            transform
              "class X : Component { 
                    extern void M(); 
                    void N() { M(); }
                    public X() { Bind(RequiredPorts.M = ProvidedPorts.N).Delayed(); }
               }
               class TestModel : Model { public TestModel() { SetRootComponents(new X()); } }"

        nothrow (fun () -> SsmValidation.validate model ssm)

    [<Test>]
    let ``model with simple instantaneous binding cycle is invalid`` () =
        check ["Root0"; "Root0"] ["M"; "N"] 
          "class X : Component { 
                extern void M(); 
                void N() { M(); }
                public X() { Bind(RequiredPorts.M = ProvidedPorts.N); }
           }
           class TestModel : Model { public TestModel() { SetRootComponents(new X()); } }"

    [<Test>]
    let ``model with recursive function is invalid`` () =
        check ["Root0"] ["N"] 
          "class X : Component { 
                void N() { N(); }
           }
           class TestModel : Model { public TestModel() { SetRootComponents(new X()); } }"

    [<Test; Ignore "Not yet implemented">]
    let ``model with recursive update function is invalid`` () =
        check ["Root0"] ["Update"] 
          "class X : Component { 
                public override void Update() { Update(); }
           }
           class TestModel : Model { public TestModel() { SetRootComponents(new X()); } }"

    [<Test>]
    let ``model with base update call is valid`` () =
        let (model, ssm) = 
            transform
                "class X : Component { 
                      public override void Update() { base.Update(); }
                 }
                 class TestModel : Model { public TestModel() { SetRootComponents(new X()); } }"

        nothrow (fun () -> SsmValidation.validate model ssm)

    [<Test>]
    let ``model with mutually recursive functions is invalid`` () =
        check ["Root0"; "Root0"] ["M"; "N"] 
          "class X : Component { 
                void M() { N(); }
                void N() { M(); }
           }
           class TestModel : Model { public TestModel() { SetRootComponents(new X()); } }"

    [<Test>]
    let ``model with long cycle involving subcomponents is invalid`` () =
        check ["Root0.t1"; "Root0.t2"; "Root0.t2"; "Root0"; "Root0"; "Root0.t1"] ["Req"; "Prov"; "Req"; "N"; "M"; "Prov"] 
          "class T : Component {
                public extern void Req();
                public void Prov() { Req(); }
           }
           class X : Component { 
                T t1 = new T();
                T t2 = new T();
                extern void M();
                void N() { M(); }
                public X() {
                    Bind(t1.RequiredPorts.Req = t2.ProvidedPorts.Prov);
                    Bind(t2.RequiredPorts.Req = ProvidedPorts.N);
                    Bind(RequiredPorts.M = t1.ProvidedPorts.Prov);
                }
           }
           class TestModel : Model { public TestModel() { SetRootComponents(new X()); } }"

    [<Test>]
    let ``model involving recursive direct subcomponent provided port call is invalid`` () =
        check ["Root0.t"; "Root0"; "Root0.t"] ["Req"; "N"; "Prov"] 
          "class T : Component {
                public extern void Req();
                public void Prov() { Req(); }
           }
           class X : Component { 
                T t = new T();
                void N() { t.Prov(); }
                public X() {
                    Bind(t.RequiredPorts.Req = ProvidedPorts.N);
                }
           }
           class TestModel : Model { public TestModel() { SetRootComponents(new X()); } }"

    [<Test>]
    let ``model with long cycle involving subcomponents boken up by delayed binding is valid`` () =
        let (model, ssm) = 
            transform
                "class T : Component {
                    public extern void Req();
                    public void Prov() { Req(); }
                 }
                 class X : Component { 
                      T t1 = new T();
                      T t2 = new T();
                      extern void M();
                      void N() { M(); }
                      public X() {
                          Bind(t1.RequiredPorts.Req = t2.ProvidedPorts.Prov);
                          Bind(t2.RequiredPorts.Req = ProvidedPorts.N).Delayed();
                          Bind(RequiredPorts.M = t1.ProvidedPorts.Prov);
                      }
                 }
                 class TestModel : Model { public TestModel() { SetRootComponents(new X()); } }"

        nothrow (fun () -> SsmValidation.validate model ssm)
