component backupRecoverySystem {
	component in {
		sourceValueField : int = 1;
		
		determineSourceValueR ( );
		
		determineSourceValueP ( ) {
			locals{}
			choice {
				true => { sourceValueField := 1; }
				true => { sourceValueField := 2; }
				true => { sourceValueField := 3; }
				true => { sourceValueField := 4; }
				true => { sourceValueField := 5; }
			}
		}
		
		getSourceValueP ( inout sourceValueInout : int ) {
			locals{}
			sourceValueInout := sourceValueField;
		}
		
		in.determineSourceValueR = instantly in.determineSourceValueP
		
		step {
			locals{}
			determineSourceValueR ( );
		}
	}

	component s1 {		
		senseSourceValueR ( inout sourceValueInout : int );
	
		getSensorValueP ( inout sensorValueInout : int ) {
			locals{}
			senseSourceValueR ( inout sensorValueInout );
		}
		
		step { locals{} }
	}
	
	component s2 {		
		senseSourceValueR ( inout sourceValueInout : int );
	
		getSensorValueP ( inout sensorValueInout : int ) {
			locals{}
			senseSourceValueR ( inout sensorValueInout );
		}
		
		step { locals{} }
	}
	
	component a1 {
		isActiveField : bool = true;
		
		getSensorValue1R ( inout sensorValueInout : int );
		getSensorValue2R ( inout sensorValueInout : int );
		doValuesMatchR ( value1In : int , value2In : int , inout matchInout : bool);
	
		getArithmeticalValueP ( inout arithmeticalValueInout : int ) {
			locals{
				int sensorValue1Local;
				int sensorValue2Local;
			}
			getSensorValue1R ( inout sensorValue1Local ) ;
			getSensorValue2R ( inout sensorValue2Local ) ;
			arithmeticalValueInout := sensorValue1Local;
		}

		doValuesMatchP ( value1In : int , value2In : int , inout matchInout : bool) {
			locals{}
			matchInout := value1In == value2In;
		}
		
		doSensorValuesMatchP ( inout sensorValuesMatchInout : bool ) {
			locals{
				int sensorValue1Local;
				int sensorValue2Local;
			}
			getSensorValue1R ( inout sensorValue1Local ) ;
			getSensorValue2R ( inout sensorValue2Local ) ;
			doValuesMatchR ( sensorValue1Local , sensorValue2Local , inout sensorValuesMatchInout);
		}
		
		setActiveArithmeticalUnitP ( unitNo : int ) {
			locals{}
			choice {
				unitNo==1 => { isActiveField := true; }
				unitNo==2 => { isActiveField := false; }
			}
		}
		
		a1.doValuesMatchR = instantly a1.doValuesMatchP

		step { locals{} }
	}
	
	component a2 {
		isActiveField : bool = false;
		
		getSensorValueR ( inout sensorValueInout : int );
	
		getArithmeticalValueP ( inout arithmeticalValueInout : int ) {
			locals{}
			getSensorValueR ( inout arithmeticalValueInout );
		}
		
		setActiveArithmeticalUnitP ( unitNo : int ) {
			locals{}
			choice {
				unitNo==1 => { isActiveField := false; }
				unitNo==2 => { isActiveField := true; }
			}
		}
		
		step { locals{} }
	}
	
	component monitor {
		doSensorValuesMatchR ( inout sensorValuesMatchInout : bool );
		switchArithmeticalUnitR_PartA ( unitNo : int );
		switchArithmeticalUnitR_PartB ( unitNo : int );
		switchArithmeticalUnitR_PartC ( unitNo : int );
		
		step {
			locals{
				bool doSensorValuesMatchLocal;
}
			doSensorValuesMatchR ( inout doSensorValuesMatchLocal) ;
			choice {
				doSensorValuesMatchLocal => { } 
				! doSensorValuesMatchLocal => {
					switchArithmeticalUnitR_PartA ( 2 ) ;
					switchArithmeticalUnitR_PartB ( 2 ) ;
					switchArithmeticalUnitR_PartC ( 2 ) ;
				}
			}
		}
	}

	component out {
		arithUnitNoField : int = 1;
		result : int = 1;
		
		getArithmeticalValue1R ( inout arithmeticalValueInout : int );
		getArithmeticalValue2R ( inout arithmeticalValueInout : int );
		
		setActiveArithmeticalUnitP ( unitNo : int ) {
			locals{}
			arithUnitNoField := unitNo;
		}
		
		step {
			locals{
				int arithmeticalValueLocal;
			}
			choice {
				arithUnitNoField==1 => {getArithmeticalValue1R ( inout arithmeticalValueLocal ); }
				arithUnitNoField==2 => {getArithmeticalValue2R ( inout arithmeticalValueLocal ); }
			}
			result := arithmeticalValueLocal;
		}
	}
	
		
	backupRecoverySystem.s1.senseSourceValueR = instantly backupRecoverySystem.in.getSourceValueP
	backupRecoverySystem.s2.senseSourceValueR = instantly backupRecoverySystem.in.getSourceValueP
	backupRecoverySystem.a1.getSensorValue1R = instantly backupRecoverySystem.s1.getSensorValueP
	backupRecoverySystem.a1.getSensorValue2R = instantly backupRecoverySystem.s2.getSensorValueP
	backupRecoverySystem.a2.getSensorValueR = instantly backupRecoverySystem.s2.getSensorValueP
	backupRecoverySystem.monitor.doSensorValuesMatchR = instantly backupRecoverySystem.a1.doSensorValuesMatchP	
	backupRecoverySystem.monitor.switchArithmeticalUnitR_PartA = instantly backupRecoverySystem.a2.setActiveArithmeticalUnitP
	backupRecoverySystem.monitor.switchArithmeticalUnitR_PartB = instantly backupRecoverySystem.a1.setActiveArithmeticalUnitP
	backupRecoverySystem.monitor.switchArithmeticalUnitR_PartC = instantly backupRecoverySystem.out.setActiveArithmeticalUnitP
	backupRecoverySystem.out.getArithmeticalValue1R = instantly backupRecoverySystem.a1.getArithmeticalValueP
	backupRecoverySystem.out.getArithmeticalValue2R = instantly backupRecoverySystem.a2.getArithmeticalValueP
	
	step {
		locals{}
		step in ;
		step monitor ;
		step out ;
	}
}
