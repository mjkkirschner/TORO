MODULE MainModule

	! test01_001

	! variables
	TASK PERS tooldata drill := [FALSE, [[300,-260,280], [0,0,0,1]], [1,[0,0,0.001],[1,0,0,0],0,0,0]];
	TASK PERS wobjdata block := [TRUE, TRUE, "", [[0,0,150], [1,0,0,0]], [[0,0,0], [1,0,0,0]]];
	TASK PERS speeddata rate := [3, 500, 5000, 1000];

	! targets
	VAR jointtarget j0 := [[-90,0,0,90,90,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR jointtarget j1 := [[-41,0,0,0,90,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p00 := [[0,0,100], [0,1,0,0], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p01 := [[0,0,30], [0,1,0,0], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p02 := [[0,0,0], [0,1,0,0], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p10 := [[70.7106781186548,70.7106781186547,0], [0.653281482438188,0.653281482438188,-0.270598050073099,-0.270598050073099], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p11 := [[21.2132034355964,21.2132034355964,0], [0.653281482438188,0.653281482438188,-0.270598050073099,-0.270598050073099], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p12 := [[0,0,0], [0.653281482438188,0.653281482438188,-0.270598050073099,-0.270598050073099], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p20 := [[1.83690953073357E-14,100,1.12478268856806E-30], [0.707106781186548,0.707106781186547,-8.65934606862825E-17,-4.32956578745078E-17], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p21 := [[5.5107285922007E-15,30,3.37434806570419E-31], [0.707106781186548,0.707106781186547,-8.65934606862825E-17,-4.32956578745078E-17], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p22 := [[0,0,0], [0.707106781186548,0.707106781186547,-8.65934606862825E-17,-4.32956578745078E-17], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p30 := [[-70.7106781186547,70.7106781186548,-3.92523114670943E-15], [0.653281482438188,0.653281482438188,0.270598050073098,0.270598050073098], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p31 := [[-21.2132034355964,21.2132034355964,-1.17756934401283E-15], [0.653281482438188,0.653281482438188,0.270598050073098,0.270598050073098], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p32 := [[0,0,0], [0.653281482438188,0.653281482438188,0.270598050073098,0.270598050073098], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p40 := [[-100,0,0], [0.5,0.5,0.5,0.5], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p41 := [[-30,0,0], [0.5,0.5,0.5,0.5], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p42 := [[0,0,0], [0.5,0.5,0.5,0.5], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p50 := [[-70.7106781186548,-70.7106781186548,3.92523114670944E-15], [0.270598050073098,0.270598050073099,0.653281482438188,0.653281482438188], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p51 := [[-21.2132034355964,-21.2132034355964,1.17756934401283E-15], [0.270598050073098,0.270598050073099,0.653281482438188,0.653281482438188], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p52 := [[0,0,0], [0.270598050073098,0.270598050073099,0.653281482438188,0.653281482438188], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p60 := [[-6.12303176911188E-15,-100,3.74927562856021E-31], [7.14979088949037E-22,4.32970878326857E-17,0.707106781186548,0.707106781186547], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p61 := [[-1.83690953073357E-15,-30,1.12478268856806E-31], [7.14979088949037E-22,4.32970878326857E-17,0.707106781186548,0.707106781186547], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p62 := [[0,0,0], [7.14979088949037E-22,4.32970878326857E-17,0.707106781186548,0.707106781186547], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p70 := [[70.7106781186548,-70.7106781186548,3.92523114670944E-15], [0.270598050073099,0.270598050073098,-0.653281482438188,-0.653281482438188], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p71 := [[21.2132034355964,-21.2132034355964,1.17756934401283E-15], [0.270598050073099,0.270598050073098,-0.653281482438188,-0.653281482438188], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p72 := [[0,0,0], [0.270598050073099,0.270598050073098,-0.653281482438188,-0.653281482438188], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p80 := [[100,-1.22460635382238E-14,7.49855125712043E-31], [0.5,0.5,-0.5,-0.5], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p81 := [[30,-3.67381906146713E-15,2.24956537713613E-31], [0.5,0.5,-0.5,-0.5], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p82 := [[0,0,0], [0.5,0.5,-0.5,-0.5], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];

	! drilling instructions
	PROC main()
		ConfL\Off;
		SingArea\Wrist;

		TPWrite("This is: test01_001");
		TPWrite("Check block and drill");
		MoveAbsJ j0, v100, z5, tool0;
		MoveAbsJ j1, v100, z5, tool0;

		TPWrite("Drilling hole 1 of 9!");
		MoveL p00, v100, z5, drill\WObj:=block;
		MoveL p01, v100, z5, drill\WObj:=block;
		MoveL p02, rate, fine, drill\WObj:=block;
		MoveL p01, rate, fine, drill\WObj:=block;
		MoveL p00, v100, z5, drill\WObj:=block;

		TPWrite("Drilling hole 2 of 9!");
		MoveL p10, v100, z5, drill\WObj:=block;
		MoveL p11, v100, z5, drill\WObj:=block;
		MoveL p12, rate, fine, drill\WObj:=block;
		MoveL p11, rate, fine, drill\WObj:=block;
		MoveL p10, v100, z5, drill\WObj:=block;

		TPWrite("Drilling hole 3 of 9!");
		MoveL p20, v100, z5, drill\WObj:=block;
		MoveL p21, v100, z5, drill\WObj:=block;
		MoveL p22, rate, fine, drill\WObj:=block;
		MoveL p21, rate, fine, drill\WObj:=block;
		MoveL p20, v100, z5, drill\WObj:=block;

		TPWrite("Drilling hole 4 of 9!");
		MoveL p30, v100, z5, drill\WObj:=block;
		MoveL p31, v100, z5, drill\WObj:=block;
		MoveL p32, rate, fine, drill\WObj:=block;
		MoveL p31, rate, fine, drill\WObj:=block;
		MoveL p30, v100, z5, drill\WObj:=block;

		TPWrite("Drilling hole 5 of 9!");
		MoveL p40, v100, z5, drill\WObj:=block;
		MoveL p41, v100, z5, drill\WObj:=block;
		MoveL p42, rate, fine, drill\WObj:=block;
		MoveL p41, rate, fine, drill\WObj:=block;
		MoveL p40, v100, z5, drill\WObj:=block;

		TPWrite("Drilling hole 6 of 9!");
		MoveL p50, v100, z5, drill\WObj:=block;
		MoveL p51, v100, z5, drill\WObj:=block;
		MoveL p52, rate, fine, drill\WObj:=block;
		MoveL p51, rate, fine, drill\WObj:=block;
		MoveL p50, v100, z5, drill\WObj:=block;

		TPWrite("Drilling hole 7 of 9!");
		MoveL p60, v100, z5, drill\WObj:=block;
		MoveL p61, v100, z5, drill\WObj:=block;
		MoveL p62, rate, fine, drill\WObj:=block;
		MoveL p61, rate, fine, drill\WObj:=block;
		MoveL p60, v100, z5, drill\WObj:=block;

		TPWrite("Drilling hole 8 of 9!");
		MoveL p70, v100, z5, drill\WObj:=block;
		MoveL p71, v100, z5, drill\WObj:=block;
		MoveL p72, rate, fine, drill\WObj:=block;
		MoveL p71, rate, fine, drill\WObj:=block;
		MoveL p70, v100, z5, drill\WObj:=block;

		TPWrite("Drilling hole 9 of 9!");
		MoveL p80, v100, z5, drill\WObj:=block;
		MoveL p81, v100, z5, drill\WObj:=block;
		MoveL p82, rate, fine, drill\WObj:=block;
		MoveL p81, rate, fine, drill\WObj:=block;
		MoveL p80, v100, z5, drill\WObj:=block;

		TPWrite("Resetting axes...");
		MoveAbsJ j1, v100, z5, tool0;
		MoveAbsJ j0, v100, z5, tool0;

		Stop;
	ENDPROC

ENDMODULE