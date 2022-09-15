using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerActionHandler : MonoBehaviour
{
    Player player;
    Card[] cardSlots;
    int selectedCardIndex = -1;
    int gateIndex = -1;
    int inputRow = -1;
    int switchIndex = -1;

    State state;
    enum State
    {
        Invalid = -1,
        WaitForMove = 0,
        SelectGate = 1,
        SelectSwitch = 2,
        SelectInput = 3,
        WaitForOpponent = 4,
    }

    void Awake()
    {
        player = GetComponent<Player>();
        cardSlots = this.gameObject.GetComponentsInChildren<Card>();
        state = State.WaitForMove;
        player.OnUpdate += updateCardUI;
        player.OnTurnStart += startTurn;
        player.OnTurnEnd += endTurn;
    }

    void Update(){

    }

    public void updateCardUI()
    {
        List<string> cards = player.GetCard();
        for (int i = 0; i < cardSlots.Length; i++)
        {
            if (i < cards.Count)
            {
                cardSlots[i].setCard(cards[i], i);
            }
            else
            {
                cardSlots[i].clearSlot();
            }
        }
    }

    public void startTurn()
    {
        Debug.Log(player.gameObject.name + " starts turn");
        state = State.WaitForMove;
    }

    public void endTurn()
    {
        state = State.WaitForOpponent;
    }

    public void onClickDraw()
    {
        if (state == State.WaitForMove)
        {
            player.DrawCard();
        }
    }

    public void onClickCard(Card card)
    {
        Debug.Log("onClickCard ");
        if(state == State.WaitForMove)
        {
            Debug.Log("onClickCard pasas");
            selectedCardIndex = card.getIndex();
            if(card.getType() == "Switch"){
                state = State.SelectSwitch;
            }
            else{
                state = State.SelectGate;
            }
        }
    }

    public void onClickCell(Gate gate)
    {
        if(state == State.SelectGate)
        {
            Debug.Log("clickGate");
            gateIndex = gate.transform.GetSiblingIndex();
            state = State.SelectInput;
            UseCard();
        }
    }

    public void onClickSwitch(Switch gate)
    {
        switchIndex = gate.transform.GetSiblingIndex();
        if(state == State.SelectSwitch)
        {
            UseCard();
        }else if(state == State.SelectInput){
            inputRow = (int)switchIndex;
            UseCard();
        }
    }

    public void onClickInput(Gate input)
    {
        if(state == State.SelectInput)
        {
            Debug.Log("clickInput");
            inputRow = (int)input.transform.GetSiblingIndex()/3;
            UseCard();
        }
    }

    void UseCard(){
        if(player.UseCard(selectedCardIndex, gateIndex, inputRow, switchIndex) == true){
            selectedCardIndex = -1;
            gateIndex = -1;
            inputRow = -1;
            switchIndex = -1;
            updateCardUI();
        }
    }
}
