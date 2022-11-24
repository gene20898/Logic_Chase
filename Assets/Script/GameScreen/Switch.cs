using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class Switch : MonoBehaviour, IDropHandler
{
    public PlayerActionHandler playerHandler;
    Image switchSprite;
    public Sprite[] switchSpriteList;
    public Board board;
    public bool isLatest = false;
    Button button;

    void Awake(){
        switchSprite = gameObject.GetComponent<Image>();
        button = gameObject.GetComponent<Button>();
        board.OnReset += reset;
    }

    public void toggle()
    {
        isLatest = true;
        if (switchSprite.sprite == switchSpriteList[0])
        {
            switchSprite.sprite = switchSpriteList[1];
        }
        else
        {
            switchSprite.sprite = switchSpriteList[0];
        }
    }

    public void reset()
    {
        switchSprite.sprite = switchSpriteList[0];
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
        if(button.interactable == true){
            playerHandler.onClickSwitch(this);
        }
    }

     public void disable(){
        button.interactable = false;
    }

    public void enable(){
        button.interactable = true;
    }
}
