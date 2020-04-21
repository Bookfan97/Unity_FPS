using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    public Transform target;
    private float startFOV, targetFOV;
    public float zoomSpeed;
    public Camera theCamera;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        startFOV = theCamera.fieldOfView;
        targetFOV = startFOV;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = target.position;
        transform.rotation = target.rotation;
        theCamera.fieldOfView = Mathf.Lerp(theCamera.fieldOfView, targetFOV, zoomSpeed * Time.deltaTime);
    }
    public void ZoomIn(float newZoom)
    {
        targetFOV = newZoom;
    }
    public void ZoomOut()
    {
        targetFOV = startFOV;
    }
}
