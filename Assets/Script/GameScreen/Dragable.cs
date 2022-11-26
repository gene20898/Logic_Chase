using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Dragable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Transform cardOriginPosision;
    int siblingIndex;
    public Image image;
    public PlayerActionHandler playerHandler;
    [HideInInspector] public Transform parentAfterDrag;
    bool isDragged = false;


    void Awake(){
        siblingIndex = transform.GetSiblingIndex();
    }

    public void OnBeginDrag(PointerEventData eventData){
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData){
        transform.position = Input.mousePosition;
        GameObject dragged = eventData.pointerDrag; 
        Card card = dragged.GetComponent<Card>();
        if(!isDragged) {
            playerHandler.onClickCard(card);
            isDragged = true;  
        }
    }

    public void OnEndDrag(PointerEventData eventData){
        transform.SetParent(parentAfterDrag);
        transform.SetSiblingIndex(siblingIndex);
        image.raycastTarget = true;
        if(eventData.pointerEnter.GetComponent<Gate>() == null) playerHandler.onUnSelectCard();
        isDragged = false;
    }
}
