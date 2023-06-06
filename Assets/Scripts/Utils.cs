using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static List<Vector3> NormalSmpling(Vector3 oriPos, float r, int pointCount)
    {
        List<Vector3> points = new List<Vector3>();

        float sqrR = r * r;
        float xMax = oriPos.x + r;
        float xMin = oriPos.x - r;
        float zMax = oriPos.z + r;
        float zMin = oriPos.z - r;

// 均匀生成正方形边界内的点，并筛选在圆内的点
        for (int i = 0; i < pointCount; i++)
        {
            float x = Random.Range(xMin, xMax);
            float z = Random.Range(zMin, zMax);
            Vector3 point = new Vector3(x, oriPos.y, z);
            float sqrDistance = (point - oriPos).sqrMagnitude;
            if (sqrDistance <= sqrR)
            {
                points.Add(point);
            }
        }

        return points;
    }
    
    // oriPos 为圆心位置，r 为半径，minDist 为两点之间的最小距离
    public static List<Vector3> PoissonDiscSampling(Vector3 oriPos, float r, float minDist, int posNum)
{
    List<Vector3> points = new List<Vector3>();
    Queue<Vector2> processList = new Queue<Vector2>();
    float cellSize = minDist / Mathf.Sqrt(2); // Cell 的大小
    int[,] grid = new int[Mathf.CeilToInt(2 * r / cellSize), Mathf.CeilToInt(2 * r / cellSize)]; // 记录每个 Cell 中的点数
    Vector2 initialPointCell = new Vector2(Mathf.Floor(oriPos.x / cellSize), Mathf.Floor(oriPos.z / cellSize)); 
    processList.Enqueue(initialPointCell);
    while (processList.Count > 0)
    {
        Vector2 centerCell = processList.Dequeue();
        for (int i = 0; i < posNum; i++) // 尝试 posNum 次寻找周围的点
        {
            float angle = Random.Range(0.0f, 2 * Mathf.PI);
            float radius = Random.Range(minDist, 2 * minDist);
            Vector2 offset = new Vector2(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle)); // 偏移量
            Vector2 newCell = centerCell + offset / cellSize;
            if (newCell.x < 0 || newCell.x >= grid.GetLength(0) || newCell.y < 0 || newCell.y >= grid.GetLength(1))
            {
                continue;
            }
            int count = 0; // 统计相邻的点数
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int checkX = Mathf.FloorToInt(newCell.x) + x;
                    int checkY = Mathf.FloorToInt(newCell.y) + y;
                    if (checkX >= 0 && checkX < grid.GetLength(0) && checkY >= 0 && checkY < grid.GetLength(1))
                    {
                        count += grid[checkX, checkY];
                    }
                }
            }
            if (count == 0)
            {
                Vector3 point = new Vector3(newCell.x * cellSize, 0, newCell.y * cellSize) + oriPos;
                if ((point - oriPos).magnitude <= r)
                {
                    points.Add(point);
                    processList.Enqueue(newCell);
                    grid[Mathf.FloorToInt(newCell.x), Mathf.FloorToInt(newCell.y)] = 1;
                }
            }
        }
    }
    return points;
}
}
