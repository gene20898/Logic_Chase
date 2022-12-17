using System.Runtime.Serialization.Json;
using System;
using System.Xml.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class StartScreen : MonoBehaviour
{

    public GameObject easyBtn;
    public GameObject hardBtn;
    public static Boolean isEasyMode = true;

    // Start is called before the first frame update
    void Start()
    {
        if(isEasyMode) changeToEasy();
        else changeToHard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startGame(){
        SceneManager.LoadScene("Assets/Scenes/Game.unity");
    }
    public void changeToHard(){
        hardBtn.SetActive(true);
        easyBtn.SetActive(false);
        isEasyMode = false;
    }

    public void changeToEasy(){
        easyBtn.SetActive(true);
        hardBtn.SetActive(false);
        isEasyMode = true;
    }

    public void loadRules(){
        SceneManager.LoadScene("Assets/Scenes/Rule.unity", LoadSceneMode.Additive);
    }

    public void loadGameMode(){
        SceneManager.LoadScene("Assets/Scenes/Mode.unity", LoadSceneMode.Additive);
    }

    public void loadAbout(){
        SceneManager.LoadScene("Assets/Scenes/About.unity", LoadSceneMode.Additive);
    }
}