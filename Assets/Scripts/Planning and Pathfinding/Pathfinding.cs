#define Smooth

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class Pathfinding : MonoBehaviour
{

    public Transform mSeeker;
    public Transform mTarget;

    NodePathfinding CurrentStartNode;
    NodePathfinding CurrentTargetNode;

    public Grid Grid;
    float hMultiplier = 1.0f;

    //FOR DEBUGGING
    int totalNodes;
    int openNodes;
    int closedNodes;

    bool EightConnectivity = true;


    /***************************************************************************/

    void Awake()
    {
        Grid = GetComponent<Grid>();
    }

    /***************************************************************************/

    public List<NodePathfinding> FindPath(Vector3 startPos, Vector3 targetPos, int iterations)
    {
        CurrentStartNode = Grid.NodeFromWorldPoint(startPos);
        CurrentTargetNode = Grid.NodeFromWorldPoint(targetPos);

        List<NodePathfinding> openSet = new List<NodePathfinding>();
        HashSet<NodePathfinding> closedSet = new HashSet<NodePathfinding>();
        openSet.Add(CurrentStartNode);
        Grid.openSet = openSet;

        int currentIteration = 0;
        NodePathfinding node = CurrentStartNode;
        while (openSet.Count > 0 && node != CurrentTargetNode && (iterations == -1 || currentIteration < iterations))
        {
            // Select best node from open list
            node = openSet[0];

            foreach (NodePathfinding nod in openSet)
            {
                if (nod.fCost < node.fCost)
                {
                    node = nod;
                }
            }

            // Manage open/closed list
            openSet.Remove(node);
            closedSet.Add(node);
            Grid.openSet = openSet;
            Grid.closedSet = closedSet;



            // Check destination
            if (node != CurrentTargetNode)
            {

                // Open neighbours
                foreach (NodePathfinding neighbour in Grid.GetNeighbours(node, EightConnectivity))
                {
                    if (!neighbour.mWalkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    //Cost of reaching my neighbor = cost of reaching ME + 
                    float neighbourGcost = (node.gCost + (GetDistance(node, neighbour) * node.mCostMultiplier));
                    if (neighbourGcost < node.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = neighbourGcost;
                        neighbour.hCost = Heuristic(neighbour, CurrentTargetNode) ;
                        neighbour.mParent = node;
                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                    }

                }

                currentIteration++;
            }
            else
            {
                //Path found!
                //Comment / Uncomment the define Smooth to deactive / active the smoothing with Bresenham.
                RetracePath(CurrentStartNode, CurrentTargetNode);
                // Path found


                totalNodes = openSet.Count + closedSet.Count;
                openNodes = openSet.Count;
                closedNodes = closedSet.Count;
                Debug.Log("Statistics:");
                Debug.LogFormat("Total nodes:  {0}", totalNodes);
                Debug.LogFormat("Open nodes:   {0}", openNodes);
                Debug.LogFormat("Closed nodes: {0}", closedNodes);
            }
        }

        return Grid.path;
    }

    /***************************************************************************/

    void RetracePath(NodePathfinding startNode, NodePathfinding endNode)
    {
        List<NodePathfinding> path = new List<NodePathfinding>();

        bool end = false;

        NodePathfinding newNode = endNode;
        while (!end)
        {
            path.Add(newNode);
            newNode = newNode.mParent;

            if (newNode == startNode)
            {
                path.Add(newNode);
                end = true;
            }
        }

        path.Reverse();
#if Smooth
        SmoothPath(path);
#else
        Grid.path = path;
#endif
    }

    /***************************************************************************/

    float GetDistance(NodePathfinding nodeA, NodePathfinding nodeB)
    {
        // Distance function
        //nodeA.mGridX   nodeB.mGridX
        //nodeA.mGridY   nodeB.mGridY
        if (EightConnectivity)
        {
            int vertDistance = Mathf.Abs(nodeA.mGridY - nodeB.mGridY);
            int horzDistance = Mathf.Abs(nodeA.mGridX - nodeB.mGridX);
            float distance = Mathf.Sqrt(Mathf.Pow(vertDistance, 2) + Mathf.Pow(horzDistance, 2));
            return distance;
        }
        else
        {
            return (Mathf.Abs(nodeA.mGridX - nodeB.mGridX)) + (Mathf.Abs(nodeA.mGridY - nodeB.mGridY));
        }
    }

    /***************************************************************************/

    float Heuristic(NodePathfinding nodeA, NodePathfinding nodeB)
    {
        // Heuristic function
        //nodeA.mGridX   nodeB.mGridX
        //nodeA.mGridY   nodeB.mGridY
        

        return Vector2.Distance(new Vector2(nodeB.mGridX, nodeB.mGridY), new Vector2(nodeA.mGridX, nodeA.mGridY)) * hMultiplier;
    }

    /***************************************************************************/

    /***************************************************************************/

    void SmoothPath(List<NodePathfinding> path)
    {
        //TODO
        //Comparamos si el nodo final es el mismo que el inicial
        //Cuando bresenham sea true, el nodo inicial se cambia al nodo que ha salido true y se vuelve a recalcular
        //Se añade ese nodo para guardarlo y se vuelve a iterar por el resto de nodos hasta que vuelva a encontrar el nodo inicial
        
        List<NodePathfinding> targetNodes = new List<NodePathfinding>();
        NodePathfinding newOriginNode = path[0];
        NodePathfinding endNode = path[path.Count - 1];
        List<NodePathfinding> auxNodeList = new List<NodePathfinding>();
        List<NodePathfinding> newNodeList = new List<NodePathfinding>();
        newNodeList = path;
        newNodeList.Reverse();

        int a = 0;

        for (a = 0; a < 10000; a++)
        {

            foreach (NodePathfinding n in newNodeList)
            {
                if (BresenhamWalkable(newOriginNode.mGridX, newOriginNode.mGridY , n.mGridX, n.mGridY))
                {
                    targetNodes.Add(n);
                    newOriginNode = n;
                    newNodeList = new List<NodePathfinding>(auxNodeList);
                    auxNodeList.Clear();
                    break;
                }

                auxNodeList.Add(n);
            }

            if (endNode == newOriginNode)
            {
                a = 999999;
            }
                
        }

        targetNodes.Add(path[0]);
        
        Grid.path = targetNodes;
        
    }

    /***************************************************************************/

    public bool BresenhamWalkable(int x, int y, int x2, int y2)
    {

        //DX1, DY1 = +- 1
        //DX2, DY2 = 0 || 1
        //En las diagonales calcular los costes de los nodos vecinos 

        //COST (A, B) = GcostB - GcostA
        //COSTBRESENHAM (A, B) = Sum(Ac->Bc) (Sumatorio de los costes de los nodos de A a B)

        // TODO: 4 Connectivity
        // TODO: Cost   

        int w = x2 - x;
        int h = y2 - y;
        int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
        if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
        if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
        if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
        int longest = Mathf.Abs(w);
        int shortest = Mathf.Abs(h);
        if (!(longest > shortest))
        {
            longest = Mathf.Abs(h);
            shortest = Mathf.Abs(w);
            if (h < 0)
            {
                dy2 = -1;
            }
            else if (h > 0)
            {
                dy2 = 1;
            }
            dx2 = 0;
        }

        int numerator = longest >> 1;
        for (int i = 0; i <= longest; i++)
        {
            if (!Grid.GetNode(x, y).mWalkable)
            {
                return false;
            }

            numerator += shortest;
            if (!(numerator < longest))
            {
                numerator -= longest;
                x += dx1;
                y += dy1;
            }
            else
            {
                x += dx2;
                y += dy2;
            }
        }

        return true;
    }

}

