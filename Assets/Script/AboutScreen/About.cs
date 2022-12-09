using System.Diagnostics;
using System.Security.Principal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class About : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void exit()
    {
        SceneManager.UnloadSceneAsync("Assets/Scenes/About.unity");
    }

}