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
    public string fileName = "Assets/Script/StartScreen/mode.txt";
    // Start is called before the first frame update
    void Start()
    {
        if(getModeFromFile()==0) {
            easyBtn.SetActive(true);
            hardBtn.SetActive(false);
        }
        else {
            easyBtn.SetActive(false);
            hardBtn.SetActive(true);
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
        saveModeToFile(1);
        hardBtn.SetActive(true);
        easyBtn.SetActive(false);
    }

    public void changeToEasy(){
        saveModeToFile(0);
        easyBtn.SetActive(true);
        hardBtn.SetActive(false);
    }

    public void loadRules(){
        SceneManager.LoadScene("Assets/Scenes/Rule.unity", LoadSceneMode.Additive);
    }

    public void loadGameMode(){
        SceneManager.LoadScene("Assets/Scenes/Mode.unity", LoadSceneMode.Additive);
    }

    public int getModeFromFile() {
        StreamReader sr = new StreamReader(fileName);
        char ch = (char)sr.Read();
        int mode = ch - '0';
        sr.Close();
        return mode;
    }

    public void saveModeToFile(int mode) {
        StreamWriter sw = new StreamWriter(fileName);
        sw.WriteLine(mode);
        sw.Close();
    }
}