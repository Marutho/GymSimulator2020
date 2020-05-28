using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquatsMachine : MonoBehaviour
{
    public GameObject Dumbell;
    bool dumbellNotInUse;

    // Start is called before the first frame update

    //time things
    public float lifeTime = 1.5f;
    protected float timer = 0f;
    private bool activateDumbell;
    private Collider col;

    void Start()
    {
        dumbellNotInUse = true;
        activateDumbell = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (activateDumbell)
        {
            timer += Time.deltaTime;
            if (timer > lifeTime)
            {
                col.GetComponent<CharacterAnimations>().haveDumbell = true;
                timer = 0F;
                dumbellNotInUse = true;
                activateDumbell = false;
                col.GetComponent<CharacterAnimations>().animator.SetTrigger("Squats");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
       if(other.CompareTag("Gymer"))
       {
            Dumbell.SetActive(false);
            if (dumbellNotInUse)
            {
                var rotationVector = transform.rotation.eulerAngles;
                rotationVector.y = 0;
                other.transform.rotation = Quaternion.Euler(rotationVector);
                other.transform.position = new Vector3(transform.position.x, 0.001f, transform.position.z);
                other.GetComponent<CharacterAnimations>().haveDumbell = true;
                other.GetComponent<CharacterAnimations>().dumbell.SetActive(true);
                dumbellNotInUse = false;
                other.GetComponent<CharacterAnimations>().animator.SetTrigger("Squats");
            }
            else
            {
                //Rotation
                var rotationVector = transform.rotation.eulerAngles;
                rotationVector.y = 0f;
                other.transform.rotation = Quaternion.Euler(rotationVector);

                activateDumbell = true;
                other.transform.position = new Vector3(transform.position.x, 0.001f, transform.position.z-1f);
                other.GetComponent<CharacterAnimations>().animator.SetTrigger("Kick");
                col = other;
            }

        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Gymer"))
        {
            Dumbell.SetActive(false);
            if (other.GetComponent<CharacterAnimations>().haveDumbell)
            {
                var rotationVector = transform.rotation.eulerAngles;
                rotationVector.y = 0;
                other.transform.rotation = Quaternion.Euler(rotationVector);
                other.transform.position = new Vector3(transform.position.x, 0.001f, transform.position.z);
                other.GetComponent<CharacterAnimations>().haveDumbell = true;
                other.GetComponent<CharacterAnimations>().dumbell.SetActive(true);
                dumbellNotInUse = false;
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
            if(other.GetComponent<CharacterAnimations>().haveDumbell)
            {
                Dumbell.SetActive(true);
                other.GetComponent<CharacterAnimations>().dumbell.SetActive(false);
                other.GetComponent<CharacterAnimations>().haveDumbell = false;
                dumbellNotInUse = true;
            }
        }
    }
}
