using UnityEngine;

public class PlayerShadowController : MonoBehaviour
{
    [Tooltip("GameObject représentant l'ombre du joueur (avec un shader overlay).")]
    public GameObject playerShadow;

    [Tooltip("Les renderers du joueur (sans l'ombre). Assigne ici tous les renderers qui affichent le joueur normal.")]
    public Renderer[] playerRenderers;

    [Tooltip("Layer des obstacles qui peuvent occlure le joueur.")]
    public LayerMask obstacleLayer;

    [Tooltip("Marge pour le raycast.")]
    public float rayMargin = 0.1f;

    void Update()
    {
        Camera cam = Camera.main;
        if (cam == null)
            return;

        Vector3 camPos = cam.transform.position;
        Vector3 playerPos = transform.position;
        Vector3 direction = (playerPos - camPos).normalized;
        float distance = Vector3.Distance(camPos, playerPos) - rayMargin;

        // On lance un raycast depuis la caméra vers le joueur
        bool occluded = Physics.Raycast(camPos, direction, out RaycastHit hit, distance, obstacleLayer);

        if (occluded)
        {
            // Le joueur est caché : on désactive ses renderers et on active son ombre
            SetPlayerRenderersEnabled(false);
            if (playerShadow != null && !playerShadow.activeSelf)
            {
                playerShadow.SetActive(true);
            }
        }
        else
        {
            // Le joueur n'est pas caché : on active ses renderers et on désactive l'ombre
            SetPlayerRenderersEnabled(true);
            if (playerShadow != null && playerShadow.activeSelf)
            {
                playerShadow.SetActive(false);
            }
        }
    }

    void SetPlayerRenderersEnabled(bool enabled)
    {
        if (playerRenderers != null)
        {
            foreach (Renderer r in playerRenderers)
            {
                if (r != null)
                {
                    r.enabled = enabled;
                }
            }
        }
    }
}
