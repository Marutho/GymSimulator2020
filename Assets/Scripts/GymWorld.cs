using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GymWorld : MonoBehaviour
{

    public World.WorldState mGymWorldState;

    private void Awake()
    {
        SetWorldState(World.WorldState.WORLD_STATE_FREE_DUMBELLM);
        SetWorldState(World.WorldState.WORLD_STATE_FREE_LEGSM);
        SetWorldState(World.WorldState.WORLD_STATE_FREE_PRESSM);
        SetWorldState(World.WorldState.WORLD_STATE_FREE_PUSHM);
        SetWorldState(World.WorldState.WORLD_STATE_FREE_SQUATSM);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Set the desired WorldState in our World.
    /// </summary>
    /// <param name="ws"></param>
    public void SetWorldState(World.WorldState ws)
    {
        mGymWorldState = mGymWorldState | ws;
    }

    /// <summary>
    /// Check if there is a specific WorldState active in our World.
    /// </summary>
    /// <param name="ws"></param>
    /// <returns></returns>
    public bool IsWorldState(World.WorldState ws)
    {
        return (mGymWorldState & ws) != 0;
    }


    /// <summary>
    /// Deactivate a WorldState in our World.
    /// </summary>
    /// <param name="ws"></param>
    public void UnsetWorldState(World.WorldState ws)
    {
        mGymWorldState = (mGymWorldState & (~ws));
    }

}
