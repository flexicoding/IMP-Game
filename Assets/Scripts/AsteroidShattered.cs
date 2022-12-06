using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidShattered : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            child.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f)) * 10, ForceMode.Impulse);
        }
    }
}
