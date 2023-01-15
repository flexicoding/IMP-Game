using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissappear : MonoBehaviour
{
    public int value = 5;

    float i = 1;
    bool wait = false;
    Vector3 random;

    void Start()
    {
        StartCoroutine(fadeOut());
        random = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));
    }

    private void Update()
    {
        transform.Rotate(random * Time.deltaTime * 5);
        if(wait)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0.1f, 0.1f, 0.1f), i * Time.deltaTime);
            i = i * 1.02f;
            if (transform.localScale.x <= 1.0f) Destroy(gameObject);
        }
    }

    IEnumerator fadeOut()
    {
        yield return new WaitForSeconds(5);
        wait = true;
    }
}
