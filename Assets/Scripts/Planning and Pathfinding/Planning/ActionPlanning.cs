using UnityEngine;
using System.Collections;

public class ActionPlanning
{
    public ActionType mActionType;
    public World.WorldState mPreconditions;
    public World.WorldState mEffects;
    public float mCost;
    public string mName;

    /***************************************************************************/

    public enum ActionType
    {
        ACTION_TYPE_NONE = -1,
        ACTION_TYPE_WALKTO_ENTRANCE,
        ACTION_TYPE_WALKTO_EXIT,
        ACTION_TYPE_WALKTO_PRESS_MACHINE,
        ACTION_TYPE_WALKTO_SQUATS_MACHINE,
        ACTION_TYPE_WALKTO_PUSHUPS_MACHINE,
        ACTION_TYPE_WALKTO_DUMBELL_MACHINE,
        ACTION_TYPE_WALKTO_LEGS_MACHINE,
        ACTION_TYPE_KICK_PRESS_MACHINE,
        ACTION_TYPE_KICK_SQUATS_MACHINE,
        ACTION_TYPE_KICK_PUSHUPS_MACHINE,
        ACTION_TYPE_KICK_DUMBELL_MACHINE,
        ACTION_TYPE_KICK_LEGS_MACHINE,
        ACTION_TYPE_START_WORKING_PRESS,
        ACTION_TYPE_START_WORKING_PUSHUPS,
        ACTION_TYPE_START_WORKING_SQUATS,
        ACTION_TYPE_START_WORKING_DUMBELL,
        ACTION_TYPE_START_WORKING_LEGS,
    }

    /***************************************************************************/

    public ActionPlanning(ActionType actionType, World.WorldState preconditions, World.WorldState effects, float cost, string name)
    {
        mActionType = actionType;
        mPreconditions = preconditions;
        mEffects = effects;
        mCost = cost;
        mName = name;
    }

    /***************************************************************************/

}
