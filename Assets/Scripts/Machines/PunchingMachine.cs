using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchingMachine : MonoBehaviour
{
    bool punchingBagNotInUse;

    //time things
    public float lifeTime = 0.5f;
    protected float timer = 0f;
    private bool activatePunching;
    private Collider col;

    // Start is called before the first frame update
    void Start()
    {
        punchingBagNotInUse = true;
        activatePunching = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (activatePunching)
        {
            timer += Time.deltaTime;
            if (timer > lifeTime)
            {
                col.GetComponent<CharacterAnimations>().havePunchingMachine = true;
                timer = 0F;
                punchingBagNotInUse = true;
                activatePunching = false;
                col.GetComponent<CharacterAnimations>().animator.SetTrigger("PunchingBag");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gymer"))
        {

            if (punchingBagNotInUse)
            {
                var rotationVector = transform.rotation.eulerAngles;
                rotationVector.y = -90;
                other.transform.rotation = Quaternion.Euler(rotationVector);

                other.transform.position = new Vector3(transform.position.x, 0.001f, transform.position.z);
                other.GetComponent<CharacterAnimations>().havePunchingMachine = true;
                punchingBagNotInUse = false;
                other.GetComponent<CharacterAnimations>().animator.SetTrigger("PunchingBag");
            }
            else
            {
                //Rotation
                var rotationVector = transform.rotation.eulerAngles;
                rotationVector.y = -90f;
                other.transform.rotation = Quaternion.Euler(rotationVector);

                activatePunching = true;
                other.transform.position = new Vector3(transform.position.x + 0.8f, 0.001f, transform.position.z);
                other.GetComponent<CharacterAnimations>().animator.SetTrigger("Punch");
                col = other;
            }

        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Gymer"))
        {
            if (other.GetComponent<CharacterAnimations>().havePunchingMachine)
            {
                var rotationVector = transform.rotation.eulerAngles;
                rotationVector.y = -90;
                other.transform.rotation = Quaternion.Euler(rotationVector);
                other.transform.position = new Vector3(transform.position.x, 0.001f, transform.position.z);
                other.GetComponent<CharacterAnimations>().havePunchingMachine = true;
                punchingBagNotInUse = false;
            }
            else
            {
                other.transform.position = new Vector3(transform.position.x + 0.8f, 0.001f, transform.position.z );
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Gymer"))
        {
            if (other.GetComponent<CharacterAnimations>().havePunchingMachine)
            {
                other.GetComponent<CharacterAnimations>().havePunchingMachine = false;
                punchingBagNotInUse = true;
            }
        }
    }
}
