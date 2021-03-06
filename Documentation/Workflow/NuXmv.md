# The NuXmv pipeline to verify SafetySharp models

## Requirements (Windows)

* NuXmv V 1.0.0 https://nuxmv.fbk.eu/

## Workflow from SCM to NuXmv (wp Method)

Note: only valid for deterministic models

```
do! ScmWorkflow.setPlainModelState scm
do! SafetySharp.Models.ScmRewriterFlattenModel.flattenModel
do! SafetySharp.Models.ScmToSam.transformIscmToSam
do! SafetySharp.Analysis.VerificationCondition.VcSamModelForModification.transformSamToVcSamForModification
do! SafetySharp.Analysis.Modelchecking.NuXmv.VcSamToNuXmvWp.transformConfiguration_fromVcSam
do! SafetySharp.Analysis.Modelchecking.NuXmv.NuXmvToString.workflow
//do! SafetySharp.Workflow.saveToFile *freshFileName*
//do! TODO: Model check
```


[comment]: # (Encoded in UMLGraph from http://plantuml.sourceforge.net/activity.html)
[comment]: # (Need to include ; in each new line:)
[comment]: # (Need to Replace '(' by %28 and  ')' by %29 )
[comment]: # (Encoder available at http://www.gravizo.com/#converter )


## Workflow from SCM to NuXmv (gwa Method)

```
do! ScmWorkflow.setPlainModelState scm
do! SafetySharp.Models.ScmRewriterFlattenModel.flattenModel
do! SafetySharp.Models.ScmToSam.transformIscmToSam
do! SafetySharp.Analysis.VerificationCondition.VcSamModelForModification.transformSamToVcSamForModification
do! SafetySharp.Analysis.VerificationCondition.VcPassiveFormGCFK09.transformProgramToSsaForm_Original // optional
do! SafetySharp.Analysis.VerificationCondition.VcGuardWithAssignmentModel.transformWorkflow
TODO: do! SafetySharp.Analysis.Modelchecking.NuXmv.GwaToNuXmv.
do! SafetySharp.Analysis.Modelchecking.NuXmv.NuXmvToString.workflow
//do! SafetySharp.Workflow.saveToFile *freshFileName*
//do! TODO: Model check
```
