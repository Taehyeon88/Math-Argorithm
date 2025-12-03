using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test13_Astar : MonoBehaviour
{
    // 0 = 벽, 1 = 평지, 2 = 숲, 3 = 진흙
    int[,] map =
{
        {0,0,0,0,0,0,0 },
        {0,1,1,1,1,1,0 },
        {0,1,2,2,1,1,0 },
        {0,1,3,1,1,1,0 },
        {0,1,1,1,1,1,0 },
        {0,0,0,0,0,0,0 }
    };

    Vector2Int start = new Vector2Int(1, 1);
    Vector2Int goal = new Vector2Int(4, 5);

    void Start()
    {
        var path = AStar(map, start, goal);

        if (path == null) Debug.Log("경로 없음");
        else
        {
            Debug.Log($"경로 길이: {path.Count}");
            foreach (var p in path)
                Debug.Log(p);
        }
    }

    List<Vector2Int> AStar(int[,] map, Vector2Int start, Vector2Int goal)
    {
        int w = map.GetLength(0);
        int h = map.GetLength(1);

        int[,] gCost = new int[w, h];                       //지금까지 온 최소 비용
        bool[,] visited = new bool[w, h];                  //확정 여부
        Vector2Int?[,] parent = new Vector2Int?[w, h];     //경로 복원용

        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
                gCost[x, y] = int.MaxValue;

        gCost[start.x, start.y] = 0;

        Vector2Int[] dirs =
        {
            new Vector2Int(1,0),
            new Vector2Int(-1,0),
            new Vector2Int(0,1),
            new Vector2Int(0,-1),
        };

        List<Vector2Int> open = new List<Vector2Int>();
        open.Add(start);

        while (open.Count > 0)
        {
            int bestIndex = 0;
            int bestF = F(open[0], gCost, goal);
            for (int i = 1; i < open.Count; i++)
            {
                int f = F(open[i], gCost, goal);
                if (f < bestF)
                {
                    bestF = f;
                    bestIndex = i;
                }
            }
            Vector2Int cur = open[bestIndex];
            open.RemoveAt(bestIndex);

            if (visited[cur.x, cur.y]) continue;
            visited[cur.x, cur.y] = true;

            if (cur == goal)
                return ReconstructPath(parent, start, goal);

            foreach (var d in dirs)
            {
                int nx = cur.x + d.x;
                int ny = cur.y + d.y;

                if (!InBounds(map, nx, ny)) continue;
                if (map[nx, ny] == 0) continue;        //벽
                if(visited[nx, ny]) continue;

                int moveCost = TileCost(map[nx, ny]);  //cur -> (nx, my) 비용
                int newG = gCost[cur.x, cur.y] + moveCost;

                // 더 싼 길 발견
                if (newG < gCost[nx, ny])
                {
                    gCost[nx, ny] = newG;
                    parent[nx, ny] = cur;

                    if (!open.Contains(new Vector2Int(nx, ny)))
                        open.Add(new Vector2Int(nx, ny));
                }
            }
        }

        return null;
    }

    int F(Vector2Int pos, int[,] gCost, Vector2Int goal)
    {
        return gCost[pos.x, pos.y] + H(pos, goal);
    }

    int H(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y- b.y);
    }

    int TileCost(int tile)
    {
        switch (tile)
        {
            case 1: return 1;   //평지
            case 2: return 2;   //숲
            case 3: return 5;   //진흙
            default: return int.MaxValue;  //0=벽 포함
        }
    }

    bool InBounds(int[,] map, int x, int y)
    {
        return x >= 0 && y >= 0 &&
               x < map.GetLength(0) &&
               y < map.GetLength(1);
    }

    List<Vector2Int> ReconstructPath(Vector2Int?[,] parent, Vector2Int start, Vector2Int goal)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int? cur = goal;

        while (cur.HasValue)
        {
            path.Add(cur.Value);
            if (cur.Value == start) break;
            cur = parent[cur.Value.x, cur.Value.y];
        }

        path.Reverse();
        return path;
    }
}
