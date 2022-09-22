using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Board : MonoBehaviour
{
    public Player player;
    public Player bot;
    Player currentPlayer;
    
    public GameObject drawButton;
    public GameObject menuButton;
    public GameObject Gate;
    public GameObject Switch;
    public GameObject winScreen;
    public GameObject loseScreen;

    Gate[] gates;
    Switch[] switchList;

    int[] firstPlayerOrder = { 8, 0, 6, 9, 2, 3, 5, 4, 7, 1 };
    public int[][] goalArray = { new int[] { 1, 1, 1, 0, 1, 1, 1 },
                                new int[] { 1, 0, 0, 0, 0, 0, 1 },
                                new int[] { 1, 1, 0, 1, 1, 1, 0 },
                                new int[] { 1, 1, 0, 1, 0, 1, 1 },
                                new int[] { 1, 0, 1, 1, 0, 0, 1 },
                                new int[] { 0, 1, 1, 1, 0, 1, 1 },
                                new int[] { 0, 1, 1, 1, 1, 1, 1 },
                                new int[] { 1, 1, 0, 0, 0, 0, 1 },
                                new int[] { 1, 1, 1, 1, 1, 1, 1 },
                                new int[] { 1, 1, 1, 1, 0, 1, 1 } };

    bool isGameOver = true;
    public int lastMove;
    int currentOutput = -1;

    public int[,] outputState = new int[4, 7];
    public LogicGate[,] gameState = new LogicGate[3, 7];

    public event Action OnReset;
    public event Action OnUpdate;

    // Start is called before the first frame update
    void Start()
    {
        switchList = Switch.GetComponentsInChildren<Switch>();
        gates = Gate.GetComponentsInChildren<Gate>();
        GameReset();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GameReset(){
        if(OnReset != null){
            OnReset();
        }

        // Set buttons to active
        drawButton.SetActive(true);
        menuButton.SetActive(true);
        
        ClearBoard();
        ResetOutput();
        updateBoard(0);

        player.Reset();
        bot.Reset();

        SetPlayersGoal();
        isGameOver = false;

        // Find first player to start

        if (Array.IndexOf(firstPlayerOrder, player.GetGoal()) < Array.IndexOf(firstPlayerOrder, bot.GetGoal())){
            currentPlayer = player;
            player.StartTurn();
        } else {
            currentPlayer = bot;
            bot.StartTurn();
        }
    }

    void SetPlayersGoal(){
        int[] goal = {-1,-1};
        while(goal[0] == goal[1]){
            goal[0] = UnityEngine.Random.Range(0,10);
            goal[1] = UnityEngine.Random.Range(0,10);
        }

        player.SetGoal(goal[0]);
        bot.SetGoal(goal[1]);

        player.SetGoalArray(goalArray[goal[0]]);
        bot.SetGoalArray(goalArray[goal[1]]);
    }

    void ClearBoard(){
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                gameState[i, j] = new Wire(outputState[i, j]);
            }
        }
    }

    void ResetOutput(){
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                outputState[i, j] = 0;
            }
        }
    }

    public void ChangeTurn(){
        if (currentPlayer == player){
            currentPlayer = bot;
            bot.StartTurn();
        } else {
            currentPlayer = player;
            player.StartTurn();
        }
    }

    public bool ToggleSwitch(int row)
    {
        if ((lastMove%4 == 0) && (row == lastMove/4))
        {
            Debug.Log("Illegal Action");
            return false;
        }
        if (outputState[0, row] == 0) outputState[0, row] = 1;
        else outputState[0, row] = 0;
        switchList[row].toggle();
        updateBoard(0);
        lastMove = row * 4;
        return true;
    }

    public bool ReplaceGate(string gate, int row, int column, int input2 = -1)
    {
        print("Action" + "row " + row + " column " + column + "input2 " + input2 + "recent: " + lastMove);
        if (row < 0 || column < 0 || column * 4 + row + 1 == lastMove || column == input2)
        {
            print("Illegal Action" + "row " + row + " column " + column + "input2 " + input2 + "recent: " + lastMove);
            return false;
        }
        else
        {
            switch (gate)
            {
                case "Wire":
                    gameState[row, column] = new Wire(outputState[row, column]);
                    break;
                case "NOT":
                    gameState[row, column] = new NOTGate(outputState[row, column]);
                    break;
                case "AND":
                    gameState[row, column] = new ANDGate(outputState[row, column], outputState[row, input2], input2);
                    break;
                case "OR":
                    gameState[row, column] = new ORGate(outputState[row, column], outputState[row, input2], input2);
                    break;
                case "NAND":
                    gameState[row, column] = new NANDGate(outputState[row, column], outputState[row, input2], input2);
                    break;
                case "NOR":
                    gameState[row, column] = new NORGate(outputState[row, column], outputState[row, input2], input2);
                    break;
                case "XOR":
                    gameState[row, column] = new XORGate(outputState[row, column], outputState[row, input2], input2);
                    break;
                case "XNOR":
                    gameState[row, column] = new XNORGate(outputState[row, column], outputState[row, input2], input2);
                    break;
            }
        }
        lastMove = column * 4 + row + 1;
        updateBoard(row);
        gates[column * 3 + row].setLatestMove();
        return true;
    }

    public bool updateBoard(int layer)
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if(i<layer){
                    if(gates[j * 3 + i].isLatest) gates[j * 3 + i].unSetLatestMove();
                }
                else{
                    if (gameState[i, j] is Wire)
                    {
                        gameState[i, j] = new Wire(outputState[i, j]);
                        gates[j * 3 + i].setGate("Wire", gameState[i, j].getInput1());
                    }
                    else if (gameState[i, j] is ANDGate)
                    {
                        gameState[i, j] = new ANDGate(outputState[i, j], outputState[i, gameState[i, j].getInput2Row()], gameState[i, j].getInput2Row());
                        gates[j * 3 + i].setGate("AND", gameState[i, j].getInput1(), gameState[i, j].getInput2Row(), gameState[i, j].getInput2());
                    }
                    else if (gameState[i, j] is ORGate)
                    {
                        gameState[i, j] = new ORGate(outputState[i, j], outputState[i, gameState[i, j].getInput2Row()], gameState[i, j].getInput2Row());
                        gates[j * 3 + i].setGate("OR", gameState[i, j].getInput1(), gameState[i, j].getInput2Row(), gameState[i, j].getInput2());
                    }
                    else if (gameState[i, j] is NOTGate)
                    {
                        gameState[i, j] = new NOTGate(outputState[i, j]);
                        gates[j * 3 + i].setGate("NOT", gameState[i, j].getInput1());
                    }
                    else if (gameState[i, j] is NANDGate)
                    {
                        gameState[i, j] = new NANDGate(outputState[i, j], outputState[i, gameState[i, j].getInput2Row()], gameState[i, j].getInput2Row());
                        gates[j * 3 + i].setGate("NAND", gameState[i, j].getInput1(), gameState[i, j].getInput2Row(), gameState[i, j].getInput2());
                    }
                    else if (gameState[i, j] is NORGate)
                    {
                        gameState[i, j] = new NORGate(outputState[i, j], outputState[i, gameState[i, j].getInput2Row()], gameState[i, j].getInput2Row());
                        gates[j * 3 + i].setGate("NOR", gameState[i, j].getInput1(), gameState[i, j].getInput2Row(), gameState[i, j].getInput2());
                    }
                    else if (gameState[i, j] is XORGate)
                    {
                        gameState[i, j] = new XORGate(outputState[i, j], outputState[i, gameState[i, j].getInput2Row()], gameState[i, j].getInput2Row());
                        gates[j * 3 + i].setGate("XOR", gameState[i, j].getInput1(), gameState[i, j].getInput2Row(), gameState[i, j].getInput2());
                    }
                    else if (gameState[i, j] is XNORGate)
                    {
                        gameState[i, j] = new XNORGate(outputState[i, j], outputState[i, gameState[i, j].getInput2Row()], gameState[i, j].getInput2Row());
                        gates[j * 3 + i].setGate("XNOR", gameState[i, j].getInput1(), gameState[i, j].getInput2Row(), gameState[i, j].getInput2());
                    }
                    outputState[i + 1, j] = gameState[i, j].getOutput();
                }
            }
        }
        updateSwitchUI();

        if(OnUpdate != null){
            OnUpdate();
        }

        checkOutput();
        return true;
    }

    void updateSwitchUI(){
        for (int i = 0; i<switchList.Length; i++){
            switchList[i].unSetLatestMove();
        }
    }

    public bool checkOutput()
    {
        int[] output = { outputState[3, 0], outputState[3, 1], outputState[3, 2], outputState[3, 3], outputState[3, 4], outputState[3, 5], outputState[3, 6] };
        if (isArrayEqual(output, goalArray[0])) currentOutput = 0;
        else if (isArrayEqual(output, goalArray[1])) currentOutput = 1;
        else if (isArrayEqual(output, goalArray[2])) currentOutput = 2;
        else if (isArrayEqual(output, goalArray[3])) currentOutput = 3;
        else if (isArrayEqual(output, goalArray[4])) currentOutput = 4;
        else if (isArrayEqual(output, goalArray[5])) currentOutput = 5;
        else if (isArrayEqual(output, goalArray[6])) currentOutput = 6;
        else if (isArrayEqual(output, goalArray[7])) currentOutput = 7;
        else if (isArrayEqual(output, goalArray[8])) currentOutput = 8;
        else if (isArrayEqual(output, goalArray[9])) currentOutput = 9;
        else currentOutput = -1;

        if (player != null && currentOutput == player.GetGoal())
        {
            Debug.Log("Player wins");
            gameOver();
            // winScreen.SetActive(true);
        }
        else if (bot != null && currentOutput == bot.GetGoal())
        {
            Debug.Log("Bot wins");
            gameOver();
            // loseScreen.SetActive(true);
        }
        return true;
    }

    public bool isArrayEqual(int[] a, int[] b)
    {
        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] != b[i]) return false;
        }
        return true;
    }

    void gameOver(){
        drawButton.SetActive(false);
    }

    public int[] getOutput()
    {
        int[] output = { outputState[3, 0], outputState[3, 1], outputState[3, 2], outputState[3, 3], outputState[3, 4], outputState[3, 5], outputState[3, 6] };
        return output;
    }
}
