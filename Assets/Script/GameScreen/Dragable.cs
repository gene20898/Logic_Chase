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
    [HideInInspector] public Transform parentAfterDrag;


    void Awake(){
        siblingIndex = transform.GetSiblingIndex();
    }

    public void OnBeginDrag(PointerEventData eventData){
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        // transform.SetAsLastSibling();
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData){
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData){
        transform.SetParent(parentAfterDrag);
        transform.SetSiblingIndex(siblingIndex);
        image.raycastTarget = true;
    }
}
