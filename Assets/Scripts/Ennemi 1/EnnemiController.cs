using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnnemiController : MonoBehaviour
{
    public Transform[] patrolPoints;
    public int targetPoint;
    public float speed;
    public float stopDuration = 2f;
    private bool isWaiting = false;

    void Start()
    {
        targetPoint = 0;
    }

    void Update()
    {
        if (isWaiting) return;

        if (transform.position == patrolPoints[targetPoint].position)
        {
            if (targetPoint == 2 || targetPoint == 3 || targetPoint == 5)
            {
                StartCoroutine(StopAtWaypoint());
            }
            else
            {
                increaseTargetInt();
            }
        }

        transform.position = Vector3.MoveTowards(transform.position, patrolPoints[targetPoint].position, speed * Time.deltaTime);
    }

    void increaseTargetInt()
    {
        targetPoint++;
        if (targetPoint >= patrolPoints.Length)
        {
            targetPoint = 0;
        }
    }

    IEnumerator StopAtWaypoint()
    {
        isWaiting = true;
        yield return new WaitForSeconds(stopDuration);
        isWaiting = false;
        increaseTargetInt();
    }


}
