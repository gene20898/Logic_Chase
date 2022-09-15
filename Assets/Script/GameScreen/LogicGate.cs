using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class LogicGate
{
    int input1, input2, input2Row;
    int output;

    public LogicGate(int input1)
    {
        this.input1 = input1;
    }

    public LogicGate(int input1, int input2, int input2Row)
    {
        this.input1 = input1;
        this.input2 = input2;
        this.input2Row = input2Row;
    }

    public int getInput1()
    {
        return input1;
    }
    public int getInput2()
    {
        return input2;
    }

    public int getOutput()
    {
        return output;
    }
    
    public bool setOutput(int output)
    {
        this.output = output;
        return true;
    }

    public int getInput2Row() { return input2Row; }

    public virtual string getName()
    {
        return "";
    }
}

class ANDGate : LogicGate
{

    public ANDGate(int input1, int input2, int input2Row) : base(input1, input2, input2Row)
    {
        calc_output();
    }

    public bool calc_output()
    {
        setOutput(getInput2() * getInput1());
        return true;
    }

    override public string getName()
    {
        return "AND";
    }

}

class ORGate : LogicGate
{

    public ORGate(int input1, int input2, int input2Row) : base(input1, input2, input2Row)
    {
        calc_output();
    }

    public bool calc_output()
    {
        int output = getInput2() + getInput1();
        if (output > 1) output = 1;
        setOutput(output);
        return true;
    }
    override public string getName()
    {
        return "OR";
    }
}

class NOTGate : LogicGate
{
    public NOTGate(int input1) : base(input1)
    {
        calc_output();
    }

    public bool calc_output()
    {
        if (getInput1() == 1) setOutput(0);
        else setOutput(1);
        return true;
    }
    override public string getName()
    {
        return "NOT";
    }
}

class NANDGate : LogicGate
{

    public NANDGate(int input1, int input2, int input2Row) : base(input1, input2, input2Row)
    {
        calc_output();
    }

    public bool calc_output()
    {
        int output = getInput2() * getInput1();
        if (output == 1) setOutput(0);
        else setOutput(1);
        return true;
    }
    override public string getName()
    {
        return "NAND";
    }
}

class NORGate : LogicGate
{
    public NORGate(int input1, int input2, int input2Row) : base(input1, input2, input2Row)
    {
        calc_output();
    }

    public bool calc_output()
    {
        int output = getInput2() + getInput1();
        if (output == 0) setOutput(1);
        else setOutput(0);
        return true;
    }
    override public string getName()
    {
        return "NOR";
    }
}

class XORGate : LogicGate
{
    public XORGate(int input1, int input2, int input2Row) : base(input1, input2, input2Row)
    {
        calc_output();
    }

    public bool calc_output()
    {
        if (getInput2() != getInput1()) setOutput(1);
        else setOutput(0);
        return true;
    }
    override public string getName()
    {
        return "XOR";
    }
}

class XNORGate : LogicGate
{
    public XNORGate(int input1, int input2, int input2Row) : base(input1, input2, input2Row)
    {
        calc_output();
    }

    public bool calc_output()
    {
        if (getInput2() == getInput1()) setOutput(1);
        else setOutput(0);
        return true;
    }
    override public string getName()
    {
        return "XNOR";
    }
}

class Wire : LogicGate
{
    public Wire(int input1) : base(input1)
    {
        calc_output();
    }

    public bool calc_output()
    {
        setOutput(getInput1());
        return true;
    }
    override public string getName()
    {
        return "Wire";
    }
}