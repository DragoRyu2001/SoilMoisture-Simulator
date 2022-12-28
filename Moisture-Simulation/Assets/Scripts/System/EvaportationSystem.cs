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

