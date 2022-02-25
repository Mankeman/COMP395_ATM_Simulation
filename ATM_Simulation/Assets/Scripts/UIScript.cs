using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIScript : MonoBehaviour
{
    //use this bool field to set the text on top of ATM
    public bool isFree;
    public Text statusText;
    // Start is called before the first frame update
    void Start()
    {
        statusText = this.GetComponentInChildren<Text>();
        isFree = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isFree)
        {
            statusText.text = "Free";
            statusText.color = Color.black;
        }
        else
        {
            statusText.text = "In Use";
            statusText.color = Color.red;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("person"))
        {
            isFree = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("person"))
        {
            isFree = true;
        }
    }
}
