﻿component simple {
	intField : int = 1;
	
	rport1 ( inout r_inout  : int );
	
	pport1 ( inout p_inout  : int )
		ensures p_inout==5
	{
		locals{
		}
		p_inout := 5;
	}
	
	simple.rport1 = instantly simple.pport1 
	
	step {
		locals{
		}
		rport1 (inout intField);
	}
}
