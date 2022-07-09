using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Label : MonoBehaviour
{
    public int index;
    //my event
    [System.Serializable]
    public class UseEvent : UnityEvent { }

    [SerializeField]
    private UseEvent useEvent = new UseEvent();
    public UseEvent onUse { get { return onUse; } set { onUse = value; } }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
