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
    public GameObject submenuBg,quitTxt,quitYes,quitNo,submenuHighlight;
    public static Boolean isEasyMode = true;
    public static Boolean showQuitMenu = false;
    public static Boolean isCoroutineRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        if(isEasyMode) changeToEasy();
        else changeToHard();
        hideQuitConfirm();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isCoroutineRunning)
        {
            StartCoroutine(toggleSubMenuCoroutine());
        }
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

    public void exit(){
        Application.Quit();
    }

    private System.Collections.IEnumerator toggleSubMenuCoroutine()
    {
        isCoroutineRunning = true;

        if (showQuitMenu)
        {
            hideQuitConfirm();
        }
        else
        {
            showQuitConfirm();
        }

        yield return new WaitForEndOfFrame();

        showQuitMenu = !showQuitMenu;

        isCoroutineRunning = false;
    }

    public void hideQuitConfirm() {
        Debug.Log("hideQuitConfirm");
        submenuHighlight.SetActive(false);
        submenuBg.SetActive(false);
        quitTxt.SetActive(false);
        quitYes.SetActive(false);
        quitNo.SetActive(false);
    }
    public void showQuitConfirm() {
        Debug.Log("showQuitConfirm");
        submenuHighlight.SetActive(true);
        submenuBg.SetActive(true);
        quitTxt.SetActive(true);
        quitYes.SetActive(true);
        quitNo.SetActive(true);
    }
}