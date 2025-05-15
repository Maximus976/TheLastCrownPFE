using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CiccleSync : MonoBehaviour
{
    public Transform player;
    public LayerMask wallLayer;
    public float dissolveDistance = 2f;
    public float falloff = 1f;

    private Dictionary<GameObject, Material[]> modifiedWalls = new();

    void Update()
    {
        Vector3 camToPlayer = player.position - Camera.main.transform.position;
        float dist = camToPlayer.magnitude;

        RaycastHit[] hits = Physics.RaycastAll(Camera.main.transform.position, camToPlayer.normalized, dist, wallLayer);
        HashSet<GameObject> seenWalls = new();

        foreach (var hit in hits)
        {
            GameObject wall = hit.collider.gameObject;
            seenWalls.Add(wall);

            if (!modifiedWalls.ContainsKey(wall))
            {
                var rend = wall.GetComponent<Renderer>();
                if (!rend) continue;

                var instancedMats = new Material[rend.materials.Length];
                for (int i = 0; i < instancedMats.Length; i++)
                    instancedMats[i] = new Material(rend.materials[i]);

                rend.materials = instancedMats;
                modifiedWalls[wall] = instancedMats;
            }

            foreach (var mat in modifiedWalls[wall])
            {
                if (!mat.HasProperty("PlayerPos")) continue;

                mat.SetVector("PlayerPos", player.position);
                mat.SetFloat("DissolveDistance", dissolveDistance);
                mat.SetFloat("DissolveFalloff", falloff);
            }
        }

        // Mur qui ne bloquent plus la vue ? disable dissolve
        foreach (var wall in modifiedWalls.Keys)
        {
            if (!seenWalls.Contains(wall))
            {
                foreach (var mat in modifiedWalls[wall])
                {
                    if (!mat.HasProperty("DissolveDistance")) continue;
                    mat.SetFloat("DissolveDistance", -999f); // éloigne le seuil, effet désactivé
                }
            }
        }
    }
}
