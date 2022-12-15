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
    [SerializeField]
    Shader lowpassShader;

    [SerializeField]
    Material lowPass;
    [SerializeField]
    private int filterIterations;

    public Texture2D influenceMap;
    Texture m_texture;

    private float timer;
    
    private float[,] map2;
    private Dictionary<Vector3, float> map = new Dictionary<Vector3, float>();


    // Start is called before the first frame update
    void Start()
    {
        timer = 0;        
        initDictionary();

        lowPass.hideFlags = HideFlags.HideAndDontSave;
        

        // Fill array with default values
        influenceMap = new Texture2D(40, 40);
        lowPass.SetTexture("_MainTex", influenceMap);


        for (int y = 0; y < 40; y++)
        {
            for (int x = 0; x < 40; x++)
            {
                influenceMap.SetPixel(y, x, new Color(0.5f, 0.5f, 0.5f));
            }
        }

        influenceMap.Apply();
        influenceMap.filterMode = FilterMode.Point;
        influenceMap.wrapMode = TextureWrapMode.Clamp;
        influenceMapImage.DisableSpriteOptimizations();
        

        goToPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        influenceMap.SetPixel((int)transform.position.x + 20, (int)transform.position.z + 20, new Color(0.0f, 0.0f, 0.0f));
        
        //Collider[] nearbyObjects = getNearbyObjects();
        /*
        foreach (Collider obj in nearbyObjects)
        {
            Vector3 pos = obj.gameObject.transform.position;
            Vector2 newPixel = new Vector2(pos.x, pos.z);

            //AddPoint(newPixel, 1.0f);
        }*/

       
        if (timer >= 0.5)
        {
            timer = 0;
            decay();
        }

        lowPassFilter();

        findPoint();
    }


    public void AddPoint(Vector2 coord, float weight)
    {
        influenceMap.SetPixel((int)coord.x + 20, (int)coord.y + 20, new Color(weight, weight, weight));
        Debug.Log(coord);
        influenceMap.Apply();
    }

    private void decay()
    {
        RenderTexture currentDestination = RenderTexture.GetTemporary(40, 40, 0);
        Graphics.Blit(influenceMap, currentDestination, lowPass, 2);
        influenceMap.ReadPixels(new Rect(0, 0, currentDestination.width, currentDestination.height), 0, 0);
        influenceMap.Apply();
    }

    private void lowPassFilter()
    {
        RenderTexture currentDestination = RenderTexture.GetTemporary(40, 40, 0);

        for (int i = 0; i < filterIterations; i++)
        {
            Graphics.Blit(influenceMap, currentDestination, lowPass, 0);
            influenceMap.ReadPixels(new Rect(0, 0, currentDestination.width, currentDestination.height), 0, 0);
            influenceMap.Apply();
            Graphics.Blit(influenceMap, currentDestination, lowPass, 1);
            influenceMap.ReadPixels(new Rect(0, 0, currentDestination.width, currentDestination.height), 0, 0);
            influenceMap.Apply();
        }
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
                    goToPos = new Vector3(x - 20f, 0.0f, y - 20f);
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
