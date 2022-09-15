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
    public Board board;
    public Player player;
    public Player opponent;
    public BotActionHandler botActionHandler;
 
    int[] playerGoal = new int[7];
    int[] opponentGoal = new int[7];
    VectorSensorComponent m_GoalSensor;

    public NNModel easyBot;
    public NNModel hardBot;
    
    void Awake()
    {
        // if(GameModeManager.gameModeManager.isHard){
        //     GetComponent<Unity.MLAgents.Policies.BehaviorParameters>().Model = hardBot;
        // }else{
        //     GetComponent<Unity.MLAgents.Policies.BehaviorParameters>().Model = easyBot;
        // }   
    }

    public override void Initialize()
    {
        Debug.Log("Initialize");
        m_GoalSensor = this.GetComponent<VectorSensorComponent>();
    }

    public override void OnEpisodeBegin()
    {
        Debug.Log("OnEpisodeBegin");
        playerGoal = player.GetGoalArray();
        opponentGoal = opponent.GetGoalArray();
    }

    public override async void CollectObservations(VectorSensor sensor)
    {
        Debug.Log("observation");
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

        // Observe cards types in player hand
        int[] cardtype = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        List<string> cards = player.GetCard(); 
        for (int i = 0; i < cards.Count; i++)
        {
            CardType card = (CardType)System.Enum.Parse(typeof(CardType), cards[i]);
            // sensor.AddObservation((int)card);
            cardtype[(int)card - 1]++;
        }
        for (int i = 0; i < cardtype.Length; i++)
        {
            sensor.AddObservation(cardtype[i]);
        }
        Debug.Log(cardtype);

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
            playerGoal = player.GetGoalArray();
            opponentGoal = opponent.GetGoalArray();

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
        Debug.Log("Observation done");
    }

    //Mask invalid action
    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        Debug.Log("WriteDiscreteActionMask");
        List<string> cards = player.GetCard();
        if (cards.Count > 5)
        {
            actionMask.SetActionEnabled(0, 0, false);
        }

        int[] cardtype = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        for (int i = 0; i < cards.Count; i++)
        {
            CardType card = (CardType)System.Enum.Parse(typeof(CardType), cards[i]);
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
        Debug.Log("WriteDiscreteActionMask done");
    }

    //Do action
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        Debug.Log("Action");
        int actionChoice = actionBuffers.DiscreteActions[0];
        int boardLocation = actionBuffers.DiscreteActions[1];
        int inputRowChoice = actionBuffers.DiscreteActions[2];
        int switchLocation = actionBuffers.DiscreteActions[3];

        botActionHandler.SetAction(actionChoice, boardLocation, inputRowChoice, switchLocation);      
      
        Debug.Log("Action Done");
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        Debug.Log("Heuristic");
        ActionSegment<int> discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = 0;
        discreteActionsOut[1] = 0;
        discreteActionsOut[2] = 0;
        discreteActionsOut[3] = 0;
    }
}