using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ClosePopUp : MonoBehaviour
{
    public GameObject popUpPanel;       // Le panel du popup
    public Image circleImage;           // Image du cercle (type Filled)
    public float holdDuration = 1.5f;   // Temps nécessaire pour charger complètement

    private float holdTimer = 0f;

    void Update()
    {
        if (popUpPanel.activeSelf)
        {
            if (Input.GetKey(KeyCode.JoystickButton2))
            {
                // On maintient : on augmente le timer
                holdTimer += Time.unscaledDeltaTime;
                circleImage.fillAmount = holdTimer / holdDuration;

                if (holdTimer >= holdDuration)
                {
                    ClosePopup();
                }
            }
            else
            {
                // Si on relâche : on vide doucement le cercle
                holdTimer = Mathf.MoveTowards(holdTimer, 0f, Time.unscaledDeltaTime * 2f);
                circleImage.fillAmount = holdTimer / holdDuration;
            }
        }
    }

    void ClosePopup()
    {
        popUpPanel.SetActive(false);
        Time.timeScale = 1f;
        holdTimer = 0f;
        circleImage.fillAmount = 0f;
    }
}