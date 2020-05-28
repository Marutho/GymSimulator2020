using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPBehave;


public class BHTree : MonoBehaviour
{
    // Start is called before the first frame update
    public Root behaviorTree;
    public Planning planner;
    public List<NodePlanning> plan;
    public NodePlanning[] nodes;
    public Unit agent;
    public CharacterAnimations chAnimations;

    void Start()
    {
        behaviorTree = new Root(

        new Sequence(
            new Sequence(
                new Action((bool planning) =>
                {
                    plan = planner.ExecutePlanner();

                    if (plan.Count > 0)
                    {
                        return Action.Result.SUCCESS;
                    }
                    else
                    {
                        return Action.Result.FAILED;
                    }

                })
                { Label = "Planning" },
                new Repeater(1,
                    new Sequence(
                        new Action((bool popActionPlan) =>
                        {
                            nodes = plan.ToArray();
                            Debug.Log(planner.GetWorld().mWorldState);
                            Debug.Log(nodes[0].mAction.mPreconditions);

                            if ((planner.GetWorld().mWorldState & nodes[0].mAction.mPreconditions) == 0)
                            {
                                planner.GetWorld().SetWorldState(nodes[0].mAction.mEffects);
                                plan.RemoveAt(0);
                                nodes = plan.ToArray();
                                Debug.Log(nodes[1].mAction.mPreconditions);
                                Debug.Log("[BT] I've entered the gym");
                                return Action.Result.SUCCESS;

                            }

                            else
                                return Action.Result.FAILED;


                        })
                        { Label = "Enter the Gym" },
                        new Repeater(3,
                            new Sequence(
                                new Action((bool resetAgent) =>
                                {
                                    agent.isPathFinished = false;
                                    agent.isWalking = false;

                                    return Action.Result.SUCCESS;
                                }),
                                new Action((bool isInMachine) =>
                                {
                                    nodes = plan.ToArray();
                                    
                                    if(!agent.isPathFinished)
                                    {
                                        if(!agent.isWalking)
                                        {
                                            GetComponent<BoxCollider>().enabled = false;

                                            /*In order to check if there's a machine from the planner, we add the fourth float, 
                                             * to get from the function the position from the world of the machine to do the pathfinding 
                                             * and in case we don't get any machine, the result will lead into a 0.0f in the
                                             * fourth float.*/

                                            Vector4 machine = planner.GetWorld().GetPathToMachine(nodes[0].mAction.mEffects);
                                            agent.GoToNextMachine(machine);
                                            chAnimations.animator.SetBool("Walking", true);
                                            return Action.Result.PROGRESS;
                                        }
                                        
                                        else
                                            return Action.Result.PROGRESS; 
                                    }

                                    else if ((planner.GetWorld().mWorldState & nodes[0].mAction.mPreconditions) != 0 && agent.isPathFinished)
                                    {
                                        GetComponent<BoxCollider>().enabled = true;
                                        chAnimations.animator.SetBool("Walking", false);
                                        Debug.Log("[BT]" + nodes[0].mAction.mName);
                                        planner.GetWorld().SetWorldState(nodes[0].mAction.mEffects);
                                        plan.RemoveAt(0);
                                        return Action.Result.SUCCESS;
                                    }

                                    else
                                        return Action.Result.FAILED;

                                })
                                { Label = "Go to Machine" },
                                new Selector(
                                        new Action((bool isMachineFree) =>
                                        {
                                            //NOTE: The planner decides which is the next action, so usually always the next action is Kick the Guy.
                                            //The ignore action should be the one that wouldn't happen so often which is that the machine is free and in that case
                                            //the action of kicking the guy should return FAILED in order to do the next one.
                                            nodes = plan.ToArray();
                                            // GetMachineFree()
                                            // Here we check if the machine it's free based on the next possible Action, which should be Work in the machine in case that
                                            // there's a guy on it or Move to another machine in case that the machine is free.

                                            if (planner.GetWorld().GetMachineFree(nodes[1].mAction.mPreconditions) != World.WorldState.WORLD_STATE_DEFAULT)
                                            {
                                                Debug.Log("[BT] Machine isn't free, lets kick the guy.");
                                                return Action.Result.FAILED;
                                            }

                                            else
                                            {
                                                Debug.Log("[BT] Machine is free!");
                                                return Action.Result.SUCCESS;
                                            }
                                        
                                        })
                                        { Label = "Check if Machine is free" },
                                        new Action((bool isGuyKicked) =>
                                        {
                                            nodes = plan.ToArray();

                                            //Animation of kicking will trigger by default thanks to the animator, no need on calling to it.
                                            if (!chAnimations.timeAnimation(5.0f))
                                                return Action.Result.PROGRESS;

                                            else if ((planner.GetWorld().mWorldState & nodes[0].mAction.mPreconditions) != 0)
                                            {
                                                Debug.Log("[BT]" + nodes[0].mAction.mName);
                                                plan.RemoveAt(0);
                                                return Action.Result.SUCCESS;
                                            }

                                            else
                                                return Action.Result.FAILED;
                                        })
                                        { Label = "Kick the guy in the Machine" }
                                ),
                                new Action((bool doTheWork) =>
                                {
                                    nodes = plan.ToArray();

                                    if (!chAnimations.timeAnimation(10.0f))
                                    {
                                        if(planner.mGymWorld.IsWorldState(planner.GetWorld().GetMachineFree(nodes[0].mAction.mPreconditions)))
                                        {
                                            planner.mGymWorld.UnsetWorldState(planner.GetWorld().GetMachineFree(nodes[0].mAction.mPreconditions));
                                        }
                                        return Action.Result.PROGRESS;
                                    }
                                        


                                    else if ((planner.GetWorld().mWorldState & nodes[0].mAction.mPreconditions) != 0)
                                    {
                                        //Set the have finished in animator.
                                        chAnimations.finishAnimation();
                                        planner.mGymWorld.SetWorldState(planner.GetWorld().GetMachineFree(nodes[0].mAction.mPreconditions));
                                        gameObject.transform.position += new Vector3(2.0f, 0.0f, 0.0f);
                                        Debug.Log("[BT]" + nodes[0].mAction.mName);
                                        planner.GetWorld().SetWorldState(nodes[0].mAction.mEffects);
                                        plan.RemoveAt(0);
                                        return Action.Result.SUCCESS;
                                    }

                                    else
                                        return Action.Result.FAILED;
                                })
                                { Label = "Do the repetitions and complete the work" }
                                )),
                        new Action((bool resetAgent) =>
                        {
                            agent.isPathFinished = false;
                            agent.isWalking = false;
                            Debug.Log("[BT] Reset the agent pathfinding values.");

                            return Action.Result.SUCCESS;
                        }),
                        new Action((bool exitGym) =>
                        {
                            nodes = plan.ToArray();

                            if (!agent.isPathFinished)
                            {
                                if (!agent.isWalking)
                                {
                                    chAnimations.disableMachinesAnimation();
                                    chAnimations.animator.Rebind();
                                    GetComponent<BoxCollider>().enabled = false;
                                    chAnimations.animator.SetBool("Walking", true);
                                    GameObject gymExit = GameObject.FindGameObjectWithTag("EXIT");
                                    Vector4 EXIT = new Vector4(gymExit.transform.position.x, gymExit.transform.position.y, gymExit.transform.position.z, 1.0f);
                                    agent.GoToNextMachine(EXIT);
                                    return Action.Result.PROGRESS;
                                }

                                else
                                    return Action.Result.PROGRESS;
                            }

                            else if ((planner.GetWorld().mWorldState & nodes[0].mAction.mPreconditions) != 0)
                            {
                                Debug.Log("[BT]" + nodes[0].mAction.mName);
                                chAnimations.animator.SetBool("Walking", false);
                                planner.GetWorld().SetWorldState(nodes[0].mAction.mEffects);
                                plan.RemoveAt(0);
                                return Action.Result.SUCCESS;
                            }
                            else
                            {
                                Debug.Log("[BT] I haven't exited the gym");
                                return Action.Result.FAILED;
                            }

                        })
                        { Label = "Exit the Gym" }
                    )
                )
            ),
            new WaitUntilStopped()
        )
    );

#if UNITY_EDITOR
        Debugger debugger = (Debugger)gameObject.AddComponent(typeof(Debugger));
        debugger.BehaviorTree = behaviorTree;
#endif

        behaviorTree.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDestroy()
    {
        StopBehaviorTree();
    }

    public void StopBehaviorTree()
    {
        if (behaviorTree != null && behaviorTree.CurrentState == Node.State.ACTIVE)
        {
            behaviorTree.Stop();
        }
    }

}
