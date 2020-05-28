using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NPBehave;

public class NPBehave1HelloWorld : MonoBehaviour
{

  private Root behaviorTree;

  /****************************************************************************/

	// Use this for initialization
	void Start ()
  {
    behaviorTree = new Root(
	    new Sequence(
		    new Action(() => Debug.Log("Hello World!"))
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
