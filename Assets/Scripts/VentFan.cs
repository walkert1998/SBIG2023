using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VentFan : MonoBehaviour
{
    public Vector3 rotation;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        rotation = new Vector3(0, speed, 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotation);
    }
}
