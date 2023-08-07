using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCColliders : MonoBehaviour
{
    public Collider head;
    public List<Collider> otherColliders;
    public List<Rigidbody> rigidbodies;
    // Start is called before the first frame update
    void Start()
    {
        PopulateColliders();
        DisableRagdoll();
    }

    [ContextMenu("Populate Colliders")]
    public void PopulateColliders()
    {
        otherColliders = new List<Collider>();
        foreach (Collider col in GetComponentsInChildren<Collider>())
        {
            otherColliders.Add(col);
        }
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rigidbodies.Add(rb);
        }
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}

    public void SetVelocity(Vector3 velocity)
    {
        foreach (Rigidbody body in rigidbodies)
        {
            if (body != null)
            {
                body.velocity = velocity;
            }
        }
    }

    public void DisableRagdoll()
    {
        foreach(Rigidbody body in rigidbodies)
        {
            if (body != null)
            {
                body.isKinematic = true;
            }
        }
    }

    public void EnableRagdoll()
    {
        foreach (Rigidbody body in rigidbodies)
        {
            if (body != null)
            {
                //if (body.GetComponent<Hat>())
                //{
                //    body.useGravity = true;
                //}
                body.isKinematic = false;
            }
        }
    }

    public void DisableDamageSensors()
    {
        foreach (Collider col in otherColliders)
        {
            if (col != null)
            {
                //if (col.GetComponent<Hat>())
                //{
                //    col.enabled = true;
                //}
                // Destroy(col.GetComponent<DamageLocation>());
            }
        }
    }
}
