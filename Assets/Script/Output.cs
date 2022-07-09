using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Output : MonoBehaviour
{
    public BoardScript board;
    public Sprite[] outputSprites;
    public Sprite[] wireSprites;
    // Start is called before the first frame update
    void Awake()
    {
        board.OnChange += UpdateOutput;
        board.OnReset += UpdateOutput;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void UpdateOutput()
    {
        Image[] outputImages = gameObject.transform.GetChild(0).GetComponentsInChildren<Image>();
        Image[] wireImages = gameObject.transform.GetChild(1).gameObject.GetComponentsInChildren<Image>();

        int[] outputs = board.getOutput();

        for (int i = 0; i < outputs.Length; i++)
        {
            if (outputs[i] == 1)
            {
                if (i % 2 == 0)
                {
                    outputImages[i].sprite = outputSprites[3];
                }
                else
                {
                    outputImages[i].sprite = outputSprites[1];
                }
                wireImages[i].sprite = wireSprites[i+7];
            }
            else
            {
                if (i % 2 == 0) 
                {
                    outputImages[i].sprite = outputSprites[2];
                }
                else
                {
                    outputImages[i].sprite = outputSprites[0];
                }
                wireImages[i].sprite = wireSprites[i];
            }
        }

    }
}
