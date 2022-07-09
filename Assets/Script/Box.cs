using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class Box : MonoBehaviour
{
    string type;
    int input;
    public Text boxText;
    public Image boxFace;
    public Sprite[] boxFaceSprites;
    BoardScript board;
    Button button;
    public bool isLatest = false;

    //my event
    [System.Serializable]
    public class UseEvent : UnityEvent { }

    [SerializeField]
    private UseEvent useEvent = new UseEvent();
    public UseEvent onUse { get { return onUse; } set { onUse = value; } }

    // Start is called before the first frame update
    void Start()
    {
        board = GameObject.Find("Board").GetComponent<BoardScript>();
        board.OnReset += clearSlot;
        button = gameObject.GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void setBox(string type, int input1, int row = -1, int input2 = 0)
    {
        unSetLatestMove();
        this.type = type;
        this.input = row;

        string input_row = "";
        switch (row)
        {
            case 0: input_row = "B"; break;
            case 1: input_row = "A"; break;
            case 2: input_row = "F"; break;
            case 3: input_row = "G"; break;
            case 4: input_row = "E"; break;
            case 5: input_row = "D"; break;
            case 6: input_row = "C"; break;
            default: input_row = ""; break;
        }
        boxText.text = input_row;
        boxText.enabled = true;

        int off_set1 = input2;
        int off_set2 = input1*2;

        switch (type)
        {
            case "AND": boxFace.sprite = boxFaceSprites[1 + off_set1 + off_set2]; break;
            case "NAND": boxFace.sprite = boxFaceSprites[5 + off_set1 + off_set2]; break;
            case "NOR": boxFace.sprite = boxFaceSprites[9 + off_set1 + off_set2]; break;
            case "NOT": boxFace.sprite = boxFaceSprites[13 + off_set2/2]; break;
            case "OR": boxFace.sprite = boxFaceSprites[15 + off_set1 + off_set2]; break;
            case "Wire": boxFace.sprite = boxFaceSprites[19 + off_set2/2]; break;
            case "XNOR": boxFace.sprite = boxFaceSprites[21 + off_set1 + off_set2]; break;
            case "XOR": boxFace.sprite = boxFaceSprites[25 + off_set1 + off_set2]; break;
            default: boxFace.sprite = boxFaceSprites[19]; break;
        }
    }
    public void clearSlot()
    {
        type = "";
        boxText.text = "";
        boxText.enabled = false;
        boxFace.sprite = boxFaceSprites[19];
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
}
