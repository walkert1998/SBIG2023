using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCHeadLook : MonoBehaviour
{
    Animator animator;
    public Transform target;
    Vector3 targetPosition;
    //NPCAI npcAI;
    public bool locked = false;
    // Start is called before the first frame update
    void Start()
    {
        SetInitialReferences();
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}

    void OnEnable()
    {
        SetInitialReferences();
    }

    void SetInitialReferences ()
    {
        animator = GetComponent<Animator>();
        //npcAI = GetComponent<NPCAI>();
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (animator.enabled)
        {
            if (!locked && target != null)
            {
                
                //if (npcAI.currentTarget != null)
                //{
                animator.SetLookAtWeight(1, 0.15f, 1.0f, 1.0f, 0.1f);
                //    //targetPosition = new Vector3(npcAI.currentTarget.position.x, npcAI.currentTarget.position.y - 0.25f, npcAI.currentTarget.position.z);
                //    //animator.SetLookAtPosition(targetPosition);
                //    animator.SetLookAtPosition(npcAI.currentTarget.position);
                //}
                //else
                //{
                //animator.SetLookAtWeight(1, 0.1f, 0.8f, 1.0f, 0.7f);
                //targetPosition = new Vector3(target.position.x, target.position.y - 0.25f, target.position.z);
                animator.SetLookAtPosition(target.position);
                //}
            }
            else
            {
                animator.SetLookAtWeight(0, 0, 0, 0, 0);
            }
        }
    }
}
