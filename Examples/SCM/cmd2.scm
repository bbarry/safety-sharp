component simple {
	intField : int = 1, 2;
	
	step {
		locals{
		}
		choice {
			intField == 1 => { intField := 10; intField := intField * 3; }
			intField == 2 => { intField := 20; intField := intField * 3; }
		}		
		intField := intField + 1;
	}
}
