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

namespace SafetySharp.Tests

open System
open System.Collections
open System.Collections.Generic
open System.Linq
open System.Linq.Expressions
open System.IO
open System.Text
open System.Reflection
open System.Runtime.CompilerServices
open SafetySharp.Modeling
open SafetySharp.Internal.Metamodel
open Microsoft.FSharp.Reflection
open NUnit.Framework

module private ObjectDumper =
 
    /// Outputs object trees.
    type private ObjectWriter () =
        let output = StringBuilder ()
        let mutable atBeginningOfLine = true
        let mutable indent = 0

        /// Appends the given string to the current line.
        member this.Append s =
            this.AddIndentation()
            Printf.bprintf output s

        /// Appends the given string to the current line and starts a new line.
        member this.AppendLine s =
            let result = this.Append s
            this.NewLine ()
            result

        /// Appends a new line to the buffer.
        member this.NewLine() =
            output.AppendLine () |> ignore
            atBeginningOfLine <- true

        /// Appends a block statement to the buffer, i.e., generates a set of curly braces on separate lines,
        /// increases the indentation and generates the given content within the block.
        member this.AppendBlockStatement content front back =
            this.EnsureNewLine ()
            this.AppendLine front
            this.IncreaseIndent ()
            content ()
            this.EnsureNewLine ()
            this.DecreaseIndent ()
            this.Append back

        /// Appends the given elements using the given content generator, using the given separator to separate each element.
        member this.AppendRepeated elements content separator =
            let mutable first = true
            for element in elements do
                if not first then
                    separator ()
                else
                    first <- false
                content element
                
        member private this.EnsureNewLine () =
            if not atBeginningOfLine then
                this.NewLine ()

        member private this.AddIndentation () =
            if atBeginningOfLine then 
                atBeginningOfLine <- false
                for i = 1 to indent do
                    output.Append ("    ") |> ignore

        /// Increases the indentation level, starting with the next line.
        member this.IncreaseIndent() = indent <- indent + 1

        /// Decreases the indentation level, starting with the next line.
        member this.DecreaseIndent() = indent <- indent - 1
            
        /// Returns the generated output.
        override this.ToString () =
            output.ToString ()

    /// Dumps the given object for debugging purposes.
    let dump (object' : obj) =
        
        let duplicationCheck = HashSet<obj> ({ new IEqualityComparer<obj> with
            member this.Equals (symbol1, symbol2) = 
                obj.ReferenceEquals (symbol1, symbol2)
            member this.GetHashCode symbol =
                RuntimeHelpers.GetHashCode symbol
        })

        let maxLevel = 5
        let currentLevel = ref 0
        let writer = ObjectWriter ()
        let asEnumerable (object' : obj) = 
            (object' :?> IEnumerable).Cast<obj> ()

        let rec dumpEnumerable (elements : obj seq) front back =
            writer.AppendBlockStatement (fun () -> writer.AppendRepeated elements dump (fun () -> writer.Append ", ")) front back

        and dumpMap (elements : obj seq) =
            let elements = Array.ofSeq elements
            if elements.Length = 0 then
                writer.Append "[]"
            else
                let keyProperty = elements.[0].GetType().GetProperty "Key"
                let valueProperty = elements.[0].GetType().GetProperty "Value"

                writer.Append "Map ["
                writer.IncreaseIndent ()
                writer.NewLine ()
                for element in elements do
                    writer.AppendBlockStatement (fun () ->
                        writer.AppendBlockStatement (fun () -> dump (keyProperty.GetValue element)) "Key =" ""
                        writer.AppendBlockStatement (fun () -> dump (valueProperty.GetValue element)) "Value =" ""
                    ) "{" "}"
                    writer.NewLine ()
                writer.DecreaseIndent ()
                writer.Append "]"

        and dumpObject (object' : obj) =
            let dumpProperties typeName (properties : PropertyInfo array) =
                writer.Append "%s" typeName
                if properties.Length > 0 then
                    writer.AppendBlockStatement (fun () ->
                        writer.AppendRepeated properties (fun property -> 
                            writer.Append "%s = " property.Name 
                            dump (property.GetValue object')
                        ) (fun () -> writer.AppendLine ",")
                    ) "{" "}"

            let objectType = object'.GetType ()
            if FSharpType.IsUnion (objectType, true) then
                let (info, values) = FSharpValue.GetUnionFields (object', objectType, true)
                writer.Append "%s" info.Name
                if values.Length = 1 then
                    writer.Append " "
                    dump values.[0]
                elif values.Length > 1 then
                    writer.AppendBlockStatement (fun () -> writer.AppendRepeated values dump (fun () -> writer.AppendLine ", ")) "(" ")"
            elif FSharpType.IsRecord (objectType, true) then
                dumpProperties objectType.Name <| FSharpType.GetRecordFields (objectType, true)
            elif FSharpType.IsTuple objectType then
                writer.Append "("
                writer.AppendRepeated (FSharpValue.GetTupleFields object') dump (fun () -> writer.Append ", ")
                writer.Append ")"
            else
                incr currentLevel 
                if !currentLevel >= maxLevel then
                    writer.Append "..."
                else
                    let bindingFlags = BindingFlags.Instance ||| BindingFlags.Public ||| BindingFlags.NonPublic
                    let properties = objectType.GetProperties bindingFlags |> Seq.where (fun property -> property.CanRead) |> Array.ofSeq
                    dumpProperties objectType.Name properties
                decr currentLevel

        and dump (object' : obj) : unit =
            try
                if duplicationCheck.Add object' = false then
                    writer.Append "#object has already been dumped elsewhere#"
                elif object' = null then
                    writer.Append "null"
                else
                    let objectType = object'.GetType ()
                    if objectType.IsArray then
                        dumpEnumerable (asEnumerable object') "[|" "|]"
                    elif objectType.FullName.StartsWith "Microsoft.FSharp.Collections.FSharpList" then
                        dumpEnumerable (asEnumerable object') "[" "]"
                    elif objectType.FullName.StartsWith "Microsoft.FSharp.Collections.FSharpMap" then
                        dumpMap <| asEnumerable object'
                    elif object' :? string then
                        writer.Append "%A" object'
                    elif objectType.FullName.StartsWith "Microsoft.FSharp.Core.FSharpOption" then
                        dump (object'.GetType().GetProperty("Value").GetValue object')
                    elif object' :? IEnumerable then
                        dumpEnumerable (asEnumerable object') "seq {" "}"
                    elif objectType.IsPrimitive then
                        writer.Append "%A" object'
                    else
                        dumpObject object'
            with e -> writer.Append "#exception#"

        dump object'
        writer.ToString ()

[<AutoOpen>]
module internal TestHelpers =
    
    /// Raises an <see cref="InvalidOperationException" /> with the given message.
    let inline invalidOp message = Printf.ksprintf invalidOp message

     /// Checks whether the given function raises an exception of the given type.
    let raisesWith<'T when 'T :> Exception> func =
        let mutable thrownException : Exception option = None
        try 
            func ()
        with 
        | :? 'T as e -> 
            thrownException <- Some <| upcast e
        | e ->
            thrownException <- Some e

        match thrownException with
        | None ->
            Assert.Fail ("Expected an exception of type '{0}', but no exception was thrown.", typeof<'T>.FullName)
            Unchecked.defaultof<'T>
        | Some (:? 'T as e) ->
            e
        | Some e ->
            let message = "Expected an exception of type '{0}', but an exception of type '{1}' was thrown instead.\n\nActual:\n{2}"
            Assert.Fail (message, typeof<'T>.FullName, e.GetType().FullName, ObjectDumper.dump e)
            Unchecked.defaultof<'T>

    /// Checks whether the given function raises an exception of the given type.
    let raises<'T when 'T :> Exception> func =
        raisesWith<'T> func |> ignore

    /// Checks whether the given function raises an <see cref="ArgumentNullException"/> for the argument with the given name.
    let raisesArgumentNullException argumentName func =
        let e = raisesWith<ArgumentNullException> func
        Assert.AreEqual (e.ParamName, argumentName, "Expected exception to be thrown for argument '{0}', but was thrown for '{1}'.", argumentName, e.ParamName)

    /// Checks whether the given function raises an <see cref="ArgumentException"/> for the argument with the given name.
    let raisesArgumentException argumentName func =
        let e = raisesWith<ArgumentException> func
        Assert.AreEqual (e.ParamName, argumentName, "Expected exception to be thrown for argument '{0}', but was thrown for '{1}'.", argumentName, e.ParamName)

    /// Asserts that the two values are equal.
    let (=?) left right =
        let result = left = right
        if not result then
            let actual = ObjectDumper.dump left
            let expected = ObjectDumper.dump right
            Assert.Fail ("Objects are not equal, even though they are expected to be equal.\n\nExpected:\n{0}\n\nActual:\n{1}", expected, actual)

    /// Asserts that the two values are not equal.
    let (<>?) left right =
        let result = left <> right
        if not result then
            Assert.Fail ("Objects are equal, even though they are expected to be different.\n\n{0}", ObjectDumper.dump left)

    /// Gets a component symbol with the given component name, with an empty update method and no fields or subcomponents.
    let emptyComponentSymbol name = { 
        Name = name
        UpdateMethod = None
        Fields = []
        ProvidedPorts = []
        RequiredPorts = []
    } 

    /// Gets a component object with the given name and component symbol, with no fields or subcomponents.
    let emptyComponentObject name symbol = 
        { Name = name; ComponentSymbol = symbol; Fields = Map.empty; Subcomponents = Map.empty; Bindings = Map.empty }