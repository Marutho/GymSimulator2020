using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NPBehave;

public class NPBehave3Events : MonoBehaviour
{

  private Root behaviorTree;

  /****************************************************************************/

	// Use this for initialization
	void Start ()
  {
    behaviorTree = new Root(
      new Service(0.5f, () => { behaviorTree.Blackboard["foo"] = !behaviorTree.Blackboard.Get<bool>("foo"); },
        new Selector(
        
          new BlackboardCondition("foo", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
            new Sequence(
              new Action(() => Debug.Log("foo")),
              new WaitUntilStopped()
            )
          ),

          new Sequence(
            new Action(() => Debug.Log("bar")),
            new WaitUntilStopped()
          )
        )
      )
    );

    behaviorTree.Start();
	}

  /****************************************************************************/
	
	// Update is called once per frame
	void Update ()
  {
		
	}

  /****************************************************************************/

}
