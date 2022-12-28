using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

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