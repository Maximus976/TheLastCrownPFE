using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossZone : MonoBehaviour
{
    public BoosIntro bossUI;
    public string bossName = "Mordred";

    private bool triggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;
            bossUI.PlayIntro(bossName);
        }
    }
}