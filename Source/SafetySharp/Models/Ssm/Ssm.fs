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

namespace SafetySharp.Models

/// Provides types and functions for working with S# models (SSM). Basically, the S# metamodel is a subset of
/// the metadata and instructions supported by the .NET common language runtime (CLR); a S# model is a view
/// of a .NET assembly.
module internal Ssm =

    /// Represents the unary operators supported by S# models.
    type internal UOp =
        | Minus
        | Not

    /// Represents the binary operators supported by S# models.
    type internal BOp =
        | Add
        | Sub
        | Mul
        | Div
        | Mod
        | And
        | Or
        | Eq
        | Ne
        | Lt
        | Le
        | Gt
        | Ge

    /// Represents a variable accessed by an expression.
    type internal Var =
        | Arg of string
        | Local of string
        | Field of string

    /// Represents an expression within the body of a S# method.
    type internal Expr = 
        | BoolExpr of bool
        | IntExpr of int
        | VarExpr of Var
        | UExpr of UOp * Expr
        | BExpr of Expr * BOp * Expr

    /// Represents a statement within the body of a S# method.
    type internal Stm =
        | NopStm
        | AsgnStm of Var * Expr
        | GotoStm of Expr * int
        | SeqStm of Stm * Stm
        | RetStm of Expr option
        | IfStm of Expr * Stm * Stm option

    /// Gets the set of statement program counters that can be executed following the execution of the
    /// statement at the given program counter. For non-branching statements, the successor is always
    /// the next statement of the method's body. Branching statements, on the other hand, typically
    /// have more than one successor. Return statements don't have any successor at all.
    let private getSuccessors methodBody pc =
        let succs = function
            | GotoStm (BoolExpr true, pc) -> Set.singleton pc
            | GotoStm (_, pc) -> [pc; pc + 1] |> Set.ofList
            | RetStm _ -> Set.empty
            | _ -> pc + 1 |> Set.singleton

        // Get the successors of the statement at the given program counter, removing 
        // the 'method end' program counter from the resulting set
        pc
        |> Array.get methodBody
        |> succs
        |> Set.remove (methodBody.Length)

    /// Gets the control flow graph, mapping each statement (identified by its program counter) to the set
    /// of program counters of its successor statements.
    let private getCfg methodBody =
        let addToSet map k v =
            match Map.tryFind k map with
            | Some v' -> map |> Map.add k (Set.union v v')
            | None    -> map |> Map.add k v

        methodBody 
        |> Array.fold (fun (succs, pc) instr ->
            (addToSet succs pc <| getSuccessors methodBody pc, pc + 1)
        ) (Map.empty, 0)
        |> fst

    /// Gets the set of all paths starting at the given statement (identified by its program counter) to the
    /// end of the method body.
    let rec private getPaths cfg pc =
        match Map.find pc cfg |> List.ofSeq with
        | [] -> [[pc]]
        | succs -> 
            succs 
            |> List.map (fun pc' -> getPaths cfg pc') 
            |> List.collect id
            |> List.map (fun pc' -> pc :: pc')

    /// Gets the join point for the given statement (identified by its program counter). The join point is the
    /// first statement that is executed on all paths starting at the given statement. A value of 'None' is returned
    /// if the paths do not converge before the method returns.
    let private getJoinPoint cfg pc =
        let common = Set.intersectMany (getPaths cfg pc |> List.map Set.ofList)
        let common = Set.difference common (Set.singleton pc)
        if Set.isEmpty common then None
        else Set.minElement common |> Some

    /// Replaces all goto statements in the given method body with structured control flow statements.
    /// If a goto cannot be removed, the method body is invalid.
    let replaceGotos methodBody =
        let cfg = getCfg methodBody

        // Transforms all statements in the range [pc, last-1]
        let rec transform pc last =
            let getJoinPoint () =
                let joinPoint = getJoinPoint cfg pc
                match joinPoint with 
                    | None -> last 
                    | Some j -> j

            if pc >= last then NopStm
            else
                match Array.get methodBody pc with
                | GotoStm (BoolExpr true, t) when t = last ->
                    NopStm
                | GotoStm (BoolExpr true, t) ->
                    let joinPoint = getJoinPoint ()
                    let thenStm = transform t joinPoint
                    let elseStm = None
                    let ifStm = IfStm (BoolExpr true, thenStm, elseStm)
                    let joinedStm = transform joinPoint last
                    SeqStm (ifStm, joinedStm)
                | GotoStm (e, t) ->
                    let joinPoint = getJoinPoint ()
                    // There might be no then-stm if the goto represents an 'if' without an 'else'
                    // (note that the C# compiler inverts the condition and switches the original then and else statements)
                    let thenStm = if cfg.[pc] |> Set.contains joinPoint then NopStm else transform t joinPoint
                    let elseStm = transform (pc + 1) joinPoint |> Some
                    let ifStm = IfStm (BoolExpr true, thenStm, elseStm)
                    let joinedStm = transform joinPoint last
                    SeqStm (ifStm, joinedStm)
                | RetStm e -> RetStm e
                | stm -> SeqStm (stm, transform (pc + 1) last)

        let last = Array.length methodBody
        transform 0 last