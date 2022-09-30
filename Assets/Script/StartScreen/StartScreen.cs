using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{

    bool easyMode = true;
    bool hardMode = false;
    public GameObject easyBtn;
    public GameObject hardBtn;
    // Start is called before the first frame update
    void Start()
    {
        if(easyMode) {
            easyBtn.SetActive(true);
            hardBtn.SetActive(false);
        }
        else {
            easyBtn.SetActive(true);
            hardBtn.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startGame(){
        SceneManager.LoadScene("Assets/Scenes/Game.unity");
    }
    public void changeToHard(){
        easyMode=false;
        hardBtn.SetActive(true);
        easyBtn.SetActive(false);
    }

    public void changeToEasy(){
        easyMode=true;
        easyBtn.SetActive(true);
        hardBtn.SetActive(false);
    }

    public void loadRules(){
        SceneManager.LoadScene("Assets/Scenes/Rule.unity", LoadSceneMode.Additive);
    }

    public void loadGameMode(){
        SceneManager.LoadScene("Assets/Scenes/Mode.unity", LoadSceneMode.Additive);
    }
}