using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

public struct SoilComponent: IComponentData
{
    public int2 position;
    public float moistureLevel;
    public MoistureState state;
    public MoistureState GetMoistureState()
    {
        if (moistureLevel > 0.75f)
            return MoistureState.WET;
        if (moistureLevel > 0.5f)
            return MoistureState.MOIST;
        if (moistureLevel > 0.25)
            return MoistureState.DRYING;
        return MoistureState.DRY;
    }
}
