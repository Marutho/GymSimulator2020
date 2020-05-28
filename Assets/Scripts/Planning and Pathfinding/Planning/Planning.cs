using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Planning : MonoBehaviour
{
    NodePlanning CurrentStartNode;
    NodePlanning CurrentTargetNode;

    World mWorld;
    public GymWorld mGymWorld;

    /***************************************************************************/

    private void Awake()
    {
        mWorld = GetComponent<World>();
    }
    

    /***************************************************************************/

    public List<NodePlanning> FindPlan(World.WorldState startWorldState, World.WorldState targetWorldState)
    {
        CurrentStartNode = new NodePlanning(startWorldState, null);
        CurrentTargetNode = new NodePlanning(targetWorldState, null);

        List<NodePlanning> openSet = new List<NodePlanning>();
        HashSet<NodePlanning> closedSet = new HashSet<NodePlanning>();
        openSet.Add(CurrentStartNode);
        mWorld.openSet = openSet;

        NodePlanning node = CurrentStartNode;
        while (openSet.Count > 0 && ((node.mWorldState & CurrentTargetNode.mWorldState) != CurrentTargetNode.mWorldState))
        {
            // Select best node from open list
            node = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < node.fCost || (openSet[i].fCost == node.fCost && openSet[i].hCost < node.hCost))
                {
                    node = openSet[i];
                }
            }

            // Manage open/closed list
            openSet.Remove(node);
            closedSet.Add(node);
            mWorld.openSet = openSet;
            mWorld.closedSet = closedSet;



            // Check destination
            if (((node.mWorldState & CurrentTargetNode.mWorldState) != CurrentTargetNode.mWorldState))
            {

                // Open neighbours
                foreach (NodePlanning neighbour in mWorld.GetNeighbours(node))
                {
                    if ( /*!neighbour.mWalkable ||*/ closedSet.Any(n => n.mWorldState == neighbour.mWorldState))
                    {
                        continue;
                    }

                    float newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
                    if (newCostToNeighbour < neighbour.gCost || !openSet.Any(n => n.mWorldState == neighbour.mWorldState))
                    {
                        neighbour.gCost = newCostToNeighbour;
                        neighbour.hCost = Heuristic(neighbour, CurrentTargetNode);
                        neighbour.mParent = node;

                        if (!openSet.Any(n => n.mWorldState == neighbour.mWorldState))
                        {
                            openSet.Add(neighbour);
                            mWorld.openSet = openSet;
                        }
                        else
                        {
                            // Find neighbour and replace
                            openSet[openSet.FindIndex(x => x.mWorldState == neighbour.mWorldState)] = neighbour;
                        }
                    }
                }
            }
            else
            {
                // Path found!

                // End node must be copied
                CurrentTargetNode.mParent = node.mParent;
                CurrentTargetNode.mAction = node.mAction;
                CurrentTargetNode.gCost = node.gCost;
                CurrentTargetNode.hCost = node.hCost;

                RetracePlan(CurrentStartNode, CurrentTargetNode);

                
                Debug.Log("Statistics:");
                Debug.LogFormat("Total nodes:  {0}", openSet.Count + closedSet.Count);
                Debug.LogFormat("Open nodes:   {0}", openSet.Count);
                Debug.LogFormat("Closed nodes: {0}", closedSet.Count);
                
            }
        }

        // Log plan
        //Debug.Log("PLAN FOUND!");
        for (int i = 0; i < mWorld.plan.Count; ++i)
        {
            Debug.LogFormat("{0} Accumulated cost: {1}, Cost of the action: {2}", mWorld.plan[i].mAction.mName, mWorld.plan[i].gCost, mWorld.plan[i].mAction.mCost);
        }

        return mWorld.plan;
    }

    /***************************************************************************/

    void RetracePlan(NodePlanning startNode, NodePlanning endNode)
    {
        List<NodePlanning> plan = new List<NodePlanning>();

        NodePlanning currentNode = endNode;

        while (currentNode != startNode)
        {
            plan.Add(currentNode);
            currentNode = currentNode.mParent;
        }
        plan.Reverse();

        mWorld.plan = plan;
    }

    /***************************************************************************/

    float GetDistance(NodePlanning nodeA, NodePlanning nodeB)
    {
        // Distance function
        /*if(nodeA.mAction != null & nodeB.mAction != null)
        {
            Debug.LogWarning("DISTANCE COST: " + (nodeB.mAction.mCost - nodeA.mAction.mCost));
            return nodeB.mAction.mCost - nodeA.mAction.mCost;
        }*/

        //Debug.LogWarning("Distance Value = " + nodeB.mAction.mCost);
        return nodeB.mAction.mCost;
    }

    /***************************************************************************/

    float Heuristic(NodePlanning nodeA, NodePlanning nodeB)
    {
        // Heuristic function
        float originalH = -World.PopulationCount((int)(nodeA.mWorldState | nodeB.mWorldState)) 
            - World.PopulationCount((int)(nodeA.mWorldState & nodeB.mWorldState));

        originalH -= 600 * Mathf.Sqrt(Mathf.Abs(originalH));
        float newH = originalH;

        return newH;
    }

    /***************************************************************************/

    public List<NodePlanning> ExecutePlanner()
    {
        World.WorldState CompleteWS = mWorld.mWorldState | mGymWorld.mGymWorldState;
        FindPlan(CompleteWS, World.WorldState.WORLD_STATE_EXIT_GYM);

        return mWorld.plan;
    }



    public World GetWorld()
    {
        return mWorld;
    }
}
