using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveMur : MonoBehaviour
{
    [Header("Références")]
    public Transform player;
    public Camera cam;
    public LayerMask wallLayer;

    [Header("Dissolve Settings")]
    public float dissolveDistance = 1.0f;
    public float dissolveFalloff = 1.0f;

    private Dictionary<GameObject, Material[]> modifiedWalls = new Dictionary<GameObject, Material[]>();

    void Update()
    {
        Vector3 camPos = cam.transform.position;
        Vector3 dir = (player.position - camPos).normalized;
        float dist = Vector3.Distance(camPos, player.position);

        RaycastHit[] hits = Physics.RaycastAll(camPos, dir, dist, wallLayer);
        HashSet<GameObject> wallsThisFrame = new HashSet<GameObject>();

        foreach (var hit in hits)
        {
            GameObject wall = hit.collider.gameObject;
            wallsThisFrame.Add(wall);

            if (!modifiedWalls.ContainsKey(wall))
            {
                Renderer rend = wall.GetComponent<Renderer>();
                if (rend == null) continue;

                Material[] originalMats = rend.sharedMaterials; // prend les materials assignés dans l’inspecteur
                Material[] instancedMats = new Material[originalMats.Length];

                for (int i = 0; i < originalMats.Length; i++)
                {
                    instancedMats[i] = new Material(originalMats[i]); // on instancie une copie
                }

                rend.materials = instancedMats; // on assigne les copies instanciées
                modifiedWalls[wall] = instancedMats;
            }

            foreach (var mat in modifiedWalls[wall])
            {
                if (mat.HasProperty("_DissolveDistance"))
                {
                    mat.SetFloat("_DissolveDistance", dissolveDistance);
                    mat.SetFloat("_DissolveFalloff", dissolveFalloff);
                    mat.SetVector("_PlayerPosition", player.position);
                }
            }
        }

        // Restaure les murs non touchés ce frame
        List<GameObject> toClear = new List<GameObject>();
        foreach (var wall in modifiedWalls.Keys)
        {
            if (!wallsThisFrame.Contains(wall))
            {
                foreach (var mat in modifiedWalls[wall])
                {
                    if (mat.HasProperty("_DissolveDistance"))
                        mat.SetFloat("_DissolveDistance", -999f); // invisible
                }

                toClear.Add(wall);
            }
        }

        foreach (var wall in toClear)
        {
            modifiedWalls.Remove(wall);
        }
    }
}