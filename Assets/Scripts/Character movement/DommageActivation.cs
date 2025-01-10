using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DommageActivation : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;
    [SerializeField] private float activationDuration = 2f;
    [SerializeField] public float clickDelay = 2f;

    private float lastClickTime;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= lastClickTime + clickDelay)
        {
            lastClickTime = Time.time;
            StartCoroutine(ActivateGameObject());
        }
    }

    private IEnumerator ActivateGameObject()
    {
        targetObject.SetActive(true);
        yield return new WaitForSeconds(activationDuration);
        targetObject.SetActive(false);
    }
}
