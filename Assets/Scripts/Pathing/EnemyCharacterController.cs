using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacterController : MonoBehaviour
{
    public Rigidbody rb;
    public float moveSpeed;

    public PathingMap pathingMap;
    public List<Vector3> currentPath;
    public int currentPathIndex;

    void Start()
    {
        currentPath = null;
    }

    void FixedUpdate()
    {
        // update current path         
        // follow current path
    }
}
