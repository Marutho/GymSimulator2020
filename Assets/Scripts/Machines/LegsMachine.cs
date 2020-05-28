using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegsMachine : MonoBehaviour
{
    bool legsNotInUse;


    //time things
    public float lifeTime = 1.5f;
    protected float timer = 0f;
    private bool activateLegs;
    private Collider col;

    void Start()
    {
        legsNotInUse = true;
        activateLegs = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (activateLegs)
        {
            timer += Time.deltaTime;
            if (timer > lifeTime)
            {
                col.GetComponent<CharacterAnimations>().haveLegs = true;
                timer = 0F;
                legsNotInUse = true;
                activateLegs = false;
                col.GetComponent<CharacterAnimations>().animator.SetTrigger("Legs");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gymer"))
        {
            if (legsNotInUse)
            {
                var rotationVector = transform.rotation.eulerAngles;
                rotationVector.y = 90;
                other.transform.rotation = Quaternion.Euler(rotationVector);
                other.transform.position = new Vector3(transform.position.x, 0.05f, transform.position.z);
                other.GetComponent<CharacterAnimations>().haveLegs = true;
                other.GetComponent<CharacterAnimations>().animator.SetTrigger("Legs");
                legsNotInUse = false;
            }
            else
            {
                //Rotation
                var rotationVector = transform.rotation.eulerAngles;
                rotationVector.y = 0f;
                other.transform.rotation = Quaternion.Euler(rotationVector);

                activateLegs = true;
                other.transform.position = new Vector3(transform.position.x, 0.001f, transform.position.z - 1f);
                other.GetComponent<CharacterAnimations>().animator.SetTrigger("Punch");
                col = other;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Gymer"))
        {
            if (other.GetComponent<CharacterAnimations>().haveLegs)
            {
                var rotationVector = transform.rotation.eulerAngles;
                rotationVector.y = 90;
                other.transform.rotation = Quaternion.Euler(rotationVector);
                other.transform.position = new Vector3(transform.position.x, 0.05f, transform.position.z);
                other.GetComponent<CharacterAnimations>().haveLegs = true;
                legsNotInUse = false;
            }
            else
            {
                other.transform.position = new Vector3(transform.position.x, 0.001f, transform.position.z - 1f);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Gymer"))
        {
            if (other.GetComponent<CharacterAnimations>().haveLegs)
            {
                other.GetComponent<CharacterAnimations>().haveLegs = false;
                legsNotInUse = true;                
            }
        }
    }
}
