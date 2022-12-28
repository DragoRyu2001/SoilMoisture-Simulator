using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Entities.UniversalDelegates;
using Unity.Rendering;
using Unity.Jobs;

public class CondensationSystem : ComponentSystem
{
    private float rainLevel = 0.05f;
    Texture2D texture;
    Color pixel;
    int iLength;
    int jLength;
    private bool onStart;
    float time = 0;
    protected override void OnUpdate()
    {
        if (SoilEntity.Instance == null) return;
        if (!SoilEntity.Instance.rain) return;

        if (texture == null)
        {

            iLength = SoilEntity.soilEntities.GetLength(0);
            jLength = SoilEntity.soilEntities.GetLength(1);
            texture = SoilEntity.Instance._Texture;
        }
        Entities.ForEach((ref SoilComponent soilComponent) =>
        {
            pixel = texture.GetPixel((int)(((float)soilComponent.position.x / (float)iLength) * texture.width), (int)(((float)soilComponent.position.y / (float)jLength) * texture.height));
            if (!pixel.Equals(Color.black))
            {
                soilComponent.moistureLevel += rainLevel * Time.DeltaTime;
            }
        });
        time += Time.DeltaTime;
        if (time > 1.5f)
        {
            time = 0;
            MoveDownPixel();
        }
    }
    private void MoveDownPixel()
    {
        Color pixel;
        for (int i = 0; i < texture.width; i++)
        {
            for (int j = 0; j < texture.height - 1; j++)
            {
                pixel = texture.GetPixel(i, j + 1);
                texture.SetPixel(i, j, pixel);
            }
            texture.SetPixel(i, texture.height - 1, texture.GetPixel(i, 0));
        }
    }
}