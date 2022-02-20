using System;
using UnityEngine;
internal class Utilities
{
    internal static float GetExp(float u, float lambda)
    {
        //throw new NotImplementedException();
        return -Mathf.Log(1 - u) / lambda;
    }
}