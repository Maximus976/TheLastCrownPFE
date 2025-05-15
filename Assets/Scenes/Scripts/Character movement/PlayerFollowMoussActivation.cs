using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollowMoussActivation : MonoBehaviour
{
    [SerializeField] private PlayerFollowMouss cameraFollowScript;
    [SerializeField] private float activationDuration = 2f;
    [SerializeField] public float clickDelay = 2f;

    private float lastClickTime;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= lastClickTime + clickDelay)
        {
            lastClickTime = Time.time;
            StartCoroutine(ActivateCameraFollow());
        }
    }

    private IEnumerator ActivateCameraFollow()
    {
        cameraFollowScript.enabled = true;
        yield return new WaitForSeconds(activationDuration);
        cameraFollowScript.enabled = false;
    }
}
