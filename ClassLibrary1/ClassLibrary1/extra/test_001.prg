MODULE MainModule

	! test_001

	! variables
	TASK PERS tooldata drill := [FALSE, [[230,-50,280], [1,0,0,0]], [1,[0,0,0.001],[1,0,0,0],0,0,0]];
	TASK PERS wobjdata block := [TRUE, TRUE, "", [[0,0,150], [1,0,0,0]], [[0,0,0], [1,0,0,0]]];
	TASK PERS speeddata rate := [3, 500, 5000, 1000];

	! targets
	VAR jointtarget j0 := [[-90,0,0,90,90,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p00 := [[-40.5452036387967,-23.8501197875275,88.2454432138516], [0.790090507707051,-0.0211624985538347,-0.241505553943799,0.563013504012534], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p01 := [[-12.163561091639,-7.15503593625824,26.4736329641555], [0.790090507707051,-0.0211624985538347,-0.241505553943799,0.563013504012534], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p02 := [[0,0,0], [0.790090507707051,-0.0211624985538347,-0.241505553943799,0.563013504012534], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p10 := [[22.5723656872247,-68.4954544991647,69.2738119366552], [0.244010354309697,-0.0195472589188708,0.391470107397082,-0.887033261311509], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p11 := [[6.77170970616741,-20.5486363497494,20.7821435809965], [0.244010354309697,-0.0195472589188708,0.391470107397082,-0.887033261311509], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p12 := [[0,0,0], [0.244010354309697,-0.0195472589188708,0.391470107397082,-0.887033261311509], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p20 := [[22.6455406828919,56.6138517072298,79.2593923901217], [0.945939451396105,-0.303632748608409,0.107285562970487,-0.0386719050868209], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p21 := [[6.79366220486757,16.9841555121689,23.7778177170365], [0.945939451396105,-0.303632748608409,0.107285562970487,-0.0386719050868209], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p22 := [[0,0,0], [0.945939451396105,-0.303632748608409,0.107285562970487,-0.0386719050868209], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p30 := [[-33.2007947037332,2.65606357629865,94.2902569586022], [0.976745076106285,-0.0359068018123801,-0.165104260364764,0.131986139784216], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p31 := [[-9.96023841111995,0.796819072889594,28.2870770875806], [0.976745076106285,-0.0359068018123801,-0.165104260364764,0.131986139784216], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p32 := [[0,0,0], [0.976745076106285,-0.0359068018123801,-0.165104260364764,0.131986139784216], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];

	! drilling instructions
	PROC main()
		ConfL\Off;
		SingArea\Wrist;

		TPWrite("This is: test_001");
		TPWrite("Check block and drill");
		MoveAbsJ j0, v100, z5, tool0;

		TPWrite("Drilling hole 0 of 4!");
		MoveL p00, v200, z5, drill\WObj:=block;
		MoveL p01, v100, z5, drill\WObj:=block;
		MoveL p02, rate, fine, drill\WObj:=block;
		MoveL p01, rate, fine, drill\WObj:=block;
		MoveL p00, v200, z5, drill\WObj:=block;

		TPWrite("Drilling hole 1 of 4!");
		MoveL p10, v200, z5, drill\WObj:=block;
		MoveL p11, v100, z5, drill\WObj:=block;
		MoveL p12, rate, fine, drill\WObj:=block;
		MoveL p11, rate, fine, drill\WObj:=block;
		MoveL p10, v200, z5, drill\WObj:=block;

		TPWrite("Drilling hole 2 of 4!");
		MoveL p20, v200, z5, drill\WObj:=block;
		MoveL p21, v100, z5, drill\WObj:=block;
		MoveL p22, rate, fine, drill\WObj:=block;
		MoveL p21, rate, fine, drill\WObj:=block;
		MoveL p20, v200, z5, drill\WObj:=block;

		TPWrite("Drilling hole 3 of 4!");
		MoveL p30, v200, z5, drill\WObj:=block;
		MoveL p31, v100, z5, drill\WObj:=block;
		MoveL p32, rate, fine, drill\WObj:=block;
		MoveL p31, rate, fine, drill\WObj:=block;
		MoveL p30, v200, z5, drill\WObj:=block;

		TPWrite("Resetting axes...");
		MoveAbsJ j0, v100, z5, tool0;

		Stop;
	ENDPROC

ENDMODULE