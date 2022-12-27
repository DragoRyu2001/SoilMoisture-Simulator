using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Ground : MonoBehaviour
{
    [SerializeField] GameObject gridRenderer;
    [SerializeField] private Color[] groundColors;
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;

    private Soil[,] soilGrid;
    private GridRenderer[,] gridRendes;
    private bool init = false;
    private float rainLevel = 0.75f;
    public static Ground Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer mesh = GetComponent<MeshRenderer>();
        Vector3 size = mesh.bounds.size;
        soilGrid = new Soil[width, height];
        gridRendes = new GridRenderer[width, height];
        for (int i = 0; i < soilGrid.GetLength(0); i++)
        {
            for (int j = 0; j < soilGrid.GetLength(1); j++)
            {
                soilGrid[i, j] = new Soil();
                GameObject obj = Instantiate(gridRenderer, this.transform);
                GridRenderer gridRend = obj.GetComponent<GridRenderer>();
                gridRendes[i, j] = gridRend;
                gridRendes[i, j].InitGrid(groundColors[0], size.x / width, size.z / height, new Vector2(i, j), Vector3.zero);
            }
        }
        soilGrid[width/2, height/2].isWaterSource = true;
        soilGrid[width / 2, height / 2].MoistureLevel = 1f;
        Rain.Instance.Init(new Vector2(width, height));
        init = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!init) return;
        for (int i = 0; i < soilGrid.GetLength(0); i++)
        {
            for (int j = 0; j < soilGrid.GetLength(1); j++)
            {
                soilGrid[i, j].UpdateSoil();
                gridRendes[i, j].UpdateColor(groundColors[(int)soilGrid[i, j].GetMoisture()]);
                ShareMoisture(i, j);
            }
        }
    }
    private void ShareMoisture(int i, int j)
    {
        float tmp;
        if (i > 0)
        {
            tmp = (soilGrid[i, j].MoistureLevel - soilGrid[i - 1, j].MoistureLevel) / 4;
            soilGrid[i, j].MoistureLevel -= soilGrid[i, j].isWaterSource ? 0 : tmp * Time.deltaTime;
            soilGrid[i - 1, j].MoistureLevel += soilGrid[i-1, j].isWaterSource ? 0 : tmp * Time.deltaTime;
        }
        if (j > 0)
        {
            tmp = (soilGrid[i, j].MoistureLevel - soilGrid[i, j - 1].MoistureLevel) / 4;
            soilGrid[i, j].MoistureLevel -= soilGrid[i, j].isWaterSource ? 0 : tmp * Time.deltaTime;
            soilGrid[i, j - 1].MoistureLevel += soilGrid[i, j-1].isWaterSource ? 0 : tmp * Time.deltaTime;
        }
        if (i < soilGrid.GetLength(0) - 1)
        {
            tmp = (soilGrid[i, j].MoistureLevel - soilGrid[i + 1, j].MoistureLevel) / 4;
            soilGrid[i, j].MoistureLevel -= soilGrid[i, j].isWaterSource ? 0 : tmp * Time.deltaTime;
            soilGrid[i + 1, j].MoistureLevel += soilGrid[i+1, j].isWaterSource ? 0 : tmp * Time.deltaTime;
        }
        if (j < soilGrid.GetLength(1) - 1)
        {
            tmp = (soilGrid[i, j].MoistureLevel - soilGrid[i, j + 1].MoistureLevel) / 4;
            soilGrid[i, j].MoistureLevel -= soilGrid[i, j].isWaterSource ? 0 : tmp * Time.deltaTime;
            soilGrid[i, j + 1].MoistureLevel += soilGrid[i, j+1].isWaterSource ? 0 : tmp * Time.deltaTime;
        }
    }
    public void ReceiveMoisture(Vector2 pos)
    {
        soilGrid[(int)pos.x, (int)pos.y].MoistureLevel += rainLevel;
    }
    public void SetMoisture(Vector2 pos, float rain)
    {
        soilGrid[(int)pos.x, (int)pos.y].MoistureLevel += rain;
    }
}
