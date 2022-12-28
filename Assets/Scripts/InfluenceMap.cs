using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D;
using UnityEngine.UI;

public class InfluenceMap : MonoBehaviour
{
    public Vector3 goToPos;

    [SerializeField]
    Image influenceMapImage;
    [SerializeField]
    LayerMask layerMask;
    [SerializeField]
    Material lowPass;
    [SerializeField]
    private int filterIterations;   // number of iterations for our low-pass filtering

    public Texture2D influenceMap;
    private float timer;
    
    // Start is called before the first frame update
    void Start()
    {
        timer = 0;        

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

        influenceMap.Apply();   // Apply changes to IM
        influenceMap.filterMode = FilterMode.Point;
        influenceMap.wrapMode = TextureWrapMode.Clamp;  // Clamp and not repeating for avoiding artifacts in corners
        influenceMapImage.DisableSpriteOptimizations();
        
        goToPos = transform.position;
        goToPos.y = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
       
        if (timer >= 0.5)
        {
            timer = 0;
            Decay();
        }

        LowPassFilter();        
    }

    // Method for adding point to IM
    public void AddPoint(Vector2 coord, float weight)
    {
        influenceMap.SetPixel((int)coord.x + 20, (int)coord.y + 20, new Color(weight, weight, weight));
        influenceMap.Apply();
    }

    // Method for step every texel towards default value 0.5
    private void Decay()
    {
        RenderTexture currentDestination = RenderTexture.GetTemporary(40, 40, 0);
        Graphics.Blit(influenceMap, currentDestination, lowPass, 2);
        influenceMap.ReadPixels(new Rect(0, 0, currentDestination.width, currentDestination.height), 0, 0);
        influenceMap.Apply();
    }

    // Method for applying lowpass filtering
    private void LowPassFilter()
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
   
    // Find highest most suitable point in IM and set as destination-coordinate
    public void FindPoint()
    {
        float highestValue = 0.0f;

        for (int y = 0; y < influenceMap.height; y++)
        {
            for (int x = 0; x < influenceMap.width; x++)
            {
                Color pixel = influenceMap.GetPixel(x, y);

                if (pixel.r > highestValue)
                {
                    float dist = Vector3.Distance(transform.position, new Vector3(x - 20, 0, y - 20));
                    Vector3 dir = new Vector3(x - 20, transform.position.y, y - 20) - transform.position;
                    if (!Physics.Raycast(transform.position, dir, dist+1, layerMask))
                    {
                        highestValue = pixel.r;
                        goToPos = new Vector3(x - 20f, 0.0f, y - 20f);
                    }
                }             
            }
        }
    }
}
