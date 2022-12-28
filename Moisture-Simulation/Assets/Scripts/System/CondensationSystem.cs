using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Entities.UniversalDelegates;
using Unity.Rendering;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;

public partial class CondensationSystem : SystemBase
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
        var moisture = new UpdateMoisture
        {
            texture = texture.GetRawTextureData<Color32>(),
            iLength = iLength,
            jLength = jLength,
            textureWidth = texture.width,
            textureHeight = texture.height,
            deltaTime = Time.DeltaTime,
            rainLevel = rainLevel
        }.ScheduleParallel();
        moisture.Complete();
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
[BurstCompile]
public partial struct UpdateMoisture : IJobEntity
{
    [NativeDisableParallelForRestriction]
    public NativeArray<Color32> texture;
    public Color pixel;
    public int iLength;
    public int jLength;
    public int textureWidth;
    public int textureHeight;
    public float deltaTime;
    public float rainLevel;
    public void Execute(ref SoilComponent soil)
    {
        int i = (int)(((float)soil.position.x / (float)iLength) * textureWidth * textureWidth);
        int j = (int)((float)soil.position.y / (float)jLength * textureHeight);
        pixel = texture[i+j];
        if (!pixel.Equals(Color.black))
        {
            soil.moistureLevel += rainLevel * deltaTime;
        }
    }
}