using UnityEngine;

public class ColorRenderCamera : MonoBehaviour
{
    public Transform Player;
    public Material material;
    public string param_name;
    public float y_offset = 20;

    void Start()
    {

    }

    void Update()
    {
        transform.position = Player.position + Vector3.up * y_offset;
        Vector3 pos = transform.position;
        material.SetVector(param_name, new Vector4(pos.x, pos.z));
    }
}

