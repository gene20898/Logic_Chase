using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine.UI;
using System;
using System.IO;
using Unity.Barracuda;
using UnityEditor;

public class AgentScript : Agent
{
    enum CardType { NoCard, Switch, Wire, NOT, AND, OR, NAND, NOR, XOR, XNOR };
    enum GateType { Wire, NOT, AND, OR, NAND, NOR, XOR, XNOR };
    public BoardScript board;
    public PlayerScript player;
    public PlayerScript opponent;
    public GameObject boxesObject;
    public GameObject switchesObject;
    public GameObject labelsObject;
    public DrawButton drawButton;
    public Card[] cards;

    Box[] boxes;
    Switch[] switches;
    Label[] labels;
    int[] playerGoal = new int[7];
    int[] opponentGoal = new int[7];
    VectorSensorComponent m_GoalSensor;

    bool isHeuristic = false;
    public float invalid = 0;

    public NNModel easyBot;
    public NNModel hardBot;

    int invalid_count = 0;
    void Start()
    {
        boxes = boxesObject.GetComponentsInChildren<Box>();
        switches = switchesObject.GetComponentsInChildren<Switch>();
        labels = labelsObject.GetComponentsInChildren<Label>();
    }

    void Awake()
    {
        if (gameObject.GetComponent<Unity.MLAgents.Policies.BehaviorParameters>().BehaviorType == Unity.MLAgents.Policies.BehaviorType.HeuristicOnly)
        {
            isHeuristic = true;
        }

        if(!isHeuristic){
            if(GameModeManager.gameModeManager.isHard){
                GetComponent<Unity.MLAgents.Policies.BehaviorParameters>().Model = hardBot;
            }else{
                GetComponent<Unity.MLAgents.Policies.BehaviorParameters>().Model = easyBot;
            }
        }
        
        // TextWriter tw = new StreamWriter(Application.dataPath + "/record" + player.gameObject.name + ".csv",false);
        // tw.WriteLine("Invalid");
        // tw.Close();
    }

    public override void Initialize()
    {
        m_GoalSensor = this.GetComponent<VectorSensorComponent>();

        // if(GameModeManager.gameModeManager.isHard){
        //     SetModel("BotAll", hardBot);
        //     print("HardBot");
        // }else{
        //     SetModel("BotAll", easyBot);
        //     print("easyBot");
        // }
    }

    public override void OnEpisodeBegin()
    {
        invalid = 0;
        playerGoal = player.getGoalArray();
        opponentGoal = opponent.getGoalArray();
    }

