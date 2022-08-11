using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.MLAgents;
using UnityEngine.SceneManagement;
using System.IO;

public class BoardScript : MonoBehaviour
{
    public int[,] outputState = new int[4, 7];
    public LogicGate[,] gameState = new LogicGate[3, 7];
    public GameObject Boxes;
    Box[] boxes;
    int[] previousOutput = new int[7];
    int currentOutput = -1;
    string recentMove = "";

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
    int[] firstPlayerOrder = { 8, 0, 6, 9, 2, 3, 5, 4, 7, 1 };
    PlayerScript[] playerList = new PlayerScript[2];

    int currentPlayer = 0;
    Card selectedCard = null;
    Box selectedBox = null;

    public GameObject drawButton;
    public GameObject ruleButton;
    public GameObject exitButton;
    public GameObject switches;

    public event Action OnChange;
    public event Action OnReset;
    public event Action OnGameOver;

    bool isGameOver = false;
    public int lastMove;

    int goalIterate = 0;
    String[] randomGoalSet;

    int turn = 0;
    public int max_turn = 300;
    public int min_turn = 0;

    float player1Re = 0;
    float player2Re = 0;
    int player1Win = 0;
    int player2Win = 0;

    int winner = 0;

    public GameObject winScreen;
    public GameObject loseScreen;
    void Start()
    {
        playerList[0] = transform.parent.Find("Player1").GetComponent<PlayerScript>();
        playerList[1] = transform.parent.Find("Player2").GetComponent<PlayerScript>();
        boxes = Boxes.GetComponentsInChildren<Box>();
        gameReset();
    }
    void Awake()
    {
    }
    void Update()
    {

    }

    public LogicGate[,] getGameState() { return gameState; }

    public bool setGameState(LogicGate[,] gameState)
    {
        this.gameState = gameState;
        updateBoard(0);
        return true;
    }

    void changeTurn()
    {
        selectedCard = null;
        selectedBox = null;

        if (OnChange != null)
        {
            OnChange();
        }
        playerList[currentPlayer].endTurn();
        if (currentPlayer == 1)
        {
            float reward = calcScore3(previousOutput, getOutput(), playerList[1].getGoalArray(), playerList[0].getGoalArray(), turn, max_turn, min_turn);
            // float reward = calcScore2(previousOutput, getOutput(), playerList[1].getGoalArray(), playerList[0].getGoalArray());
            player1Re += reward;
            // print("Player2 Reward: " + reward + " Total: " + player1Re);
            playerList[1].GiveReward(reward);
            playerList[0].startTurn();
            currentPlayer = 0;
        }
        else
        {
            float reward = calcScore3(previousOutput, getOutput(), playerList[0].getGoalArray(), playerList[1].getGoalArray(), turn, max_turn, min_turn);
            // float reward = calcScore2(previousOutput, getOutput(), playerList[0].getGoalArray(), playerList[1].getGoalArray());
            player2Re += reward;
            // print("Player1 Reward: " + reward + " Total: " + player2Re);
            playerList[0].GiveReward(reward);
            playerList[1].startTurn();
            currentPlayer = 1;
        }
        turn++;
        // if (turn >= max_turn)
        // {
        //     playerList[0].GiveReward(0);
        //     playerList[1].GiveReward(0);
        //     playerList[0].endEpisode();
        //     playerList[1].endEpisode();
        //     goalIterate--;
        //     gameReset();
        // }
    }

    public void onClickDrawButton()
    {
        if (!isGameOver)
        {
            selectedCard = null;
            selectedBox = null;

            bool isDrawed = playerList[currentPlayer].drawCard(2);
            if (isDrawed)
            {
                previousOutput = getOutput();
                changeTurn();
            }
        }
    }

    public void onClickCard(Card card)
    {
        if (!isGameOver)
        {
            selectedBox = null;
            selectedCard = card;
        }
    }

    public void onClickBox(Box box)
    {
        if (!isGameOver)
        {
            int index = box.transform.GetSiblingIndex();
            int row = index % 3;
            int col = index / 3;

            if (selectedCard != null)
            {
                string cardType = selectedCard.getType();
                int cardIndex = selectedCard.getIndex();
                if (cardType != "Switch")
                {
                    if (selectedBox == null)
                    {
                        if (cardType == "Wire" || cardType == "NOT")
                        {
                            bool result = replaceGate(cardType, row, col);
                            if (result == true)
                            {
                                playerList[currentPlayer].useCard(cardIndex);
                                if (cardType == "Wire")
                                {
                                    // box.clearSlot();
                                }
                                else
                                {
                                    // box.setBox(cardType, gameState[row, col].getInput1());
                                }
                                selectedCard = null;
                                changeTurn();
                            }
                        }
                        else
                        {
                            selectedBox = box;
                        }
                    }

                }
            }
        }
    }

