using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Burst;

public partial class SoilColour : SystemBase
{
    private Color soilColor;
    protected override void OnUpdate()
    {
        NativeArray<Color> colors = new NativeArray<Color>(new Color[4] { new Color(0, 1, 0, 1), new Color(0, .7f, 0f, 1), new Color(0, .5f, 0f, 1), new Color(0, 0.5f, 1, 1) }, Allocator.TempJob);
        var update = new ColorUpdate
        {
            color = colors
        }.ScheduleParallel();
        update.Complete();
        colors.Dispose();
        //Entities.ForEach((ref SoilComponent soil, ref URPMaterialPropertyBaseColor color) =>
        //{
        //    soilColor = this.color[(int)soil.GetMoistureState()];
        //    color.Value = new float4(soilColor.r, soilColor.g, soilColor.b, soilColor.a);
        //});
    }

}
[BurstCompile]
public partial struct ColorUpdate: IJobEntity
{
    [NativeDisableParallelForRestriction]
    public NativeArray<Color> color;
    public Color soilColor;
    public void Execute(ref SoilComponent soil, ref URPMaterialPropertyBaseColor color)
    {
        soilColor = this.color[(int)soil.GetMoistureState()];
        color.Value = new float4(soilColor.r, soilColor.g, soilColor.b, soilColor.a);
    }
}