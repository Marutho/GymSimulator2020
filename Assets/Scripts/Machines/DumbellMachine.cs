using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumbellMachine : MonoBehaviour
{
    public GameObject Dumbell1;
    public GameObject Dumbell2;
    bool dumbellsNotInUse;

    //time things
    public float lifeTime = 2f;
    protected float timer = 0f;
    private bool activateDumbell;
    private Collider col;
    // Start is called before the first frame update
    void Start()
    {
        dumbellsNotInUse = true;
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
                col.GetComponent<CharacterAnimations>().haveDoubleDumbells = true;
                timer = 0F;
                dumbellsNotInUse = true;
                activateDumbell = false;
                col.GetComponent<CharacterAnimations>().animator.SetTrigger("Dumbell");
                col.GetComponent<CharacterAnimations>().doubleDumbell1.SetActive(true);
                col.GetComponent<CharacterAnimations>().doubleDumbell2.SetActive(true);

            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gymer"))
        {
            Dumbell1.SetActive(false);
            Dumbell2.SetActive(false);
            if (dumbellsNotInUse)
            {
                var rotationVector = transform.rotation.eulerAngles;
                rotationVector.y = 180;
                other.transform.rotation = Quaternion.Euler(rotationVector);
                other.transform.position = new Vector3(transform.position.x, 0.001f, transform.position.z);
                other.GetComponent<CharacterAnimations>().haveDoubleDumbells = true;
                other.GetComponent<CharacterAnimations>().doubleDumbell1.SetActive(true);
                other.GetComponent<CharacterAnimations>().doubleDumbell2.SetActive(true);
                dumbellsNotInUse = false;
                other.GetComponent<CharacterAnimations>().animator.SetTrigger("Dumbell");
            }
            else
            {
                var rotationVector = transform.rotation.eulerAngles;
                rotationVector.y = 0;
                other.transform.rotation = Quaternion.Euler(rotationVector);

                activateDumbell = true;
                other.transform.position = new Vector3(transform.position.x, 0.001f, transform.position.z - 0.8f);
                other.GetComponent<CharacterAnimations>().animator.SetTrigger("Punch");
                col = other;
            }

        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Gymer"))
        {
            Dumbell1.SetActive(false);
            Dumbell2.SetActive(false);
            if (other.GetComponent<CharacterAnimations>().haveDoubleDumbells)
            {
                //Rotation
                var rotationVector = transform.rotation.eulerAngles;
                rotationVector.y = 0;
                other.transform.rotation = Quaternion.Euler(rotationVector);

                other.transform.position = new Vector3(transform.position.x, 0.001f, transform.position.z);
                other.GetComponent<CharacterAnimations>().haveDoubleDumbells = true;
                other.GetComponent<CharacterAnimations>().doubleDumbell1.SetActive(true);
                other.GetComponent<CharacterAnimations>().doubleDumbell2.SetActive(true);
                dumbellsNotInUse = false;
            }
            else
            {
                other.transform.position = new Vector3(transform.position.x, 0.001f, transform.position.z - 0.8f);
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Gymer"))
        {
            if (other.GetComponent<CharacterAnimations>().haveDoubleDumbells)
            {
                Dumbell1.SetActive(true);
                Dumbell2.SetActive(true);
                other.GetComponent<CharacterAnimations>().doubleDumbell1.SetActive(false);
                other.GetComponent<CharacterAnimations>().doubleDumbell2.SetActive(false);
                other.GetComponent<CharacterAnimations>().haveDoubleDumbells = false;
                dumbellsNotInUse = true;
            }
        }
    }
}
