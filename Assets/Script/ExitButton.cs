using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ExitButton : MonoBehaviour
{
    public Sprite[] rules;
    public Image rule;

    public GameObject next_button;
    public GameObject prev_button;
    int page_num = 0;
    // Start is called before the first frame update
    void Start()
    {
        prev_button.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void exit()
    {
        SceneManager.UnloadSceneAsync("Assets/Scenes/Rule.unity");
    }
    public void next()
    {
        prev_button.SetActive(true);
        page_num++;
        if (page_num == rules.Length - 1)
        {
            next_button.SetActive(false);
        }
        rule.sprite = rules[page_num];
    }
    public void prev()
    {
        next_button.SetActive(true);
        page_num--;
        if (page_num == 0)
        {
            prev_button.SetActive(false);
        }
        rule.sprite = rules[page_num];
    }
}
