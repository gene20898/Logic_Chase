using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

public class Card : MonoBehaviour
{
    string type;
    int index;
    TextMeshProUGUI cardText;
    Image cardFace;
    public Sprite[] cardFaceSprites;

    void Awake(){
        cardText = this.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        cardFace = this.gameObject.GetComponentInChildren<Image>();
    }

    public void setCard(string cardType, int cardIndex)
    {
        this.type = cardType;
        index = cardIndex;
        setCardUI(type);
    }

    public void setCardUI(string cardType)
    {
        cardText.text = cardType;
        switch(cardType){
            case "AND": cardFace.sprite = cardFaceSprites[0]; break;
            case "NAND": cardFace.sprite = cardFaceSprites[1]; break;
            case "NOR": cardFace.sprite = cardFaceSprites[2]; break;
            case "NOT": cardFace.sprite = cardFaceSprites[3]; break;
            case "OR": cardFace.sprite = cardFaceSprites[4]; break;
            case "Switch": cardFace.sprite = cardFaceSprites[5]; break;
            case "Wire": cardFace.sprite = cardFaceSprites[6]; break;
            case "XNOR": cardFace.sprite = cardFaceSprites[7]; break;
            case "XOR": cardFace.sprite = cardFaceSprites[8]; break;
            default: cardFace.sprite = cardFaceSprites[5]; break;
        }
        cardText.enabled = true;
        cardFace.enabled = true;
        gameObject.GetComponent<Button>().interactable = true;
    }

    public void clearSlot()
    {
        type = "";
        clearUI();
    }

    public void clearUI()
    {
        cardText.text = "";
        cardFace.sprite = cardFaceSprites[5];
        cardText.enabled = false;
        cardFace.enabled = false;
        gameObject.GetComponent<Button>().interactable = false;
    }

    public void hide()
    {
        cardText.text = "";
        cardFace.sprite = cardFaceSprites[9];
    }

    public string getType()
    {
        return type;
    }

    public int getIndex()
    {
        return index;
    }
}