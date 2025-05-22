using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyPoison : MonoBehaviour
{
    public float duration = 2f;

    void Start()
    {
        Destroy(gameObject, duration);
    }
}