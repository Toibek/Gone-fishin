using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LureCam : MonoBehaviour
{
    public Vector2 defaultPosition;
    public float followSpeed;
    [Space]
    public float zoomSpeed;
    public float lureSize;
    public float defaSize;

    Camera cam;
    Transform Lure;
    private void Start()
    {
        cam = GetComponent<Camera>();
    }
    void FixedUpdate()
    {
        if (Lure == null && GameObject.FindGameObjectWithTag("Lure") != null)
            Lure = GameObject.FindGameObjectWithTag("Lure").transform;
        else if(Lure != null)
        {
            float spd = Mathf.Clamp(followSpeed * Time.deltaTime, 0, Vector2.Distance(transform.position, Lure.position));
            Vector3 newpos = Vector2.MoveTowards(transform.position, Lure.position, spd);
            newpos.z = transform.position.z;
            transform.position = newpos;

            cam.orthographicSize = Mathf.MoveTowards(cam.orthographicSize, lureSize,followSpeed*Time.deltaTime);
        }
        else
        {
            float spd = Mathf.Clamp(followSpeed * Time.deltaTime, 0, Vector2.Distance(transform.position, defaultPosition));
            Vector3 newpos = Vector2.MoveTowards(transform.position, defaultPosition, spd);
            newpos.z = transform.position.z;
            transform.position = newpos;

            cam.orthographicSize = Mathf.MoveTowards(cam.orthographicSize, defaSize, followSpeed * Time.deltaTime);
        }
    }
}
