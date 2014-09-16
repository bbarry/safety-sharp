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

namespace SafetySharp.Internal.Modelchecking.Prism

// semantics of prism
// http://www.prismmodelchecker.org/doc/semantics.pdf

// AST based on:
// http://www.prismmodelchecker.org/manual/ThePRISMLanguage/Introduction

// Chapter Constants
type internal Constant =
    | Integer of int
    | Double of double
    | Boolean of bool

// Chapter Modules And Variables
type internal Identifier = {
    Name : string;
}
    with
        static member reserved = [ "A"; "bool"; "clock"; "const"; "ctmc"; "C"; "double"; "dtmc"; "E"; "endinit"; "endinvariant"; "endmodule"; "endrewards"; "endsystem"; "false"; "formula"; "filter"; "func"; "F"; "global"; "G"; "init"; "invariant"; "I"; "int"; "label"; "max"; "mdp"; "min"; "module"; "X"; "nondeterministic"; "Pmax"; "Pmin"; "P"; "probabilistic"; "prob"; "pta"; "rate"; "rewards"; "Rmax"; "Rmin"; "R"; "S"; "stochastic"; "system"; "true"; "U"; "W"]
        


///////////////////////////
// MODEL
///////////////////////////

// Chapter Expressions
// Prism differentiates between expressions and predicates. We treat them the same.
// Note: We didn't consolidate the expressions and the functions here to be consistent with
//       the type Properties (and there we didn't do it to keep it understandable)
type internal Expression =
    | Constant of Constant
    | Variable of Name:Identifier
    | Formula of Name:Identifier
    // Expressions with operators known from Propositional Logic
    | UnaryNegation  of Operand:Expression                                  // !
    | BinaryMultiplication of Left:Expression * Right:Expression            // *
    | BinaryDivision of Left:Expression * Right:Expression                  // / be cautious: Always performs floating point operation. 22/7 is 3.14... instead of 3, even on integers
    | BinaryAddition of Left:Expression * Right:Expression                  // +
    | BinarySubstraction of Left:Expression * Right:Expression              // -
    | BinaryLessThan of Left:Expression * Right:Expression                  // <
    | BinaryLessEqual of Left:Expression * Right:Expression                 // <=
    | BinaryGreaterEqual of Left:Expression * Right:Expression              // >=
    | BinaryGreaterThan of Left:Expression * Right:Expression               // >
    | BinaryConjunction of Left:Expression * Right:Expression               // &
    | BinaryDisjunction of Left:Expression * Right:Expression               // |
    | BinaryIfAndOnlyIf of Left:Expression * Right:Expression               // <=>
    | BinaryImplication of Left:Expression * Right:Expression               // =>
    | TenaryIfThenElse of If:Expression * Then:Expression * Else:Expression // ? :
    // Functions
    | FunctionMin of Expression list
    | FunctionMax of Expression list
    | FunctionFloor of Expression
    | FunctionCeil of Expression
    | FunctionPow of Base:Expression * Power:Expression // Base^Power = Number
    | FunctionMod of Dividend:Expression * Divisor:Expression // Dividend % Divisor
    | FunctionLog of Base:Expression * Number:Expression // Log_Base(Number) = Power


// Chapter Modules And Variables
[<RequireQualifiedAccess>]
type internal VariableType =
    | Bool
    | IntRange of From:Expression * To:Expression
    | Int //only usable in few kinds of model analysis (e.g. simulation)
    | Clock //only usable in PTAs

type internal VariableDeclaration = {
    // A variable declaration in a module
    Name : Identifier;
    Type : VariableType;
    InitialValue : string option; //when initial value is omitted"; "lowest value is assumed (!= NuXmv where indeterministic initial value is assumed)
}

// Chapter Constants
type internal ConstantDeclaration = {
    Name : Identifier;
    //Type : 
    Value : Expression;
}

// Chapter Commands

type internal Update =
    //equivalent examples for an update (unnecessary things can be left out):
    //    (x1'=x1)&(x2'=x2)
    //    (x1'=x1)
    //    true
    (Identifier * Expression) list

type ActionLabel = Identifier

type internal Action =
    | Synchronized of ActionLabel:ActionLabel //if two commands have the same action, they make their transition simultaneously
    | NoActionLabel

