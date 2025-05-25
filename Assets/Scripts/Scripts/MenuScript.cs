using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public class MenuScript : MonoBehaviour
{
    public List<CustomMenuItem> menuItems;
    private int currentIndex = 0;
    private bool inputLocked = false;

    [Header("Audio")]
    public AudioSource navigateAudioSource;
    public AudioSource selectAudioSource;

    void Start()
    {
        if (menuItems.Count > 0)
        {
            UpdateSelection();
        }
    }

    void Update()
    {
        if (menuItems.Count == 0)
            return;

        float vertical = Input.GetAxisRaw("Vertical");

        if (!inputLocked && Mathf.Abs(vertical) > 0.5f)
        {
            currentIndex = (vertical < 0)
                ? (currentIndex + 1) % menuItems.Count
                : (currentIndex - 1 + menuItems.Count) % menuItems.Count;

            PlayNavigateSound();
            UpdateSelection();
            StartCoroutine(UnlockInputAfterDelay(0.2f));
        }

        if (Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            PlaySelectSound();
            menuItems[currentIndex].Select();
        }
    }

    public void UpdateSelection()
    {
        currentIndex = Mathf.Clamp(currentIndex, 0, menuItems.Count - 1);
        for (int i = 0; i < menuItems.Count; i++)
        {
            menuItems[i].SetSelected(i == currentIndex);
        }
    }

    private IEnumerator UnlockInputAfterDelay(float delay)
    {
        inputLocked = true;
        yield return new WaitForSeconds(delay);
        inputLocked = false;
    }

    private void PlayNavigateSound()
    {
        if (navigateAudioSource != null)
            navigateAudioSource.Play();
    }

    private void PlaySelectSound()
    {
        if (selectAudioSource != null)
            selectAudioSource.Play();
    }
}