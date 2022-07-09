using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    public static GameModeManager gameModeManager;
    public bool isHard = false;
    private void Awake(){
        DontDestroyOnLoad(this.gameObject);
        if (gameModeManager == null) {
            gameModeManager = this;
        } else {
            Destroy(gameObject); // Used Destroy instead of DestroyObject
        }
    }

    public void SetHardMode(bool isHard){
        this.isHard = isHard;
    }
}
