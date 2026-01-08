using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Chunk : MonoBehaviour
{
    public float StartY { get; private set; }
    public float EndY { get; private set; }

    public void Init(float startY, float endY)
    {
        StartY = startY;
        EndY = endY;

        transform.position = Vector3.up * startY;
    }

}
