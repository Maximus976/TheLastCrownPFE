using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveEffect : MonoBehaviour
{
    private MeshRenderer meshrenderer;
    private MaterialPropertyBlock propertyblock;
    public  float fadeduration;
    private float fadetime;
    private void Start()
    {
        meshrenderer = GetComponent<MeshRenderer>();
        propertyblock = new MaterialPropertyBlock();
        meshrenderer.GetPropertyBlock(propertyblock);
        fadetime=fadeduration;
    }
    private void Update()
    {
        fadetime -= Time.deltaTime;
        float faderatio = fadetime / fadeduration;
        print("Debug");
        propertyblock.SetFloat("_Dissolve", 1-faderatio);
        meshrenderer.SetPropertyBlock(propertyblock);
    }

}
