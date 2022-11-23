using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class InfluenceMap : MonoBehaviour
{
    public Vector3 goToPos;

    [SerializeField]
    Grid grid;
    [SerializeField]
    Image influenceMapImage;
    [SerializeField]
    MeshFilter planeMesh;

    Texture2D influenceMap;
    Texture m_texture;

    private float timer;
    private int filterIterations = 1;
    private int[] xFilter = {1, 2, 1};
    private int[] yFilter = {1, 2, 1};

    private float[,] map2;
    private Dictionary<Vector3, float> map = new Dictionary<Vector3, float>();

    // Filter kernel
    float filterOffset;


    // Start is called before the first frame update
    void Start()
    {
        timer = 0;        
        initDictionary();

        

        

        // Fill array with default values
        influenceMap = new Texture2D(20, 20);
        influenceMapImage.material.SetTexture("_ImTex", influenceMap);


        for (int y = 0; y < 20; y++)
        {
            for (int x = 0; x < 20; x++)
            {
                influenceMap.SetPixel(y, x, new Color(0.5f, 0.5f, 0.5f));
            }
        }

        influenceMap.Apply();
        influenceMap.filterMode = FilterMode.Point;
        influenceMapImage.DisableSpriteOptimizations();

        goToPos = transform.position;

        

        //filterOffset = 1 / (influenceMap.width * influenceMap.height);

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        Graphics.Blit(influenceMap as Texture, influenceMapImage.material, pass: 0);
        Graphics.Blit(influenceMap as Texture, influenceMapImage.material, pass: 1);

        //influenceMap = (Texture2D)influenceMapImage.material.GetTexture("_ImTex");

        if (timer > 0.2f)
        {
            influenceMap.SetPixel((int)transform.position.x + 10, (int)transform.position.z + 10, new Color(0.0f, 0.0f, 0.0f));
            timer = 0;
            Collider[] nearbyObjects = getNearbyObjects();

            foreach (Collider obj in nearbyObjects)
            {
                Vector3 pos = grid.WorldToCell(obj.gameObject.transform.position);

                //print((pos.x + 10) + ", " + (pos.y + 10));
                //print(influenceMap.width);
                
                Vector2 newPixel = new Vector2(pos.x + 10, pos.z + 10);

                influenceMap.SetPixel((int)newPixel.x, (int)newPixel.y, new Color(1.0f, 1.0f, 1.0f));
            }
            influenceMap.Apply();

            //updateMap();
            findPoint();
            //Sprite IMsprite = Sprite.Create(influenceMap, new Rect(0, 0, influenceMap.width, influenceMap.width), new Vector2(0.5f, 0.5f));
            //influenceMapImage.overrideSprite = IMsprite;

            print(influenceMap.GetPixel(5, 5));

        }
    }

    private void updateMap()
    {
        for (int i = 0; i < filterIterations; i++)
        {
            influenceMap = lowPassFilterX(influenceMap);
            influenceMap = lowPassFilterY(influenceMap);
        }
    }

    private Texture2D lowPassFilterX(Texture2D tex)
    {
        for (int y = 0; y < influenceMap.height; y++)
        {
            for (int x = 0; x < influenceMap.width; x++)
            {
                Color right = new Color(0,0,0);
                Color left = new Color(0,0,0);

                for (int i = 0; i < xFilter.Length/2; i++)
                {
                    right += influenceMap.GetPixel(x + (i+1), y) * xFilter[(xFilter.Length/2) + (i+1)];
                    left += influenceMap.GetPixel(x - (i+1), y) * xFilter[(xFilter.Length/2) - (i+1)];
                }

                Color center = influenceMap.GetPixel(x, y) * xFilter[xFilter.Length/2];

                Color newColor = (right + left + center) / (xFilter.Sum());

                tex.SetPixel(x, y, newColor);
            }
        }

        tex.Apply();
        return tex;
    }
    private Texture2D lowPassFilterY(Texture2D tex)
    {
        for (int y = 0; y < influenceMap.height; y++)
        {
            for (int x = 0; x < influenceMap.width; x++)
            {

                Color up = new Color(0,0,0);
                Color down = new Color(0,0,0);

                for (int i = 0; i < yFilter.Length / 2; i++)
                {
                    print(yFilter.Length / 2);
                    up += influenceMap.GetPixel(x, y + (i+1)) * yFilter[(yFilter.Length/2) + (i+1)];
                    down += influenceMap.GetPixel(x, y - (i+1)) * yFilter[(yFilter.Length/2) - (i+1)];
                }

                Color center = influenceMap.GetPixel(x, y) * yFilter[yFilter.Length / 2];

                Color newColor = (up + down + center) / (yFilter.Sum());

                tex.SetPixel(x, y, newColor);
            }
        }

        tex.Apply();
        return tex;

    }
    private void findPoint()
    {
        float highestValue = 0.0f;

        for (int y = 0; y < influenceMap.height; y++)
        {
            for (int x = 0; x < influenceMap.width; x++)
            {
                Color pixel = influenceMap.GetPixel(x, y);

                if (pixel.r > highestValue)
                {
                    highestValue = pixel.r;
                    goToPos = new Vector3(x - 10f, 0.0f, y - 10f);
                }
            }
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
