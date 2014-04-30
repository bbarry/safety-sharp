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

module Generator

open System
open System.Diagnostics
open System.Globalization
open System.IO
open System.Text
open System.Threading

//====================================================================================================================
// Metamodel code generator
//====================================================================================================================

// This F# script generates C# code for tree-shaped elements and visitors. From the provided metadata provided partial
// C# classes are generated for the elements and visitors containing all of the following boilerplate code:
// - Get-only properties
// - Constructors taking values for all properties, optionally performing validation
// - Additional validation is supported through a partial Validate() method
// - A With...(...) method for each property that creates a copy of the object, changing only the value of the given
//   property; if the property value has not changed, no copy is made and the original object is returned
// - An Add...(...) method for each property of collection type that creates a copy of the object, adding the given
//   values to the collection property; if the collection has not changed, no copy is made and the original object 
//   is returned
// - An Update(...) method that creates a new instance of the object if any of the property values have changed; if
//   none have changed, no copy is made and the original object is returned
// - Constructors, With* methods and Update() all take inherited properties into account, generating the appropriate
//   code and methods instead of relying on chains of virtual function calls
// - Accept() methods for metamodel visitors
// - Equals() methods, equality operators and GetHashCode() with value equality

// Set the thread culture to the invariant culture to avoid any possible problems by localized ToString() output
Thread.CurrentThread.CurrentCulture <- CultureInfo.InvariantCulture;
Thread.CurrentThread.CurrentUICulture <- CultureInfo.InvariantCulture;

//====================================================================================================================
// F# type definitions
//====================================================================================================================

/// <summary>
///     Indicates whether a property is a collection type, wrapping the property's type in one of the 
///     System.Collections.Immutable, if necessary. 
/// </summary>
type CollectionType = 
    | Singleton
    | Array
    | List

/// <summary>
///     Indicates the kind of parameter validation that should be performed by the generated constructors before 
///     assigning the given value to the property.
/// </summary>
type Validation =
    | None
    | NotNull
    | NotNullOrWhitespace
    | InRange

/// <summary>
///     Provides metadata for a class property that should be generated.
/// </summary>
type Property = { 
    Name : string
    Type : string
    CollectionType : CollectionType
    Validation : Validation
    Comment : string
    CanBeNull : bool
}

/// <summary>
///     Provides metadata for a class that should be generated.
/// </summary>
type Class = { 
    Name : string
    Base : string
    IsAbstract : bool
    Properties : Property list 
}

/// <summary>
///     Provides metadata for a namespace that should be generated.
/// </summary>
type Namespace = { 
    Name : string
    Classes : Class list 
}

/// <summary>
///     Represents the generic or the non-generic version of a visitor.
/// </summary>
type VisitorType = {
    VisitorType : string
    VisitorCommentType : string
    ParamType : string
    ParamTypeSpecifier : string
    ReturnType : string
    IsGeneric : bool
}

/// <summary>
///     Provides context information for the code generator.
/// </summary>
type Context = {
    Elements : Namespace list
    OutputFile : string
    BaseClass : string
    VisitorName : string
    RewriterName : string
    VisitorNamespace : string
    Public : bool
}

//====================================================================================================================
// CodeWriter helper class
//====================================================================================================================

