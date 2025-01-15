using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;// Nécessaire pour les événements de pointer

public class SelectionBouton : MonoBehaviour
{
    [Header("Buttons Settings")]
    public List<Button> buttons; // Liste des boutons à modifier
    public Color normalColor = Color.white; // Couleur par défaut
    public Color hoverColor = Color.gray; // Couleur au survol
    public float transitionSpeed = 5f; // Vitesse de transition des couleurs

    private Dictionary<Button, Image> buttonImages = new Dictionary<Button, Image>();
    private Dictionary<Button, bool> isHovering = new Dictionary<Button, bool>();

    void Start()
    {
        foreach (var button in buttons)
        {
            if (button == null) continue;

            // Récupère l'image de chaque bouton
            var buttonImage = button.GetComponent<Image>();

            if (buttonImage != null)
            {
                buttonImages[button] = buttonImage;
                isHovering[button] = false;

                // Assurez-vous que le bouton commence avec la couleur normale
                buttonImage.color = normalColor;

                // Ajoute les événements de survol et sortie
                var eventTrigger = button.gameObject.AddComponent<EventTrigger>();

                // PointerEnter
                var pointerEnter = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.PointerEnter
                };
                pointerEnter.callback.AddListener((eventData) => OnPointerEnter(button));
                eventTrigger.triggers.Add(pointerEnter);

                // PointerExit
                var pointerExit = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.PointerExit
                };
                pointerExit.callback.AddListener((eventData) => OnPointerExit(button));
                eventTrigger.triggers.Add(pointerExit);
            }
            else
            {
                Debug.LogWarning($"Aucun composant Image trouvé sur le bouton {button.name} !");
            }
        }
    }

    void Update()
    {
        foreach (var button in buttons)
        {
            if (buttonImages.ContainsKey(button))
            {
                // Transition fluide entre les couleurs
                var targetColor = isHovering[button] ? hoverColor : normalColor;
                buttonImages[button].color = Color.Lerp(
                    buttonImages[button].color,
                    targetColor,
                    Time.deltaTime * transitionSpeed
                );
            }
        }
    }

    private void OnPointerEnter(Button button)
    {
        if (isHovering.ContainsKey(button))
        {
            isHovering[button] = true;
        }
    }

    private void OnPointerExit(Button button)
    {
        if (isHovering.ContainsKey(button))
        {
            isHovering[button] = false;
        }
    }
}