    public void onClickLabel(Label label)
    {
        if (selectedBox != null && selectedCard != null && !isGameOver)
        {
            int input = label.index;
            string cardType = selectedCard.getType();
            int cardIndex = selectedCard.getIndex();
            int index = selectedBox.transform.GetSiblingIndex();
            int row = index % 3;
            int col = index / 3;
            bool result = replaceGate(cardType, row, col, input);
            if (result == true)
            {
                playerList[currentPlayer].useCard(cardIndex);

                // selectedBox.setBox(cardType, gameState[row, col].getInput1(), label.index, gameState[row, col].getInput2());
                selectedCard = null;
                selectedBox = null;
                changeTurn();
            }
        }
    }

    public void onClickSwitch(Switch switchButton)
    {
        if (selectedCard != null && !isGameOver)
        {
            string cardType = selectedCard.getType();
            int cardIndex = selectedCard.getIndex();

            if (cardType == "Switch")
            {
                bool result = toggleSwitch(switchButton.index);
                if (result == true)
                {
                    playerList[currentPlayer].useCard(cardIndex);
                    switchButton.toggle();
                    selectedCard = null;
                    changeTurn();
                }
            }
        }
    }

    public bool toggleSwitch(int position)
    {
        if (String.Equals(position.ToString(), recentMove))
        {
            print("Illegal Action");
            return false;
        }
        if (outputState[0, position] == 0) outputState[0, position] = 1;
        else outputState[0, position] = 0;
        updateBoard(0);
        recentMove = position.ToString();
        lastMove = position * 4;
        return true;
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

        if (playerList[0] != null && currentOutput == playerList[0].getGoal())
        {
            player1Win++;
            winner = 0;
            // print("Player 1 wins: " + player1Win + " Player 2 wins: " + player2Win);

            float reward = 0;
            if (turn < max_turn)
            {
                reward = 4 * (1 - (float)turn / max_turn);
            }
            playerList[0].GiveReward(1 + reward);
            playerList[1].GiveReward(-5);
            playerList[0].endEpisode();
            playerList[1].endEpisode();

            gameOver();
            winScreen.SetActive(true);
        }
        else if (playerList[1] != null && currentOutput == playerList[1].getGoal())
        {
            player2Win++;
            winner = 1;
            // print("Player 1 wins: " + player1Win + " Player 2 wins: " + player2Win);

            float reward = 0;
            if (turn < max_turn)
            {
                reward = 4 * (1 - (float)turn / max_turn);
            }
            playerList[1].GiveReward(1 + reward);
            playerList[0].GiveReward(-5);
            playerList[1].endEpisode();
            playerList[0].endEpisode();

            gameOver();
            loseScreen.SetActive(true);
        }
        return true;
    }

    public int[] getOutput()
    {
        int[] output = { outputState[3, 0], outputState[3, 1], outputState[3, 2], outputState[3, 3], outputState[3, 4], outputState[3, 5], outputState[3, 6] };
        return output;
    }

