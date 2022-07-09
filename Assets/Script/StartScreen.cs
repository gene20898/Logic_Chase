using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startGame(){
        SceneManager.LoadScene("Assets/Scenes/Bot vs bot.unity");
    }
    public void exit(){
        Application.Quit();
    }

    public void loadRules(){
        SceneManager.LoadScene("Assets/Scenes/Rule.unity", LoadSceneMode.Additive);
    }

    public void loadGameMode(){
        SceneManager.LoadScene("Assets/Scenes/Mode.unity", LoadSceneMode.Additive);
    }
}
