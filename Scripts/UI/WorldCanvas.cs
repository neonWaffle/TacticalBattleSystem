using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCanvas : MonoBehaviour
{
    new Camera camera;

    float initialSize;

    void Start()
    {
        camera = Camera.main;
        initialSize = transform.localScale.x / Vector3.Distance(camera.transform.position, transform.position);
    }

    void Update()
    {
        transform.forward = camera.transform.forward;
        transform.localScale = Vector3.Distance(camera.transform.position, transform.position) * initialSize * Vector3.one;
    }
}
