using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DalleInterrupteur : MonoBehaviour
{
    public Transform dalle;              // La dalle à enfoncer
    public float enfoncementY = -0.2f;   // Distance vers le bas
    public float vitesseDalle = 2f;

    public Transform pillier1;
    public Transform pillier2;
    public float hauteurPilliers = 3f;
    public float vitessePilliers = 1f;
    public float delaiEntrePilliers = 1f;

    [Header("Sons")]
    public AudioSource pillier1Sound;
    public AudioSource pillier2Sound;

    private Vector3 dalleInitPos;
    private bool actif = false;

    private void Start()
    {
        dalleInitPos = dalle.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !actif)
        {
            actif = true;
            StartCoroutine(ActiverSequence());
        }
    }

    private IEnumerator ActiverSequence()
    {
        // 1. Enfoncer la dalle
        Vector3 dalleCible = dalleInitPos + new Vector3(0, enfoncementY, 0);
        while (Vector3.Distance(dalle.position, dalleCible) > 0.01f)
        {
            dalle.position = Vector3.MoveTowards(dalle.position, dalleCible, vitesseDalle * Time.deltaTime);
            yield return null;
        }

        // 2. Lever le premier pillier
        if (pillier1Sound != null)
            pillier1Sound.Play();

        Vector3 posP1Init = pillier1.position;
        Vector3 posP1Final = posP1Init + new Vector3(0, hauteurPilliers, 0);
        while (Vector3.Distance(pillier1.position, posP1Final) > 0.01f)
        {
            pillier1.position = Vector3.MoveTowards(pillier1.position, posP1Final, vitessePilliers * Time.deltaTime);
            yield return null;
        }

        // 3. Petit délai
        yield return new WaitForSeconds(delaiEntrePilliers);

        // 4. Lever le second pillier
        if (pillier2Sound != null)
            pillier2Sound.Play();

        Vector3 posP2Init = pillier2.position;
        Vector3 posP2Final = posP2Init + new Vector3(0, hauteurPilliers, 0);
        while (Vector3.Distance(pillier2.position, posP2Final) > 0.01f)
        {
            pillier2.position = Vector3.MoveTowards(pillier2.position, posP2Final, vitessePilliers * Time.deltaTime);
            yield return null;
        }
    }
}