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

module internal ModelWorkflow =

    open SafetySharp.Workflow

    let getModel<'model,'trackinginfo when 'model :> IModel and 'trackinginfo :> ITrackingInfo>
                    : WorkflowFunction<'model*('trackinginfo option),'model*('trackinginfo option),'model> = workflow {
        let! (model,trackingInfo) = getState
        return model
    }


    // We track back to the first model of the workflow. The first model has "None" trackingInfo
    let getTrackingInfo<'model,'trackinginfo when 'model :> IModel and 'trackinginfo :> ITrackingInfo>
                    : WorkflowFunction<'model*('trackinginfo option),'model*('trackinginfo option),'trackinginfo option> = workflow {
        let! (model,trackingInfo) = getState
        return trackingInfo
    }