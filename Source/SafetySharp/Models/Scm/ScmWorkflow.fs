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

namespace SafetySharp.Models

module internal ScmWorkflow =
    open SafetySharp.Workflow
    open Scm
    
    type ScmModel = CompDecl
    
    type IScmModel<'state> =
        interface
            abstract getModel : ScmModel
            abstract setModel : ScmModel -> 'state
        end
        
        
    let getModel<'state when 'state :> IScmModel<'state>> : WorkflowFunction<'state,'state,CompDecl> = workflow {
        let! state = getState
        let model = state.getModel
        return model
    }
    
    let setModel<'state when 'state :> IScmModel<'state>> (model:ScmModel) : WorkflowFunction<'state,'state,unit> = workflow {
        let! state = getState
        let newState = state.setModel model
        do! updateState newState
    }

    type PlainScmModel(model:ScmModel) =
        class end
            with
                member this.getModel : ScmModel = model
                interface IScmModel<PlainScmModel> with
                    member this.getModel : ScmModel = model
                    member this.setModel (model:ScmModel) = PlainScmModel(model)
    
    type PlainScmModelWorkflowState = WorkflowState<PlainScmModel>
    type PlainScmModelWorkflowFunction<'returnType> = WorkflowFunction<PlainScmModel,PlainScmModel,'returnType>

    let createPlainScmWorkFlowState (model:ScmModel) : PlainScmModelWorkflowState =
        WorkflowState<PlainScmModel>.stateInit (PlainScmModel(model))
    
    let setPlainModelState (model:ScmModel) = workflow {
        do! updateState (PlainScmModel(model))
    }
    
    let toPlainModelState<'state when 'state :> IScmModel<'state>> : WorkflowFunction<'state,PlainScmModel,unit> = workflow {
        let! state = getState
        do! setPlainModelState state.getModel
    }
