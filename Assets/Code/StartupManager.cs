using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartupManager : MonoBehaviour
{
    private void Start()
    {
        PermDataHolder data = GameObject.Find("DataHolder").GetComponent<PermDataHolder>();

        data.Setup();

        SceneManager.LoadScene(1);
    }
}