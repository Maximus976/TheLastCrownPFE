using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class cameracutscene : MonoBehaviour
{
    public CinemachineVirtualCamera cutsceneCamera;
    public float cutsceneDuration = 2.5f;

    private int defaultPriority = 0;
    private int activePriority = 20;

    private void Start()
    {
        if (cutsceneCamera != null)
        {
            cutsceneCamera.Priority = defaultPriority;
        }
    }

    public void PlayCutscene()
    {
        if (cutsceneCamera != null)
        {
            Debug.Log("?? Cutscene démarrée");
            cutsceneCamera.Priority = activePriority;
            StartCoroutine(CutsceneRoutine());
        }
    }

    private IEnumerator CutsceneRoutine()
    {
        yield return new WaitForSeconds(cutsceneDuration);
        Debug.Log("?? Cutscene terminée");

        if (cutsceneCamera != null)
        {
            cutsceneCamera.Priority = defaultPriority;
        }
    }
}
