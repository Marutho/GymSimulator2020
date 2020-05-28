using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class World : MonoBehaviour
{
    public List<NodePlanning> openSet;
    public HashSet<NodePlanning> closedSet;

    public List<NodePlanning> plan;

    public WorldState mWorldState;
    public GymWorld gym;

    public List<ActionPlanning> mActionList;
    public WorldState[] targetMachines;
    public Transform[] machines;


    /***************************************************************************/


    //BITWISE OPERATIONS
    // & COMPARE => WS & WS_CLOSE_ENEMY
    // | ACTIVATE => WS = WS | WS_CLOSE_ENEMY
    // ~ DEACTIVATE = WS = ~WS_CLOSE_ENEMY

    //CREATE OUR OWN WORLD / NPC, WHAT STATES IT WOULD HAVE AND WHAT ACTIONS WILL TAKE.

    [System.Flags]
    public enum WorldState
    {
        WORLD_STATE_NONE = 0,
        WORLD_STATE_CLOSETO_PUSHM = 1,
        WORLD_STATE_CLOSETO_PRESSM = 2,
        WORLD_STATE_CLOSETO_SQUATSM = 4,
        WORLD_STATE_CLOSETO_DUMBELLM = 8,
        WORLD_STATE_CLOSETO_LEGSM = 16,
        WORLD_STATE_FREE_PUSHM = 32,
        WORLD_STATE_FREE_PRESSM = 64,
        WORLD_STATE_FREE_SQUATSM = 128,
        WORLD_STATE_FREE_DUMBELLM = 256,
        WORLD_STATE_FREE_LEGSM = 512,
        WORLD_STATE_COMPLETED_PUSHM = 1024,
        WORLD_STATE_COMPLETED_PRESSM = 2048,
        WORLD_STATE_COMPLETED_SQUATSM = 4096,
        WORLD_STATE_COMPLETED_DUMBELLM = 8192,
        WORLD_STATE_COMPLETED_LEGSM = 16384,
        WORLD_STATE_ENTER_GYM = 32768,
        WORLD_STATE_EXIT_GYM = 65536,
        WORLD_STATE_DEFAULT = 131072
    }

    /***************************************************************************/

    void Awake()
    {
        ResetWorld();
        mActionList = new List<ActionPlanning>();
        GetTargetMachines();
        
        mActionList.Add(
          new ActionPlanning(
            //ACTION
            ActionPlanning.ActionType.ACTION_TYPE_WALKTO_ENTRANCE,
            //PRECONDITIONS
            WorldState.WORLD_STATE_NONE,
            //EFFECT
            WorldState.WORLD_STATE_ENTER_GYM,
            //COST AND NAME
            1.0f, "Walk to Gym")
        );

        mActionList.Add(
          new ActionPlanning(
            ActionPlanning.ActionType.ACTION_TYPE_WALKTO_PUSHUPS_MACHINE,
            WorldState.WORLD_STATE_ENTER_GYM,
            WorldState.WORLD_STATE_CLOSETO_PUSHM,
            CalculateDynamicCost(machines[1].position), "Go to Pushups Machine")
        );

        mActionList.Add(
          new ActionPlanning(
            ActionPlanning.ActionType.ACTION_TYPE_START_WORKING_PUSHUPS,
            WorldState.WORLD_STATE_CLOSETO_PUSHM | WorldState.WORLD_STATE_FREE_PUSHM,
            WorldState.WORLD_STATE_COMPLETED_PUSHM,
            10.0f, "Work on Pushups Machine")
        );

        mActionList.Add(
          new ActionPlanning(
            ActionPlanning.ActionType.ACTION_TYPE_KICK_PUSHUPS_MACHINE,
            WorldState.WORLD_STATE_CLOSETO_PUSHM,
            WorldState.WORLD_STATE_FREE_PUSHM,
            200.0f, "Kick the guy in the Pushups Machine")
        );

        mActionList.Add(
          new ActionPlanning(
            ActionPlanning.ActionType.ACTION_TYPE_WALKTO_PRESS_MACHINE,
            WorldState.WORLD_STATE_ENTER_GYM,
            WorldState.WORLD_STATE_CLOSETO_PRESSM,
            CalculateDynamicCost(machines[0].position), "Go to Press Machine")
        );

        mActionList.Add(
          new ActionPlanning(
            ActionPlanning.ActionType.ACTION_TYPE_START_WORKING_PRESS,
            WorldState.WORLD_STATE_CLOSETO_PRESSM | WorldState.WORLD_STATE_FREE_PRESSM,
            WorldState.WORLD_STATE_COMPLETED_PRESSM,
            10.0f, "Work on Press Machine")
        );

        mActionList.Add(
          new ActionPlanning(
            ActionPlanning.ActionType.ACTION_TYPE_KICK_PRESS_MACHINE,
            WorldState.WORLD_STATE_CLOSETO_PRESSM,
            WorldState.WORLD_STATE_FREE_PRESSM,
            200.0f, "Kick the guy in the Press Machine")
        );

        mActionList.Add(
          new ActionPlanning(
            ActionPlanning.ActionType.ACTION_TYPE_WALKTO_DUMBELL_MACHINE,
            WorldState.WORLD_STATE_ENTER_GYM,
            WorldState.WORLD_STATE_CLOSETO_DUMBELLM,
            CalculateDynamicCost(machines[3].position), "Go to Dumbell Machine")
        );

        mActionList.Add(
          new ActionPlanning(
            ActionPlanning.ActionType.ACTION_TYPE_START_WORKING_DUMBELL,
            WorldState.WORLD_STATE_CLOSETO_DUMBELLM | WorldState.WORLD_STATE_FREE_DUMBELLM,
            WorldState.WORLD_STATE_COMPLETED_DUMBELLM,
            10.0f, "Work on Dumbell Machine")
        );

        mActionList.Add(
          new ActionPlanning(
            ActionPlanning.ActionType.ACTION_TYPE_KICK_DUMBELL_MACHINE,
            WorldState.WORLD_STATE_CLOSETO_DUMBELLM,
            WorldState.WORLD_STATE_FREE_DUMBELLM,
            200.0f, "Kick the guy in the Dumbell Machine")
        );

        mActionList.Add(
          new ActionPlanning(
            ActionPlanning.ActionType.ACTION_TYPE_WALKTO_LEGS_MACHINE,
            WorldState.WORLD_STATE_ENTER_GYM,
            WorldState.WORLD_STATE_CLOSETO_LEGSM,
            CalculateDynamicCost(machines[4].position), "Go to Legs Machine")
        );

        mActionList.Add(
          new ActionPlanning(
            ActionPlanning.ActionType.ACTION_TYPE_START_WORKING_LEGS,
            WorldState.WORLD_STATE_CLOSETO_LEGSM | WorldState.WORLD_STATE_FREE_LEGSM,
            WorldState.WORLD_STATE_COMPLETED_LEGSM,
            10.0f, "Work on Legs Machine")
        );

        mActionList.Add(
          new ActionPlanning(
            ActionPlanning.ActionType.ACTION_TYPE_KICK_LEGS_MACHINE,
            WorldState.WORLD_STATE_CLOSETO_LEGSM,
            WorldState.WORLD_STATE_FREE_LEGSM,
            200.0f, "Kick the guy in the Legs Machine")
        );


        mActionList.Add(
          new ActionPlanning(
            ActionPlanning.ActionType.ACTION_TYPE_WALKTO_SQUATS_MACHINE,
            WorldState.WORLD_STATE_ENTER_GYM,
            WorldState.WORLD_STATE_CLOSETO_SQUATSM,
            CalculateDynamicCost(machines[2].position), "Go to Squats Machine")
        );

        mActionList.Add(
          new ActionPlanning(
            ActionPlanning.ActionType.ACTION_TYPE_START_WORKING_SQUATS,
            WorldState.WORLD_STATE_CLOSETO_SQUATSM | WorldState.WORLD_STATE_FREE_SQUATSM,
            WorldState.WORLD_STATE_COMPLETED_SQUATSM,
            10.0f, "Work on Squats Machine")
        );

        mActionList.Add(
          new ActionPlanning(
            ActionPlanning.ActionType.ACTION_TYPE_KICK_SQUATS_MACHINE,
            WorldState.WORLD_STATE_CLOSETO_SQUATSM,
            WorldState.WORLD_STATE_FREE_SQUATSM,
            200.0f, "Kick the guy in the Squats Machine")
        );

        mActionList.Add(
          new ActionPlanning(
            ActionPlanning.ActionType.ACTION_TYPE_WALKTO_EXIT,
            targetMachines[0] | targetMachines[1] | targetMachines[2],
            WorldState.WORLD_STATE_EXIT_GYM,
            1.0f, "Leave the training and go home")
        );

        ResetWorld();
    }

    /***************************************************************************/

    public List<NodePlanning> GetNeighbours(NodePlanning node)
    {
        List<NodePlanning> neighbours = new List<NodePlanning>();

        foreach (ActionPlanning action in mActionList)
        {
            //Hacer en el bh para comparar
            //WorldState completeWS = node.mWorldState | gym.mGymWorldState;

            // If preconditions are met we can apply effects and the new state is valid
            if ((node.mWorldState & action.mPreconditions) == action.mPreconditions)
            {
                // Apply action and effects
                NodePlanning newNodePlanning = new NodePlanning(node.mWorldState | action.mEffects, action);
                neighbours.Add(newNodePlanning);
            }
        }

        return neighbours;
    }

    /***************************************************************************/

    public static int PopulationCount(int n)
    {
        return System.Convert.ToString(n, 2).ToCharArray().Count(c => c == '1');
    }

    /***************************************************************************/

    public void GetTargetMachines()
    {
        targetMachines = new WorldState[3];

        List<WorldState> completedWorldStates = new List<WorldState>();
        completedWorldStates.Add(WorldState.WORLD_STATE_COMPLETED_PRESSM);
        completedWorldStates.Add(WorldState.WORLD_STATE_COMPLETED_LEGSM);
        completedWorldStates.Add(WorldState.WORLD_STATE_COMPLETED_DUMBELLM);
        completedWorldStates.Add(WorldState.WORLD_STATE_COMPLETED_PUSHM);
        completedWorldStates.Add(WorldState.WORLD_STATE_COMPLETED_SQUATSM);
        
        for (int i = 0; i<3; i++)
        {
            int random = Random.Range(0, 5-i);
            targetMachines[i] = completedWorldStates.ElementAt(random);
            completedWorldStates.RemoveAt(random);
            Debug.Log(targetMachines[i]);
        }

    }

    /// <summary>
    /// Set the desired WorldState in our World.
    /// </summary>
    /// <param name="ws"></param>
    public void SetWorldState(WorldState ws)
    {
        mWorldState = mWorldState | ws;
    }

    /// <summary>
    /// Check if there is a specific WorldState active in our World.
    /// </summary>
    /// <param name="ws"></param>
    /// <returns></returns>
    public bool IsWorldState(WorldState ws)
    {
        return (mWorldState & ws) != 0;
    }


    /// <summary>
    /// Deactivate a WorldState in our World.
    /// </summary>
    /// <param name="ws"></param>
    public void UnsetWorldState(WorldState ws)
    {
        mWorldState = (mWorldState & (~ws)) ;
    }


    /// <summary>
    /// Change the value of the World to None (0)
    /// </summary>
    public void ResetWorld()
    {
        mWorldState = WorldState.WORLD_STATE_NONE;
        Debug.Log("[RESETWORLD] WorldState: " + mWorldState);
    }

    public WorldState GetMachineFree(WorldState ws)
    {
        switch(ws)
        {
            case (WorldState.WORLD_STATE_CLOSETO_DUMBELLM | WorldState.WORLD_STATE_FREE_DUMBELLM):
                return WorldState.WORLD_STATE_FREE_DUMBELLM;

            case (WorldState.WORLD_STATE_CLOSETO_LEGSM | WorldState.WORLD_STATE_FREE_LEGSM):
                return WorldState.WORLD_STATE_FREE_LEGSM;

            case (WorldState.WORLD_STATE_CLOSETO_PUSHM | WorldState.WORLD_STATE_FREE_PUSHM):
                return WorldState.WORLD_STATE_FREE_PUSHM;

            case (WorldState.WORLD_STATE_CLOSETO_PRESSM | WorldState.WORLD_STATE_FREE_PRESSM):
                return WorldState.WORLD_STATE_FREE_PRESSM;

            case (WorldState.WORLD_STATE_CLOSETO_SQUATSM | WorldState.WORLD_STATE_FREE_SQUATSM):
                return WorldState.WORLD_STATE_FREE_SQUATSM;

            default:
                return WorldState.WORLD_STATE_DEFAULT;
        }
    }

    public Vector4 GetPathToMachine(WorldState ws)
    {

        switch (ws)
        {
            case (WorldState.WORLD_STATE_CLOSETO_DUMBELLM):
                return new Vector4(machines[3].transform.position.x, machines[3].transform.position.y, machines[3].transform.position.z, 1.0f);

            case (WorldState.WORLD_STATE_CLOSETO_LEGSM):
                return new Vector4(machines[4].transform.position.x, machines[4].transform.position.y, machines[4].transform.position.z, 1.0f);

            case (WorldState.WORLD_STATE_CLOSETO_PUSHM):
                return new Vector4(machines[1].transform.position.x, machines[1].transform.position.y, machines[1].transform.position.z, 1.0f);

            case (WorldState.WORLD_STATE_CLOSETO_PRESSM):
                return new Vector4(machines[0].transform.position.x, machines[0].transform.position.y, machines[0].transform.position.z, 1.0f);

            case (WorldState.WORLD_STATE_CLOSETO_SQUATSM):
                return new Vector4(machines[2].transform.position.x, machines[2].transform.position.y, machines[2].transform.position.z, 1.0f);

            default:
                return new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
        }
    }

    public float CalculateDynamicCost(Vector3 target)
    {
        return Mathf.Abs(Vector3.Distance(gameObject.transform.position, target)) * 30.0f;
    }
}