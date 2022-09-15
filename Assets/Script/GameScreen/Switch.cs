using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Switch : MonoBehaviour
{
    Image switchSprite;
    public Sprite[] switchSpriteList;
    public Board board;
    bool isLatest = false;

    void Awake(){
        switchSprite = gameObject.GetComponent<Image>();
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

    public void unSetLatestMove(){
        if(isLatest){
            isLatest = false;
        }
    }
}