/// <summary>
///     Generates C# code in an in-memory buffer and allows the generated code to be stored in a file.
/// </summary>
type CodeWriter() as this =
    let output = new StringBuilder()
    let mutable atBeginningOfLine = true
    let mutable indent = 0
    do this.AppendHeader()

    /// <summary>
    ///     Appends the given string to the current line.
    /// </summary>
    member public this.Append (s : string) =
        this.AddIndentation()
        output.Append s |> ignore

    /// <summary>
    ///     Appends the given string to the current line and starts a new line.
    /// </summary>
    member public this.AppendLine s =
        this.Append s
        this.NewLine()

    /// <summary>
    ///     Appends a new line to the buffer.
    /// </summary>
    member public this.NewLine() =
        output.AppendLine() |> ignore
        atBeginningOfLine <- true

    /// <summary>
    ///     Appends a block statement to the buffer, i.e., generates a set of curly braces on separate lines,
    ///     increases the indentation and generates the given content within the block.
    /// </summary>
    member public this.AppendBlockStatement content =
        this.EnsureNewLine()
        this.AppendLine("{")
        this.IncreaseIndent()

        content()

        this.EnsureNewLine()
        this.DecreaseIndent()
        this.Append("}")

        this.NewLine()

    /// <summary>
    ///     Writes the generated code to the file at the given path.
    /// </summary>
    member public this.WriteToFile path =
        File.WriteAllText(path, output.ToString())

    member private this.EnsureNewLine() =
        if not atBeginningOfLine then
            this.NewLine()

    member private this.AddIndentation() =
        if atBeginningOfLine then 
            atBeginningOfLine <- false
            for i = 1 to indent do
                output.Append("    ") |> ignore

    /// <summary>
    ///     Increases the indentation level, starting with the next line.
    /// </summary>
    member public this.IncreaseIndent() = indent <- indent + 1

    /// <summary>
    ///     Decreases the indentation level, starting with the next line.
    /// </summary>
    member public this.DecreaseIndent() = indent <- indent - 1

    /// <summary>
    ///     Writes a header that indicates that the file has been generated by a tool.
    /// </summary>
    member private this.AppendHeader() =
        this.AppendLine("// The MIT License (MIT)")
        this.AppendLine("//")
        this.AppendLine("// Copyright (c) 2014, Institute for Software & Systems Engineering")
        this.AppendLine("//")
        this.AppendLine("// Permission is hereby granted, free of charge, to any person obtaining a copy")
        this.AppendLine("// of this software and associated documentation files (the \"Software\"), to deal")
        this.AppendLine("// in the Software without restriction, including without limitation the rights")
        this.AppendLine("// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell")
        this.AppendLine("// copies of the Software, and to permit persons to whom the Software is")
        this.AppendLine("// furnished to do so, subject to the following conditions:")
        this.AppendLine("//")
        this.AppendLine("// The above copyright notice and this permission notice shall be included in")
        this.AppendLine("// all copies or substantial portions of the Software.")
        this.AppendLine("//")
        this.AppendLine("// THE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR")
        this.AppendLine("// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,")
        this.AppendLine("// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE")
        this.AppendLine("// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER")
        this.AppendLine("// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,")
        this.AppendLine("// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN")
        this.AppendLine("// THE SOFTWARE.")
        this.NewLine()

        this.AppendLine("// ------------------------------------------------------------------------------")
        this.AppendLine("// <auto-generated>")
        this.AppendLine("//     Generated by the Safety Sharp Code Generator.")
        this.AppendLine(sprintf "//     %s, %s" (DateTime.Now.ToLongDateString()) (DateTime.Now.ToLongTimeString()))
        this.AppendLine("//")
        this.AppendLine("//     Changes to this file may cause incorrect behavior and will be lost if")
        this.AppendLine("//     the code is regenerated.")
        this.AppendLine("// </auto-generated>")
        this.AppendLine("// ------------------------------------------------------------------------------")
        this.NewLine()

//====================================================================================================================
// Metamodel element code generation
//====================================================================================================================

