using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour
{
    [SerializeField]
    CharacterController controller;
    [SerializeField]
    private InfluenceMap InfluenceMap;

    [SerializeField]
    private float movementSpeed;

    //private Vector3 moveStep = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dest = InfluenceMap.goToPos;
        //print("dest: " + dest);
        //print("pos: " + transform.position);
        if (transform.position != dest)
        {
            Vector3 dir = dest - new Vector3(transform.position.x, 0.0f, transform.position.z);
            dir.Normalize();
            controller.Move(dir * movementSpeed * Time.deltaTime);
        }
    }
}
