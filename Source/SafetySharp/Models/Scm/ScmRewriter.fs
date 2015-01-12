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

namespace SafetySharp.Models.Scm

module internal ScmRewriter =
    open ScmHelpers

    type ScmModel = CompDecl //may change, but I hope it does not
    
    type ScmRewriterSubcomponent = {
        ChildPath : CompPath;
        ParentPath : CompPath;
        ChildCompDecl : CompDecl;
        ParentCompDecl : CompDecl;
        
        // Forwarder
        // For each of these. Map goes from:old -> to:new. Old entity lives always in ChildPath, new in ParentPath
        // So no path is necessary
        ArtificialFieldsOldToNew : Map<Field,Field>
        ArtificialFaultsOldToNew : Map<Fault,Fault>
        ArtificialReqPortOldToNew : Map<ReqPort,ReqPort>
        ArtificialProvPortOldToNew : Map<ProvPort,ProvPort>

        //Maps from new path to old path (TODO: when not necessary, delete); or change to newToOrigin
        ArtificialFieldsNewToOld : Map<FieldPath,FieldPath> 
        ArtificialFaultsNewToOld : Map<FaultPath,FaultPath>        
        ArtificialReqPortNewToOld : Map<ReqPortPath,ReqPortPath>
        ArtificialProvPortNewToOld : Map<ProvPortPath,ProvPortPath>
        
        FaultsToRewrite : FaultDecl list    //declared in parent
        ProvPortsToRewrite : ProvPortDecl list    //declared in parent
        StepsToRewrite : StepDecl list    //declared in parent
        ArtificialStep : (ReqPort*ProvPort) option
    }
        with
            static member createEmptyFromPath (model:CompDecl) (path:CompPath) =
                {
                    ScmRewriterSubcomponent.ChildPath = path;
                    ScmRewriterSubcomponent.ParentPath = path.Tail;
                    ScmRewriterSubcomponent.ChildCompDecl = model.getDescendantUsingPath path;
                    ScmRewriterSubcomponent.ParentCompDecl = model.getDescendantUsingPath path.Tail;
                    ScmRewriterSubcomponent.ArtificialFieldsOldToNew = Map.empty<Field,Field>;
                    ScmRewriterSubcomponent.ArtificialFaultsOldToNew = Map.empty<Fault,Fault>;
                    ScmRewriterSubcomponent.ArtificialReqPortOldToNew = Map.empty<ReqPort,ReqPort>;
                    ScmRewriterSubcomponent.ArtificialProvPortOldToNew = Map.empty<ProvPort,ProvPort>;
                    ScmRewriterSubcomponent.ArtificialFieldsNewToOld = Map.empty<FieldPath,FieldPath>;
                    ScmRewriterSubcomponent.ArtificialFaultsNewToOld = Map.empty<FaultPath,FaultPath>;
                    ScmRewriterSubcomponent.ArtificialReqPortNewToOld = Map.empty<ReqPortPath,ReqPortPath>;
                    ScmRewriterSubcomponent.ArtificialProvPortNewToOld = Map.empty<ProvPortPath,ProvPortPath>;
                    ScmRewriterSubcomponent.FaultsToRewrite = [];
                    ScmRewriterSubcomponent.ProvPortsToRewrite = [];
                    ScmRewriterSubcomponent.StepsToRewrite = [];
                    ScmRewriterSubcomponent.ArtificialStep = None;
                }
            member infos.oldToNewMaps1 =                
                    (infos.ArtificialReqPortOldToNew,infos.ArtificialFaultsOldToNew,Map.empty<Var,Var>,infos.ArtificialFieldsOldToNew)
            member infos.oldToNewMaps2 =                
                    (infos.ArtificialFaultsOldToNew)
                
    [<RequireQualifiedAccess>]
    type BehaviorWithLocation = 
        // only inline statements in the root-component. Thus we do not need a path to a subcomponent
        | InProvPort of ProvPortDecl * BehaviorDecl
        | InFault of FaultDecl * BehaviorDecl
        | InStep of StepDecl * BehaviorDecl
            with
                member beh.Behavior =
                    match beh with
                        | InProvPort (_,beh) -> beh
                        | InFault (_,beh) -> beh
                        | InStep (_,beh) -> beh

    type ScmRewriterInlineBehavior = {
        BehaviorToReplace : BehaviorWithLocation;
        InlinedBehavior : BehaviorDecl;
        CallToReplace : StmPath option;
        (*ArtificialLocalVarOldToNew : Map<VarDecl,VarDecl>;*)
    }
        with
            static member createEmptyFromBehavior (behaviorWithLocaltion:BehaviorWithLocation) =
                {
                    ScmRewriterInlineBehavior.BehaviorToReplace = behaviorWithLocaltion;
                    ScmRewriterInlineBehavior.InlinedBehavior = behaviorWithLocaltion.Behavior;
                    ScmRewriterInlineBehavior.CallToReplace = None;
                }
            
    type ScmRewriteState = {
        Model : ScmModel;
        TakenNames : Set<string>;
        ChangedSubcomponents : ScmRewriterSubcomponent option;
        // TODO: Optimization: Add parent of ComponentToRemove here. Thus, when a change to the componentToRemove is done, only its parent needs to be updated and not the whole model.
        //       The writeBack to the model can happen, when a component gets deleted
        // Flag, which determines, if something was changed (needed for fixpoint iteration)
        BehaviorToInline : ScmRewriterInlineBehavior option;
        Tainted : bool;
    }
        with
            static member initial (scm:ScmModel) = 
                {
                    ScmRewriteState.Model = scm;
                    ScmRewriteState.TakenNames = scm.getTakenNames () |> Set.ofList;
                    ScmRewriteState.ChangedSubcomponents = None;
                    ScmRewriteState.BehaviorToInline = None;
                    ScmRewriteState.Tainted = false;
                }

                
    type ScmRewriteFunction<'a> = ScmRewriteFunction of (ScmRewriteState -> 'a * ScmRewriteState)
    
    let iterateToFixpoint (s:ScmRewriteFunction<unit>) =
        match s with
            | ScmRewriteFunction (functionToIterate) ->            
                let adjust_tainted_and_call (state:ScmRewriteState) : (bool*ScmRewriteState) =
                    // 1) Tainted is set to false
                    // 2) function is called
                    // 3) Tainted is set to true, if (at least one option applies)
                    //      a) it was true before the function call
                    //      b) the functionToIterate sets tainted to true 
                    let wasTaintedBefore = state.Tainted
                    let stateButUntainted =
                        { state with
                            ScmRewriteState.Tainted = false;
                        }
                    let (_,stateAfterCall) = functionToIterate stateButUntainted
                    let taintedByCall = stateAfterCall.Tainted
                    let newState =
                        { stateAfterCall with
                            ScmRewriteState.Tainted = wasTaintedBefore || taintedByCall;
                        }
                    (taintedByCall,newState)
                let rec iterate (state:ScmRewriteState) : (unit*ScmRewriteState) =                    
                    let (taintedByCall,stateAfterOneCall) = adjust_tainted_and_call state
                    if taintedByCall then
                        //((),stateAfterOneCall)
                        iterate stateAfterOneCall
                    else
                        ((),stateAfterOneCall)
                ScmRewriteFunction (iterate)

    // TODO:
    //   - RewriteElement should return, if it made a change
    //   - smallest element only gets called once
    //   - liftToFixpoint repeats a small element, until it doesn't change something anymore
    //   - so levelUpField levels up one field, levelUpFields levels up fields, until nothing possible anymore


    let runState (ScmRewriteFunction s) a = s a
    let getState = ScmRewriteFunction (fun s -> (s,s)) //Called in workflow: (implicitly) gets state (s) from workflow; assign this State s to the let!; and set (in this case keep)State of workflow to s
    let putState s = ScmRewriteFunction (fun _ -> ((),s)) //Called in workflow: ignore state (_) from workflow; assign nothing () to the let!; and set State of workflow to the new state s
    let putStateAndReturn s returnValue = ScmRewriteFunction (fun _ -> (returnValue,s))//Called in workflow: ignore state (_) from workflow; assign returnValue to the let!; and set State of workflow to the new state s

    // the computational expression "scmRewrite" is defined here
    type ScmRewriter() =
        member this.Return(a) = 
            ScmRewriteFunction (fun s -> (a,s))
        member this.Bind(m,k) =
            ScmRewriteFunction (fun state -> 
                let (a,state') = runState m state 
                runState (k a) state')
        member this.ReturnFrom (m) =
            m

    let scmRewrite = new ScmRewriter()

    
    // helpers

    

    let getCompletlyFreshName (basedOn:string) : ScmRewriteFunction<string> = scmRewrite {
            let! state = getState
            let newName = 
                let existsName (nameCandidate:string) : bool =
                    state.TakenNames.Contains nameCandidate
                let rec inventName numberSuffix : string =
                    // If desired name does not exist, get name with the lowest numberSuffix.
                    // This is not really beautiful, but finally leads to a free name, (because domain is finite).
                    let nameCandidate = sprintf "%s_art%i" basedOn numberSuffix
                    if existsName nameCandidate = false then
                        nameCandidate
                    else
                        inventName (numberSuffix+1)
                if existsName basedOn = false then
                    basedOn
                else
                    inventName 0
            let modifiedState =
                { state with
                    ScmRewriteState.TakenNames = state.TakenNames.Add newName;
                    ScmRewriteState.Tainted = true; // if tainted, set tainted to true
                }
            return! putStateAndReturn modifiedState newName
        }

    let getUnusedFieldName (basedOn:string) : ScmRewriteFunction<Field> = scmRewrite {
            let! name = getCompletlyFreshName basedOn
            return Field(name)
        }

    let getUnusedFaultName (basedOn:string) : ScmRewriteFunction<Fault> = scmRewrite {
            let! name = getCompletlyFreshName basedOn
            return Fault.Fault(name)
        }
            
    let getUnusedReqPortName (basedOn:string) : ScmRewriteFunction<ReqPort> = scmRewrite {
            let! name = getCompletlyFreshName basedOn
            return ReqPort(name)
        }

    let getUnusedProvPortName (basedOn:string) : ScmRewriteFunction<ProvPort> = scmRewrite {
            let! name = getCompletlyFreshName basedOn
            return ProvPort(name)
        }

    let getUnusedVarName (basedOn:string) : ScmRewriteFunction<Var> = scmRewrite {
            let! name = getCompletlyFreshName basedOn
            return Var(name)
        }



    // real rewriting rules

    // here the partial rewrite rules        
    let selectSubComponent : ScmRewriteFunction<unit> = scmRewrite {
            let! state = getState
            if (state.ChangedSubcomponents.IsSome) then
                // do not modify old tainted state here
                return ()
            else
                if state.Model.Subs = [] then
                // nothing to do, we are done
                    return ()
                else
                    // find component with no subcomponent, which is not the root. there must exist at least one
                    let rec findLeaf (parentPath:CompPath) (node:CompDecl) : CompPath =
                        let nodePath = node.Comp::parentPath
                        if node.Subs=[] then
                            nodePath
                        else
                            let firstChild = node.Subs.Head
                            findLeaf nodePath firstChild
                    let leaf = findLeaf ([]) (state.Model)
                    let selectedComponent = ScmRewriterSubcomponent.createEmptyFromPath state.Model leaf
                    let modifiedState =
                        { state with
                            ScmRewriteState.ChangedSubcomponents = Some(selectedComponent);       
                        }
                    return! putState modifiedState
        }

    let levelUpField : ScmRewriteFunction<unit> = scmRewrite {
            let! state = getState
            if (state.ChangedSubcomponents.IsNone) then
                // do not modify old tainted state here
                return! putState state // (alternative is to "return ()"
            else
                let infos = state.ChangedSubcomponents.Value
                // parent is target, child is source
                if infos.ChildCompDecl.Fields.IsEmpty then
                    // do not modify old tainted state here
                    return! putState state
                else
                    let fieldDecl = infos.ChildCompDecl.Fields.Head
                    let field = fieldDecl.Field
                    let newChildCompDecl = infos.ChildCompDecl.removeField fieldDecl
                    let! transformedField = getUnusedFieldName (sprintf "%s_%s" infos.ChildCompDecl.getName field.getName)
                    let transformedFieldDecl = 
                        {fieldDecl with
                            FieldDecl.Field = transformedField;
                        }                    
                    let newParentCompDecl = infos.ParentCompDecl.replaceChild(infos.ChildCompDecl,newChildCompDecl)
                                                                .addField(transformedFieldDecl)
                    let newChangedSubcomponents =
                        { infos with
                            ScmRewriterSubcomponent.ChildCompDecl = newChildCompDecl;
                            ScmRewriterSubcomponent.ParentCompDecl = newParentCompDecl;
                            ScmRewriterSubcomponent.ArtificialFieldsOldToNew = infos.ArtificialFieldsOldToNew.Add( field,transformedField );
                            ScmRewriterSubcomponent.ArtificialFieldsNewToOld = infos.ArtificialFieldsNewToOld.Add( (infos.ParentPath,transformedField), (infos.ChildPath,field) );
                        }
                    let modifiedState =
                        { state with
                            ScmRewriteState.ChangedSubcomponents = Some(newChangedSubcomponents);
                            ScmRewriteState.Tainted = true; // if tainted, set tainted to true
                        }
                    return! putState modifiedState
        }
    let levelUpFault : ScmRewriteFunction<unit> = scmRewrite {
            // TODO: No example and no test, yet
            let! state = getState
            if (state.ChangedSubcomponents.IsNone) then
                // do not modify old tainted state here
                return! putState state // (alternative is to "return ()"
            else
                let infos = state.ChangedSubcomponents.Value
                // parent is target, child is source
                if infos.ChildCompDecl.Faults.IsEmpty then
                    // do not modify old tainted state here
                    return! putState state
                else
                    let faultDecl = infos.ChildCompDecl.Faults.Head
                    let fault = faultDecl.Fault
                    let newChildCompDecl = infos.ChildCompDecl.removeFault faultDecl
                    let! transformedFault = getUnusedFaultName (sprintf "%s_%s" infos.ChildCompDecl.getName fault.getName)
                    let transformedFaultDecl = 
                        {faultDecl with
                            FaultDecl.Fault = transformedFault;
                        }                    
                    let newParentCompDecl = infos.ParentCompDecl.replaceChild(infos.ChildCompDecl,newChildCompDecl)
                                                                .addFault(transformedFaultDecl)
                    let newChangedSubcomponents =
                        { infos with
                            ScmRewriterSubcomponent.ChildCompDecl = newChildCompDecl;
                            ScmRewriterSubcomponent.ParentCompDecl = newParentCompDecl;
                            ScmRewriterSubcomponent.ArtificialFaultsOldToNew = infos.ArtificialFaultsOldToNew.Add( fault,transformedFault );
                            ScmRewriterSubcomponent.ArtificialFaultsNewToOld = infos.ArtificialFaultsNewToOld.Add( (infos.ParentPath,transformedFault), (infos.ChildPath,fault) );
                            ScmRewriterSubcomponent.FaultsToRewrite = transformedFaultDecl::infos.FaultsToRewrite;
                        }
                    let modifiedState =
                        { state with
                            ScmRewriteState.ChangedSubcomponents = Some(newChangedSubcomponents);
                            ScmRewriteState.Tainted = true; // if tainted, set tainted to true
                        }
                    return! putState modifiedState
        }
    let levelUpReqPort : ScmRewriteFunction<unit> = scmRewrite {
            let! state = getState
            if (state.ChangedSubcomponents.IsNone) then
                // do not modify old tainted state here
                return! putState state // (alternative is to "return ()"
            else
                let infos = state.ChangedSubcomponents.Value
                // parent is target, child is source
                if infos.ChildCompDecl.ReqPorts.IsEmpty then
                    // do not modify old tainted state here
                    return! putState state
                else
                    let reqPortDecl = infos.ChildCompDecl.ReqPorts.Head
                    let reqPort = reqPortDecl.ReqPort
                    let newChildCompDecl = infos.ChildCompDecl.removeReqPort reqPortDecl
                    let! transformedReqPort = getUnusedReqPortName (sprintf "%s_%s" infos.ChildCompDecl.getName reqPort.getName)
                    let transformedReqPortDecl = 
                        {reqPortDecl with
                            ReqPortDecl.ReqPort = transformedReqPort;
                        }                    
                    let newParentCompDecl = infos.ParentCompDecl.replaceChild(infos.ChildCompDecl,newChildCompDecl)
                                                                .addReqPort(transformedReqPortDecl)
                    let newChangedSubcomponents =
                        { infos with
                            ScmRewriterSubcomponent.ChildCompDecl = newChildCompDecl;
                            ScmRewriterSubcomponent.ParentCompDecl = newParentCompDecl;
                            ScmRewriterSubcomponent.ArtificialReqPortOldToNew = infos.ArtificialReqPortOldToNew.Add( reqPort,transformedReqPort );
                            ScmRewriterSubcomponent.ArtificialReqPortNewToOld = infos.ArtificialReqPortNewToOld.Add( (infos.ParentPath,transformedReqPort), (infos.ChildPath,reqPort) );
                        }
                    let modifiedState =
                        { state with
                            ScmRewriteState.ChangedSubcomponents = Some(newChangedSubcomponents);
                            ScmRewriteState.Tainted = true; // if tainted, set tainted to true
                        }
                    return! putState modifiedState
        }
    let levelUpProvPort : ScmRewriteFunction<unit> = scmRewrite {
            let! state = getState
            if (state.ChangedSubcomponents.IsNone) then
                // do not modify old tainted state here
                return! putState state // (alternative is to "return ()"
            else
                let infos = state.ChangedSubcomponents.Value
                // parent is target, child is source
                if infos.ChildCompDecl.ProvPorts.IsEmpty then
                    // do not modify old tainted state here
                    return! putState state
                else
                    let provPortDecl = infos.ChildCompDecl.ProvPorts.Head
                    let provPort = provPortDecl.ProvPort
                    let newChildCompDecl = infos.ChildCompDecl.removeProvPort provPortDecl
                    let! transformedProvPort = getUnusedProvPortName (sprintf "%s_%s" infos.ChildCompDecl.getName provPort.getName)
                    let transformedProvPortDecl = 
                        {provPortDecl with
                            ProvPortDecl.ProvPort = transformedProvPort;
                        }                    
                    let newParentCompDecl = infos.ParentCompDecl.replaceChild(infos.ChildCompDecl,newChildCompDecl)
                                                                .addProvPort(transformedProvPortDecl)
                    let newChangedSubcomponents =
                        { infos with
                            ScmRewriterSubcomponent.ChildCompDecl = newChildCompDecl;
                            ScmRewriterSubcomponent.ParentCompDecl = newParentCompDecl;
                            ScmRewriterSubcomponent.ArtificialProvPortOldToNew = infos.ArtificialProvPortOldToNew.Add( provPort,transformedProvPort );
                            ScmRewriterSubcomponent.ArtificialProvPortNewToOld = infos.ArtificialProvPortNewToOld.Add( (infos.ParentPath,transformedProvPort), (infos.ChildPath,provPort) );
                            ScmRewriterSubcomponent.ProvPortsToRewrite = transformedProvPortDecl::infos.ProvPortsToRewrite;
                        }
                    let modifiedState =
                        { state with
                            ScmRewriteState.ChangedSubcomponents = Some(newChangedSubcomponents);
                            ScmRewriteState.Tainted = true; // if tainted, set tainted to true
                        }
                    return! putState modifiedState
        }
       
    let levelUpAndRewriteBindingDeclaredInChild : ScmRewriteFunction<unit> = scmRewrite {
            // Cases: (view from parent, where sub1 is selected)                    
            //   Declared in parent (done in rule rewriteBindingDeclaredInParent)
            //    - x      -> x      (nothing to do)
            //    - x      -> sub1.x (target)
            //    - x      -> sub2.x (nothing to do)
            //    - sub1.x -> x      (source)
            //    - sub1.x -> sub1.x (source and target)
            //    - sub1.x -> sub2.x (source)
            //    - sub2.x -> x      (nothing to do)
            //    - sub2.x -> sub1.x (target)
            //    - sub2.x -> sub2.x (nothing to do)
            //   Declared in child (done here)
            //    - sub1.x -> sub1.x (source and target)
            let! state = getState
            if (state.ChangedSubcomponents.IsNone) then
                // do not modify old tainted state here
                return! putState state // (alternative is to "return ()"
            else
                let infos = state.ChangedSubcomponents.Value
                // parent is target, child is source
                if infos.ChildCompDecl.Bindings.IsEmpty then
                    // do not modify old tainted state here
                    return! putState state
                else
                    let bindingDecl = infos.ChildCompDecl.Bindings.Head
                    assert (bindingDecl.Source.Comp = None) //because the subcomponent has itself no subcomponent (we chose it so), it cannot have a binding from a subcomponent
                    assert (bindingDecl.Target.Comp = None) //because the subcomponent has itself no subcomponent (we chose it so), it cannot have a binding to a subcomponent
                    let newChildCompDecl = infos.ChildCompDecl.removeBinding bindingDecl
                    let newTarget =
                        let newReqPort = infos.ArtificialReqPortOldToNew.Item (bindingDecl.Target.ReqPort)
                        {
                            BndTarget.Comp = None;
                            BndTarget.ReqPort = newReqPort;
                        }
                    let newSource =
                        let newProvPort = infos.ArtificialProvPortOldToNew.Item (bindingDecl.Source.ProvPort)
                        {
                            BndSrc.Comp = None;
                            BndSrc.ProvPort = newProvPort;
                        }                    
                    let transformedBinding = 
                        {
                            BndDecl.Target = newTarget;
                            BndDecl.Source = newSource;
                            BndDecl.Kind = bindingDecl.Kind;
                        }                    
                    
                    let newParentCompDecl = infos.ParentCompDecl.replaceChild(infos.ChildCompDecl,newChildCompDecl)
                                                                .addBinding(transformedBinding)
                    let newChangedSubcomponents =
                        { infos with
                            ScmRewriterSubcomponent.ChildCompDecl = newChildCompDecl;
                            ScmRewriterSubcomponent.ParentCompDecl = newParentCompDecl;
                        }
                    let modifiedState =
                        { state with
                            ScmRewriteState.ChangedSubcomponents = Some(newChangedSubcomponents);
                            ScmRewriteState.Tainted = true; // if tainted, set tainted to true
                        }
                    return! putState modifiedState
        }
       
    let rewriteBindingDeclaredInParent : ScmRewriteFunction<unit> = scmRewrite {
            // Cases: (view from parent, where sub1 is selected)                    
            //   Declared in parent (done here)
            //    - x      -> x      (nothing to do)
            //    - x      -> sub1.x (target)
            //    - x      -> sub2.x (nothing to do)
            //    - sub1.x -> x      (source)
            //    - sub1.x -> sub1.x (source and target)
            //    - sub1.x -> sub2.x (source)
            //    - sub2.x -> x      (nothing to do)
            //    - sub2.x -> sub1.x (target)
            //    - sub2.x -> sub2.x (nothing to do)
            //   Declared in child (done in rule levelUpAndRewriteBindingDeclaredInChild)
            //    - sub1.x -> sub1.x (source and target)
            let! state = getState
            if (state.ChangedSubcomponents.IsNone) then
                // do not modify old tainted state here
                return! putState state // (alternative is to "return ()"
            else
                let infos = state.ChangedSubcomponents.Value
                // parent is target, child is source
                let bindingToRewrite : BndDecl option =
                    let targetIsChild (bndDecl:BndDecl) =
                        match bndDecl.Target.Comp with
                            | None -> false
                            | Some (comp) -> comp = infos.ChildCompDecl.Comp
                    let sourceIsChild (bndDecl:BndDecl) =
                        match bndDecl.Source.Comp with
                            | None -> false
                            | Some (comp) -> comp = infos.ChildCompDecl.Comp
                    infos.ParentCompDecl.Bindings |> List.tryFind (fun bndDecl -> (targetIsChild bndDecl) || (sourceIsChild bndDecl) )
                if bindingToRewrite.IsNone then
                    // do not modify old tainted state here
                    return! putState state
                else
                    let bindingToRewrite = bindingToRewrite.Value
                    
                    let newSource =
                        match bindingToRewrite.Source.Comp with
                            | None -> bindingToRewrite.Source
                            | Some (comp) ->
                                if comp = infos.ChildCompDecl.Comp then
                                    let port = infos.ArtificialProvPortOldToNew.Item (bindingToRewrite.Source.ProvPort)
                                    {
                                        BndSrc.Comp = None;
                                        BndSrc.ProvPort = port
                                    }
                                else
                                    bindingToRewrite.Source
                    let newTarget =
                        match bindingToRewrite.Target.Comp with
                            | None -> bindingToRewrite.Target
                            | Some (comp) ->
                                if comp = infos.ChildCompDecl.Comp then
                                    let port = infos.ArtificialReqPortOldToNew.Item (bindingToRewrite.Target.ReqPort)
                                    {
                                        BndTarget.Comp = None;
                                        BndTarget.ReqPort = port
                                    }
                                else
                                    bindingToRewrite.Target
                    let transformedBinding = 
                        {
                            BndDecl.Target = newTarget;
                            BndDecl.Source = newSource;
                            BndDecl.Kind = bindingToRewrite.Kind;
                        }
                    let newParentCompDecl = infos.ParentCompDecl.replaceBinding(bindingToRewrite,transformedBinding)
                    let newChangedSubcomponents =
                        { infos with
                            ScmRewriterSubcomponent.ParentCompDecl = newParentCompDecl;
                            //Note: Child really stayed the same
                        }
                    let modifiedState =
                        { state with
                            ScmRewriteState.ChangedSubcomponents = Some(newChangedSubcomponents);
                            ScmRewriteState.Tainted = true; // if tainted, set tainted to true
                        }
                    return! putState modifiedState
        }

    let convertStepToPort : ScmRewriteFunction<unit> = scmRewrite {
            let createArtificialStep : ScmRewriteFunction<unit> = scmRewrite {
                let! state = getState
                if (state.ChangedSubcomponents.IsNone) then
                    // do not modify old tainted state here
                    return! putState state // (alternative is to "return ()"
                else
                    let infos = state.ChangedSubcomponents.Value
                    if infos.ChildCompDecl.Steps.IsEmpty then
                        // do not modify old tainted state here
                        return! putState state
                    else
                        if infos.ArtificialStep = None then
                            let! reqPort = getUnusedReqPortName  (sprintf "%s_step" infos.ChildCompDecl.Comp.getName)
                            let! provPort = getUnusedProvPortName (sprintf "%s_step" infos.ChildCompDecl.Comp.getName) // TODO: create  and
                            
                            let newReqPortDecl = 
                                {
                                    ReqPortDecl.ReqPort = reqPort;
                                    ReqPortDecl.Params = [];
                                }
                            let newBindingDecl = 
                                {
                                    BndDecl.Target = {BndTarget.Comp = None; BndTarget.ReqPort = reqPort};
                                    BndDecl.Source = {BndSrc.Comp = None; BndSrc.ProvPort = provPort};
                                    BndDecl.Kind = BndKind.Instantaneous;
                                }
                                
                            let newChildCompDecl = infos.ChildCompDecl.addReqPort(newReqPortDecl)
                                                                      .addBinding(newBindingDecl)
                            let newParentCompDecl = infos.ParentCompDecl.replaceChild(infos.ChildCompDecl,newChildCompDecl)
                            let newInfos =
                                { infos with
                                    ScmRewriterSubcomponent.ChildCompDecl = newChildCompDecl;
                                    ScmRewriterSubcomponent.ParentCompDecl = newParentCompDecl;
                                    ScmRewriterSubcomponent.ArtificialStep = Some((reqPort,provPort))
                                }
                            let modifiedState =
                                { state with
                                    ScmRewriteState.ChangedSubcomponents = Some(newInfos);
                                    ScmRewriteState.Tainted = true; // if tainted, set tainted to true
                                }
                            return! putState modifiedState
                        else
                            //If we already have an artificial name, use it and do not generate a binding and a reqport
                            return ()
            }
            do! createArtificialStep
            
            // replace step to required port and provided port and binding, add a link from subcomponent path to new required port
            let! state = getState
            if (state.ChangedSubcomponents.IsNone) then
                // do not modify old tainted state here
                return! putState state // (alternative is to "return ()"
            else
                let infos = state.ChangedSubcomponents.Value
                if infos.ChildCompDecl.Steps.IsEmpty then
                    // do not modify old tainted state here
                    return! putState state
                else
                    let (reqPort,provPort) = infos.ArtificialStep.Value

                    let stepToConvert = infos.ChildCompDecl.Steps.Head
                    let newProvPortDecl =
                        {
                            ProvPortDecl.FaultExpr = stepToConvert.FaultExpr;
                            ProvPortDecl.ProvPort = provPort;
                            ProvPortDecl.Params = [];
                            ProvPortDecl.Behavior = stepToConvert.Behavior;
                        }
                    let newChildCompDecl = infos.ChildCompDecl.removeStep(stepToConvert)
                                                              .addProvPort(newProvPortDecl)
                    let newParentCompDecl = infos.ParentCompDecl.replaceChild(infos.ChildCompDecl,newChildCompDecl)

                    let newChangedSubcomponents =
                        { infos with
                            ScmRewriterSubcomponent.ChildCompDecl = newChildCompDecl;
                            ScmRewriterSubcomponent.ParentCompDecl = newParentCompDecl;
                            ScmRewriterSubcomponent.ProvPortsToRewrite = (newProvPortDecl)::infos.ProvPortsToRewrite;
                            ScmRewriterSubcomponent.StepsToRewrite = infos.ParentCompDecl.Steps; //It is necessary to set this once. But it seems, that it does not harm to set it multiple times
                        }
                    let modifiedState =
                        { state with
                            ScmRewriteState.ChangedSubcomponents = Some(newChangedSubcomponents);
                            ScmRewriteState.Tainted = true; // if tainted, set tainted to true
                        }
                    return! putState modifiedState
                        
        }

    let rewriteParentStep : ScmRewriteFunction<unit> = scmRewrite {
            // here, additionally instead of "step subcomponent" the converted step must be called
            
            let! state = getState
            if (state.ChangedSubcomponents.IsNone) then
                // do not modify old tainted state here
                return! putState state // (alternative is to "return ()"
            else if state.ChangedSubcomponents.Value.ArtificialStep.IsNone then
                return! putState state // (alternative is to "return ()"
            else
                let infos = state.ChangedSubcomponents.Value
                
                if infos.StepsToRewrite.IsEmpty then
                        // do not modify old tainted state here
                        return! putState state
                else
                    let stepToRewrite = infos.StepsToRewrite.Head

                    let (reqPort,_) = infos.ArtificialStep.Value
                
                    let rewriteStep (step:StepDecl) : StepDecl =
                        let rec rewriteStm (stm:Stm) : Stm =
                            match stm with
                                | Stm.Block (smnts) ->
                                    let newStmnts = smnts |> List.map rewriteStm
                                    Stm.Block(newStmnts)
                                | Stm.Choice (choices:(Expr * Stm) list) ->
                                    let newChoices = choices |> List.map (fun (expr,stm) -> (expr,rewriteStm stm) )
                                    Stm.Choice(newChoices)
                                | Stm.StepComp (comp) ->
                                    Stm.CallPort (reqPort,[])
                                | _ -> stm
                        let newBehavior =
                            { step.Behavior with
                                BehaviorDecl.Body = rewriteStm step.Behavior.Body;
                            }
                        { step with
                            StepDecl.Behavior = newBehavior;
                        }

                    let newStep = rewriteStep stepToRewrite                
                    let newParentCompDecl = infos.ParentCompDecl.replaceStep(stepToRewrite,newStep);
                    let newChangedSubcomponents =
                        { infos with
                            ScmRewriterSubcomponent.ParentCompDecl = newParentCompDecl;
                            ScmRewriterSubcomponent.StepsToRewrite = infos.StepsToRewrite.Tail;
                        }
                    let modifiedState =
                        { state with
                            ScmRewriteState.ChangedSubcomponents = Some(newChangedSubcomponents);
                            ScmRewriteState.Tainted = true; // if tainted, set tainted to true
                        }
                    return! putState modifiedState

        }

    let rewriteProvPort : ScmRewriteFunction<unit> = scmRewrite {
            // replace reqPorts and fields by their proper names, replace Fault Expressions
            
            let! state = getState
            if (state.ChangedSubcomponents.IsNone) then
                // do not modify old tainted state here
                return! putState state // (alternative is to "return ()"
            else
                let infos = state.ChangedSubcomponents.Value
                if infos.ProvPortsToRewrite.IsEmpty then
                    // do not modify old tainted state here
                    return! putState state
                else
                    // we are in a parent Component!!!
                    let provPortToRewrite = infos.ProvPortsToRewrite.Head
                    
                    let rewrittenProvPort =
                        {
                            ProvPortDecl.FaultExpr = rewriteFaultExprOption infos.oldToNewMaps2 provPortToRewrite.FaultExpr;
                            ProvPortDecl.ProvPort = provPortToRewrite.ProvPort;
                            ProvPortDecl.Params = provPortToRewrite.Params; // The getUnusedxxxName-Functions also ensured, that the names of new fields and faults,... do not overlap with any param. So we can keep it
                            ProvPortDecl.Behavior = rewriteBehavior infos.oldToNewMaps1 provPortToRewrite.Behavior;
                        }
                    let newParentCompDecl = infos.ParentCompDecl.replaceProvPort(provPortToRewrite,rewrittenProvPort)

                    let newChangedSubcomponents =
                        { infos with
                            ScmRewriterSubcomponent.ParentCompDecl = newParentCompDecl;
                            ScmRewriterSubcomponent.ProvPortsToRewrite = infos.ProvPortsToRewrite.Tail;
                        }
                    let modifiedState =
                        { state with
                            ScmRewriteState.ChangedSubcomponents = Some(newChangedSubcomponents);
                            ScmRewriteState.Tainted = true; // if tainted, set tainted to true
                        }
                    return! putState modifiedState
        }

    let rewriteFaults : ScmRewriteFunction<unit> = scmRewrite {
            // replace reqPorts and fields by their proper names, replace Fault Expressions
            let! state = getState
            if (state.ChangedSubcomponents.IsNone) then
                // do not modify old tainted state here
                return! putState state // (alternative is to "return ()"
            else
                let infos = state.ChangedSubcomponents.Value
                if infos.FaultsToRewrite.IsEmpty then
                    // do not modify old tainted state here
                    return! putState state
                else
                    // we are in a parent Component!!!
                    let faultToRewrite = infos.FaultsToRewrite.Head
                    
                    let rewrittenFault =
                        {
                            FaultDecl.Fault = faultToRewrite.Fault;
                            FaultDecl.Step = rewriteBehavior infos.oldToNewMaps1 faultToRewrite.Step;
                        }
                    let newParentCompDecl = infos.ParentCompDecl.replaceFault(faultToRewrite,rewrittenFault)

                    let newChangedSubcomponents =
                        { infos with
                            ScmRewriterSubcomponent.ParentCompDecl = newParentCompDecl;
                            ScmRewriterSubcomponent.ProvPortsToRewrite = infos.ProvPortsToRewrite.Tail;
                        }
                    let modifiedState =
                        { state with
                            ScmRewriteState.ChangedSubcomponents = Some(newChangedSubcomponents);
                            ScmRewriteState.Tainted = true; // if tainted, set tainted to true
                        }
                    return! putState modifiedState
        }
    let assertSubcomponentEmpty : ScmRewriteFunction<unit> = scmRewrite {
            let! state = getState
            if (state.ChangedSubcomponents.IsNone) then
                // do not modify old tainted state here
                return! putState state // (alternative is to "return ()"
            else
                let infos = state.ChangedSubcomponents.Value
                assert (infos.ChildCompDecl.Subs = [])
                assert (infos.ChildCompDecl.Fields = [])
                assert (infos.ChildCompDecl.Faults = [])
                assert (infos.ChildCompDecl.ReqPorts = [])
                assert (infos.ChildCompDecl.ProvPorts = [])
                assert (infos.ChildCompDecl.Bindings = [])
                return ()
        }
    let removeSubComponent : ScmRewriteFunction<unit> = scmRewrite {            
            let! state = getState
            if (state.ChangedSubcomponents.IsNone) then
                // do not modify old tainted state here
                return! putState state // (alternative is to "return ()"
            else
                let infos = state.ChangedSubcomponents.Value
                let newParentCompDecl = infos.ParentCompDecl.removeChild(infos.ChildCompDecl)
                let newChangedSubcomponents =
                    { infos with
                        ScmRewriterSubcomponent.ParentCompDecl = newParentCompDecl;
                    }
                let modifiedState =
                    { state with
                        ScmRewriteState.ChangedSubcomponents = Some(newChangedSubcomponents);
                        ScmRewriteState.Tainted = true; // if tainted, set tainted to true
                    }
                return! putState modifiedState
                
        }        
    let writeBackChangesIntoModel  : ScmRewriteFunction<unit> = scmRewrite {
            let! state = getState
            if (state.ChangedSubcomponents.IsNone) then
                // do not modify old tainted state here
                return! putState state // (alternative is to "return ()"
            else
                let changedSubcomponents = state.ChangedSubcomponents.Value
                let newModel = state.Model.replaceDescendant changedSubcomponents.ParentPath changedSubcomponents.ParentCompDecl
                let modifiedState =
                    { state with
                        ScmRewriteState.Model = newModel;
                        ScmRewriteState.ChangedSubcomponents = None;
                        ScmRewriteState.Tainted = true; // if tainted, set tainted to true
                    }
                return! putState modifiedState
        }
    let assertNoSubcomponent : ScmRewriteFunction<unit> = scmRewrite {
            let! state = getState
            assert (state.Model.Subs=[])
            return ()
        }
        
    let levelUpSubcomponent : ScmRewriteFunction<unit> = scmRewrite {
            // idea: first level up every item of a component,
            //       then rewrite every code accessing to some specific element of it
            do! selectSubComponent
            do! (iterateToFixpoint levelUpField) //Invariant: Imagine ChangedSubcomponents are written back into model. Fieldaccess (read/write) is either on the "real" field or on a "forwarded field" (map entry in ArtificialFieldsOldToNew exists, and new field exists)
            do! (iterateToFixpoint levelUpFault)
            do! (iterateToFixpoint convertStepToPort)
            do! (iterateToFixpoint levelUpReqPort)
            do! (iterateToFixpoint levelUpProvPort)
            do! (iterateToFixpoint levelUpAndRewriteBindingDeclaredInChild)
            do! (iterateToFixpoint rewriteBindingDeclaredInParent)
            do! (iterateToFixpoint rewriteParentStep)
            do! (iterateToFixpoint rewriteProvPort)
            do! (iterateToFixpoint rewriteFaults)
            do! assertSubcomponentEmpty
            do! removeSubComponent
            do! writeBackChangesIntoModel
        }
                





    let findBehaviorToInline : ScmRewriteFunction<unit> = scmRewrite {    
        let! state = getState

        let rec callingDepth (stm:Stm) (currentLevel:int) (stopAtLevel:int) : int =
            let rec maxLevel (stmnts:Stm list) (accMaxLevel:int) : int =
                if (stmnts=[]) then
                    accMaxLevel
                else if accMaxLevel=stopAtLevel then
                    stopAtLevel //early quit: Stop searching at depth stopAtLevel
                else
                    let headLevel = callingDepth stmnts.Head currentLevel stopAtLevel
                    let head_or_acc = max currentLevel accMaxLevel
                    maxLevel stmnts.Tail head_or_acc
            match stm with
                | Stm.AssignVar (_) -> currentLevel
                | Stm.AssignField (_) -> currentLevel
                | Stm.AssignFault (_) -> currentLevel
                | Stm.Block (stmnts) ->
                    maxLevel stmnts currentLevel
                | Stm.Choice (choices:(Expr * Stm) list) ->                    
                    let stmnts =
                        choices |> List.map (fun (expr,stm) -> stm)
                    maxLevel stmnts currentLevel          
                | Stm.CallPort (reqPort,_params) ->
                    let binding = state.Model.getBindingOfLocalReqPort reqPort
                    //if binding.Kind= BndKind.Delayed then
                    //    failwith "Delayed Bindings cannot be inlined yet" // doesn't matter for depth
                    let provPortsStmts =
                        binding.Source.ProvPort
                            |> state.Model.getProvPortDecls
                            |> List.map (fun portDecl -> portDecl.Behavior.Body)
                    maxLevel provPortsStmts (currentLevel+1)
                | Stm.StepComp (comp) ->
                    failwith "BUG: In this phase Stm.StepComp should not be in any statement"
                | Stm.StepFault (fault) ->
                    let faultStmts =
                        state.Model.Faults
                            |> List.map (fun fault -> fault.Step.Body)
                    maxLevel faultStmts (currentLevel+1)

        if (state.BehaviorToInline.IsSome) then
            return ()
        else
            // try to find a behavior, which only contains port calls, which themselves do not call ports            
            // (level calculated by "callingDepth" is exactly 1)            
            let tryFindInProvPorts () : BehaviorWithLocation option =
                let encapsulateResult (port:ProvPortDecl option) : BehaviorWithLocation option =
                    match port with
                        | None -> None
                        | Some (portDecl) -> Some(BehaviorWithLocation.InProvPort(portDecl,portDecl.Behavior))
                state.Model.ProvPorts |> List.tryFind (fun port -> (callingDepth port.Behavior.Body 0 2)=1)
                                      |> encapsulateResult

            let tryFindInFaultDecls () : BehaviorWithLocation option =
                let encapsulateResult (port:FaultDecl option) : BehaviorWithLocation option =
                    match port with
                        | None -> None
                        | Some (faultDecl) -> Some(BehaviorWithLocation.InFault(faultDecl,faultDecl.Step))
                state.Model.Faults|> List.tryFind (fun fault -> (callingDepth fault.Step.Body 0 2)=1)
                                  |> encapsulateResult

            let tryFindInStep () : BehaviorWithLocation option =
                let encapsulateResult (port:StepDecl option) : BehaviorWithLocation option =
                    match port with
                        | None -> None
                        | Some (stepDecl) -> Some(BehaviorWithLocation.InStep(stepDecl,stepDecl.Behavior))
                state.Model.Steps|> List.tryFind (fun step -> (callingDepth step.Behavior.Body 0 2)=1)
                                 |> encapsulateResult

            let candidateToInline : BehaviorWithLocation option =
                match tryFindInProvPorts () with
                    | Some(x) -> Some(x)
                    | None ->
                        match tryFindInFaultDecls () with
                            | Some(x) -> Some(x)
                            | None -> tryFindInStep ()

            match candidateToInline with
                | None -> return ()
                | Some (behaviorToInline) ->
                    let rewriterBehaviorToInline = ScmRewriterInlineBehavior.createEmptyFromBehavior behaviorToInline
                    let modifiedState =
                        { state with
                            ScmRewriteState.BehaviorToInline = Some(rewriterBehaviorToInline);
                            ScmRewriteState.Tainted = true; // if tainted, set tainted to true
                        }
                    return! putState modifiedState
        }
    
    let findCallToInline : ScmRewriteFunction<unit> = scmRewrite {
            let! state = getState
            if (state.BehaviorToInline.IsNone) then
                return ()
            else
                let behaviorToInline = state.BehaviorToInline.Value
                if behaviorToInline.CallToReplace.IsSome then
                    return ()
                else
                    let rec findCall (stm:Stm) (currentPath:StmPath) : (StmPath option) =
                        match stm with
                            | Stm.AssignVar (_) -> None
                            | Stm.AssignField (_) -> None
                            | Stm.AssignFault (_) -> None
                            | Stm.Block (stmnts) ->
                                stmnts |> List.map2 (fun index stm -> (index,stm)) ([0..(stmnts.Length-1)])
                                       |> List.tryPick( fun (index,stm) -> findCall stm (index::currentPath))
                            | Stm.Choice (choices:(Expr * Stm) list) ->
                                choices |> List.map2 (fun index stm -> (index,stm)) ([0..(choices.Length-1)])
                                        |> List.tryPick( fun (index,(guard,stm)) -> findCall stm (index::currentPath))
                            | Stm.CallPort (_) ->
                                Some(currentPath)
                            | Stm.StepComp (comp) ->
                                failwith "BUG: In this phase Stm.StepComp should not be in any statement"
                                Some(currentPath)
                            | Stm.StepFault (_) ->
                                Some(currentPath)                    
                    let callToInline = findCall behaviorToInline.InlinedBehavior.Body []
                    match callToInline with
                        | None -> return ()
                        | Some (path:StmPath) ->
                            let newBehaviorToInline =
                                { behaviorToInline with
                                    ScmRewriterInlineBehavior.CallToReplace=Some(path);
                                }
                            let modifiedState =
                                { state with
                                    ScmRewriteState.BehaviorToInline = Some(newBehaviorToInline);
                                    ScmRewriteState.Tainted = true; // if tainted, set tainted to true
                                }
                            return! putState modifiedState
        }

    let inlineCall : ScmRewriteFunction<unit> = scmRewrite {
        return ()
        (*
            let! state = getState
            if (state.BehaviorToInline.IsNone) then
                return ()
            else
                let behaviorToInline = state.BehaviorToInline.Value
                if behaviorToInline.CallToReplace.IsNone then
                    return ()
                else
                    let pathToCallToReplace = behaviorToInline.CallToReplace.Value
                    let body = behaviorToInline.InlinedBehavior.Body
                    let callToReplace = body.getSubStatement pathToCallToReplace
                    let (addedLocals,newStatement) = 
                        match callToReplace with
                            | Stm.AssignVar (_) -> failwith "BUG: Nothing to be inlined at desired position"
                            | Stm.AssignField (_) -> failwith "BUG: Nothing to be inlined at desired position"
                            | Stm.AssignFault (_) -> failwith "BUG: Nothing to be inlined at desired position"
                            | Stm.Block (_) -> failwith "BUG: Nothing to be inlined at desired position"
                            | Stm.Choice (_) -> failwith "BUG: Nothing to be inlined at desired position"  
                            | Stm.CallPort (reqPort,_params) ->
                                let binding = state.Model.getBindingOfLocalReqPort reqPort
                                if binding.Kind= BndKind.Delayed then
                                    failwith "Delayed Bindings cannot be inlined yet"
                                let provPortDecls = binding.Source.ProvPort |> state.Model.getProvPortDecls
                                assert (provPortDecls.Length = 1) //exactly one provPortDecl should exist. Assume uniteProvPortDecls was called
                                let provPortDecl = provPortDecls.Head
                                let newLocalFieldsTuple =
                                    // Note: assure, no name clashes and inside always fresh names are used
                                    let! transformedVarName (var:VarDecl) = 
                                        getUnusedVarName (sprintf "%s_%s" provPortDecl.getName var.getName)
                                    provPortDecl.Behavior.Locals |> List.map (fun varDecl -> (varDecl,transformedVarName varDecl) )
                                let newLocalFieldsMap =
                                    newLocalFieldsTuple |> List.map (fun (oldVarDecl,newVar) -> (oldVarDecl.Var,newVar))
                                                        |> Map.ofList
                                let newLocalFieldDecls =
                                    let createNewVarDecl (oldVarDecl:VarDecl,newVar:Var) : VarDecl =
                                        { oldVarDecl with
                                            VarDecl.Var = newVar
                                        }
                                    newLocalFieldsTuple |> List.map (fun entry -> createNewVarDecl entry)
                                // replace Local:VarDecl by new Local:VarDecl in Statement
                                let newStatement = rewriteStm (Map.empty,Map.empty,newLocalFieldsMap,Map.empty) (provPortDecl.Behavior.Body)
                                // replace Params by their actual Fields or LocalVars declared in the parameters of the caller
                                
                                // here we need to take a close look: if an expression was used as parameter, add an assignment to a local variable
                                // add this local variable to the other local variables.
                                // Otherwise replace the names in-text. Also replace a varCall to a FieldCall in expressions and assignments
                                let zippedParameters =
                                    List.zip _params provPortDecl.Params
                                //let (newStatementsFromParam,newLocalFieldsFromParam) =
                                (*
                                let temp =
                                    let transform (reqParam:Param) (provParam:ParamDecl) =
                                        match 
                                        ()
                                    zippedParameters |> List.map (fun (reqParam,provParam) -> transform reqParam provParam)                                    
                                *)
                                let newStatement =
                                    newStatement
                                (newLocalFieldDecls,newStatement)
                            | Stm.StepComp (comp) ->
                                failwith "BUG: In this phase Stm.StepComp should not be in any statement"
                            | Stm.StepFault (fault) ->
                                failwith "BUG: In this phase Stm.StepFault should not be in any statement"
                    let newBody = body.replaceSubStatement pathToCallToReplace newStatement
                    let newBehavior =
                        {
                            BehaviorDecl.Body = newBody;
                            BehaviorDecl.Locals = behaviorToInline.InlinedBehavior.Locals @ addedLocals;
                        }
                    let newRewriterBehaviorToInline =
                        { behaviorToInline with
                            ScmRewriterInlineBehavior.CallToReplace = None;
                            ScmRewriterInlineBehavior.InlinedBehavior = newBehavior;
                        }
                    let modifiedState =
                        { state with
                            ScmRewriteState.BehaviorToInline = Some(newRewriterBehaviorToInline);
                            ScmRewriteState.Tainted = true; // if tainted, set tainted to true
                        }
                    return! putState modifiedState
                       *)
        }   

    let inlineBehavior : ScmRewriteFunction<unit> = scmRewrite {
            // Assert: only inline statements in the root-component

            do! (iterateToFixpoint (scmRewrite {
                do! findCallToInline
                do! inlineCall
            }))
        }

    let writeBackChangedBehavior : ScmRewriteFunction<unit> = scmRewrite {
            // Assert: only inline statements in the root-component 
            let! state = getState
            if (state.BehaviorToInline.IsNone) then
                return ()
            else
                let behaviorToInline = state.BehaviorToInline.Value
                let newModel =
                    match behaviorToInline.BehaviorToReplace with
                        | BehaviorWithLocation.InProvPort (provPortDecl,beh) ->
                            let newProvPort =
                                { provPortDecl with
                                    ProvPortDecl.Behavior = behaviorToInline.InlinedBehavior;
                                }
                            state.Model.replaceProvPort(provPortDecl,newProvPort) 
                        | BehaviorWithLocation.InFault (faultDecl,beh) ->
                            let newFault =
                                { faultDecl with
                                    FaultDecl.Step = behaviorToInline.InlinedBehavior;
                                }
                            state.Model.replaceFault(faultDecl,newFault) 
                        | BehaviorWithLocation.InStep (stepDecl,beh) ->
                            let newStep =
                                { stepDecl with
                                    StepDecl.Behavior = behaviorToInline.InlinedBehavior;
                                }
                            state.Model.replaceStep (stepDecl,newStep) 
                let modifiedState =
                    { state with
                        ScmRewriteState.Model = newModel;
                        ScmRewriteState.BehaviorToInline = None;
                        ScmRewriteState.Tainted = true; // if tainted, set tainted to true
                    }
                return! putState modifiedState
        }


        
    let findAndInlineBehavior : ScmRewriteFunction<unit> = scmRewrite {
            // Assert: only inline statements in the root-component
            do! findBehaviorToInline            
            do! inlineBehavior
            do! writeBackChangedBehavior
        }








    let checkConsistency : ScmRewriteFunction<unit> = scmRewrite {
            return ()
        }







    let levelUpAndInline : ScmRewriteFunction<unit> = scmRewrite {
            // level up everything
            do! (iterateToFixpoint levelUpSubcomponent)
            do! assertNoSubcomponent
            do! checkConsistency
            
            // convert faults
            // do! replaceFaultsByPortsAndFields
            // do! replaceStepFaultByCallPort
            // do! uniteProvPortDecls //for each ProvPort: replace all ProvPortDecls with the same ProvPort with one ProvPortDecl: Make a guarded command, which differentiates between the different faults
            // do! uniteStep  //for each StepDecl: replace all StepDecls one StepDecl: Make a guarded command, which differentiates between the different faults
            // do! checkConsistency
            
            // inline everything beginning with the main step
            do! (iterateToFixpoint findAndInlineBehavior)
            do! checkConsistency
        }
