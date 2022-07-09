using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMode : MonoBehaviour
{
    public GameObject arrow;
    // Start is called before the first frame update
    void Start()
    {
        if(GameModeManager.gameModeManager.isHard){
            arrow.GetComponent<RectTransform>().anchoredPosition = new Vector2(-460.0f,-145.0f);
        }else{
            arrow.GetComponent<RectTransform>().anchoredPosition = new Vector2(-460.0f,72.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void exit()
    {
        SceneManager.UnloadSceneAsync("Assets/Scenes/Mode.unity");
    }
    public void hardMode(){
        arrow.GetComponent<RectTransform>().anchoredPosition = new Vector2(-460.0f,-145.0f);
        GameModeManager.gameModeManager.SetHardMode(true);
    }
    public void easyMode(){
        arrow.GetComponent<RectTransform>().anchoredPosition = new Vector2(-460.0f,72.0f);
        GameModeManager.gameModeManager.SetHardMode(false);
    }

}
