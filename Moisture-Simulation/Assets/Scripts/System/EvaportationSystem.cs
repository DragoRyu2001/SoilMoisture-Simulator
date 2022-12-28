using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Entities.UniversalDelegates;
using Unity.Rendering;
using static UnityEditor.PlayerSettings;
using Unity.Jobs;
using Unity.Burst;

public partial class EvaportationSystem : SystemBase
{
    private float dryRate = 0.01f;
    protected override void OnUpdate()
    {
        new Evaporate { dryRate = dryRate, deltaTime = Time.DeltaTime }.Schedule();
        //Entities.ForEach((ref SoilComponent soilComponent) =>
        //{
        //    if (soilComponent.moistureLevel > 0)
        //        soilComponent.moistureLevel -= dryRate * Time.DeltaTime;
        //    else
        //        soilComponent.moistureLevel = 0;

        //});
    }
}
[BurstCompile]
public partial struct Evaporate : IJobEntity
{
    public float dryRate;
    public float deltaTime;
    public void Execute(ref SoilComponent soilComponent)
    {
        if (soilComponent.moistureLevel > 0)
            soilComponent.moistureLevel -= dryRate * deltaTime;
        else
            soilComponent.moistureLevel = 0;
    }
}

