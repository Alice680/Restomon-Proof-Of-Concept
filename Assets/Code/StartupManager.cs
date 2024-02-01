using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Handles anything that needs to be taken care of it launch. 
 * 
 * Notes:
 * Only runs one time.
 * May not even need. Keeping for now just to be safe.
 */
public class StartupManager : MonoBehaviour
{
    private void Start()
    {
        PermDataHolder data = GameObject.Find("DataHolder").GetComponent<PermDataHolder>();

        data.SetupData();

        SceneManager.LoadScene(1);
    }
}