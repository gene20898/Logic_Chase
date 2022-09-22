using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Output : MonoBehaviour
{
    public Board board;
    public Sprite[] outputSprites;
    public Sprite[] wireSprites;

    void Awake()
    {
        board.OnUpdate += UpdateOutput;
        board.OnReset += UpdateOutput;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void UpdateOutput()
    {
        Image[] outputImages = gameObject.transform.GetChild(0).GetComponentsInChildren<Image>();
        Image[] wireImages = gameObject.transform.GetChild(1).gameObject.GetComponentsInChildren<Image>();

        int[] outputs = board.getOutput();

        for (int i = 0; i < outputs.Length; i++)
        {
            if (outputs[i] == 1)
            {
                outputImages[i].sprite = outputSprites[1];
                wireImages[i].sprite = wireSprites[i+7];
            }
            else
            {
                outputImages[i].sprite = outputSprites[0];
                wireImages[i].sprite = wireSprites[i];
            }
        }
    }
}
