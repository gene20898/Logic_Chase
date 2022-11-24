using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class PlayerActionHandler : MonoBehaviour
{
    Player player;
    Card[] cardSlots;
    
    public GameObject Switch;
    public GameObject Gate;

    Gate[] gates;
    Switch[] switches;

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
        gates = Gate.GetComponentsInChildren<Gate>();
        switches = Switch.GetComponentsInChildren<Switch>();

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
        //Debug.Log(player.gameObject.name + " starts turn");
        state = State.WaitForMove;
    }

    public void endTurn()
    {
        state = State.WaitForOpponent;
    }

    public void onClickDraw()
    {
        if (state == State.WaitForMove || state == State.SelectSwitch || state == State.SelectGate)
        {
            player.DrawCard();
            
        }
    }

    public void onClickCard(Card card)
    {
        if(state == State.WaitForMove || state == State.SelectSwitch || state == State.SelectGate)
        {
            selectedCardIndex = card.getIndex();
            if(card.getType() == "Switch"){
                state = State.SelectSwitch;
            }
            else{
                state = State.SelectGate;
            }
            unHideInvalidCell();
            hideInvalidCell(card);
        }    
    }

    public void onClickCell(Gate gate)
    {
        if(state == State.SelectGate)
        {
            gateIndex = gate.transform.GetSiblingIndex();
            previewGate(gate, selectedCardIndex);
            state = State.SelectInput;
            UseCard();
        }
    }

    void previewGate(Gate gate, int index){
        if(selectedCardIndex != -1){
            gate.GetComponent<Gate>().setPreviewGateImage(player.getCardType(index));
            List<string> cards = new List<string>(player.GetCard());
            cards.RemoveAt(selectedCardIndex);
            for (int i = 0; i < cardSlots.Length; i++)
            {
                if (i < cards.Count)
                {
                    cardSlots[i].setCardUI(cards[i]);
                }
                else
                {
                    cardSlots[i].clearUI();
                }
            }
            hideInvalidInput2(gate);
        }
    }
    
    void hideInvalidInput2(Gate gate){
        int index = gate.transform.GetSiblingIndex();
        int column = index%3;
        int row = index/3;

        disableAllGates();

        if(column > 0) {
            for(int i=0; i< gates.Length; i++){
                if(i%3 == column-1 && i!=index-1) gates[i].enable();
            }
        }
        else {
            for(int i=0; i< switches.Length; i++){
                if(i!=row) switches[i].enable();
            }
        }
        gates[index].enable();
    }

    void disableAllGates(){
        for(int i=0; i< gates.Length; i++){
                gates[i].disable();
        }
        for(int i=0; i< switches.Length; i++){
            switches[i].disable();
        }
    }

    void hideInvalidCell(Card card){
        switch(card.getType()){
            case "Switch":
                for(int i=0; i< gates.Length; i++){
                    gates[i].disable();
                }
                break;
            default:
                for(int i=0; i< switches.Length; i++){
                    switches[i].disable();
                }
                break;
        }
    }

    void unHideInvalidCell(){
        for(int i=0; i< gates.Length; i++){
            gates[i].enable();
        }
        for(int i=0; i< switches.Length; i++){
            switches[i].enable();
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