    public override async void CollectObservations(VectorSensor sensor)
    {
        //Observe board output
        for (int i = 0; i < board.outputState.GetLength(0); i++)
        {
            for (int j = 0; j < board.outputState.GetLength(1); j++)
            {
                sensor.AddObservation(board.outputState[i, j]);
            }
        }

        //Observe logic gates on board 
        for (int i = 0; i < board.gameState.GetLength(0); i++)
        {
            for (int j = 0; j < board.gameState.GetLength(1); j++)
            {
                GateType gate = (GateType)System.Enum.Parse(typeof(GateType), board.gameState[i, j].getName());
                // sensor.AddObservation((int)gate);
                sensor.AddOneHotObservation((int)gate, 8);
            }
        }

        //Observe input on logic gate's second terminal
        for (int i = 0; i < board.gameState.GetLength(0); i++)
        {
            for (int j = 0; j < board.gameState.GetLength(1); j++)
            {
                GateType gate = (GateType)System.Enum.Parse(typeof(GateType), board.gameState[i, j].getName());
                if (gate == GateType.Wire || gate == GateType.NOT)
                {
                    // sensor.AddObservation(0);
                    sensor.AddOneHotObservation(0, 8);
                }
                else
                {
                    // sensor.AddObservation((int)board.gameState[i, j].getInput2Row() + 1);
                    sensor.AddOneHotObservation((int)board.gameState[i, j].getInput2Row() + 1, 8);
                }
            }
        }

        //Observe cards in player hand

        // for (int i = 0; i < 7; i++)
        // {
        //     if (i < player.cards.Count)
        //     {
        //         CardType card = (CardType)System.Enum.Parse(typeof(CardType), player.cards[i]);
        //         // sensor.AddObservation((int)card);
        //         sensor.AddOneHotObservation((int)card, 10);
        //     }
        //     else
        //     {
        //         // sensor.AddObservation((int)CardType.NoCard);
        //         sensor.AddOneHotObservation((int)CardType.NoCard, 10);
        //     }

        // }

        // Observe cards types in player hand
        int[] cardtype = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        for (int i = 0; i < player.cards.Count; i++)
        {
            CardType card = (CardType)System.Enum.Parse(typeof(CardType), player.cards[i]);
            // sensor.AddObservation((int)card);
            cardtype[(int)card - 1]++;
        }
        for (int i = 0; i < cardtype.Length; i++)
        {
            sensor.AddObservation(cardtype[i]);
        }

        //Observe enable bit
        for (int i = 0; i < board.outputState.GetLength(0); i++)
        {
            for (int j = 0; j < board.outputState.GetLength(1); j++)
            {
                if (board.lastMove == (i * 7) + j)
                {
                    sensor.AddObservation(0);
                }
                else
                {
                    sensor.AddObservation(1);
                }
            }
        }

        if (m_GoalSensor is object)
        {
            m_GoalSensor.GetSensor().Reset();
            playerGoal = player.getGoalArray();
            opponentGoal = opponent.getGoalArray();

            //Observe player's goal
            for (int i = 0; i < 7; i++)
            {
                m_GoalSensor.GetSensor().AddObservation(playerGoal[i]);
            }

            //Observe opponent's goal
            for (int i = 0; i < 7; i++)
            {
                m_GoalSensor.GetSensor().AddObservation(opponentGoal[i]);
            }
        }
    }

    //Mask invalid action
    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        if (player.cards.Count > 5)
        {
            actionMask.SetActionEnabled(0, 0, false);
        }

        int[] cardtype = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        for (int i = 0; i < player.cards.Count; i++)
        {
            CardType card = (CardType)System.Enum.Parse(typeof(CardType), player.cards[i]);
            cardtype[(int)card - 1]++;
        }

        for (int i = 1; i < cardtype.Length + 1; i++)
        {
            if (cardtype[i - 1] == 0)
            {
                actionMask.SetActionEnabled(0, i, false);
            }
        }

