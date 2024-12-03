using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : MonoBehaviour
{
    [SerializeField] private Transform TargetToFlee;
    [SerializeField] private float DistanceToFlee = 5f;
    //[SerializeField] private Patrol PatrolBehaviour; 

    private Vector3 wanderPosition;

    private float timeLastWandered = 0;

    public IAstarAI AstarAI;

    void OnEnable()
    {
        AstarAI = GetComponent<IAstarAI>();
        // Update the destination right before searching for a path as well.
        // This is enough in theory, but this script will also update the destination every
        // frame as the destination is used for debugging and may be used for other things by other
        // scripts as well. So it makes sense that it is up to date every frame.

        if (TargetToFlee is null)
        {
            return;
        }

        if (AstarAI != null)
        {
            AstarAI.onSearchPath += UpdateFlee;

            var wanderposition = Random.insideUnitCircle;
            AstarAI.destination = transform.position + new Vector3(wanderposition.x, 0, wanderposition.y) * DistanceToFlee;
            timeLastWandered = Time.time;
            wanderPosition = AstarAI.destination;
            AstarAI.isStopped = false;
        }
    }

    void OnDisable()
    {
        if (AstarAI != null) AstarAI.onSearchPath -= UpdateFlee;
    }


    private void UpdateFlee()
    {
        var offsetVector = transform.position - TargetToFlee.position;
        offsetVector.y = 0;

        if (offsetVector.magnitude > DistanceToFlee)
        {
            if (wanderPosition != Vector3.zero)
            {
                var wanderOffsetVector = transform.position - wanderPosition;
                wanderOffsetVector.y = 0;

                //recalculate wander position
                if (wanderOffsetVector.magnitude < 0.5f || Time.time - timeLastWandered > 10f)
                {
                    var wanderposition = Random.insideUnitCircle;

                    AstarAI.destination = transform.position + new Vector3(wanderposition.x, 0, wanderposition.y) * DistanceToFlee;
                    timeLastWandered = Time.time;
                    wanderPosition = AstarAI.destination;
                }
            }

            return;
        }

        var normalizedOffset = offsetVector.normalized;

        var desiredposition = transform.position + (normalizedOffset * DistanceToFlee);

        AstarAI.destination = desiredposition;
        AstarAI.isStopped = false;
    }


    private void OnDrawGizmos()
    {
        if (AstarAI is null)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(AstarAI.destination, 0.1f);
    }
}
