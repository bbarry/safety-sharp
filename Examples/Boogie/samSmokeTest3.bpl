var i: int;

procedure Main() returns ()
	modifies i;
{
	var counter: int;
	
	Initialize: 
		i := 0;
		counter := 0;
		goto LoopHead,End;
	
	LoopHead:
		assume counter < 5;
		call Loop ();
		assert i == 200;
		counter := counter + 1;
		goto LoopHead,End;
	
	End:
		assume counter == 5;
		return;
}

procedure Loop() returns ()
	modifies i;
	ensures i==200;
{	
	Initialize: 
		i := 0;
		goto Node1BlockPart1;
		
	Node1BlockPart1:
		// Case distinction of Node 2
		goto Node2Option1,Node2Option2;	
	
	Node2Option1:
		assume true;		
		i := 200;
		goto Node1BlockPart2;	
	
	Node2Option2:
		assume false;
		goto Node1BlockPart2;	
	
	Node1BlockPart2:
		//merged node 2
		return;	
}