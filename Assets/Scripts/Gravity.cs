using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    void Update()
    {
        transform.Translate(Vector3.down * Time.deltaTime);
    }
}
