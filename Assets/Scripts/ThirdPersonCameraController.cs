using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
    public Transform target;
    public Transform player;

    public float rotationSpeed;

    private float mouseX, mouseY;
    private Vector3 localPosition;
    private float cameraDistance;

    // Start is called before the first frame update
    void Start()
    {
        localPosition = transform.localPosition;
        cameraDistance = Vector3.Distance(transform.position, target.position);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        CamMove();
    }

    void CamMove()
    {
        mouseX += Input.GetAxis("Mouse X") * rotationSpeed;
        mouseY -= Input.GetAxis("Mouse Y") * rotationSpeed;

        //This is to restrict the camara y.
        mouseY = Mathf.Clamp(mouseY, -35, 60);

        transform.LookAt(target);

        target.rotation = Quaternion.Euler(mouseY, mouseX, 0);
        player.rotation = Quaternion.Euler(0, mouseX, 0);

        //See if camara is in obstructed.
        Vector3 direction = (transform.position - target.transform.position).normalized;
        Ray ray = new Ray(target.transform.position, direction);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, cameraDistance))
        {
            if(hit.collider != null && hit.transform.tag != "Player")
            {
                transform.position = hit.point;
            }
            else
            {
                transform.localPosition = localPosition;
            }
        }
        else
        {
            transform.localPosition = localPosition;
        }
    }
}
