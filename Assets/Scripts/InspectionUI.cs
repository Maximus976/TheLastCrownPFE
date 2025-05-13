using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InspectionUI : MonoBehaviour
{
    public static InspectionUI Instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text descriptionText;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        panel.SetActive(false);
    }

    public void Show(string description)
    {
        descriptionText.text = description;
        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    public bool IsVisible() => panel.activeSelf;
}