using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followCamera : MonoBehaviour
{
    private float camerax;
    private float cameray;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        camerax = Camera.main.transform.position.x;
        cameray = Camera.main.transform.position.y;
        transform.position = new Vector2(camerax, (cameray-0.286f));
    }
}
