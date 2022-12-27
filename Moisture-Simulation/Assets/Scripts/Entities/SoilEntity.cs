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
    [SerializeField] Mesh mesh;
    [SerializeField] Material material;
    [SerializeField] private int width = 10;
    [SerializeField] private Texture2D _texture;
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
        soilEntities = new Entity[width, width];
        material.color = new Color(1, 0, 0);
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        soilArchetype = entityManager.CreateArchetype(typeof(SoilComponent), typeof(RenderMesh), typeof(RenderBounds), typeof(Translation), typeof(Scale), typeof(LocalToWorld), typeof(URPMaterialPropertyBaseColor));
        NativeArray<Entity> entityArr = new NativeArray<Entity>(10000, Allocator.Temp);

        entityManager.CreateEntity(soilArchetype, entityArr);
        MeshRenderer _mesh = GetComponent<MeshRenderer>();
        float entScale = _mesh.bounds.size.x / (width * mesh.bounds.size.x);
        int i = 0;
        int j = 0;
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
            entityManager.SetComponentData(entity, new Translation { Value = GridPosition(pos, mesh.bounds.size.x * entScale) });
            entityManager.SetComponentData(entity, new Scale { Value = entScale });
            entityManager.SetComponentData(entity, new SoilComponent { moistureLevel = UnityEngine.Random.Range(0, 1f), position = pos });
            RenderMeshUtility.AddComponents(entity, entityManager, new RenderMeshDescription(mesh, material));

            soilEntities[i, j] = entity;
            j++;
        }
        entityArr.Dispose();
        rain = true;
    }
    private float3 GridPosition(int2 position, float3 soilSize)
    {
        float x = position.x * soilSize.x + (0.5f * soilSize.x);
        float z = position.y * soilSize.z + (0.5f * soilSize.z);
        return new float3(x, 0, z);
    }
    private NativeArray<Entity> FindNeighbours(int2 pos)
    {
        int size = 0;
        if (pos.x > 0)
            size++;
        if(pos.x < soilEntities.GetLength(0) - 1)
            size++;
        if (pos.y > 0)
            size++;
        if(pos.y < soilEntities.GetLength(0) - 1)
            size++;

        
        NativeArray<Entity> entities = new NativeArray<Entity>(size, Allocator.Persistent);
        int i = 0;
        if (pos.x > 0)
        {
            entities[i++] = soilEntities[pos.x - 1, pos.y];
        }
        if (pos.y > 0)
        {
            entities[i++] = soilEntities[pos.x, pos.y - 1];
        }
        if (pos.x < soilEntities.GetLength(0) - 1)
        {
            entities[i++] = soilEntities[pos.x + 1, pos.y];
        }
        if (pos.y < soilEntities.GetLength(1) - 1)
        {
            entities[i++] = soilEntities[pos.x, pos.y + 1];
        }
        return entities;
    }

}