let generateCode context =

    /// <summary>
    ///     The code writer that is used to generate the C# code.
    /// </summary>
    let output = new CodeWriter()

     /// <summary>
    ///     The visibility modifier that should be used for the generated classes.
    /// </summary>
    let visibility = if context.Public then "public" else "internal"

    /// <summary>
    ///     Ensures that the first character of the given string is lower case.
    /// </summary>
    let startWithLowerCase (s : string) =
        sprintf "%c%s" <| Char.ToLower(s.[0]) <| s.Substring(1)

    /// <summary>
    ///     Ensures that the given string is a valid C# identifier, prefixing the name with '@' to escape reserved C# keywords.
    /// </summary>
    let getValidCSharpIdentifier (s : string) = 
        let s = startWithLowerCase s
        match s with // List of C# keywords taken from Roslyn source code
        | "bool" | "byte" | "sbyte" | "short" | "ushort" | "int" | "uint" | "long" | "ulong" | "double" | "float" | "decimal" 
        | "string" | "char" | "object" | "typeof" | "sizeof" | "null" | "true" | "false" | "if" | "else"  | "while" | "for"
        | "foreach" | "do" | "switch" | "case" | "default" | "lock" | "try" | "throw" | "catch" | "finally" | "goto" | "break"
        | "continue" | "return" | "public" | "private" | "internal" | "protected" | "static" | "readonly" | "sealed" | "const"
        | "new" | "override" | "abstract" | "virtual" | "partial" | "ref" | "out" | "in" | "where" | "params" | "this" | "base"
        | "namespace" | "using" | "class" | "struct" | "interface" | "delegate" | "checked" | "get" | "set" | "add" | "remove"
        | "operator" | "implicit" | "explicit" | "fixed" | "extern" | "event" | "enum" | "unsafe"
            -> sprintf "@%s" s 
        | _
            -> s

    /// <summary>
    ///     Applies the projection function to the property list and joins the resulting string using the given separator.
    /// </summary>
    /// <param name="separator">The separator that should be placed between two adjacent property projections.</param>
    /// <param name="proj">The projection function that maps a property to a string.</param>
    /// <param name="p">The properties that should be joined.</param>
    let joinProperties separator proj (p : Property list) = 
        let collected = p |> List.map proj
        String.Join(separator, collected)

    /// <summary>
    ///     Writes the required using statements to the output.
    /// </summary>
    let writeUsings elements n =
        // Default system namespaces
        output.AppendLine("using System;")
        output.AppendLine("using System.Linq;")
        output.AppendLine("using System.Collections.Immutable;");
        output.NewLine()

        // The namespaces defined in the metadata; however, do not include the namespace we're currently in
        output.AppendLine("using SafetySharp.Utilities;");
        for n in elements |> List.filter (fun n' -> n'.Name <> n) do
            output.AppendLine(sprintf "using %s;" n.Name)

    /// <summary>
    ///     Gets the C# type of the property, depending on whether the property is a collection.
    /// </summary>
    let getType (p : Property) =
        match p.CollectionType with
        | Singleton -> p.Type
        | Array -> sprintf "ImmutableArray<%s>" p.Type
        | List -> sprintf "ImmutableList<%s>" p.Type

    /// <summary>
    ///     Writes the given message to the console using the given color.
    /// </summary>
    let writeColored message color =
        let c' = Console.ForegroundColor
        Console.ForegroundColor <- color

        printfn "%s" message
        Console.ForegroundColor <- c'

        Debug.WriteLine(message)

    /// <summary>
    ///     Creates a visitor type information object based on the visitor's type parameter. The non-generic visitor
    ///     version is generated if the given type parameter string is empty.
    /// </summary>
    let createVisitorType typeParam =
        if typeParam = "" then
            { 
                VisitorType = context.VisitorName
                VisitorCommentType = context.VisitorName
                ParamType = ""
                ParamTypeSpecifier = ""
                ReturnType = "void"
                IsGeneric = false 
            }
        else
            { 
                VisitorType = sprintf "%s<%s>" context.VisitorName typeParam
                VisitorCommentType = sprintf "%s{%s}" context.VisitorName typeParam;
                ParamType = typeParam
                ParamTypeSpecifier = sprintf "<%s>" typeParam
                ReturnType = typeParam
                IsGeneric = true
            }


    /// <summary>
    ///     The list of all classes defined by the metadata.
    /// </summary>
    let classes = context.Elements |> List.collect (fun n -> n.Classes)

    /// <summary>
    ///     The list of all non-abstract classes defined by the metadata.
    /// </summary>
    let nonIsAbstractClasses = classes |> List.filter (fun c -> not c.IsAbstract)

    /// <summary>
    ///     Gets all properties that the given class transitively inherits from its base classes. The properties are ordered
    ///     as they appear in metadata, with properties of the base classes preceding all properties of the deriving class.
    /// </summary>
    let rec getInheritedProperties (c : Class) = 
        if c.Base = context.BaseClass then 
            []
        else
            let baseClass = classes |> List.filter (fun c' -> c'.Name = c.Base)
            if baseClass |> List.length <> 1 then
                failwithf "Class '%s' has unknown base '%s'." c.Name c.Base
            let baseClass = baseClass |> List.head
            getInheritedProperties baseClass @ baseClass.Properties

    /// <summary>
    ///     Gets all properties of the given class, including the ones the class defines iteself as well as all properties
    ///     transitively inherited from its base classes. The properties are ordered as they appear in metadata, with 
    ///     properties of the base classes preceding all properties of the deriving class.
    /// </summary>
    let allProperties (c : Class) = 
        getInheritedProperties c @ c.Properties

    /// <summary>
    ///     Checks whether the given type name refers to a rewriteable type, i.e., a type defined by the metadata.
    /// </summary>
    let isRewriteable typeName = 
        classes |> List.exists (fun c -> c.Name = typeName)

    /// <summary>
    ///     Generates the read-only declaration for the given property.
    /// </summary>
    let generateProperty (p : Property) =
        output.AppendLine("/// <summary>")
        output.AppendLine(sprintf "///     Gets %s" <| startWithLowerCase p.Comment)
        output.AppendLine("/// </summary>")
        output.AppendLine(sprintf "public %s %s { get; private set; }" <| getType p <| p.Name)

    /// <summary>
    ///     Generates the assertion statements for the validation of the given property.
    /// </summary>
    let generateValidation (p : Property) =
        let parameterName = getValidCSharpIdentifier p.Name
        match p.Validation with
        | None ->
            ()
        | NotNull -> 
            output.AppendLine(sprintf "Assert.ArgumentNotNull(%s, () => %s);" parameterName parameterName)
        | NotNullOrWhitespace -> 
            output.AppendLine(sprintf "Assert.ArgumentNotNullOrWhitespace(%s, () => %s);" parameterName parameterName)
        | InRange ->
            output.AppendLine(sprintf "Assert.ArgumentInRange(%s, () => %s);" parameterName parameterName)
        
    /// <summary>
    ///     Generates the constructor for the given class, having parameters for all inherited and non-inherited properties.
    /// </summary>
    let generateConstructor (c : Class) = 
        // Generate the doc comment
        output.AppendLine("/// <summary>")
        output.AppendLine(sprintf "///     Initializes a new instance of the <see cref=\"%s\" /> class." <| c.Name)
        output.AppendLine("/// </summary>")
        for p in allProperties c do
            output.AppendLine(sprintf "/// <param name=\"%s\">%s</param>" <| startWithLowerCase p.Name <| p.Comment)

        // Generate the constructor signature
        let parameters = allProperties c |> joinProperties ", " (fun p -> sprintf "%s %s" <| getType p <| getValidCSharpIdentifier p.Name)
        let visibility = if c.IsAbstract then "protected" else "public"
        output.AppendLine(sprintf "%s %s(%s)" visibility c.Name parameters)

        // Generate the call to the base constructor
        output.IncreaseIndent()
        let baseParams = getInheritedProperties c |> joinProperties ", " (fun p -> getValidCSharpIdentifier p.Name)
        output.AppendLine(sprintf ": base(%s)" baseParams)
        output.DecreaseIndent()

        // Generate the constructor body
        output.AppendBlockStatement <| fun () ->
            // Generate the parameter validation assertions
            let validatedProperties = c.Properties |> List.filter (fun p -> p.Validation <> None)
            if validatedProperties |> List.length > 0 then
                for p in validatedProperties do
                    generateValidation p
                output.NewLine()

            // Generate the call to the partial Validate() function
            if c.Properties |> List.length > 0 then
                let parameters = c.Properties |> joinProperties ", " (fun p -> getValidCSharpIdentifier p.Name)
                output.AppendLine(sprintf "Validate(%s);" parameters)

            // Generate the property assignments
            for p in c.Properties do
                output.AppendLine(sprintf "%s = %s;" p.Name <| getValidCSharpIdentifier p.Name)

    /// <summary>
    ///     Generates the partial Validate() method. Only properties declared by the class can be validated with this method;
    ///     inherited properties should be validated by the base classes.
    /// </summary>
    let generateValidateMethod (c : Class) =
        // Generate the doc comment
        output.AppendLine("/// <summary>")
        output.AppendLine("///     Validates all of the given property values.")
        output.AppendLine("/// </summary>")

        for p in c.Properties do
            output.AppendLine(sprintf "/// <param name=\"%s\">%s</param>" <| startWithLowerCase p.Name <| p.Comment)

        // Generate the method signature
        let parameters = c.Properties |> joinProperties ", " (fun p' -> sprintf "%s %s" <| getType p' <| getValidCSharpIdentifier p'.Name)
        output.AppendLine(sprintf "partial void Validate(%s);" parameters)

    /// <summary>
    ///     Generates the With...() methods, one for each inherited and non-inherited property.
    /// </summary>
    let generateWithMethods (c: Class) =
        for p in allProperties c do
            // Generate the doc comment
            output.AppendLine("/// <summary>")
            output.AppendLine(sprintf "///     Creates a copy of the <see cref=\"%s\" /> instance, changing only the value of the" c.Name)
            output.AppendLine(sprintf "///     <see cref=\"%s.%s\" /> property; if the property value has not changed, " c.Name p.Name)
            output.AppendLine("///     no copy is made and the original object is returned.")
            output.AppendLine("/// </summary>")
            output.AppendLine(sprintf "/// <param name=\"%s\">%s</param>" <| startWithLowerCase p.Name <| p.Comment)

            // Generate the method signature
            output.AppendLine(sprintf "public %s With%s(%s %s)" c.Name p.Name <| getType p <| getValidCSharpIdentifier p.Name)

            // Generate the method body
            output.AppendBlockStatement <| fun () ->
                // We have to call the Update method with values for all properties; we use the property getters in all
                // cases except for the property we're changing with this method
                let parameters = allProperties c |> joinProperties ", " (fun p' -> 
                    if p' = p then getValidCSharpIdentifier p'.Name
                    else p'.Name
                )
                output.AppendLine(sprintf "return Update(%s);" parameters)

            output.NewLine()

    /// <summary>
    ///     Generates the Add...() methods, one for each inherited and non-inherited collection property.
    /// </summary>
    let generateAddMethods (c : Class) =
        let collectionProperties = allProperties c |> List.filter (fun p -> p.CollectionType <> Singleton)
        for p in collectionProperties do
            // Generate the doc comment
            output.AppendLine("/// <summary>")
            output.AppendLine(sprintf "///     Creates a copy of the <see cref=\"%s\" /> instance, adding the given values to the" c.Name)
            output.AppendLine(sprintf "///     <see cref=\"%s.%s\" /> collection; if <paramref name=\"%s\" /> is empty, " c.Name p.Name <| getValidCSharpIdentifier p.Name)
            output.AppendLine("///     no copy is made and the original instance is returned.")
            output.AppendLine("/// </summary>")
            output.AppendLine(sprintf "/// <param name=\"%s\">%s</param>" <| startWithLowerCase p.Name <| p.Comment)

            // Generate the method signature
            output.AppendLine(sprintf "public %s Add%s(params %s[] %s)" c.Name p.Name p.Type <| getValidCSharpIdentifier p.Name)

            // Generate the method body; we're reusing the corresponding With...() method here
            output.AppendBlockStatement <| fun () ->
                output.AppendLine(sprintf "return With%s(%s.AddRange(%s));" p.Name p.Name <| getValidCSharpIdentifier p.Name)

            output.NewLine()

    /// <summary>
    ///     Generates the Update() method for the class.
    /// </summary>
    let generateUpdateMethod (c : Class) =
        // Generate the doc comment
        output.AppendLine("/// <summary>")
        output.AppendLine(sprintf "///     Creates a new instance of the <see cref=\"%s\" /> class if any of the property values" c.Name)
        output.AppendLine(sprintf "///     have changed; if none have changed, no copy is made and the original instance is returned.")
        output.AppendLine("/// </summary>")

        for p in allProperties c do
            output.AppendLine(sprintf "/// <param name=\"%s\">%s</param>" <| startWithLowerCase p.Name <| p.Comment)

        // Generate the method signature
        let parameters = allProperties c |> joinProperties ", " (fun p' -> sprintf "%s %s" <| getType p' <| getValidCSharpIdentifier p'.Name)
        output.AppendLine(sprintf "public %s Update(%s)" c.Name parameters)

        // Generate the method body
        output.AppendBlockStatement <| fun () ->
            // Optimization: We're only creating a new instance if at least one property has actually changed
            // Generate the condition that checks for property changes
            let checkModification = allProperties c |> joinProperties " || " (fun p' -> sprintf "%s != %s" p'.Name <| getValidCSharpIdentifier p'.Name)
            output.AppendLine(sprintf "if (%s)" checkModification)

            // Generate the body of the if statement, creating a new instance of the type
            output.IncreaseIndent()
            let parameters = allProperties c |> joinProperties ", " (fun p' -> getValidCSharpIdentifier p'.Name)
            output.AppendLine(sprintf "return new %s(%s);" c.Name parameters)
            output.DecreaseIndent()

            // Generate the return statement that returns the original instance
            output.NewLine()
            output.AppendLine("return this;")

    /// <summary>
    ///     Generates the generic or the non-generic version of the Accept() method, depending on the given visitor type.
    /// </summary>
    let generateAcceptMethod (c : Class) visitorType =
        // Generate the doc comment
        output.AppendLine("/// <summary>")
        output.AppendLine("///     Implements the visitor pattern, calling <paramref name=\"visitor\" />'s")
        output.AppendLine(sprintf "///     <see cref=\"%s.Visit%s(%s)\" /> method." visitorType.VisitorCommentType c.Name c.Name)
        output.AppendLine("/// </summary>")
        if visitorType.IsGeneric then
            output.AppendLine("/// <typeparam name=\"TResult\">The type of the value returned by <paramref name=\"visitor\" />.</typeparam>")
        output.AppendLine("/// <param name=\"visitor\">The visitor the type-specific visit method should be invoked on.</param>")

        // Generate the method signature
        output.AppendLine(sprintf "public override %s Accept%s(%s%s visitor)" visitorType.ReturnType visitorType.ParamTypeSpecifier context.VisitorName visitorType.ParamTypeSpecifier)

        // Generate the method body
        output.AppendBlockStatement <| fun () ->
            let returnKeyword = if visitorType.IsGeneric then "return " else ""
            output.AppendLine("Assert.ArgumentNotNull(visitor, () => visitor);")
            output.AppendLine(sprintf "%svisitor.Visit%s(this);" returnKeyword c.Name)

    /// <summary>
    ///     Generates the Equals methods for the given class.
    /// </summary>
    let generateEqualsMethods (c : Class) =
        // Generate the doc comment of the untyped Equals method
        output.AppendLine("/// <summary>")
        output.AppendLine("///     Determines whether <paramref name=\"obj\" /> is equal to the current instance.")
        output.AppendLine("/// </summary>")
        output.AppendLine("/// <param name=\"obj\">The object to compare with the current instance.</param>")
        output.AppendLine("/// <returns>")
        output.AppendLine("///     <c>true</c> if <paramref name=\"obj\" /> is equal to the current instance; otherwise, <c>false</c>.")
        output.AppendLine("/// </returns>")

        // Generate the method signature of the untyped Equals method
        output.AppendLine("public override bool Equals(object obj)")

        // Generate the method body of the untyped Equals method
        output.AppendBlockStatement <| fun () ->
            output.AppendLine("if (ReferenceEquals(null, obj))")
            output.IncreaseIndent()
            output.AppendLine("return false;")
            output.DecreaseIndent()
            output.NewLine()

            output.AppendLine("if (ReferenceEquals(this, obj))")
            output.IncreaseIndent()
            output.AppendLine("	return true;")
            output.DecreaseIndent()
            output.NewLine()

            output.AppendLine("if (obj.GetType() != GetType())")
            output.IncreaseIndent()
            output.AppendLine("	return false;")
            output.NewLine()
            output.DecreaseIndent()

            output.AppendLine(sprintf "return Equals((%s)obj);" c.Name)

        // Generate the doc comment of the typed Equals method
        output.NewLine()
        output.AppendLine("/// <summary>")
        output.AppendLine("///     Determines whether <paramref name=\"other\" /> is equal to the current instance.")
        output.AppendLine("/// </summary>")
        output.AppendLine(sprintf "/// <param name=\"other\">The <see cref=\"%s\" /> to compare with the current instance.</param>" c.Name)
        output.AppendLine("/// <returns>")
        output.AppendLine("///     <c>true</c> if <paramref name=\"other\" /> is equal to the current instance; otherwise, <c>false</c>.")
        output.AppendLine("/// </returns>")

        // Generate the method signature of the typed Equals method
        output.AppendLine(sprintf "public override bool Equals(%s other)" context.BaseClass)

        // Generate the method body of the typed Equals method
        output.AppendBlockStatement <| fun () ->
            output.AppendLine("Assert.ArgumentNotNull(other, () => other);")
            output.NewLine()

            output.AppendLine(sprintf "var element = other as %s;" c.Name)
            output.AppendLine("if (element == null)")
            output.IncreaseIndent()
            output.AppendLine("return false;")
            output.DecreaseIndent()
            output.NewLine()

            if allProperties c |> List.length = 0 then
                output.AppendLine("return true;")
            else
                let properties = 
                    allProperties c 
                    |> joinProperties " && " (fun p' -> 
                        if p'.CanBeNull then
                            if p'.CollectionType <> Singleton then
                                sprintf "((%s == null && element.%s == null) || (%s != null && element.%s != null && %s.SequenceEqual(element.%s)))" p'.Name p'.Name p'.Name p'.Name p'.Name p'.Name
                            else
                                sprintf "Object.Equals(%s, element.%s)" p'.Name p'.Name
                        else
                            let equalsMethod = if p'.CollectionType <> Singleton then "SequenceEqual" else "Equals"
                            sprintf "%s.%s(element.%s)" p'.Name equalsMethod p'.Name)
                output.AppendLine(sprintf "return %s;" properties)

    /// <summary>
    ///     Generates the GetHashCode method for the given class.
    /// </summary>
    let generateGetHashCodeMethod (c : Class) =
        // Generate the doc comment
        output.AppendLine("/// <summary>")
        output.AppendLine("///     Gets the hash code for the current instance.")
        output.AppendLine("/// </summary>")

        // Generate the method signature
        output.AppendLine("public override int GetHashCode()")

        // Generate the method body
        output.AppendBlockStatement <| fun () ->
            output.AppendLine("unchecked")
            output.AppendBlockStatement <| fun () ->
                output.AppendLine("var hash = (int)2166136261;")
                for p in allProperties c do
                    if p.CanBeNull then
                        output.AppendLine(sprintf "if (%s != null)" p.Name)
                        output.IncreaseIndent()

                    output.AppendLine(sprintf "hash = hash * 16777619 ^ %s.GetHashCode();" p.Name)

                    if p.CanBeNull then
                        output.DecreaseIndent()
                output.AppendLine("return hash;")
    
    /// <summary>
    ///     Generates the equality operators for the given class.
    /// </summary>
    let generateEqualityOperators (c : Class) =
        // Generate the doc comment for the equality operator
        output.AppendLine("/// <summary>")
        output.AppendLine("///     Checks whether <paramref name=\"left\" /> and <paramref name=\"right\" /> are equal.")
        output.AppendLine("/// </summary>")
        output.AppendLine("/// <param name=\"left\">The element on the left hand side of the equality operator.</param>")
        output.AppendLine("/// <param name=\"right\">The element on the right hand side of the equality operator.</param>")

        // Generate the method signature for the equality operator
        output.AppendLine(sprintf "public static bool operator ==(%s left, %s right)" c.Name c.Name)

        // Generate the method body for the equality operator
        output.AppendBlockStatement <| fun () ->
            output.AppendLine("return Equals(left, right);")

        // Generate the doc comment for the inequality operator
        output.NewLine()
        output.AppendLine("/// <summary>")
        output.AppendLine("///     Checks whether <paramref name=\"left\" /> and <paramref name=\"right\" /> are not equal.")
        output.AppendLine("/// </summary>")
        output.AppendLine("/// <param name=\"left\">The element on the left hand side of the inequality operator.</param>")
        output.AppendLine("/// <param name=\"right\">The element on the right hand side of the inequality operator.</param>")

        // Generate the method signature for the inequality operator
        output.AppendLine(sprintf "public static bool operator !=(%s left, %s right)" c.Name c.Name)

        // Generate the method body for the inequality operator
        output.AppendBlockStatement <| fun () ->
            output.AppendLine("return !Equals(left, right);")

    /// <summary>
    ///     Generates a class declaration for the given class metadata.
    /// </summary>
    let generateClass (c : Class) =
        // Generate the class declaration
        let IsAbstractKeyword = if c.IsAbstract then " abstract " else " "
        output.AppendLine(sprintf "%s%spartial class %s : %s" visibility IsAbstractKeyword c.Name c.Base)

        // Sanity check of property values
        for p in c.Properties do
            if p.CanBeNull && p.CollectionType = Array then
                failwithf "Sanity check failed for '%s.%s': Arrays cannot be null." c.Name p.Name
            if p.CanBeNull && (p.Validation = NotNull || p.Validation = NotNullOrWhitespace) then
                failwithf "Sanity check failed for '%s.%s': Property can be null but not-null validation is enabled." c.Name p.Name
            if not p.CanBeNull && p.Validation = None && p.CollectionType = List then
                failwithf "Sanity check failed for '%s.%s': List cannot be null but value is never validated." c.Name p.Name

        // Generate the class members
        output.AppendBlockStatement <| fun () ->
            // Generate all properties
            for p in c.Properties do
                generateProperty p
                output.NewLine()

            // Generate the constructor; might be empty for classes without properties, but we do not care
            generateConstructor c

            // If the class has any properties, generate the Validate() method
            if c.Properties |> List.length > 0 then
                output.NewLine()
                generateValidateMethod c

            // If the class isn't abstract and actually has properties, generate the With...(), Add...(), and Update() methods
            if not c.IsAbstract && allProperties c |> List.length > 0 then
                output.NewLine()
                generateWithMethods c
                generateAddMethods c
                generateUpdateMethod c

            // For non-abstract classes, generate some methods 
            if not c.IsAbstract then
                // Generate the Accept() methods
                output.NewLine()
                generateAcceptMethod c <| createVisitorType ""
                output.NewLine()
                generateAcceptMethod c <| createVisitorType "TResult"

                // Generate the implementations of the equality operators, the Equals methods and the GetHashCode method
                output.NewLine()
                generateEqualsMethods c

                output.NewLine()
                generateGetHashCodeMethod c

                output.NewLine()
                generateEqualityOperators c

    /// <summary>
    ///     Generates the C# code for the given namespace metadata.
    /// </summary>
    let generateNamespace (n : Namespace) =
        // Generate the namespace
        output.AppendLine(sprintf "namespace %s" n.Name)

        // Generate the namespace members
        output.AppendBlockStatement <| fun () ->
            writeUsings context.Elements n.Name

            for c in n.Classes do
                output.NewLine()
                generateClass c

    //====================================================================================================================
    // Visitors code generation
    //====================================================================================================================

    /// <summary>
    ///     Generates the generic or the non-generic version of the visitor class, depending on the given visitor type.
    /// </summary>
    let generateVisitor visitorType =
        // Generate the class
        output.AppendLine(sprintf "%s abstract partial class %s" visibility visitorType.VisitorType)

        // Generate a Visit...() method for each non-abstract class
        output.AppendBlockStatement <| fun () ->
            let mutable first = true
            for c in nonIsAbstractClasses do
                if not first then
                    output.NewLine()

                first <- false

                // Generate the doc comment
                let parameterName = getValidCSharpIdentifier c.Name
                output.AppendLine("/// <summary>")
                output.AppendLine(sprintf "///     Visits an element of type <see cref=\"%s\" />." c.Name)
                output.AppendLine("/// </summary>")
                output.AppendLine(sprintf "/// <param name=\"%s\">The <see cref=\"%s\" /> instance that should be visited.</param>" <| startWithLowerCase c.Name <| c.Name)

                // Generate the method signature
                output.AppendLine(sprintf "public virtual %s Visit%s(%s %s)" visitorType.ReturnType c.Name c.Name parameterName)

                // Generate the method body
                output.AppendBlockStatement <| fun () ->
                    output.AppendLine(sprintf "Assert.ArgumentNotNull(%s, () => %s);" parameterName parameterName)
                    if visitorType.IsGeneric then
                        output.AppendLine(sprintf "return default(%s);" visitorType.ParamType)

    /// <summary>
    ///     Generates the element tree rewriter class.
    /// </summary>
    let generateRewriter () =
        // Generate the class
        output.AppendLine(sprintf "%s abstract partial class %s : %s<%s>" visibility context.RewriterName context.VisitorName context.BaseClass)

        // Generate a Visit...() method with the rewriting logic for each non-abstract class
        output.AppendBlockStatement <| fun () ->
            let mutable first = true
            for c in nonIsAbstractClasses do
                if not first then
                    output.NewLine()

                first <- false

                // Generate the doc comment
                let parameterName = getValidCSharpIdentifier c.Name
                output.AppendLine("/// <summary>")
                output.AppendLine(sprintf "///     Rewrites an element of type <see cref=\"%s\" />." c.Name)
                output.AppendLine("/// </summary>")
                output.AppendLine(sprintf "/// <param name=\"%s\">The <see cref=\"%s\" /> instance that should be rewritten.</param>" <| startWithLowerCase c.Name <| c.Name)

                // Generate the method signature
                output.AppendLine(sprintf "public override %s Visit%s(%s %s)" context.BaseClass c.Name c.Name parameterName)

                // Generate the method body
                output.AppendBlockStatement <| fun () ->
                    output.AppendLine(sprintf "Assert.ArgumentNotNull(%s, () => %s);" parameterName parameterName)

                    // If the class has no properties, just return the same instance; there's nothing to rewrite
                    let properties = allProperties c
                    if properties |> List.length = 0 then
                        output.AppendLine(sprintf "return %s;" parameterName)
                    else
                        output.NewLine()

                        // Generate a local variable for each property that holds the result of rewrite
                        for p in properties do
                            if isRewriteable p.Type then
                                // Call Visit() recursively for rewriteable types, casting the types back to the original type
                                // Collection types do not require casts as a special overload of the Visit() method is called
                                let cast = if p.CollectionType <> Singleton then "" else sprintf "(%s)" p.Type
                                output.AppendLine(sprintf "var %s = %sVisit(%s.%s);" <| getValidCSharpIdentifier p.Name <| cast <| parameterName <| p.Name)
                            else
                                // To handle non-rewriteable types uniformly, we're generating local variables for them as well
                                output.AppendLine(sprintf "var %s = %s.%s;" <| getValidCSharpIdentifier p.Name <| parameterName <| p.Name)

                        // Generate the call to the Update() function, passing the local variables
                        let parameters = allProperties c |> joinProperties ", " (fun p' -> getValidCSharpIdentifier p'.Name)
                        output.AppendLine(sprintf "return %s.Update(%s);" parameterName parameters)

    /// <summary>
    ///     Generates the non-generic and generic visitors as well as the rewriter classes
    /// </summary>
    let generateVisitors () =
        output.AppendLine(sprintf "namespace %s" context.VisitorNamespace)
        output.AppendBlockStatement <| fun () ->
            writeUsings context.Elements context.VisitorNamespace

            output.NewLine()
            generateVisitor <| createVisitorType ""

            output.NewLine()
            generateVisitor <| createVisitorType "TResult"

            output.NewLine()
            generateRewriter ()

    try
        // Generate the code for the elements
        for n in context.Elements do
            generateNamespace n
            output.NewLine()

        // Generate the code for the visitors
        generateVisitors ()

        // Write the generate code to the file and show a success message
        output.WriteToFile context.OutputFile
        writeColored <| sprintf "Generated C# file '%s'." context.OutputFile <| ConsoleColor.DarkGreen
    with
        | e ->
            writeColored e.Message ConsoleColor.DarkRed
            writeColored e.StackTrace ConsoleColor.White