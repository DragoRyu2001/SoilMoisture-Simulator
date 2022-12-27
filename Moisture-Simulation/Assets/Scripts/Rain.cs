using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rain : MonoBehaviour
{
    [SerializeField] Texture2D texture;

    public static Rain Instance;
    private readonly float rainLevel = 0.1f;
    private bool startRaining = false;
    private void Awake()
    {
        if(Instance==null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    public void Init(Vector2 gridDim)
    {
        InvokeRepeating(nameof(MoveClouds), 0f, 1f);
        MoveClouds();
        startRaining = true;
    }
    private void Update()
    {
        if (!startRaining) return;   
        RainUpdate();
    }
    private void MoveClouds()
    {
        Color pixel;
        for(int i =0; i< texture.width; i++)
        {
            for(int j =0; j< texture.height-1; j++)
            {
                pixel = texture.GetPixel(i, j+1);
                texture.SetPixel(i, j, pixel);
            }
            texture.SetPixel(i, texture.height - 1, texture.GetPixel(i, 0));
        }
    }
    private void RainUpdate()
    {
        Color pixel;
        
        for(int i =0; i< texture.width; i++)
        {
            for (int j = 0; j< texture.height; j++)
            {
                pixel = texture.GetPixel(i, j);
                if(pixel != Color.black)
                {
                    Ground.Instance.SetMoisture(new Vector2(i, j), rainLevel * Time.deltaTime);
                }
            }
        }
    }
}
