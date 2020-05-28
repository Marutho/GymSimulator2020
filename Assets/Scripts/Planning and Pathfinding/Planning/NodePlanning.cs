using UnityEngine;
using System.Collections;

public class NodePlanning
{
    public World.WorldState mWorldState;

    public ActionPlanning mAction;

    public float gCost;
    public float hCost;

    public NodePlanning mParent;

    /***************************************************************************/

    public NodePlanning(World.WorldState worldState, ActionPlanning action)
    {
        mWorldState = worldState;
        mAction = action;
        gCost = 0.0f;
        hCost = 0.0f;
        mParent = null;
    }

    /***************************************************************************/

    public float fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    /***************************************************************************/

    public bool Equals(NodePlanning other)
    {
        return mWorldState == other.mWorldState;
    }

    /***************************************************************************/

}
