using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class Gate : MonoBehaviour, IDropHandler
{
    string type;
    int input;
    public Board board;
    TextMeshProUGUI boxText;
    Image boxFace;
    public Sprite[] boxFaceSprites;
    Button button;
    
    public bool isLatest = false;

    // Start is called before the first frame update

    void Awake(){
        boxText = this.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        boxFace = this.gameObject.GetComponentInChildren<Image>();
        button = gameObject.GetComponent<Button>();
        board.OnReset += clearSlot;
    }

    public void setGate(string type, int input1, int inputRow = -1, int input2 = 0)
    {
        unSetLatestMove();
        this.type = type;
        this.input = inputRow;

        string[] inputLabel = {"B", "A", "F", "G", "E", "D", "C"};
        if(inputRow != -1) boxText.text = inputLabel[inputRow];
        else boxText.text = "";
        boxText.enabled = true;

        int off_set1 = input2;
        int off_set2 = input1*2;

        switch (type)
        {
            case "AND": boxFace.sprite = boxFaceSprites[off_set1 + off_set2]; break;
            case "NAND": boxFace.sprite = boxFaceSprites[4 + off_set1 + off_set2]; break;
            case "NOR": boxFace.sprite = boxFaceSprites[8 + off_set1 + off_set2]; break;
            case "NOT": boxFace.sprite = boxFaceSprites[12 + off_set2/2]; break;
            case "OR": boxFace.sprite = boxFaceSprites[14 + off_set1 + off_set2]; break;
            case "Wire": boxFace.sprite = boxFaceSprites[18 + off_set2/2]; break;
            case "XNOR": boxFace.sprite = boxFaceSprites[20 + off_set1 + off_set2]; break;
            case "XOR": boxFace.sprite = boxFaceSprites[24 + off_set1 + off_set2]; break;
            default: boxFace.sprite = boxFaceSprites[18]; break;
        }
    }

    public void clearSlot()
    {
        boxText.text = "";
        boxText.enabled = false;
        boxFace.sprite = boxFaceSprites[18];
    }

    public void setLatestMove()
    {
        isLatest = true;
        button.interactable = false;
    }

    public void unSetLatestMove()
    {
        isLatest = false;
        button.interactable = true;
    }

    public void OnDrop(PointerEventData eventData){
        Debug.Log(type+" on drop");
    }
}

