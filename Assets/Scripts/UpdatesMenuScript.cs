using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpdatesMenuScript : MonoBehaviour
{
    [SerializeField]
    private floatSO crystals;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Crystals "+crystals.Value;
    }
}
