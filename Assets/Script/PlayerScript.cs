using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.MLAgents;

public class PlayerScript : MonoBehaviour
{
    enum CardType { NoCard, Switch, Wire, NOT, AND, OR, NAND, NOR, XOR, XNOR };
    public BoardScript board;
    public GameObject playerNum;
    Image playerNumImage;
    public Sprite[] numSpriteArray;
    int goal = 0;
    int[] goalArray = new int[7];
    public List<string> cards = new List<string>();
    public GameObject cardPrefab;
    public Button drawButton;
    public Agent agent;
    bool isHeuristic = false;
    bool isInput = false;
    bool isTurn = false;
    int[] input = new int[] { 0, 0, 0, 0};
    bool isGameOver = false;
    Card selectedCard = null;
    Box selectedBox = null;
    EnvironmentParameters curriculumParams;
    void Awake()
    {
        curriculumParams = Academy.Instance.EnvironmentParameters;
        board.OnReset += Reset;
        board.OnGameOver += GameOver;

        this.playerNumImage = playerNum.GetComponent<Image>();
        if (agent.GetComponent<Unity.MLAgents.Policies.BehaviorParameters>().BehaviorType == Unity.MLAgents.Policies.BehaviorType.HeuristicOnly)
        {
            isHeuristic = true;
        }
        drawCard(4);
    }

    void Update()
    {
        if (isHeuristic && isTurn && isInput)
        {
            isInput = false;
            agent.RequestDecision();
        }
    }

    public void setGoal(int goal)
    {
        this.goal = goal;
        this.playerNumImage.sprite = numSpriteArray[goal];
    }

    public void setGoalArray(int[] goalArray)
    {
        this.goalArray = goalArray;
    }
    
    public int getGoal()
    {
        return goal;
    }

    public int[] getGoalArray()
    {
        return goalArray;
    }

    public void startTurn()
    {
        // print(gameObject.name + " start turn");
        if (agent != null)
        {
            if (isHeuristic)
            {
                isTurn = true;
            }
            else
            {
                agent.RequestDecision();
            }
        }
        for (int i = 0; i < cards.Count; i++)
        {
            gameObject.transform.GetChild(0).transform.GetChild(i).gameObject.GetComponent<Button>().interactable = true;
        }
    }

    public void endTurn()
    {
        isTurn = false;
        selectedCard = null;
        selectedBox = null;
        for (int i = 0; i < 7; i++)
        {
            gameObject.transform.GetChild(0).transform.GetChild(i).gameObject.GetComponent<Button>().interactable = false;
        }
    }

    public Boolean drawCard(int amount)
    {
        int cardNum = (int)curriculumParams.GetWithDefault("my_environment_parameter", 8);

        Boolean randomCard(int n)
        {
            if(cardNum != 8){
                n = cardNum;
            }
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
            return true;
        }

        if (cards.Count > 5)
        {
            return false;
        }
        else
        {
            Boolean allWires = true;
            foreach (String card in cards)
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
            updateCardUI();
        }
        return true;
    }

    public void resetHand()
    {
        cards.Clear();
        drawCard(4);
    }

    public void useCard(int index)
    {
        if(!isGameOver){
            cards.RemoveAt(index);
            updateCardUI();
        }
    }

    public void updateCardUI()
    {
        GameObject playerHand = gameObject.transform.GetChild(0).gameObject;
        Card[] cardSlots = playerHand.GetComponentsInChildren<Card>();
        for (int i = 0; i < cardSlots.Length; i++)
        {
            if (i < cards.Count)
            {
                cardSlots[i].setCard(cards[i], i);
                if (!isHeuristic)
                {
                    cardSlots[i].hide();
                }
            }
            else
            {
                cardSlots[i].clearSlot();
            }
        }
    }

    public void GiveReward(float reward)
    {
        if (agent != null)
        {
            // print(gameObject.name + " GiveReward: " + reward);
            agent.AddReward(reward);
        }
    }

    public void SetReward(float reward)
    {
        if (agent != null)
        {
            // print(gameObject.name + " GiveReward: " + reward);
            agent.SetReward(reward);
        }
    }
    public void endEpisode()
    {
        if (agent != null)
        {
            agent.EndEpisode();
        }
    }

    public int[] getInput()
    {
        isInput = false;
        return input;
    }

    public void onClickDrawButton()
    {
        if (!isGameOver && isTurn)
        {
            selectedCard = null;
            selectedBox = null;
            input[0] = 0;
            input[1] = 0;
            input[2] = 0;
            input[3] = 0;
            isInput = true;
        }
    }

    public void onClickCard(Card card)
    {
        if (!isGameOver && isTurn)
        {
            selectedBox = null;
            input[1] = 0;

            selectedCard = card;
            // input[0] = card.transform.GetSiblingIndex() + 1;
            CardType cardType = (CardType)System.Enum.Parse(typeof(CardType), card.getType());
            switch((int)cardType){
                case 0: input[0] = 0; break;
                case 1: input[0] = 1; break;
                case 2: input[0] = 2; break;
                case 3: input[0] = 3; break;
                case 4: input[0] = 4; break;
                case 5: input[0] = 5; break;
                case 6: input[0] = 6; break;
                case 7: input[0] = 7; break;
                case 8: input[0] = 8; break;
                case 9: input[0] = 9; break;
                default: input[0] = 0; break;
            }
        }
    }

    public void onClickBox(Box box)
    {
        if (!isGameOver && isTurn)
        {
            input[1] = box.transform.GetSiblingIndex();
            if (selectedCard != null)
            {
                string cardType = selectedCard.getType();
                if (cardType != "Switch")
                {
                    if (selectedBox == null)
                    {
                        if (cardType == "Wire" || cardType == "NOT")
                        {
                            input[2] = 0;
                            input[3] = 0;
                            isInput = true;
                            selectedCard = null;
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
        if (!isGameOver && isTurn)
        {
            input[2] = label.transform.GetSiblingIndex();
            if (selectedBox != null && selectedCard != null && !isGameOver)
            {
                input[3] = 0;
                isInput = true;
            }
        }
    }

    public void onClickSwitch(Switch switchButton)
    {
        if (!isGameOver && isTurn)
        {
            input[3] = switchButton.transform.GetSiblingIndex();
            if (selectedCard != null && !isGameOver)
            {
                string cardType = selectedCard.getType();
                if (cardType == "Switch")
                {
                    input[1] = 0;
                    input[2] = 0;
                    isInput = true;
                }
            }
        }
    }

    void Reset()
    {
        isGameOver = false;
    }

    void GameOver()
    {
        isGameOver = true;
        cards.Clear();
        updateCardUI();
    }
}
