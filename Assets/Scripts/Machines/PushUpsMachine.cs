using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushUpsMachine : MonoBehaviour
{
    bool pushupsNotInUse;
    // Start is called before the first frame update

    //time things
    public float lifeTime = 1.5f;
    protected float timer = 0f;
    private bool activatePushups;
    private Collider col;

    void Start()
    {
        pushupsNotInUse = true;
        activatePushups = false;

}

    // Update is called once per frame
    void Update()
    {
        if (activatePushups)
        {
            timer += Time.deltaTime;
            if (timer > lifeTime)
            {
                col.GetComponent<CharacterAnimations>().havePushUps = true;
                timer = 0F;
                pushupsNotInUse = true;
                activatePushups = false;
                col.GetComponent<CharacterAnimations>().animator.SetTrigger("PushUp");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gymer"))
        {
            if (pushupsNotInUse)
            {
                var rotationVector = transform.rotation.eulerAngles;
                rotationVector.y = 0;
                other.transform.rotation = Quaternion.Euler(rotationVector);
                other.transform.position = new Vector3(transform.position.x, 0.02f, transform.position.z);
                other.GetComponent<CharacterAnimations>().havePushUps = true;
                other.GetComponent<CharacterAnimations>().animator.SetTrigger("PushUp");
                pushupsNotInUse = false;
            }
            else
            {
                //Rotation
                var rotationVector = transform.rotation.eulerAngles;
                rotationVector.y = -90f;
                other.transform.rotation = Quaternion.Euler(rotationVector);

                activatePushups = true;
                other.transform.position = new Vector3(transform.position.x + 1f, 0.001f, transform.position.z);
                other.GetComponent<CharacterAnimations>().animator.SetTrigger("Kick");
                col = other;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Gymer"))
        {
            if (other.GetComponent<CharacterAnimations>().havePushUps)
            {
                // other.transform.position = new Vector3(-9.964f, 1.192093e-07f, 10.867f);
                var rotationVector = transform.rotation.eulerAngles;
                rotationVector.y = 0;
                other.transform.rotation = Quaternion.Euler(rotationVector);
                other.transform.position = new Vector3(transform.position.x, 0.02f, transform.position.z);
                other.GetComponent<CharacterAnimations>().havePushUps = true;
                pushupsNotInUse = false;
            }
            else
            {
                other.transform.position = new Vector3(transform.position.x+1f, 0.001f, transform.position.z);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Gymer"))
        {
            if (other.GetComponent<CharacterAnimations>().havePushUps)
            {
                other.GetComponent<CharacterAnimations>().havePushUps = false;
                pushupsNotInUse = true;
            }
        }
    }
}