type internal Command = {
    // Example for a Command: "[] x=0 -> 0.8:(x'=0) + 0.2:(x'=1);"
    // where "x=0" is a guard,
    //       "x'=0" is an update
    //       "0.8" is the associated probability of update "x'=0"
    // Another deterministic example: "[] x=1 & y!=2 -> (x'=2);"
    //        This is equivalent to "[] x=1 & y!=2 -> 1.0:(x'=2);"
    // A CTMC example: "[] x=0 -> 50:(x'=1) + 60:(x'=2);"
    //       "50" is the associate rate of update "x'=1"
    // An example with an action: "[serve] s=0 -> 1:(s'=1)";
    //       "serve" is the action for synchronization purposes
    Action : Action; //if two commands have the same action, they make their transition simultaneously
    Guard : Expression;
    //(Update is the update, where for each Identifier the new value is defined. And Expression its associate probability or rate)
    Updates : (Expression*Update) list
}

// Parallel Composition
type internal ModelType =
    | MDP // Markov Decision Process
    | DTMC // Discrete Time Markov Chain
    | CTMC // Continuous Time Markov Chain
    | PTA // Probabilistic Timed Automata

type internal Choice =
    | NondeterministicChoice // MDP
    | ProbabilisticChoice // DTMC
    | RaceCondition // CTMC

// CTMC
type internal ProbabilityOrRate =
    | Probability //MDP, DTMC
    | Rate //CTMC

type internal Module =
    | ModuleRenaming of Name:Identifier * CloneOf:Identifier * Renamings:((Identifier*Identifier) list) // Chapter Module Renaming
    | Module of Name:Identifier * Variables:(VariableDeclaration list) * Commands:(Command list)
    | ModulePta of Name:Identifier * Variables:(VariableDeclaration list) * Invariant : Expression * Commands:(Command list)


// Chapter Formulas And Labels
type internal Formula = { //used in models
    Name : Identifier;
    Formula : Expression;
}

type internal Label = { //used in properties
    Name : Identifier;
    Label : Expression; //must be boolean
}
    with
        static member builtinInitLabel     = {Label.Name={Identifier.Name="init"};Label.Label=Expression.Constant(Constant.Boolean(true));} //TODO: improve, as it has no expression
        static member builtinDeadlockLabel = {Label.Name={Identifier.Name="deadlock"};Label.Label=Expression.Constant(Constant.Boolean(true));} //TODO: improve, as it has no expression

// Chapter Costs and Rewards
type internal Reward = //can also be used for costs or time
    | StateReward of Guard : Expression * Reward : Expression
    | TransitionReward of Action : Action * Guard : Expression * Reward : Expression

type internal RewardStructure = {
    Label : Label option;
    Rewards : Reward list;
}

// Chapter Process Algebra Operators
type internal ProcessAlgebraicExpression =
    //Standard is OnActions over all Modules
    | Module of Name:Identifier
    | OnActions of Ms:ProcessAlgebraicExpression list                                  // M1 || M2
    | Asynchronous of Ms:ProcessAlgebraicExpression list                               // M1 ||| M2
    | OnSomeActions of M1:ProcessAlgebraicExpression * M2:ProcessAlgebraicExpression * Actions:(ActionLabel list) // M1 |[a,b,...]| M2
    | HideActions of M:ProcessAlgebraicExpression * Actions:(ActionLabel list)                                    // M / {a,b,...}
    | RenameActions of M:ProcessAlgebraicExpression * Renamings:((ActionLabel*ActionLabel) list)                  // M {a<-b,c<-d,...} from a to b. From:ActionLabel*To:ActionLabel

type internal PrismModel = {
    ModelType : ModelType;
    Constants: ConstantDeclaration list;
    InitModule : Expression option; //Chapter Multiple Initial States e.g. "x+y=1"
    GlobalVariables : VariableDeclaration list;
    Modules : Module list;
    Formulas : Formula list;
    Labels : Label list;
    Rewards : RewardStructure list;
    ParallelComposition : ProcessAlgebraicExpression option;
}

///////////////////////////
// PROPERTIES
///////////////////////////

// Chapter Property Specification -> Syntax and Semantics

// Prisms property specification language subsumes
//  * PCTL   for DTMCs, MDPs and PTAs
//  * CSL   for CTMCs (is extension of PCTL)
//  * probabilistic LTL   for DTMCs and MDPs
//  * PCTL*   for DTMCs and MDPs
//  (* CTL   for non-probabilistic verification)

type internal ProbabilityQuery =
    | LessEqual    // <=
    | LessThan     // <
    | Equal        // =
    | GreaterEqual // >=
    | GreaterThan  // >
    | Calculate    // =?            
    
