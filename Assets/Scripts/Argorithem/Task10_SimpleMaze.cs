using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Task10_SimpleMaze : MonoBehaviour
{
    [Header("미로 생성 변수")]
    [SerializeField] int mazeSize = 9;
    [SerializeField][Range(0,1)] float roadSpawnRate = 0.75f; 

    [Header("비주얼 오브젝트")]
    [SerializeField] GameObject wall;
    [SerializeField] GameObject road;
    [SerializeField] GameObject mark;
    [SerializeField] GameObject levelParent;

    //1=벽, 0=길 (아주 작은 예시 맵)
    int[,] map;
    bool[,] visited;    //방문 기록
    List<(int, int)> markedRoot = new List<(int,int)>();
    Vector2Int goal;
    Vector2Int[] dirs = { new(1, 0), new(-1, 0), new(0, 1), new(0, -1) };

    void Start()
    {
        GenerateMaze();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ShowMarkedRoot();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Initialize();
            GenerateMaze();
        }
    }

    void GenerateMaze()
    {
        map = new int[mazeSize, mazeSize];
        goal = new Vector2Int(mazeSize - 2, mazeSize - 2);

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if(x == 0 || y == 0 || x == map.GetLength(0) - 1 || y == map.GetLength(1) - 1)  //맵 외곽에 벽생성
                    map[x, y] = 1;
                else                                   //길 혹은 벽으로 랜덤 생성
                {
                    float randomValue = UnityEngine.Random.value;
                    if (randomValue <= roadSpawnRate) map[x, y] = 0;
                    else map[x, y] = 1;
                }
            }
        }

        visited = new bool[map.GetLength(0), map.GetLength(1)];
        bool ok = SearchMaze(1, 1, new List<(int,int)>());  //시작점 (1,1)
        Debug.Log(ok ? "출구 찾음!" : "출구  없음");
        if (!ok) GenerateMaze();
        else
        {
            levelParent = new GameObject();
            levelParent.transform.SetParent(transform);

            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                { 
                    if (map[x, y] == 0 || (x == mazeSize - 2 && y == mazeSize - 2))
                    {
                        GameObject obj = Instantiate(road, new Vector3(x, 0, y), Quaternion.identity);
                        obj.transform.SetParent(levelParent.transform);
                    }
                    else if (map[x, y] == 1)
                    {
                        GameObject obj = Instantiate(wall, new Vector3(x, 0, y), Quaternion.identity);
                        obj.transform.SetParent(levelParent.transform);
                    }
                }
            }
        }
    }

    bool SearchMaze(int x, int y, List<(int,int)> root)
    {
        //범위/벽/재방문 체크
        if (x < 0 || y < 0 || x >= map.GetLength(0) || y >= map.GetLength(1)) return false;
        if (map[x, y] == 1 || visited[x, y]) return false;

        //방문 표시
        visited[x, y] = true;
        root.Add((x, y));
        Debug.Log($"이동: ({x},{y})");

        //목표 도달?
        if (x == goal.x && y == goal.y)
        {
            markedRoot = root;
            return true;
        }

        //4방향 재귀 탐색
        foreach (var d in dirs)
        {
            List<(int,int)> rootTemp = new List<(int, int)>(root);
            if (SearchMaze(x + d.x, y + d.y, rootTemp)) return true;
        }

        //막혔으면 되돌아감
        Debug.Log($"되돌아감: ({x}, {y})");
        root.RemoveAt(root.Count - 1);
        return false;
    }

    void ShowMarkedRoot()
    {
        for (int x = 0; x < mazeSize; x++)
        {
            for (int y = 0; y < mazeSize; y++)
            {
                if (markedRoot.Contains((x, y)))
                {
                    GameObject obj = Instantiate(mark, new Vector3(x, 0, y), Quaternion.identity);
                    obj.transform.SetParent(levelParent.transform);
                }
            }
        }
    }

    void Initialize()
    {
        Destroy(levelParent);
    }
}
