using JetBrains.Annotations;
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

    //private float[] influenceMap;
    Texture2D influenceMap;
    private float timer;

    // [Object, värdet] -> Influence map [position, värde]
    private float[,] map2;
    private Dictionary<Vector3, float> map = new Dictionary<Vector3, float>();

    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        //influenceMap = new float[(int)Mathf.Pow(20, 2)];
        initDictionary();

        //print(influenceMap.Length);
        //print(map);
        //printDict();

        // Fill array with default values
        influenceMap = new Texture2D(20, 20);
        for (int y = 0; y < 20; y++)
        {
            for (int x = 0; x < 20; x++)
            {
                influenceMap.SetPixel(y, x, new Color(0.5f, 0.5f, 0.5f));
            }
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        //print(grid.WorldToCell(transform.position));

        timer += Time.deltaTime;

        if (timer > 0.1f)
        {
            timer = 0;
            Collider[] nearbyObjects = getNearbyObjects();

            foreach (Collider obj in nearbyObjects)
            {
                Vector3 pos = grid.WorldToCell(obj.gameObject.transform.position);

                print((pos.x + 10) + ", " + (pos.y + 10));
                print(influenceMap.width);

                influenceMap.SetPixel((int)pos.x + 10, (int)pos.z + 10, new Color(1.0f, 1.0f, 1.0f));
                
                /*
                if(map.ContainsKey(pos))
                {
                    print("contains");
                    map[pos] = 5.0f;
                    //printDict();
                }
                */
            }

            influenceMap.Apply();
            Sprite IMsprite = Sprite.Create(influenceMap, new Rect(0, 0, influenceMap.width, influenceMap.width), new Vector2(0.5f, 0.5f));
            influenceMapImage.overrideSprite = IMsprite;
        

        }
    }


    private Collider[] getNearbyObjects()
    {
        Vector3 position = transform.position;
        Collider[] nearbyObjects = Physics.OverlapSphere(position, 5.0f, LayerMask.GetMask("GameObject"));

        return nearbyObjects;
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 5);
    }

    void initDictionary()
    {
        for (int i = -10; i < 10; i++){
            for (int j = -10; j < 10; j++){
                map.Add(new Vector3((float)i, 0.0f, (float)j), 0.5f);
            }
        }
    }

    void printDict()
    {
        foreach(KeyValuePair<Vector3, float> entry in map)
        {
            print("entry "+ entry.Key);
        }
    }
}


public class Node
{
    Vector3 position;

    public Node(Vector3 pos)
    {
        this.position = pos;
    }

    public Vector3 getPosition()
    {
        return this.position;
    }
}
