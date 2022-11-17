using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InfluenceMap : MonoBehaviour
{
    [SerializeField]
    Grid grid;
    [SerializeField]
    Image influenceMapImage;
    [SerializeField]
    MeshFilter planeMesh;

    private float[] influenceMap;
    private float timer;

    private float[,] testArr;



    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        influenceMap = new float[(int)Mathf.Pow(20, 2)];

        testArr = new float[20, 20];

        print(influenceMap.Length);

        // Fill array with default values
        for (int i = 0; i < influenceMap.Length; i++)
        {
            influenceMap[i] = 0.5f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //print(grid.WorldToCell(transform.position));

        timer += Time.deltaTime;

        if (timer > 0.5f)
        {
            timer = 0;
            Collider[] nearbyObjects = getNearbyObjects();

            foreach (Collider obj in nearbyObjects)
            {
                Vector3 pos = grid.WorldToCell(obj.gameObject.transform.position);
                print(pos);
                testArr[(int)pos.x, (int)pos.z] = 5.0f;
            }

            print(testArr);

        }

    }


    private Collider[] getNearbyObjects()
    {
        Vector3 position = transform.position;
        Collider[] nearbyObjects = Physics.OverlapSphere(position, 5.0f, LayerMask.GetMask("GameObject"));

        if (nearbyObjects.Length != 0)
        {
            print(nearbyObjects);
        }

        return nearbyObjects;
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 5);
    }
}