    public bool isArrayEqual(int[] a, int[] b)
    {
        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] != b[i]) return false;
        }
        return true;
    }

    public bool updateBoard(int layer)
    {
        previousOutput = getOutput();
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if(i<layer){
                    if(boxes[j * 3 + i].isLatest) boxes[j * 3 + i].unSetLatestMove();
                }
                else{
                    if (gameState[i, j] is Wire)
                    {
                        gameState[i, j] = new Wire(outputState[i, j]);
                        boxes[j * 3 + i].setBox("Wire", gameState[i, j].getInput1());
                    }
                    else if (gameState[i, j] is ANDGate)
                    {
                        gameState[i, j] = new ANDGate(outputState[i, j], outputState[i, gameState[i, j].getInput2Row()], gameState[i, j].getInput2Row());
                        boxes[j * 3 + i].setBox("AND", gameState[i, j].getInput1(), gameState[i, j].getInput2Row(), gameState[i, j].getInput2());
                    }
                    else if (gameState[i, j] is ORGate)
                    {
                        gameState[i, j] = new ORGate(outputState[i, j], outputState[i, gameState[i, j].getInput2Row()], gameState[i, j].getInput2Row());
                        boxes[j * 3 + i].setBox("OR", gameState[i, j].getInput1(), gameState[i, j].getInput2Row(), gameState[i, j].getInput2());
                    }
                    else if (gameState[i, j] is NOTGate)
                    {
                        gameState[i, j] = new NOTGate(outputState[i, j]);
                        boxes[j * 3 + i].setBox("NOT", gameState[i, j].getInput1());
                    }
                    else if (gameState[i, j] is NANDGate)
                    {
                        gameState[i, j] = new NANDGate(outputState[i, j], outputState[i, gameState[i, j].getInput2Row()], gameState[i, j].getInput2Row());
                        boxes[j * 3 + i].setBox("NAND", gameState[i, j].getInput1(), gameState[i, j].getInput2Row(), gameState[i, j].getInput2());
                    }
                    else if (gameState[i, j] is NORGate)
                    {
                        gameState[i, j] = new NORGate(outputState[i, j], outputState[i, gameState[i, j].getInput2Row()], gameState[i, j].getInput2Row());
                        boxes[j * 3 + i].setBox("NOR", gameState[i, j].getInput1(), gameState[i, j].getInput2Row(), gameState[i, j].getInput2());
                    }
                    else if (gameState[i, j] is XORGate)
                    {
                        gameState[i, j] = new XORGate(outputState[i, j], outputState[i, gameState[i, j].getInput2Row()], gameState[i, j].getInput2Row());
                        boxes[j * 3 + i].setBox("XOR", gameState[i, j].getInput1(), gameState[i, j].getInput2Row(), gameState[i, j].getInput2());
                    }
                    else if (gameState[i, j] is XNORGate)
                    {
                        gameState[i, j] = new XNORGate(outputState[i, j], outputState[i, gameState[i, j].getInput2Row()], gameState[i, j].getInput2Row());
                        boxes[j * 3 + i].setBox("XNOR", gameState[i, j].getInput1(), gameState[i, j].getInput2Row(), gameState[i, j].getInput2());
                    }
                    outputState[i + 1, j] = gameState[i, j].getOutput();
                }
            }
        }
        updateSwitchUI();
        checkOutput();
        return true;
    }

    void updateSwitchUI(){
        Switch[] switchList = switches.GetComponentsInChildren<Switch>();
        for (int i = 0; i<switchList.Length; i++){
            switchList[i].unSetLatestMove();
        }
    }

    public bool allWires()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if (!(gameState[i, j] is Wire)) return false;
            }
        }
        return true;
    }

    public bool replaceGate(string gate, int row, int column, int input2 = -1)
    {
        if (row < 0 || column < 0 || String.Equals(String.Concat(row.ToString(), column.ToString()), recentMove) || column == input2)
        {
            print("Illegal Action" + "row " + row + " column " + column + "input2 " + input2 + "recent: " + recentMove);
            return false;
        }
        else
        {
            switch (gate)
            {
                case "Wire":
                    // if (gameState[row, column] is Wire)
                    // {
                    //     print("Illegal Action Wire");
                    //     return false;
                    // }
                    // else
                    // {
                    //     gameState[row, column] = new Wire(outputState[row, column]);
                    // }
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
        recentMove = String.Concat(row.ToString(), column.ToString());
        lastMove = column * 4 + row + 1;
        updateBoard(row);
        boxes[column * 3 + row].setLatestMove();
        return true;
    }

    public bool clearBoard()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                gameState[i, j] = new Wire(outputState[i, j]);
            }
        }
        return true;
    }

    public bool resetOutput()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                outputState[i, j] = 0;
            }
        }
        return true;
    }

    void gameReset()
    {
        winScreen.SetActive(false);
        loseScreen.SetActive(false);

        drawButton.SetActive(true);
        ruleButton.SetActive(true);
        exitButton.SetActive(true);

        player1Re = 0;
        player2Re = 0;
        turn = 0;


        currentOutput = -1;
        recentMove = "";
        lastMove = -1;
        for (int i = 0; i < 7; i++)
        {
            previousOutput[i] = 0;
        }

        clearBoard();
        resetOutput();

        int bot_goal = -1, player_goal = -1;

        if (goalIterate >= 90)
        {
            goalIterate = 0;
        }

        if (goalIterate == 0)
        {
            randomGoalSet = shuffleArray();
        }

        int[] goals = convertGamesToInt(randomGoalSet[goalIterate]);
        goalIterate++;

        player_goal = goals[0];
        bot_goal = goals[1];

        playerList[0].setGoal(player_goal);
        playerList[1].setGoal(bot_goal);

        playerList[0].setGoalArray(goalArray[player_goal]);
        playerList[1].setGoalArray(goalArray[bot_goal]);

        playerList[0].resetHand();
        playerList[1].resetHand();

        if (Array.IndexOf(firstPlayerOrder, player_goal) < Array.IndexOf(firstPlayerOrder, bot_goal))
        {
            currentPlayer = 0;
            playerList[0].startTurn();
            playerList[1].endTurn();

        }
        else
        {
            currentPlayer = 1;
            playerList[1].startTurn();
            playerList[0].endTurn();

        }

        if (OnReset != null)
        {
            OnReset();
        }

        isGameOver = false;
    }

    public float calcScore(int[] previousState, int[] currentState, int[] yourGoal, int[] opponentGoal)
    {
        float score = 0, reachGoalFactor = 1, stopOpponentFactor = 1;
        int[] differences = compareArray(previousState, currentState);
        int[] goalDifference = compareArray(yourGoal, opponentGoal);
        int[] currentYourGoalSimilar = compareArray(currentState, yourGoal);
        int[] currentOpponentGoalSimilar = compareArray(currentState, opponentGoal);
        int[] previousYourGoalSimilar = compareArray(previousState, yourGoal);
        int[] previousOpponentGoalsSimilar = compareArray(previousState, opponentGoal);

        if (!isArrayEqual(previousState, currentState))
        {
            //Similar(+) - Difference(-) to your goal than previous 7-seg
            //play to reach your goal --> more matters when you are close to reach the goal
            reachGoalFactor = countZeros(previousYourGoalSimilar) / (countZeros(goalDifference) + 2);
            score += (countZeros(currentYourGoalSimilar) - countZeros(previousYourGoalSimilar)) * reachGoalFactor;

            //Similar(-) - Difference(+) to opponent's goal than previous 7-seg
            //play to stop opponent's goal --> more matters when opponenet is close to reach the goal
            stopOpponentFactor = countZeros(previousOpponentGoalsSimilar) / (countZeros(goalDifference) + 2);
            score += (countZeros(previousOpponentGoalsSimilar) - countZeros(currentOpponentGoalSimilar)) * stopOpponentFactor;
        }
        return score / 10;
    }

    public float calcScore2(int[] previousState, int[] currentState, int[] yourGoal, int[] opponentGoal)
    {
        float score = 0, reachGoalFactor = 1, stopOpponentFactor = 1;
        int[] differences = compareArray(previousState, currentState);
        int[] goalDifference = compareArray(yourGoal, opponentGoal);
        int[] currentYourGoalSimilar = compareArray(currentState, yourGoal);
        int[] currentOpponentGoalSimilar = compareArray(currentState, opponentGoal);
        int[] previousYourGoalSimilar = compareArray(previousState, yourGoal);
        int[] previousOpponentGoalsSimilar = compareArray(previousState, opponentGoal);

        //Similar(+) - Difference(-) to your goal than previous 7-seg
        //play to reach your goal --> more matters when you are close to reach the goal
        //similar goal focus on reach the goal
        reachGoalFactor = countZeros(previousYourGoalSimilar) * (countZeros(goalDifference));
        score += (countZeros(currentYourGoalSimilar) - countZeros(previousYourGoalSimilar)) * reachGoalFactor;

        //Similar(-) - Difference(+) to opponent's goal than previous 7-seg
        //play to stop opponent's goal --> more matters when opponenet is close to reach the goal
        //different goal focus on stop opponent
        stopOpponentFactor = countZeros(previousOpponentGoalsSimilar) * (7 - countZeros(goalDifference));
        score += (countZeros(previousOpponentGoalsSimilar) - countZeros(currentOpponentGoalSimilar)) * stopOpponentFactor;

        if (score > 0) return (float)Math.Log(1 + score) / 60;
        // if (score < 0) return (float)-Math.Log(1 + Math.Abs(score)) / 6;

        return 0;
    }

    public float calcScore3(int[] previousState, int[] currentState, int[] yourGoal, int[] opponentGoal, int turn, int max_turn, int min_turn)
    {
        float score = 0, reachGoalFactor = 1, stopOpponentFactor = 1;
        int[] differences = compareArray(previousState, currentState);
        int[] goalDifference = compareArray(yourGoal, opponentGoal);
        int[] currentYourGoalSimilar = compareArray(currentState, yourGoal);
        int[] currentOpponentGoalSimilar = compareArray(currentState, opponentGoal);
        int[] previousYourGoalSimilar = compareArray(previousState, yourGoal);
        int[] previousOpponentGoalsSimilar = compareArray(previousState, opponentGoal);

        reachGoalFactor = countZeros(previousYourGoalSimilar) * (countZeros(goalDifference));
        score += (countZeros(currentYourGoalSimilar) - countZeros(previousYourGoalSimilar)) * reachGoalFactor;

        stopOpponentFactor = countZeros(previousOpponentGoalsSimilar) * (7 - countZeros(goalDifference));
        score += (countZeros(previousOpponentGoalsSimilar) - countZeros(currentOpponentGoalSimilar)) * stopOpponentFactor;

        if (score > 0)
        {
            score = (float)Math.Log(1 + score) / 6;
            //get full score until turn 60, the game immediately ends at turn 150
            if(turn > min_turn){
                if (turn < max_turn) score -= score * (turn - min_turn) / (max_turn - min_turn);
                else score = 0;
            }
        }
        else if (score < 0)
        {
            score = (float)-Math.Log(1 + Math.Abs(score)) / 6;
            //get full score until turn 60, the game immediately ends at turn 150
            // if (turn > min_turn && turn < max_turn) score -= (score + 1) * (turn - min_turn) / (max_turn - min_turn);
            // else score = -1;
        }
        return (float)score * 0.75f;
    }

    public float calcScore4(int[] previousState, int[] currentState, int[] yourGoal, int[] opponentGoal, int turn, int max_turn, int min_turn)
    {
        float score = 0;
        int[] currentYourGoalSimilar = compareArray(currentState, yourGoal);
        int[] previousYourGoalSimilar = compareArray(previousState, yourGoal);
        float currentGoalCount = countZeros(currentYourGoalSimilar);
        float previousGoalCount = countZeros(previousYourGoalSimilar);
        if (currentGoalCount - previousGoalCount > 0)
        {
            score = (float)currentGoalCount * currentGoalCount / 2000;
        }
        return score;
    }

    public int[] compareArray(int[] arr1, int[] arr2)
    {
        int[] difference = new int[arr1.Length];
        for (int i = 0; i < arr1.Length; i++)
        {
            if (arr1[i] == arr2[i]) difference[i] = 0;
            else difference[i] = 1;
        }
        return difference;
    }

    public float countZeros(int[] arr)
    {
        float count = 0;
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] == 0) count++;
        }
        return count;
    }

    public String[] shuffleArray()
    {
        String[] randomGames = {"01","02","03","04","05","06","07","08","09",
                            "10","12","13","14","15","16","17","18","19",
                            "20","21","23","24","25","26","27","28","29",
                            "30","31","32","34","35","36","37","38","39",
                            "40","41","42","43","45","46","47","48","49",
                            "50","51","52","53","54","56","57","58","59",
                            "60","61","62","63","64","65","67","68","69",
                            "70","71","72","73","74","75","76","78","79",
                            "80","81","82","83","84","85","86","87","89",
                            "90","91","92","93","94","95","96","97","98"};

        for (int i = 0; i < randomGames.Length - 1; i++)
        {
            int rnd = UnityEngine.Random.Range(i, randomGames.Length);
            String tmp = randomGames[rnd];
            randomGames[rnd] = randomGames[i];
            randomGames[i] = tmp;
        }
        return randomGames;
    }

    public int[] convertGamesToInt(String goalStr)
    {
        goalStr.Substring(0, 1);
        int[] goal = { int.Parse(goalStr.Substring(0, 1)), int.Parse(goalStr.Substring(1, 1)) };
        return goal;
    }

    void gameOver(){
        drawButton.SetActive(false);
        ruleButton.SetActive(false);
        exitButton.SetActive(false);

        isGameOver = true;
        if (OnGameOver != null)
        {
            OnGameOver();
        }
    }

    public void resume()
    {
        gameReset();
    }

    public void exit()
    {
        SceneManager.LoadScene("Assets/Scenes/Start.unity");
    }

    public void loadRules(){
        SceneManager.LoadScene("Assets/Scenes/Rule.unity", LoadSceneMode.Additive);
    }
}
