using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour
{
    public float Speed = 1.0f;
    public GameObject Astar;
    public bool isWalking;
    public bool isPathFinished;
    List<NodePathfinding> mPath;
    
    int targetIndex;

    /***************************************************************************/

    void Start()
    {
        isWalking = false;
        isPathFinished = false;
    }

    /***************************************************************************/

    public void GoToNextMachine(Vector4 machinePos)
    {
        isWalking = false;
        isPathFinished = false;
        Astar.GetComponent<Pathfinding>().mTarget.position = machinePos;

        if(machinePos.w == 1.0f)
            mPath = Astar.GetComponent<Pathfinding>().FindPath(transform.position, machinePos, -1);

        // If a path was found follow it
        if (mPath != null)
        {
            isWalking = true;
            targetIndex = 0;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    public void StopWalking()
    {
        StopCoroutine("FollowPath");
    }

    /***************************************************************************/

    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = mPath[0].mWorldPosition;
        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= mPath.Count)
                {
                    isPathFinished = true;
                    yield break;
                }
                currentWaypoint = mPath[targetIndex].mWorldPosition;
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, Speed * Time.deltaTime);
            transform.LookAt(currentWaypoint);
            yield return null;

        }
    }

    /***************************************************************************/
    
    public void OnDrawGizmos()
    {
        if (mPath != null)
        {
            for (int i = targetIndex; i < mPath.Count; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(mPath[i].mWorldPosition, Vector3.one);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, mPath[i].mWorldPosition);
                }
                else
                {
                    Gizmos.DrawLine(mPath[i - 1].mWorldPosition, mPath[i].mWorldPosition);
                }
            }
        }
    }

    /***************************************************************************/

}
