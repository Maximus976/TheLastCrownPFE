using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyMap : MonoBehaviour
{
    [Header("UI Elements")]
    public Dropdown actionDropdown;  // Le Dropdown pour choisir l'action
    public Text currentKeyText;      // Le texte qui affichera la touche actuelle pour l'action
    public Button changeKeyButton;   // Le bouton pour changer la touche
    public Button saveButton;        // Le bouton pour sauvegarder les changements

    private string[] actions = { "Déplacer à gauche", "Déplacer à droite", "Sauter", "Attaquer" };  // Les actions à remapper
    private string[] controls = { "A", "D", "Space", "LeftControl" };  // Les touches par défaut
    private bool isWaitingForKey = false; // Vérifie si nous attendons une nouvelle touche

    private int currentActionIndex = 0;  // Indice de l'action actuelle sélectionnée

    void Start()
    {
        // Initialisation du Dropdown
        actionDropdown.AddOptions(new System.Collections.Generic.List<string>(actions));
        actionDropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        // Initialisation des boutons
        changeKeyButton.onClick.AddListener(OnChangeKeyButtonClicked);
        saveButton.onClick.AddListener(OnSaveButtonClicked);

        // Afficher la touche actuelle pour l'action par défaut
        UpdateKeyDisplay();
    }

    // Mise à jour du texte affichant la touche actuelle pour l'action
    void UpdateKeyDisplay()
    {
        currentKeyText.text = "Touche actuelle: " + controls[currentActionIndex];
    }

    // Lorsqu'une nouvelle action est sélectionnée dans le Dropdown
    void OnDropdownValueChanged(int index)
    {
        currentActionIndex = index;
        UpdateKeyDisplay();
    }

    // Lorsque l'utilisateur clique sur le bouton pour changer la touche
    void OnChangeKeyButtonClicked()
    {
        if (!isWaitingForKey)
        {
            StartCoroutine(WaitForKey());
        }
    }

    // Attendre que l'utilisateur appuie sur une touche
    IEnumerator WaitForKey()
    {
        isWaitingForKey = true;
        currentKeyText.text = "Appuyez sur une touche...";

        // Attendre que l'utilisateur appuie sur une touche
        while (!Input.anyKeyDown)
        {
            yield return null;
        }

        // Récupérer la touche pressée
        KeyCode pressedKey = GetPressedKey();

        // Assigner la nouvelle touche
        controls[currentActionIndex] = pressedKey.ToString();
        UpdateKeyDisplay();

        isWaitingForKey = false;
    }

    // Récupérer la touche pressée
    KeyCode GetPressedKey()
    {
        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
            {
                return key;
            }
        }
        return KeyCode.None; // Retourne un code vide si aucune touche n'est pressée
    }

    // Sauvegarder les changements de touches (peut être utilisé pour sauvegarder dans PlayerPrefs)
    void OnSaveButtonClicked()
    {
        // Exemple de sauvegarde dans PlayerPrefs, mais vous pouvez adapter selon votre système
        for (int i = 0; i < actions.Length; i++)
        {
            PlayerPrefs.SetString(actions[i], controls[i]);
        }
        PlayerPrefs.Save();

        Debug.Log("Touche(s) sauvegardée(s) !");
    }
}