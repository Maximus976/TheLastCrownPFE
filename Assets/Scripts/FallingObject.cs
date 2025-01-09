using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObject : MonoBehaviour
{
    [Header("Menu Items Settings")]
    public RectTransform[] menuItems;  // Liste des �l�ments du menu (boutons, images, etc.)
    public Vector3[] targetPositions; // Positions cibles pour chaque �l�ment
    public float speed = 5f;          // Vitesse du mouvement vers la position cible
    public float delay = 0.2f;        // D�lai entre chaque �l�ment

    [Header("Rotation Settings")]
    public Vector3 targetRotation = new Vector3(0, 0, 360); // Rotation finale (par d�faut : un tour complet)
    public float rotationSpeed = 2f;                        // Vitesse de la rotation (plus bas = plus fluide)
    public float rotationDuration = 1f;                     // Dur�e de la rotation avant de se bloquer

    void Start()
    {
        StartCoroutine(MoveMenuItems()); // D�marre l'animation des �l�ments du menu
    }

    IEnumerator MoveMenuItems()
    {
        // Parcourir chaque �l�ment du menu
        for (int i = 0; i < menuItems.Length; i++)
        {
            RectTransform item = menuItems[i];

            // D�placer l'�l�ment vers sa position cible
            yield return StartCoroutine(MoveToPosition(item, targetPositions[i]));

            // Effectuer une rotation imm�diatement apr�s l'arriv�e
            yield return StartCoroutine(SmoothRotateItem(item));
        }
    }

    IEnumerator MoveToPosition(RectTransform item, Vector3 target)
    {
        // Tant que l'�l�ment n'est pas arriv� � destination
        while (Vector3.Distance(item.localPosition, target) > 0.1f)
        {
            // D�placement constant vers la cible
            item.localPosition = Vector3.MoveTowards(item.localPosition, target, speed * Time.deltaTime);
            yield return null; // Attendre le prochain frame
        }

        // Corrige la position finale pour �viter les petits �carts
        item.localPosition = target;
    }

    IEnumerator SmoothRotateItem(RectTransform item)
    {
        Quaternion initialRotation = item.localRotation;
        Quaternion finalRotation = Quaternion.Euler(targetRotation);
        float elapsedTime = 0f;

        // Effectuer une interpolation fluide de la rotation
        while (elapsedTime < rotationDuration)
        {
            // Interpoler la rotation
            item.localRotation = Quaternion.Lerp(initialRotation, finalRotation, elapsedTime / rotationDuration);
            elapsedTime += Time.deltaTime * rotationSpeed;
            yield return null; // Attendre le prochain frame
        }

        // Bloque la rotation (fixe la rotation finale pr�cis�ment)
        item.localRotation = finalRotation;
    }
}



