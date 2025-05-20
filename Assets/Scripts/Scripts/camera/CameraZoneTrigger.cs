using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoneTrigger : MonoBehaviour
{
    public CinemachineVirtualCamera targetCamera;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SetCameraPriority(targetCamera);
        }
    }

    void SetCameraPriority(CinemachineVirtualCamera cam)
    {
        CinemachineVirtualCamera[] allCams = FindObjectsOfType<CinemachineVirtualCamera>();
        foreach (var vcam in allCams)
        {
            vcam.Priority = (vcam == cam) ? 20 : 10;
        }
    }
}
