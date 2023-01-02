using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Rendering;
using Unity.Mathematics;

public partial class CloudColour : SystemBase
{
    float4 visibleColor = new(0, 1, 1, 1);
    float4 invisibleColor = new(0, 1, 1, 0);
    protected override void OnUpdate()
    {
        var cloudUpdate = new CloudColor
        {
            visibleColor = visibleColor,
            invisibleColor = invisibleColor
        }.ScheduleParallel();
    }
}
public partial struct CloudColor : IJobEntity
{
    public float4 visibleColor, invisibleColor;
    public void Execute(ref RainComponent rain, ref URPMaterialPropertyBaseColor baseColor)
    {
        baseColor.Value = rain.rain ? visibleColor : invisibleColor;
    }
}