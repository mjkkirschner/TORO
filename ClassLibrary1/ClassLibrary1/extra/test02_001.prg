MODULE MainModule

	! test02_001

	! variables
	TASK PERS tooldata drill := [FALSE, [[300,-260,280], [0,0,0,1]], [1,[0,0,0.001],[1,0,0,0],0,0,0]];
	TASK PERS wobjdata block := [TRUE, TRUE, "", [[0,0,150], [1,0,0,0]], [[0,0,0], [1,0,0,0]]];
	TASK PERS speeddata rate := [3, 500, 5000, 1000];

	! targets
	VAR jointtarget j0 := [[-90,0,0,90,90,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR jointtarget j1 := [[-41,0,0,0,90,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p00 := [[1.83690953073357E-14,0,100], [0,1,0,-9.18454765366783E-17], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p01 := [[5.5107285922007E-15,0,30], [0,1,0,-9.18454765366783E-17], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p02 := [[0,0,0], [0,1,0,-9.18454765366783E-17], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p10 := [[17.364817766693,-1.04083408558608E-15,98.4807753012208], [0.00759612349389579,0.992403876506124,-0.0868240888334661,-0.086824088833467], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p11 := [[5.20944533000791,-3.12250225675825E-16,29.5442325903662], [0.00759612349389579,0.992403876506124,-0.0868240888334661,-0.086824088833467], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p12 := [[0,0,0], [0.00759612349389579,0.992403876506124,-0.0868240888334661,-0.086824088833467], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p20 := [[34.2020143325669,-1.38777878078145E-15,93.9692620785908], [0.0301536896070456,0.969846310392959,-0.171010071662836,-0.171010071662835], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p21 := [[10.2606042997701,-4.16333634234434E-16,28.1907786235773], [0.0301536896070456,0.969846310392959,-0.171010071662836,-0.171010071662835], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p22 := [[0,0,0], [0.0301536896070456,0.969846310392959,-0.171010071662836,-0.171010071662835], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p30 := [[50,-5.55111512312578E-15,86.6025403784438], [0.0669872981077809,0.933012701892218,-0.249999999999999,-0.25], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p31 := [[15,-1.66533453693773E-15,25.9807621135332], [0.0669872981077809,0.933012701892218,-0.249999999999999,-0.25], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p32 := [[0,0,0], [0.0669872981077809,0.933012701892218,-0.249999999999999,-0.25], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p40 := [[64.278760968654,5.55111512312578E-15,76.6044443118978], [0.116977778440511,0.883022221559491,-0.32139380484327,-0.32139380484327], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p41 := [[19.2836282905962,1.66533453693773E-15,22.9813332935693], [0.116977778440511,0.883022221559491,-0.32139380484327,-0.32139380484327], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p42 := [[0,0,0], [0.116977778440511,0.883022221559491,-0.32139380484327,-0.32139380484327], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p50 := [[76.6044443118978,8.32667268468867E-15,64.2787609686539], [0.17860619515673,0.82139380484327,-0.383022221559489,-0.383022221559489], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p51 := [[22.9813332935693,2.4980018054066E-15,19.2836282905962], [0.17860619515673,0.82139380484327,-0.383022221559489,-0.383022221559489], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p52 := [[0,0,0], [0.17860619515673,0.82139380484327,-0.383022221559489,-0.383022221559489], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p60 := [[86.6025403784439,1.11022302462516E-14,50], [0.25,0.75,-0.433012701892219,-0.433012701892219], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p61 := [[25.9807621135332,3.33066907387547E-15,15], [0.25,0.75,-0.433012701892219,-0.433012701892219], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p62 := [[0,0,0], [0.25,0.75,-0.433012701892219,-0.433012701892219], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p70 := [[93.9692620785908,5.55111512312578E-15,34.2020143325669], [0.328989928337166,0.671010071662834,-0.469846310392954,-0.469846310392954], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p71 := [[28.1907786235773,1.66533453693773E-15,10.2606042997701], [0.328989928337166,0.671010071662834,-0.469846310392954,-0.469846310392954], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p72 := [[0,0,0], [0.328989928337166,0.671010071662834,-0.469846310392954,-0.469846310392954], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p80 := [[98.4807753012208,3.12250225675825E-15,17.364817766693], [0.413175911166535,0.586824088833465,-0.492403876506104,-0.492403876506104], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p81 := [[29.5442325903662,9.36750677027476E-16,5.20944533000791], [0.413175911166535,0.586824088833465,-0.492403876506104,-0.492403876506104], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p82 := [[0,0,0], [0.413175911166535,0.586824088833465,-0.492403876506104,-0.492403876506104], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p90 := [[100,-7.49855125712043E-31,-1.22460635382238E-14], [0.5,0.5,-0.5,-0.5], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p91 := [[30,-2.24956537713613E-31,-3.67381906146713E-15], [0.5,0.5,-0.5,-0.5], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p92 := [[0,0,0], [0.5,0.5,-0.5,-0.5], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p100 := [[98.4807753012208,-1.38777878078145E-15,-17.364817766693], [0.586824088833465,0.413175911166535,-0.492403876506104,-0.492403876506104], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p101 := [[29.5442325903662,-4.16333634234434E-16,-5.20944533000791], [0.586824088833465,0.413175911166535,-0.492403876506104,-0.492403876506104], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p102 := [[0,0,0], [0.586824088833465,0.413175911166535,-0.492403876506104,-0.492403876506104], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p110 := [[93.9692620785908,-2.77555756156289E-15,-34.2020143325669], [0.671010071662834,0.328989928337166,-0.469846310392954,-0.469846310392954], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p111 := [[28.1907786235772,-8.32667268468867E-16,-10.2606042997701], [0.671010071662834,0.328989928337166,-0.469846310392954,-0.469846310392954], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];
	VAR robtarget p112 := [[0,0,0], [0.671010071662834,0.328989928337166,-0.469846310392954,-0.469846310392954], [0,0,0,0], [9E9,9E9,9E9,9E9,9E9,9E9]];

	! drilling instructions
	PROC main()
		ConfL\Off;
		SingArea\Wrist;

		TPWrite("This is: test02_001");
		TPWrite("Check block and drill");
		MoveAbsJ j0, v100, z5, tool0;
		MoveAbsJ j1, v100, z5, tool0;

		TPWrite("Drilling hole 1 of 12!");
		MoveL p00, v100, z5, drill\WObj:=block;
		MoveL p01, v100, z5, drill\WObj:=block;
		MoveL p02, rate, fine, drill\WObj:=block;
		MoveL p01, rate, fine, drill\WObj:=block;
		MoveL p00, v100, z5, drill\WObj:=block;

		TPWrite("Drilling hole 2 of 12!");
		MoveL p10, v100, z5, drill\WObj:=block;
		MoveL p11, v100, z5, drill\WObj:=block;
		MoveL p12, rate, fine, drill\WObj:=block;
		MoveL p11, rate, fine, drill\WObj:=block;
		MoveL p10, v100, z5, drill\WObj:=block;

		TPWrite("Drilling hole 3 of 12!");
		MoveL p20, v100, z5, drill\WObj:=block;
		MoveL p21, v100, z5, drill\WObj:=block;
		MoveL p22, rate, fine, drill\WObj:=block;
		MoveL p21, rate, fine, drill\WObj:=block;
		MoveL p20, v100, z5, drill\WObj:=block;

		TPWrite("Drilling hole 4 of 12!");
		MoveL p30, v100, z5, drill\WObj:=block;
		MoveL p31, v100, z5, drill\WObj:=block;
		MoveL p32, rate, fine, drill\WObj:=block;
		MoveL p31, rate, fine, drill\WObj:=block;
		MoveL p30, v100, z5, drill\WObj:=block;

		TPWrite("Drilling hole 5 of 12!");
		MoveL p40, v100, z5, drill\WObj:=block;
		MoveL p41, v100, z5, drill\WObj:=block;
		MoveL p42, rate, fine, drill\WObj:=block;
		MoveL p41, rate, fine, drill\WObj:=block;
		MoveL p40, v100, z5, drill\WObj:=block;

		TPWrite("Drilling hole 6 of 12!");
		MoveL p50, v100, z5, drill\WObj:=block;
		MoveL p51, v100, z5, drill\WObj:=block;
		MoveL p52, rate, fine, drill\WObj:=block;
		MoveL p51, rate, fine, drill\WObj:=block;
		MoveL p50, v100, z5, drill\WObj:=block;

		TPWrite("Drilling hole 7 of 12!");
		MoveL p60, v100, z5, drill\WObj:=block;
		MoveL p61, v100, z5, drill\WObj:=block;
		MoveL p62, rate, fine, drill\WObj:=block;
		MoveL p61, rate, fine, drill\WObj:=block;
		MoveL p60, v100, z5, drill\WObj:=block;

		TPWrite("Drilling hole 8 of 12!");
		MoveL p70, v100, z5, drill\WObj:=block;
		MoveL p71, v100, z5, drill\WObj:=block;
		MoveL p72, rate, fine, drill\WObj:=block;
		MoveL p71, rate, fine, drill\WObj:=block;
		MoveL p70, v100, z5, drill\WObj:=block;

		TPWrite("Drilling hole 9 of 12!");
		MoveL p80, v100, z5, drill\WObj:=block;
		MoveL p81, v100, z5, drill\WObj:=block;
		MoveL p82, rate, fine, drill\WObj:=block;
		MoveL p81, rate, fine, drill\WObj:=block;
		MoveL p80, v100, z5, drill\WObj:=block;

		TPWrite("Drilling hole 10 of 12!");
		MoveL p90, v100, z5, drill\WObj:=block;
		MoveL p91, v100, z5, drill\WObj:=block;
		MoveL p92, rate, fine, drill\WObj:=block;
		MoveL p91, rate, fine, drill\WObj:=block;
		MoveL p90, v100, z5, drill\WObj:=block;

		TPWrite("Drilling hole 11 of 12!");
		MoveL p100, v100, z5, drill\WObj:=block;
		MoveL p101, v100, z5, drill\WObj:=block;
		MoveL p102, rate, fine, drill\WObj:=block;
		MoveL p101, rate, fine, drill\WObj:=block;
		MoveL p100, v100, z5, drill\WObj:=block;

		TPWrite("Drilling hole 12 of 12!");
		MoveL p110, v100, z5, drill\WObj:=block;
		MoveL p111, v100, z5, drill\WObj:=block;
		MoveL p112, rate, fine, drill\WObj:=block;
		MoveL p111, rate, fine, drill\WObj:=block;
		MoveL p110, v100, z5, drill\WObj:=block;

		TPWrite("Resetting axes...");
		MoveAbsJ j1, v100, z5, tool0;
		MoveAbsJ j0, v100, z5, tool0;

		Stop;
	ENDPROC

ENDMODULE