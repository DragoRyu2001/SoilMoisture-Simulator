using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Entities.UniversalDelegates;
using Unity.Rendering;
using static UnityEditor.PlayerSettings;

public class EvaportationSystem : ComponentSystem
{
    private float dryRate = 0.01f;
    protected override void OnUpdate()
    {
        Entities.ForEach((ref SoilComponent soilComponent) =>
        {
            if (soilComponent.moistureLevel > 0)
                soilComponent.moistureLevel -= dryRate * Time.DeltaTime;
            else
                soilComponent.moistureLevel = 0;

        });
    }
}
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
            Debug.Log("Texture==null");
            iLength = SoilEntity.soilEntities.GetLength(0);
            jLength = SoilEntity.soilEntities.GetLength(1);
            texture = SoilEntity.Instance._Texture;
            Debug.Log("Mock value of 25: " + (int)(((float)25 / (float)jLength) * texture.height));
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
        if(time>1.5f)
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
public class SoilColour : ComponentSystem
{
    private readonly Color[] color = new Color[4] { new Color(0, 1, 0, 1), new Color(0, .75f, 0.1f, 1), new Color(0, .6f, 0.2f, 1), new Color(0, 1, 1, 1) };
    private Color soilColor;
    protected override void OnUpdate()
    {
        Entities.ForEach((ref SoilComponent soil, ref URPMaterialPropertyBaseColor color) =>
        {
            soilColor = this.color[(int)soil.GetMoistureState()];
            color.Value = new float4(soilColor.r, soilColor.g, soilColor.b, soilColor.a);
        });
    }

}
public class ShareMoisture : ComponentSystem
{
    Entity[,]soilEntities;
    protected override void OnUpdate()
    {
        int2 pos;
        if(soilEntities==null)
        {
            soilEntities = SoilEntity.soilEntities;
        }
        Entities.ForEach((ref SoilComponent soil) =>
        {
            var sol = GetComponentDataFromEntity<SoilComponent>();
            pos = soil.position;
            SoilComponent soltmp;
            if (soil.position.x > 0)
            {
                soltmp = sol[soilEntities[pos.x - 1, pos.y]];
                float tmp = (soil.moistureLevel - soltmp.moistureLevel) / 16;
                soil.moistureLevel -= tmp * Time.DeltaTime;
                soltmp.moistureLevel += tmp * Time.DeltaTime;
            }
            if (soil.position.y > 0)
            {
                soltmp = sol[soilEntities[pos.x, pos.y - 1]];
                float tmp = (soil.moistureLevel - soltmp.moistureLevel) / 16;
                soil.moistureLevel -= tmp * Time.DeltaTime;
                soltmp.moistureLevel += tmp * Time.DeltaTime;
            }
            if (soil.position.x < soilEntities.GetLength(0) - 1)
            {
                soltmp = sol[soilEntities[pos.x + 1, pos.y]];
                float tmp = (soil.moistureLevel - soltmp.moistureLevel) / 16;
                soil.moistureLevel -= tmp * Time.DeltaTime;
                soltmp.moistureLevel += tmp * Time.DeltaTime;
            }
            if (soil.position.y < soilEntities.GetLength(1) - 1)
            {
                soltmp = sol[soilEntities[pos.x, pos.y + 1]];
                float tmp = (soil.moistureLevel - soltmp.moistureLevel) / 16;
                soil.moistureLevel -= tmp * Time.DeltaTime;
                soltmp.moistureLevel += tmp * Time.DeltaTime;
            }
        });
    }
}
