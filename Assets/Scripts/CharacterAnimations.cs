using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimations : MonoBehaviour
{
    public Animator animator;
    public GameObject dumbell;
    public GameObject doubleDumbell1;
    public GameObject doubleDumbell2;
    public GameObject hand1;
    public GameObject hand2;
    public GameObject leg1;
    public bool haveDumbell;
    public bool haveDoubleDumbells;
    public bool haveLegs;
    public bool havePushUps;
    public bool havePunchingMachine;

    private bool checkTime;
    private float timePassed;

    Rigidbody rb;
    BoxCollider bc;
    // Start is called before the first frame update
    void Start()
    {
        hand1.GetComponent<BoxCollider>().enabled = false;
        hand2.GetComponent<BoxCollider>().enabled = false;
        leg1.GetComponent<BoxCollider>().enabled = false;
        haveDumbell = false;
        haveDoubleDumbells = false;
        haveLegs = false;
        havePushUps = false;
        havePunchingMachine = false;
        rb = GetComponent<Rigidbody>();
        bc = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        aboutDumbell();
        aboutDumbells();
        checkBoxCollider();
        punchCollider();
        kickCollider();
        punchCollider2();
    }

    //para cuando meta un meco a una persona
    void punchCollider()
    {
        if (isPlaying("Punch"))
        { //con esta funcion sabemos si sigue en marcha la animacion con ese nombre
            hand1.GetComponent<BoxCollider>().enabled = true;
        }
    }

    //para cuando meta mecos a la bolsa de lucha
    void punchCollider2()
    {
        if (isPlaying("PunchingBag"))
        { //con esta funcion sabemos si sigue en marcha la animacion con ese nombre
            hand1.GetComponent<BoxCollider>().enabled = true;
            hand2.GetComponent<BoxCollider>().enabled = true;
        }
    }

    void kickCollider()
    {
        if (isPlaying("Kick"))
        { //con esta funcion sabemos si sigue en marcha la animacion con ese nombre
            leg1.GetComponent<BoxCollider>().enabled = true;
        }
        else
        {
            leg1.GetComponent<BoxCollider>().enabled = false;
        }
    }

    void checkBoxCollider()
    {
        if (!isPlaying("PunchingBag"))
        {
            hand2.GetComponent<BoxCollider>().enabled = false;
            if (!isPlaying("PunchingBag"))
            {
                hand1.GetComponent<BoxCollider>().enabled = false;
            }
        }

    }

    void aboutDumbell()
    {
        //no es un switch porque no hay getter del nombre aunque parezca increible
        if (isPlaying("Squats"))
        { //con esta funcion sabemos si sigue en marcha la animacion con ese nombre
            dumbell.SetActive(true);
        }
        else if (isPlaying("Pushups") || isPlaying("Press") || isPlaying("Dumbell") || isPlaying("Walking"))
        {
            dumbell.SetActive(false);
        }
        else if (isPlaying("Hitted"))
        {

        }
    }

    void aboutDumbells()
    {
        //no es un switch porque no hay getter del nombre aunque parezca increible
        if (isPlaying("Dumbell"))
        { //con esta funcion sabemos si sigue en marcha la animacion con ese nombre
            doubleDumbell1.SetActive(true);
            doubleDumbell2.SetActive(true);
        }
        else if (isPlaying("Pushups") || isPlaying("Press")  || isPlaying("Squats") || isPlaying("Walking"))
        {
            doubleDumbell1.SetActive(false);
            doubleDumbell2.SetActive(false);
        }
        else if (isPlaying("Hitted"))
        {

        }
    }
    //con esta funcion sabemos si sigue en marcha la animacion con ese nombre
    bool isPlaying(string animationExercise){
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(animationExercise))
        { 
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Punch") || other.CompareTag("Kick"))
        {
            // dumbell.SetActive(false);
            var rotationVector = transform.rotation.eulerAngles;
            rotationVector.y = Random.Range(-180f,180f);//cae hacia un lado aleatorio
            transform.rotation = Quaternion.Euler(rotationVector);
            haveDumbell = false;
            haveDoubleDumbells = false;
            haveLegs = false;
            havePushUps = false;
            havePunchingMachine = false;
            tag = "Dead";
            animator.SetTrigger("Hitted");

            rb.detectCollisions = false;
            bc.enabled = false;
            //aqui meteriamos lo de desactivar los scripts
            GetComponent<BHTree>().behaviorTree.Stop();
            GetComponent<BHTree>().enabled = false;
            GetComponent<Planning>().enabled = false;
            GetComponent<Pathfinding>().enabled = false;
            GetComponent<World>().enabled = false;
            GetComponent<Unit>().enabled = false;
        }
    }
    
    public bool timeAnimation(float animTime)
    {
        if(!checkTime)
        {
            timePassed = 0.0f;
            checkTime = true;
        }

        timePassed += Time.deltaTime;

        if (timePassed >= animTime)
            checkTime = false;

        return (timePassed >= animTime);
    }

    public void disableMachinesAnimation()
    {
        haveDumbell = false;
        haveDoubleDumbells = false;
        haveLegs = false;
        havePushUps = false;
        havePunchingMachine = false;
    }

    public void finishAnimation()
    {
        if (haveDoubleDumbells)
            animator.SetBool("DumbellFinished", true);

        else if (haveDumbell)
            animator.SetBool("SquatsFinished", true);

        else if (haveLegs)
            animator.SetBool("LegsFinished", true);

        else if (havePunchingMachine)
            animator.SetBool("PunchingBagFinished", true);

        else if (havePushUps)
            animator.SetBool("PushUpsFinished", true);
    }

    public bool checkMachines()
    {
        if (haveDoubleDumbells || haveDumbell || haveLegs || havePunchingMachine || havePushUps)
            return true;

        return false;
    }
}
