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

namespace SafetySharp.Analysis.VerificationCondition
module internal VcSamWorkflow =
    open SafetySharp.Workflow
    type VcSamPgm = VcSam.Pgm

    type IVcSamModel<'state> =
        interface
            abstract getModel : VcSam.Pgm
            abstract setModel : VcSam.Pgm -> 'state
        end
           
    let getModel<'state when 'state :> IVcSamModel<'state>> : WorkflowFunction<'state,'state,VcSam.Pgm> = workflow {
        let! state = getState
        let model = state.getModel
        return model
    }    
    
    let setModel<'state when 'state :> IVcSamModel<'state>> (model:VcSam.Pgm) : WorkflowFunction<'state,'state,unit> = workflow {
        let! state = getState
        let newState = state.setModel model
        do! updateState newState
    }

    
    type VcExpr = SafetySharp.Models.Sam.Expr
    type SamToVcWorkflowFunction<'stateWithSam> = WorkflowFunction<'stateWithSam,VcExpr,unit>



    // VcSam: PlainModel
    type PlainModel(model:VcSam.Pgm) =
        class end
            with
                member this.getModel : VcSam.Pgm = model
                interface IVcSamModel<PlainModel> with
                    member this.getModel : VcSam.Pgm = model
                    member this.setModel (model:VcSam.Pgm) = PlainModel(model)
    
    type PlainModelWorkflowState = WorkflowState<PlainModel>
    type PlainModelWorkflowFunction<'returnType> = WorkflowFunction<PlainModel,PlainModel,'returnType>

    let createPlainModelWorkFlowState (model:VcSam.Pgm) : PlainModelWorkflowState =
        WorkflowState<PlainModel>.stateInit (PlainModel(model))
    
    let setPlainModelState (model:VcSam.Pgm) = workflow {
        do! updateState (PlainModel(model))
    }
    
    let toPlainModelState<'state when 'state :> IVcSamModel<'state>> : WorkflowFunction<'state,PlainModel,unit> = workflow {
        let! state = getState
        do! setPlainModelState state.getModel
    }
    

module internal VcSamModelForModification =
    open SafetySharp.Workflow
    open VcSamWorkflow

    let rec translateStm (stmIdCounter:int ref) (stm : SafetySharp.Models.Sam.Stm) : VcSam.Stm =
        do stmIdCounter := stmIdCounter.Value + 1
        let freshId = Some(stmIdCounter.Value)
        match stm with
            | SafetySharp.Models.Sam.Stm.Block(statements) ->
                VcSam.Stm.Block(freshId,statements |> List.map (translateStm stmIdCounter) )
            | SafetySharp.Models.Sam.Stm.Choice (clauses) ->                
                if clauses = [] then
                    (VcSam.Stm.Assume(freshId,VcSam.Expr.Literal(VcSam.Val.BoolVal(false))))
                else
                    let translateClause ( clause :SafetySharp.Models.Sam.Clause) : VcSam.Stm =
                        do stmIdCounter := stmIdCounter.Value + 1
                        let freshIdForGuard = Some(stmIdCounter.Value)
                        VcSam.Stm.Block(freshId,[VcSam.Stm.Assume(freshIdForGuard,clause.Guard);translateStm stmIdCounter clause.Statement]) // the guard is now an assumption
                    VcSam.Stm.Choice(freshId,clauses |> List.map translateClause)
            | SafetySharp.Models.Sam.Stm.Write (variable,expression) ->
                VcSam.Stm.Write (freshId,variable,expression)
                
    let translatePgm (stmIdCounter:int ref) (pgm : SafetySharp.Models.Sam.Pgm ) : VcSam.Pgm =
        {
            VcSam.Pgm.Globals = pgm.Globals;
            VcSam.Pgm.Locals = pgm.Locals;
            VcSam.Pgm.Body = translateStm stmIdCounter pgm.Body;
        }
        
    let rec getMaximalStmId (stm:VcSam.Stm) : int =
        match stm with
            | VcSam.Stm.Assert (sid,_) ->
                sid.Value
            | VcSam.Stm.Assume (sid,_) ->
                sid.Value
            | VcSam.Stm.Block (sid,statements) ->
                statements |> List.map getMaximalStmId
                           |> List.max
            | VcSam.Stm.Choice (sid,choices) ->
                choices |> List.map getMaximalStmId
                        |> List.max
            | VcSam.Stm.Write (sid,_,_) ->
                sid.Value

    // VcSam: Model with generator for fresh versions of variables
    type ModelForModification =
        {
            StmIdCounter : int ref;
            Model : VcSam.Pgm;
        }
            with
                static member initial (model:SafetySharp.Models.Sam.Pgm) =
                    let counter = ref 0
                    {
                        ModelForModification.StmIdCounter = counter;
                        ModelForModification.Model = translatePgm counter model;
                    }                    
                static member initial (model:VcSam.Pgm) =
                    let counter = ref (getMaximalStmId model.Body)
                    {
                        ModelForModification.StmIdCounter = counter;
                        ModelForModification.Model = model;
                    }
                member this.getFreshId (): int =
                    do this.StmIdCounter:=this.StmIdCounter.Value + 1
                    this.StmIdCounter.Value

                interface IVcSamModel<ModelForModification> with
                    member this.getModel : VcSam.Pgm = this.Model
                    member this.setModel (model:VcSam.Pgm) =
                        { this with
                            ModelForModification.Model = model;
                        }
    
    type ModelForModificationWorkflowState = WorkflowState<ModelForModification>
    type ModelForModificationWorkflowFunction<'returnType> = WorkflowFunction<ModelForModification,ModelForModification,'returnType>

    let setModelForModificationState (model:VcSam.Pgm) = workflow {
        let! oldState = getModel
        do! setModel model
    }
    
    
    open SafetySharp.Models
    type SamToVcSamWorkflowFunction<'stateWithSam> = WorkflowFunction<'stateWithSam,ModelForModification,unit>
    type VcSamToVcWorkflowFunction<'stateWithVcSam> = WorkflowFunction<'stateWithVcSam,VcExpr,unit>
        
    let transformSamToVcSam<'stateWithSam when 'stateWithSam :> SamWorkflow.ISamModel<'stateWithSam>> :
                        SamToVcSamWorkflowFunction<'stateWithSam> = workflow {
        let! model = SafetySharp.Models.SamWorkflow.getModel
        let newModel = (ModelForModification.initial model)
        do! updateState newModel
    }

    let transformIVcSamToVcModelForModification<'state when 'state :> IVcSamModel<'state>>
                     : WorkflowFunction<'state,ModelForModification,unit> = workflow {
        let! model = getModel
        let newModel = (ModelForModification.initial model)
        do! updateState newModel
    }