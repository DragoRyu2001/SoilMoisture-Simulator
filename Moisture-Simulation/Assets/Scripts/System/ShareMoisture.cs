using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Entities.UniversalDelegates;
using Unity.Rendering;

public class ShareMoisture : ComponentSystem
{
    Entity[,] soilEntities;
    protected override void OnUpdate()
    {
        int2 pos;
        if (soilEntities == null)
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