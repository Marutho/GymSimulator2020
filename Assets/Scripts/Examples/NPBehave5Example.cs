using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NPBehave;

public class NPBehave5Example : MonoBehaviour
{

  private Root  mBehaviorTree;

  List<string>  mPlan;
  int           mPlanSteps        = 0;
  int           mCurrentAction    = -1;

  float         mTimeStartAction  = 0.0f;
  float         mTimeActionLast   = 1.0f;

  /****************************************************************************/

	// Use this for initialization
	void Start ()
  {
    mPlan = new List<string>(10);

    mBehaviorTree = new Root(
      new Sequence(
        new Action( (bool planning) =>
        {
          Debug.Log("Planning...");

          // Clear old plan and replan
          mPlan.Clear();
          mPlan.Add("GETKNIFE");
          mPlan.Add("KILLENEMY");

          mPlanSteps      = mPlan.Count;
          mCurrentAction  = -1;

          Debug.Log("Planned in " + mPlanSteps + " steps" );

          if( mPlan.Count > 0 ){
            return Action.Result.SUCCESS;
          }
          else
          {
            return Action.Result.FAILED;
          }    
        }){ Label = "Planning" },
        new Repeater( -1,
          new Sequence(
            new Action( (bool nextActionAvailable) =>
            {
              mCurrentAction++;
              mTimeStartAction = Time.time;

              if( mCurrentAction >= mPlan.Count)
              {
                return Action.Result.FAILED;
              }
              else
              {
                return Action.Result.SUCCESS;
              }
            }),
            new Selector(
              new Action( (bool knifeOwned) =>
              {
                if( mPlan[mCurrentAction] == "GETKNIFE")
                {
                  // Check preconditions

                  // Apply positive effects

                  // Apply negative effects

                  // If execution succeeded return "success". Otherwise return "failed".
                  if( Time.time > mTimeStartAction + mTimeActionLast ){
                    Debug.Log( "Knife is mine" );
                    mTimeStartAction = Time.time;
                    return Action.Result.SUCCESS;
                  }
                  else
                  {
                    // Action in progress
                    return Action.Result.PROGRESS;
                  }
                }
                else
                {
                  return Action.Result.FAILED;
                }
              }){ Label = "GettingKnife" },
              new Action( (bool killed) =>
              {
                if ( mPlan[mCurrentAction] == "KILLENEMY")
                {
                  // Check preconditions

                  // Apply positive effects

                  // Apply negative effects

                  // If execution succeeded return "success". Otherwise return "failed".
                  if( Time.time > mTimeStartAction + mTimeActionLast ){
                    Debug.Log( "Enemy killed!" );
                    mTimeStartAction = Time.time;
                    return Action.Result.SUCCESS;
                  }
                  else
                  {
                    // Action in progress
                    return Action.Result.PROGRESS;
                  }
                }
                else
                {
                  return Action.Result.FAILED;
                }
              }){ Label = "Killing" }
              //... Action3
              //... ActionN
            ) // Selector
          ) // Sequence
        ) // Repeater
      ) // Sequence
    );

    // attach the debugger component if executed in editor (helps to debug in the inspector) 
#if UNITY_EDITOR
        Debugger debugger = (Debugger)this.gameObject.AddComponent(typeof(Debugger));
        debugger.BehaviorTree = mBehaviorTree;
#endif

    mBehaviorTree.Start();
	}

  /****************************************************************************/
	
	// Update is called once per frame
	void Update ()
  {
		
	}

  /****************************************************************************/

  public void OnDestroy()
    {
        StopBehaviorTree();
    }


  /****************************************************************************/

  public void StopBehaviorTree()
  {
      if ( mBehaviorTree != null && mBehaviorTree.CurrentState == Node.State.ACTIVE )
      {
          mBehaviorTree.Stop();
      }
  }

  /****************************************************************************/



}
