using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class Switch : MonoBehaviour
{
    public int index;

    public Image switchSprite;
    public Sprite[] switchSpriteList;
    BoardScript board;
    bool isLatest = false;

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
        board.OnReset += reset;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void toggle()
    {
        isLatest = true;
        if (switchSprite.sprite == switchSpriteList[0])
        {
            switchSprite.sprite = switchSpriteList[3];
        }
        else
        {
            switchSprite.sprite = switchSpriteList[2];
        }
    }

    public void reset()
    {
        switchSprite.sprite = switchSpriteList[0];
    }

    public void unSetLatestMove(){
        if(isLatest){
            isLatest = false;
            if (switchSprite.sprite == switchSpriteList[2])
            {
                switchSprite.sprite = switchSpriteList[0];
            }
            else
            {
                switchSprite.sprite = switchSpriteList[1];
            }
        }
    }
}
