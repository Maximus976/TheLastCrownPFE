using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class NextSceneDoor : MonoBehaviour
{
    public string teleportPointName = "BouclierSpawn";
    public Canvas loadingCanvas;
    public UnityEngine.UI.Image fadeImage;
    public float fadeDuration = 1f;
    public float loadingDuration = 2f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(TeleportSequence(other.gameObject));
        }
    }

    IEnumerator TeleportSequence(GameObject player)
    {
        // Bloquer le joueur (désactiver contrôle)
        var controller = player.GetComponent<CustomMovement>();
        if (controller != null)
            controller.enabled = false;

        // Stopper les animations
        Animator animator = player.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetFloat("Speed", 0f); // exemple selon ton Animator
            animator.SetBool("isRunning", false); // exemple selon ton Animator
            animator.speed = 0f; // stoppe l'animation
        }

        // Fade in écran noir (transparent ? opaque)
        yield return StartCoroutine(Fade(0f, 1f));

        // Activer canvas loading
        if (loadingCanvas != null)
            loadingCanvas.gameObject.SetActive(true);

        // Téléportation
        GameObject targetPoint = GameObject.Find(teleportPointName);
        if (targetPoint != null)
        {
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }

            player.transform.position = targetPoint.transform.position;
            player.transform.rotation = targetPoint.transform.rotation;

            if (rb)
            {
                yield return new WaitForSeconds(0.1f);
                rb.isKinematic = false;
            }
        }
        else
        {
            Debug.LogError("Point de téléportation non trouvé : " + teleportPointName);
        }

        // Attendre la durée du canvas loading
        yield return new WaitForSeconds(loadingDuration);

        // Désactiver canvas loading
        if (loadingCanvas != null)
            loadingCanvas.gameObject.SetActive(false);

        // Fade out écran noir (opaque ? transparent)
        yield return StartCoroutine(Fade(1f, 0f));

        // Débloquer joueur (réactiver contrôle)
        if (controller != null)
            controller.enabled = true;

        // Reprendre animations
        if (animator != null)
        {
            animator.speed = 1f; // reprendre animation normale
            // Remets à jour tes bools/floats ici si besoin
        }
    }

    IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float timer = 0f;
        Color c = fadeImage.color;

        if (startAlpha < endAlpha && !fadeImage.gameObject.activeSelf)
            fadeImage.gameObject.SetActive(true);

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, timer / fadeDuration);
            fadeImage.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }
        fadeImage.color = new Color(c.r, c.g, c.b, endAlpha);

        if (startAlpha > endAlpha)
            fadeImage.gameObject.SetActive(false);
    }
}