// Note: Although we could have consolidated some discriminated union cases (e.g. all ltl into one case) we didn't do it to keep it understandable.
type internal Property =   
    | Constant of Constant
    | Variable of Name:Identifier
    | Formula of Name:Identifier
    | Label of Name:Identifier
    | Property of Name:Identifier //a property can also use the result of another (labeled) property as input
    // Expressions with operators known from Propositional Logic
    | UnaryNegation  of Operand:Property                                  // !
    | BinaryMultiplication of Left:Property * Right:Property            // *
    | BinaryDivision of Left:Property * Right:Property                  // / be cautious: Always performs floating point operation. 22/7 is 3.14... instead of 3, even on integers
    | BinaryAddition of Left:Property * Right:Property                  // +
    | BinarySubstraction of Left:Property * Right:Property              // -
    | BinaryLessThan of Left:Property * Right:Property                  // <
    | BinaryLessEqual of Left:Property * Right:Property                 // <=
    | BinaryGreaterEqual of Left:Property * Right:Property              // >=
    | BinaryGreaterThan of Left:Property * Right:Property               // >
    | BinaryConjunction of Left:Property * Right:Property               // &
    | BinaryDisjunction of Left:Property * Right:Property               // |
    | BinaryIfAndOnlyIf of Left:Property * Right:Property               // <=>
    | BinaryImplication of Left:Property * Right:Property               // =>
    | TenaryIfThenElse of If:Property * Then:Property * Else:Property // ? :
    // Functions
    | FunctionMin of Property list
    | FunctionMax of Property list
    | FunctionFloor of Property
    | FunctionCeil of Property
    | FunctionPow of Base:Property * Power:Property // Base^Power = Number
    | FunctionMod of Dividend:Property * Divisor:Property // Dividend % Divisor
    | FunctionLog of Base:Property * Number:Property // Log_Base(Number) = Power
    // Functions only usable in properties
    | FunctionMultiAchievability of Goal1:Property * Goal2:Property //Multi-Objective Property "achievability": Bool*Bool->Bool
    | FunctionMultiNumerical of SearchBestValueFor:Property * Constraints:(Property list) //Multi-Objective Property "numerical": Double*(Bool list)->Double
    | FunctionMultiPareto of SearchBestValueFor1:Property * SearchBestValueFor2:Property //Multi-Objective Property "Pareto": Double*Double->Void
    // LTL-Formula
    | LtlUnaryNext of Operand:Property
    | LtlUnaryEventually of Operand:Property // Finally
    | LtlUnaryAlways of Operand:Property // Globally
    | LtlBinaryUntil of Left:Property * Right:Property
    | LtlBinaryWeakUntil of Left:Property * Right:Property
    | LtlBinaryRelease of Left:Property * Right:Property
    // Probability
    | ProbabilityOfProperty of Query:ProbabilityQuery * Operand:Property
    // Steady State
    | SteadyState
    //Reward
    | RewardReachability of Property
    | RewardCumulative
    | RewardInstantaneous
    | RewardSteadyState
    //CTL
    | ForAllPathsGlobally
    | ForAllPathsFinally    
    | ExistsPathGlobally    
    | ExistsPathFinally
    // Filters
    | FilterMin
    | FilterMax
    | FilterArgmin
    | FilterArgmax
    | FilterCount
    | FilterSum
    | FilterAvg
    | FilterFirst
    | FilterRange
    | FilterForall    
    | FilterExists    
    | FilterPrint    
    | FilterPrintall
    | FilterState

// not every combination is possible
// prism seems to differentiate between pathproperties and properties
// Here are some properties tested with the dice-example
// Property "P>0 [F d=6]" is possible
// Property "F d=6" is not possible
// Property "F d=6" is not possible
// Property "A[F d=6]" is possible
// Property "P>0 [F (d=6 & (X s=7))]" is possible. (Note: Without additional parentheses around "X s=7" it doesn't work)
// Property "E [F (d=6 & (X s=7))]" is able to be parsed but not verified ("Error: (Non-probabilistic) LTL model checking is not supported."
// Property "E [F (d=6 & A [X s=7]]" is able to be parsed but not verified ("Error: CTL model checking of the A X operator is not yet supported.")
// Property "E [F d=6 & A [F s=7]]" is possible
// Property "filter(printall,P=? [F d=4],P>0 [F d>3])" is possible
// Property "E [F d=6 & A [F P>0 [ s=7 ]]]" is possible
// Property "P>0 [ P>0 [F s=7 ]]" is possible
// Property "P>0 [ P>0 [s=0 ]]" is possible
// Property "P>0 [(F (d=6 & (X s=7)))& P>0 [F d=6]]" is possible
// Property "P=? [F d=min(3,4)]" is possible

// Thus:
//  - Only LTL-Formulas need to be in a "P~x[...]"
//  - P~s can be nested
//  - CTL* is not possible

///////////////////////////
// Command Line
///////////////////////////


