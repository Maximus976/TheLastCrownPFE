using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogController : MonoBehaviour

{
    public Material fogMaterial;
    public float maskRadius = 3f; // Rayon de dispersion de la brume

    void Update()
    {
        if (fogMaterial != null)
        {
            Vector3 playerPos = transform.position;
            fogMaterial.SetVector("_MaskPos", new Vector4(playerPos.x, playerPos.y, playerPos.z, 1));
            fogMaterial.SetFloat("_MaskRadius", maskRadius);
        }
    }
}

