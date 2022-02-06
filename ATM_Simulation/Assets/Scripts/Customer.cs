using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Customer : MonoBehaviour
{
    void Start()
    {
        ReadLines();
    }
    void ReadLines()
    {
        string[] customers = System.IO.File.ReadAllLines(@"Assets/Scripts/2-4.txt");
        foreach (string line in customers)
        {
            Debug.Log('\n' + line);
        }
    }
}
