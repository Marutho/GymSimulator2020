using UnityEngine;
using System.Collections;

public class NodeGrid
{

    public bool mWalkable;
    public Vector3 mWorldPosition;
    public int mGridX;
    public int mGridY;
    public float mCostMultiplier;

    public float gCost;
    public float hCost;
    public NodeGrid mParent;

    /***************************************************************************/

    public NodeGrid(bool walkable, Vector3 worldPosition, int gridX, int gridY, float costMultiplier)
    {
        mWalkable = walkable;
        mWorldPosition = worldPosition;
        mGridX = gridX;
        mGridY = gridY;
        mCostMultiplier = costMultiplier;
    }

    /***************************************************************************/

    public float fCost
    {
        get
        {
            return (gCost  + hCost);
        }
    }

    /***************************************************************************/

}
