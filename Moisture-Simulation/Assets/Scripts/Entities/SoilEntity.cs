using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Rendering;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using System.Xml;
using System;
using static Unity.Burst.Intrinsics.X86.Avx;
using Unity.Entities.UniversalDelegates;

public class SoilEntity : MonoBehaviour
{
    [SerializeField] Mesh soilMesh;
    [SerializeField] Mesh rainMesh;
    [SerializeField] Material soilMaterial;
    [SerializeField] Material rainMaterial;
    [SerializeField] private int width = 10;
    [SerializeField] private Texture2D _texture;


    private enum EntityType
    {
        SOIL,
        CLOUD
    }
    public bool rain = false;
    public static Entity[,] soilEntities;
    public static SoilEntity Instance;
    public Texture2D _Texture
    {
        get { return _texture; }
        private set { _texture = value; }
    }
    EntityManager entityManager;
    EntityArchetype soilArchetype;
    private EntityArchetype rainArchetype;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    [BurstCompatible]
    private void Start()
    {
        //soilEntities = new Entity[width, width];
        //material.color = new Color(1, 0, 0);
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        soilArchetype = entityManager.CreateArchetype(typeof(SoilComponent), typeof(RenderMesh),
                                                        typeof(RenderBounds), typeof(Translation),
                                                        typeof(Scale), typeof(LocalToWorld), typeof(URPMaterialPropertyBaseColor));

        rainArchetype = entityManager.CreateArchetype(typeof(RainComponent),typeof(RenderMesh),
                                                        typeof(RenderBounds), typeof(Translation),
                                                        typeof(Scale), typeof(LocalToWorld), typeof(URPMaterialPropertyBaseColor));

        soilEntities = GenerateEntityArray(soilArchetype, width, 0f, soilMaterial, soilMesh, EntityType.SOIL);
        GenerateEntityArray(rainArchetype, width, 5f, rainMaterial, rainMesh, EntityType.CLOUD);

        rain = true;
    }

    private Entity[,] GenerateEntityArray(EntityArchetype entityArchetype, int dimension, float yPos, Material material, Mesh mesh, EntityType type)
    {
        Entity[,] entityArray = new Entity[dimension, dimension];
        NativeArray<Entity> entityArr = new NativeArray<Entity>(10000, Allocator.Temp);
        int i = 0;
        int j = 0;
        MeshRenderer _mesh = GetComponent<MeshRenderer>();

        entityManager.CreateEntity(entityArchetype, entityArr);
        float entScale = _mesh.bounds.size.x / (width * mesh.bounds.size.x);
        int2 pos;
        foreach (Entity entity in entityArr)
        {
            if (j == width)
            {
                i++;
                j = 0;
            }
            if (i == width)
            {
                break;
            }
            pos = new int2(i, j);
            entityManager.SetComponentData(entity, new Translation { Value = GridPosition(pos, mesh.bounds.size.x * entScale, yPos) });
            entityManager.SetComponentData(entity, new Scale { Value = entScale });
            switch (type)
            {
                case EntityType.SOIL:
                    entityManager.SetComponentData(entity, new SoilComponent { moistureLevel = UnityEngine.Random.Range(0, 1f), position = pos });
                    break;
                case EntityType.CLOUD:
                    entityManager.SetComponentData(entity, new RainComponent { position = pos });
                    break;
            }
            RenderMeshUtility.AddComponents(entity, entityManager, new RenderMeshDescription(mesh, material));

            entityArray[i, j] = entity;
            j++;
        }
        entityArr.Dispose();
        return entityArray;
    }

    private float3 GridPosition(int2 position, float3 soilSize, float yPos)
    {
        float x = position.x * soilSize.x + (0.5f * soilSize.x);
        float z = position.y * soilSize.z + (0.5f * soilSize.z);
        return new float3(x, yPos, z);
    }

}



