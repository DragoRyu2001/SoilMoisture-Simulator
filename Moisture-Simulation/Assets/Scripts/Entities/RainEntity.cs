using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;

public class RainEntity : MonoBehaviour
{
    private EntityManager entityManager;
    private EntityArchetype rainArchetype;

    private void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        //float entScale = _mesh.bounds.size.x / (width * Mesh.bounds.size.x);
    }
}
