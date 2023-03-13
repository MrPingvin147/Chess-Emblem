using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceCanvas : MonoBehaviour
{
    private Canvas canvas;
    Camera cam;

    float currentTime;
    float time = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponent<Canvas>();
        cam = Camera.main;

        canvas.worldCamera = cam;
        currentTime = time;

        transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
    }

    private void LateUpdate()
    {
        currentTime -= Time.deltaTime;

        if (currentTime < 0)
        {
            transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
            currentTime = time;
        }
        
    }
}
