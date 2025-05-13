using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleSceneNarration : MonoBehaviour
{
    public TMP_Text messageText;
    public string message;
    public float autoReturnTime = 5f;
    public string returnScene = "Exploration";

    private float timer = 0f;
    private bool exited = false;

    void Start()
    {
        if (messageText != null)
            messageText.text = message;

        Debug.Log("Narration started. Will return to: " + returnScene);
    }

    void Update()
    {
        if (exited) return;

        timer += Time.deltaTime;

        if (timer >= autoReturnTime)
        {
            Debug.Log("Auto-return triggered");
            ExitScene();
        }

        if (Input.GetKeyDown(KeyCode.JoystickButton2))
        {
            Debug.Log("Manual return triggered (JoystickButton2)");
            ExitScene();
        }
    }

    void ExitScene()
    {
        exited = true;
        Debug.Log("Loading scene: " + returnScene);
        SceneManager.LoadScene(returnScene);
    }
}