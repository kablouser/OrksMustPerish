using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeTrap : MonoBehaviour
{
    public float slow = 5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<EnemyCharacterController>())
        {
            //SET SLOW SPEED
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<EnemyCharacterController>())
        {
            //SET SLOW SPEED TO 0
        }
    }
}
