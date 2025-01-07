using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public Transform followObj;
    private Vector3 _offset;

    private void Start()
    {
        _offset = followObj.position - transform.position;
    }

    void Update()
    {
        transform.position = followObj.position - _offset;
    }
}
