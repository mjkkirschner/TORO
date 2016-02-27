MODULE MainModule

	! test00_001

	! variables
	TASK PERS tooldata drill := [FALSE, [[300,-260,280], [0,0,0,1]], [1,[0,0,0.001],[1,0,0,0],0,0,0]];
	TASK PERS wobjdata block := [TRUE, TRUE, "", [[0,0,150], [1,0,0,0]], [[0,0,0], [1,0,0,0]]];
	TASK PERS speeddata rate := [3, 500, 5000, 1000];

	! targets
	VAR jointtarget j0 := [[-90,0,0,90,90,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR jointtarget j1 := [[-41,0,0,0,90,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p00 := [[22.6455406828919,56.6138517072298,79.2593923901217], [0.303632748608409,0.945939451396106,-0.0386719050868208,-0.107285562970487], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p01 := [[6.79366220486757,16.9841555121689,23.7778177170365], [0.303632748608409,0.945939451396106,-0.0386719050868208,-0.107285562970487], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p02 := [[0,0,0], [0.303632748608409,0.945939451396106,-0.0386719050868208,-0.107285562970487], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p10 := [[-33.2007947037332,2.65606357629865,94.2902569586022], [0.0359068018123806,0.976745076106272,0.131986139784214,0.165104260364762], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p11 := [[-9.96023841111995,0.796819072889596,28.2870770875807], [0.0359068018123806,0.976745076106272,0.131986139784214,0.165104260364762], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p12 := [[0,0,0], [0.0359068018123806,0.976745076106272,0.131986139784214,0.165104260364762], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p20 := [[-40.5452036387967,-23.8501197875275,88.2454432138516], [0.0211624985538339,0.790090507707084,0.563013504012557,0.241505553943809], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p21 := [[-12.163561091639,-7.15503593625823,26.4736329641555], [0.0211624985538339,0.790090507707084,0.563013504012557,0.241505553943809], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p22 := [[0,0,0], [0.0211624985538339,0.790090507707084,0.563013504012557,0.241505553943809], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p30 := [[22.5723656872247,-68.4954544991647,69.2738119366552], [0.0195472589188699,0.244010354309708,-0.887033261311551,-0.391470107397102], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p31 := [[6.77170970616741,-20.5486363497494,20.7821435809965], [0.0195472589188699,0.244010354309708,-0.887033261311551,-0.391470107397102], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p32 := [[0,0,0], [0.0195472589188699,0.244010354309708,-0.887033261311551,-0.391470107397102], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];

	! drilling instructions
	PROC main()
		ConfL\Off;
		SingArea\Wrist;

		TPWrite("This is: test00_001");
		TPWrite("Check block and drill");
		MoveAbsJ j0, v100, z5, tool0;
		MoveAbsJ j1, v100, z5, tool0;

		TPWrite("Drilling hole 1 of 4!");
		MoveL p00, v100, z5, drill\WObj:=block;
		MoveL p01, v100, z5, drill\WObj:=block;
		MoveL p02, rate, fine, drill\WObj:=block;
		MoveL p01, rate, fine, drill\WObj:=block;
		MoveL p00, v100, z5, drill\WObj:=block;

		TPWrite("Drilling hole 2 of 4!");
		MoveL p10, v100, z5, drill\WObj:=block;
		MoveL p11, v100, z5, drill\WObj:=block;
		MoveL p12, rate, fine, drill\WObj:=block;
		MoveL p11, rate, fine, drill\WObj:=block;
		MoveL p10, v100, z5, drill\WObj:=block;

		TPWrite("Drilling hole 3 of 4!");
		MoveL p20, v100, z5, drill\WObj:=block;
		MoveL p21, v100, z5, drill\WObj:=block;
		MoveL p22, rate, fine, drill\WObj:=block;
		MoveL p21, rate, fine, drill\WObj:=block;
		MoveL p20, v100, z5, drill\WObj:=block;

		TPWrite("Drilling hole 4 of 4!");
		MoveL p30, v100, z5, drill\WObj:=block;
		MoveL p31, v100, z5, drill\WObj:=block;
		MoveL p32, rate, fine, drill\WObj:=block;
		MoveL p31, rate, fine, drill\WObj:=block;
		MoveL p30, v100, z5, drill\WObj:=block;

		TPWrite("Resetting axes...");
		MoveAbsJ j1, v100, z5, tool0;
		MoveAbsJ j0, v100, z5, tool0;

		Stop;
	ENDPROC

ENDMODULE