using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Playerinspect : MonoBehaviour
{
    [SerializeField] private Image holdProgressImage;
    [SerializeField] private float holdDuration = 1.0f;

    private InspectableObject currentZone;
    private float holdTimer = 0f;
    private bool isHolding = false;

    void Update()
    {
        bool holding = Input.GetKey(KeyCode.JoystickButton2);

        if (currentZone != null && holding)
        {
            if (!isHolding)
            {
                isHolding = true;
                holdTimer = 0f;
                holdProgressImage.gameObject.SetActive(true);
            }

            holdTimer += Time.deltaTime;
            holdProgressImage.fillAmount = holdTimer / holdDuration;

            if (holdTimer >= holdDuration)
            {
                if (!InspectionUI.Instance.IsVisible())
                {
                    InspectionUI.Instance.Show(currentZone.GetDescription());
                }
                else
                {
                    InspectionUI.Instance.Hide();
                }

                // Réinitialise tout
                isHolding = false;
                holdProgressImage.fillAmount = 0f;
                holdProgressImage.gameObject.SetActive(false);
            }
        }
        else if (!holding && isHolding)
        {
            // Si on relâche avant la fin
            isHolding = false;
            holdTimer = 0f;
            holdProgressImage.fillAmount = 0f;
            holdProgressImage.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        InspectableObject zone = other.GetComponent<InspectableObject>();
        if (zone != null)
            currentZone = zone;
    }

    private void OnTriggerExit(Collider other)
    {
        InspectableObject zone = other.GetComponent<InspectableObject>();
        if (zone != null && zone == currentZone)
            currentZone = null;
    }
}