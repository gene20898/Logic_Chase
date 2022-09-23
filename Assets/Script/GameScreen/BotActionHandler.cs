using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BotActionHandler : MonoBehaviour
{
    enum CardType { NoCard, Switch, Wire, NOT, AND, OR, NAND, NOR, XOR, XNOR };
    enum GateType { Wire, NOT, AND, OR, NAND, NOR, XOR, XNOR };

    Player bot;
    public AgentScript agent;
    public TextMeshProUGUI cardCountText;

    int actionChoice = -1;
    int gateIndex = -1;
    int inputRow = -1;
    int switchIndex = -1;

    int invalid_count = 0;
    bool isTurn = false;

    void Awake()
    {
        bot = GetComponent<Player>();
        bot.OnUpdate += updateCardCount;
        bot.OnTurnStart += startTurn;
        bot.OnTurnEnd += endTurn;
    }

    public void updateCardCount()
    {
        cardCountText.text = 'x'+bot.GetCard().Count.ToString();
    }

    public void startTurn()
    {
        isTurn = true;
        agent.RequestDecision();
    }

    public void endTurn()
    {
        isTurn = false;
    }

    public void SetAction(int actionChoice, int boardLocation, int inputRowChoice, int switchLocation){
        this.actionChoice = actionChoice;
        this.gateIndex = boardLocation;
        this.inputRow = inputRowChoice;
        this.switchIndex = switchLocation;

        PlayCard();
    }

    public void PlayCard(){
        Debug.Log(bot.gameObject.name + " action " + actionChoice + " " + gateIndex + " " + inputRow + " " + switchIndex);
        bool result = false;
        int cardIndex = FindCardIndex(actionChoice);

        if(actionChoice != 0 && cardIndex == -1){
            Debug.Log("invalid card choice");
            agent.RequestDecision();
            return;
        }

        switch(actionChoice){
            case 0: 
                result = bot.DrawCard();
                break;
            case 1: 
                result = bot.UseCard(cardIndex, -1, -1, switchIndex);
                break;
            case 2: case 3:
                result = bot.UseCard(cardIndex, gateIndex);
                break;
            default:
                if(inputRow == (int)Mathf.Floor((float)gateIndex / 3)){
                    if(invalid_count > 3){
                        RandomInput();
                    }else{
                        invalid_count++;
                    }
                }
                result = bot.UseCard(cardIndex, gateIndex, inputRow, switchIndex);
                break;
        }

        if(result == false){
            Debug.Log("invalid action");
            agent.RequestDecision();
        }
    }

    int FindCardIndex(int actionChoice){
        CardType type = (CardType)actionChoice;
        string cardtype = type.ToString();
        List<string> cards = bot.GetCard();

        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i] == cardtype)
            {
                return i;
            }
        }
        return -1;
    }

    void RandomInput(){
        {
            while (inputRow == (int)Mathf.Floor((float)gateIndex / 3))
            {
                inputRow = UnityEngine.Random.Range(0, 6);
            }
            invalid_count = 0;
        }
    }
}
