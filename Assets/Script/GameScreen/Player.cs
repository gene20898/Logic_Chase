using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Player : MonoBehaviour 
{
    public Board board;

    int goal;
    int[] goalArray;
    public Image goalImage;
    public Sprite[] goalSpriteArray;

    List<string> cards = new List<string>();
    bool isTurn = false;
    public event Action OnUpdate;
    public event Action OnTurnStart;
    public event Action OnTurnEnd;

    public void SetGoal(int goal){
        this.goal = goal;
        goalImage.sprite = goalSpriteArray[goal];
    }
    
    public int GetGoal(){
        return goal;
    }

     public void SetGoalArray(int[] goalArray)
    {
        this.goalArray = goalArray;
    }

    public int[] GetGoalArray()
    {
        return goalArray;
    }

    public List<string> GetCard(){
        return cards;
    }
    
    public void StartTurn(){
        Debug.Log(this.gameObject.name + " starts turn");
        print(cards);
        isTurn = true;
        if(OnTurnStart != null){
            OnTurnStart();
        }
    }

    public void EndTurn(){
        //Debug.Log(this.gameObject.name + " ends turn");
        isTurn = false;
        if(OnTurnEnd != null){
            OnTurnEnd();
        }
        board.ChangeTurn();
    }
    
    public void Reset(){
        //Debug.Log(this.gameObject.name + "Reset");
        cards.Clear();
        AddCard(4);
        if(OnUpdate != null){
            OnUpdate();
        }
    }

    void AddCard(int amount){
        void randomCard(int n)
        {
            int num = UnityEngine.Random.Range(0, n + 1);
            switch (num)
            {
                case 0: cards.Add("NOT"); break;
                case 1: cards.Add("OR"); break;
                case 2: cards.Add("Switch"); break;
                case 3: cards.Add("NOR"); break;
                case 4: cards.Add("NAND"); break;
                case 5: cards.Add("AND"); break;
                case 6: cards.Add("XNOR"); break;
                case 7: cards.Add("XOR"); break;
                case 8: cards.Add("Wire"); break;
            }
        }

        bool allWires = true;
        foreach (string card in cards)
        {
            if (card != "Wire")
            {
                allWires = false;
                break;
            }
        }
        if (allWires && (cards.Count > 3))
        {
            randomCard(8);
            randomCard(7);
        }
        else
        {
            for (int i = 0; i < amount; i++)
            {
                randomCard(8);
            }
        }
    }

    public bool DrawCard(){
        if(isTurn && cards.Count < 6){
            AddCard(2);
            if(OnUpdate != null){
                OnUpdate();
            }
            this.EndTurn();
            return true;
        }else{
            return false;
        }
    }

    public bool UseCard(int cardIndex, int gateIndex = -1, int inputRow = -1, int switchIndex = -1){
        Debug.Log("CardIndex" + cardIndex + " GateIndex" + gateIndex + " InputRow" + inputRow + " SwitchIndex" + switchIndex);
        string cardType = cards[cardIndex];
        int row = gateIndex % 3;
        int col = gateIndex / 3;
        bool result = false;
        if(cardType == "Switch" && switchIndex != -1) result = board.ToggleSwitch(switchIndex) == true;
        else if ((cardType == "Wire" || cardType == "NOT") && gateIndex != -1) result = board.ReplaceGate(cardType, row, col) == true;
        else if (gateIndex != -1 && inputRow != -1) result = board.ReplaceGate(cardType, row, col, inputRow);
        print(result);
        
        if(result == true){
            cards.RemoveAt(cardIndex);
            if(OnUpdate != null){
                OnUpdate();
            }
            this.EndTurn();
            return true;
        }
        else{
            return false;
        }
    }
}
