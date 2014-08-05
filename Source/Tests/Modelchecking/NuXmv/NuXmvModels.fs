﻿namespace SafetySharp.Tests.Modelchecking.NuXmv.Models

module Models =
    let ``incomplete-case-distinction`` =
          "MODULE main\n"
        + "	VAR\n"
        + "		x : boolean;\n"
        + "	ASSIGN\n"
        + "		init(x) := TRUE;\n"
        + "		next(x) :=\n"
        + "			case\n"
        + "				x = TRUE : FALSE;\n"
        + "			esac;\n"

    let ``incomplete-instantiation`` =
          "MODULE module_a(x,y)\n"
        + "\n"
        + "MODULE main\n"
        + "	VAR\n"
        + "		x : boolean;\n"
        + "		y : module_a(x);\n"
        
    let ``not-fully-defined1`` =
          "MODULE main\n"
        + "	VAR\n"
        + "		x : boolean;\n"
        + "	INIT\n"
        + "		TRUE\n"
        + "	TRANS\n"
        + "		next(x) != FALSE & next(x) != TRUE\n"

    let ``not-fully-defined2`` =
          "MODULE main\n"
        + "	VAR\n"
        + "		x : boolean;\n"
        + "	INIT\n"
        + "		x = TRUE\n"
        + "	TRANS\n"
        + "		(x = TRUE & next(x) = TRUE )\n"

    let ``fully-defined`` =
          "MODULE main\n"
        + "	VAR\n"
        + "		x : boolean;\n"
        + "	INIT\n"
        + "		x = TRUE\n"
        + "	TRANS\n"
        + "		(x = TRUE & next(x) = TRUE ) | (x = FALSE & next(x) = FALSE )\n"
                
    let ``wrong-syntax1`` =
          "MODULE main\n"
        + "	VAR\n"
        + "		x : booleaan;\n"

    let ``wrong-syntax2`` =
          "MODULE main\n"
        + "	VAR\n"
        + "		x : boolean;\n"
        + "	INIT\n"
        
    let ``simple-indeterministic`` =
          "MODULE main\n"
        + "	VAR\n"
        + "		x : boolean;\n"

    let ``range-not-respected`` =
           "MODULE main\n"
         + "	VAR\n"
         + "		x : 1..5;\n"
         + "	ASSIGN\n"
         + "		init(x) := 1;\n"
         + "		next(x) := x + 1;\n"
    
    let ``simple-deterministic`` =
          "MODULE main\n"
        + "	VAR\n"
        + "		x : boolean;\n"
        + "	ASSIGN\n"
        + "		init(x) := TRUE;\n"
        + "		next(x) :=\n"
        + "			case\n"
        + "				x = TRUE : FALSE;\n"
        + "				x = FALSE : TRUE;\n"
        + "			esac;\n"

    (*  
        For tests: Copy&Paste
        
        FileSystem.WriteToAsciiFile filename model

        let filename = "Modelchecking/NuXmv/incomplete-case-distinction.smv"
        let model = Models.``incomplete-case-distinction``
        let filename = "Modelchecking/NuXmv/incomplete-instantiation.smv"
        let model = Models.``incomplete-instantiation``
        let filename = "Modelchecking/NuXmv/not-fully-defined1.smv"
        let model = Models.``not-fully-defined1``
        let filename = "Modelchecking/NuXmv/not-fully-defined2.smv"
        let model = Models.``not-fully-defined2``
        let filename = "Modelchecking/NuXmv/fully-defined.smv"
        let model = Models.``fully-defined``
        let filename = "Modelchecking/NuXmv/wrong-syntax1.smv"
        let model = Models.``wrong-syntax1``
        let filename = "Modelchecking/NuXmv/wrong-syntax2.smv"
        let model = Models.``wrong-syntax2``
        let filename = "Modelchecking/NuXmv/simple-indeterministic.smv"
        let model = Models.``simple-indeterministic``
        let filename = "Modelchecking/NuXmv/range-not-respected.smv"
        let model = Models.``range-not-respected``
        let filename = "Modelchecking/NuXmv/simple-deterministic.smv"
        let model = Models.``simple-deterministic``

    *)