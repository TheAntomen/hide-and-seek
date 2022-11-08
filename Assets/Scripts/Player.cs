using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Vector3 pos = transform.localPosition;
        //pos.y += Input.GetAxis("Jump") * Time.deltaTime * 20;


        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.up, 1);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.up, 1);
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.Rotate(Vector3.up, 1);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Rotate(Vector3.up, 1);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(Vector3.up, 1);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(Vector3.up, 1);
        }


        pos.x += Input.GetAxis("Horizontal") * Time.deltaTime * 20;
        pos.z += Input.GetAxis("Vertical") * Time.deltaTime * 20;

        transform.localPosition = pos;

 


    }
}
