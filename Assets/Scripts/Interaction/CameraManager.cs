using System.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera playerCamera;       // active par défaut
    public Camera doorCamera;         // désactivée par défaut

    public Transform doorTransform;   // La porte à faire descendre
    public Vector3 doorOffset = new Vector3(0, -3f, 0); // Déplacement vers le bas
    public float doorMoveSpeed = 2f;
    public float cameraHoldTime = 3f;

    private Vector3 doorStartPosition;

    private void Start()
    {
        if (doorTransform != null)
            doorStartPosition = doorTransform.position;

        if (doorCamera != null)
            doorCamera.enabled = false;

        if (playerCamera != null)
            playerCamera.enabled = true;
    }

    public void ShowDoorSequence()
    {
        StartCoroutine(SwitchCameraAndMoveDoor());
    }

    IEnumerator SwitchCameraAndMoveDoor()
    {
        playerCamera.enabled = false;
        doorCamera.enabled = true;

        yield return StartCoroutine(MoveDoorDown());

        yield return new WaitForSeconds(cameraHoldTime);

        doorCamera.enabled = false;
        playerCamera.enabled = true;
    }

    IEnumerator MoveDoorDown()
    {
        Vector3 targetPosition = doorStartPosition + doorOffset;

        while (Vector3.Distance(doorTransform.position, targetPosition) > 0.01f)
        {
            doorTransform.position = Vector3.MoveTowards(doorTransform.position, targetPosition, doorMoveSpeed * Time.deltaTime);
            yield return null;
        }
    }
}