using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
public struct RainComponent : IComponentData
{
    public bool rain;
    public int2 position;
}
