using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridRenderer : MonoBehaviour
{
    private Color baseColor = new Color(1f, 1f, 1f);
    private Material material;
    Vector3 originalSize;
    Vector2 gridPos;
    Vector3 manipulatedSize;
    public void InitGrid(Color color, float width, float height, Vector2 localOffset, Vector3 startPos)
    {
        gridPos = localOffset;

        //Set Position
        Vector3 pos = startPos + new Vector3((width * localOffset.x) +(width/ 2), startPos.y+.1f, (height * localOffset.y)+(height/2));
        transform.position = pos;

        //Set Mesh Size
        MeshRenderer mesh = GetComponent<MeshRenderer>();
        originalSize = new Vector3(mesh.bounds.size.x, mesh.bounds.size.y, mesh.bounds.size.z);
        manipulatedSize.x = width/ originalSize.x;
        manipulatedSize.y =  0.01f/ originalSize.y;
        manipulatedSize.z = height/ originalSize.z;
        transform.localScale = manipulatedSize;

        //Set Color
        material = mesh.material;
        baseColor = color;
        material.color = baseColor;
    }
    public void UpdateColor(Color updateColor)
    {
        if (updateColor == baseColor) return;
        material.color = updateColor;
        baseColor = updateColor;
    }
    private void OnMouseDrag()
    {
        Debug.Log("this was pressed");
        Ground.Instance.ReceiveMoisture(gridPos);
    }
}