        if (board.lastMove % 4 == 0)
        {
            actionMask.SetActionEnabled(3, (int)Mathf.Floor((float)board.lastMove / 4), false);
        }
        else
        {
            actionMask.SetActionEnabled(1, board.lastMove - (int)Mathf.Ceil((float)board.lastMove / 4), false);
        }
    }
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        int actionChoice = actionBuffers.DiscreteActions[0];
        int boardLocation = actionBuffers.DiscreteActions[1];
        int inputRowChoice = actionBuffers.DiscreteActions[2];
        int switchLocation = actionBuffers.DiscreteActions[3];

        // print(player.gameObject.name + " action " + String.Join(" ", new List<int>(actionBuffers.DiscreteActions).ConvertAll(i => i.ToString()).ToArray()));
        if (actionChoice == 0)
        {
            if (player.cards.Count > 5)
            {
                // print(player.gameObject.name + " INVALID: cannot draw");
                if (!isHeuristic)
                {
                    invalid += -0.01f;
                    AddReward(-0.01f);
                    RequestDecision();
                }
            }
            else
            {
                board.onClickDrawButton();
            }
        }
        else
        {
            CardType type = (CardType)actionChoice;
            string cardtype = type.ToString();
            int cardIndex = 0;
            bool noCard = true;

            for (int i = 0; i < cards.Length; i++)
            {
                if (cards[i].getType() == cardtype)
                {
                    cardIndex = i;
                    noCard = false;
                    break;
                }
            }

            if (noCard)
            {
                // print(player.gameObject.name + "INVALID: " + actionChoice + " doesn't exist");
                if (!isHeuristic)
                {
                    invalid += -0.01f;
                    AddReward(-0.01f);
                    RequestDecision();
                }
            }


            // int cardIndex = actionChoice - 1;
            // if (cardIndex + 1 > player.cards.Count)
            // {
            //     print(player.gameObject.name + "INVALID: " + actionChoice + " doesn't exist");
            //     if (!isHeuristic)
            //     {
            //         invalid += -0.01f;
            //         AddReward(-0.01f);
            //         RequestDecision();
            //     }
            // }
            else
            {
                board.onClickCard(cards[cardIndex]);

                string selectedCard = player.cards[cardIndex];
                if (selectedCard.Equals("Switch"))
                {
                    if ((board.lastMove % 4 == 0) && (switchLocation == (int)Mathf.Floor((float)board.lastMove / 4)))
                    {
                        // print("INVALID: same as last move (switch)");
                        if (!isHeuristic)
                        {
                            invalid += -0.01f;
                            AddReward(-0.01f);
                            RequestDecision();
                        }
                    }
                    else
                    {
                        board.onClickSwitch(switches[switchLocation]);
                    }
                }
                else
                {
                    if (board.lastMove % 4 != 0 && boardLocation == board.lastMove - (int)Mathf.Ceil((float)board.lastMove / 4))
                    {
                        // print("INVALID: same as last move (gate)");
                        if (!isHeuristic)
                        {
                            invalid += -0.01f;
                            AddReward(-0.01f);
                            RequestDecision();
                        }
                    }
                    else if (selectedCard.Equals("Wire") || selectedCard.Equals("NOT"))
                    {
                        int row = (int)Mathf.Floor((float)boardLocation / 3);
                        int col = boardLocation % 3;
                        // if (board.gameState[col, row] is Wire && selectedCard.Equals("Wire"))
                        // {
                        //     // print("INVALID: cannot place wire on wire");
                        //     if (!isHeuristic)
                        //     {
                        //         invalid += -0.01f;
                        //         AddReward(-0.01f);
                        //         RequestDecision();
                        //     }
                        // }
                        // else
                        // {
                        //     board.onClickBox(boxes[boardLocation]);
                        // }
                        board.onClickBox(boxes[boardLocation]);
                    }
                    else
                    {
                        if (invalid_count > 2)
                        {
                            while (inputRowChoice == (int)Mathf.Floor((float)boardLocation / 3))
                            {
                                inputRowChoice = UnityEngine.Random.Range(0, 6);
                            }
                            board.onClickBox(boxes[boardLocation]);
                            board.onClickLabel(labels[inputRowChoice]);
                            invalid_count = 0;
                        }
                        else
                        {
                            if (inputRowChoice == (int)Mathf.Floor((float)boardLocation / 3))
                            {
                                // print("INVALID: wrong input");
                                invalid_count++;
                                if (!isHeuristic)
                                {
                                    invalid += -0.01f;
                                    AddReward(-0.01f);
                                    RequestDecision();
                                }
                            }
                            else
                            {
                                board.onClickBox(boxes[boardLocation]);
                                board.onClickLabel(labels[inputRowChoice]);
                            }
                        }
                    }
                }
            }
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        int[] input = player.getInput();
        // print(gameObject.transform.parent.gameObject.name + " get " + String.Join(" ", new List<int>(input).ConvertAll(i => i.ToString()).ToArray()));
        ActionSegment<int> discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = input[0];
        discreteActionsOut[1] = input[1];
        discreteActionsOut[2] = input[2];
        discreteActionsOut[3] = input[3];
    }
}


