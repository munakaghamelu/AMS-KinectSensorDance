using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraMovement : MonoBehaviour
{

    private const float minAngleY = 0.0f;
    private const float maxAngleY = 50.0f;

    public GameObject lookAt;
    public Transform camTransform;
    public Dropdown bodyPart;

    private Camera cam;


    private float distance = 5.0f;
    private float currentX = 0.0f;
    private float currentY = 0.0f;

    private void Start()
    {
        camTransform = transform;
        cam = Camera.main;
        
    }

    private void Update()
    {
        currentX += Input.GetAxis("Horizontal");
        currentY += Input.GetAxis("Vertical");
        distance -= Input.GetAxisRaw("Mouse ScrollWheel");
      
        currentY = Mathf.Clamp(currentY, minAngleY, maxAngleY);
    }
    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 direction = new Vector3(0, 0,-distance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        camTransform.position = lookAt.transform.position + rotation * direction;
        camTransform.LookAt(lookAt.transform.position);
    }

    public void focusBody()
    {
        lookAt = GameObject.FindGameObjectWithTag(bodyPart.options[bodyPart.value].text);
    }
}
