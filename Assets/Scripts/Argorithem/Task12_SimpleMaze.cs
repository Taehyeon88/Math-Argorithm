using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Task12_SimpleMaze : MonoBehaviour
{
    [Header("미로 생성 변수")]
    [SerializeField] int mazeSize = 9;

    [Header("비주얼 오브젝트")]
    [SerializeField] Color roadColor = Color.white;
    [SerializeField] Color forestColor = Color.green;
    [SerializeField] Color mudeColor = Color.red;
    [SerializeField] GameObject wall;
    [SerializeField] GameObject floor;
    [SerializeField] GameObject mark;
    [SerializeField] GameObject player;       //플레이어 오브젝트
    [SerializeField] Button showPathButton;   //최단 거리 시각화 버튼
    [SerializeField] Button autoMoveButton;   //플레이어 자동 이동 버튼

    GameObject levelParent;

    //1=벽, 0=길 (아주 작은 예시 맵)
    int[,] map;
    bool[,] visited;    //방문 기록
    List<Vector2Int> path;
    Vector2Int start = new Vector2Int(1, 1);
    Vector2Int goal;
    Vector2Int[] dirs = { new(1, 0), new(-1, 0), new(0, 1), new(0, -1) };

    bool playerMoving = false;

    void Start()
    {
        showPathButton.onClick.AddListener(ShowShortestPath);
        autoMoveButton.onClick.AddListener(() =>
        {
            if (!playerMoving)
            {
                StartCoroutine(PlayerMove());
                playerMoving = true;
            }
        });

        GenerateMaze();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (playerMoving)
            {
                Debug.Log("플레이어가 이동중입니다.");
                return;
            }
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
                if (x == 0 || y == 0 || x == map.GetLength(0) - 1 || y == map.GetLength(1) - 1)  //맵 외곽에 벽생성
                    map[x, y] = 0;
                else                                   //길 혹은 벽으로 랜덤 생성
                {
                    float randomValue = UnityEngine.Random.value;
                    if (randomValue <= 0.25f) map[x, y] = 0;
                    else if (randomValue <= 0.65f) map[x, y] = 1;
                    else if (randomValue <= 0.85f) map[x, y] = 2;
                    else map[x, y] = 3;
                }
            }
        }

        visited = new bool[map.GetLength(0), map.GetLength(1)];
        bool ok = SearchMaze(start.x, start.y);
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
                    if (map[x, y] == 0 && (x != mazeSize - 2 || y != mazeSize - 2))
                    {
                        GameObject obj = Instantiate(wall, new Vector3(x, 0, y), Quaternion.identity);
                        obj.transform.SetParent(levelParent.transform);
                    }
                    else
                    {
                        GameObject obj = Instantiate(floor, new Vector3(x, 0, y), Quaternion.identity);
                        obj.transform.SetParent(levelParent.transform);

                        Material mat = obj.transform.GetChild(0).GetComponent<Renderer>().material;
                        switch (map[x, y])
                        {
                            case 1:
                                mat.color = roadColor; break;
                            case 2:
                                mat.color = forestColor; break;
                            case 3:
                                mat.color = mudeColor; break;
                        }
                    }
                }
            }

            //최단 거리 탐색 (최소 비용)
            path = Dijkstra(map, start, goal);

            if (path == null) Debug.Log("경로 없음");
            else
            {
                Debug.Log($"경로 길이: {path.Count}");
                foreach (var p in path)
                    Debug.Log(p);
            }
        }
    }

    bool SearchMaze(int x, int y)
    {
        //범위/벽/재방문 체크
        if (x < 0 || y < 0 || x >= map.GetLength(0) || y >= map.GetLength(1)) return false;
        if (map[x, y] == 0 || visited[x, y]) return false;

        //방문 표시
        visited[x, y] = true;
        Debug.Log($"이동: ({x},{y})");

        //목표 도달?
        if (x == goal.x && y == goal.y) return true;

        //4방향 재귀 탐색
        foreach (var d in dirs)
            if (SearchMaze(x + d.x, y + d.y)) return true;

        //막혔으면 되돌아감
        Debug.Log($"되돌아감: ({x}, {y})");
        return false;
    }

    void ShowShortestPath()
    {
        foreach (var p in path)
        {
            GameObject obj = Instantiate(mark, new Vector3(p.x, 0, p.y), Quaternion.identity);
            obj.transform.SetParent(levelParent.transform);
        }
    }

    IEnumerator PlayerMove()
    {
        float delayTime = 0.3f;      //플레이어가 다음 칸으로 이동까지의 대기시간

        if (player == null)
        {
            Debug.LogError("인스팩터창에 playerObject가 할당되지 않았습니다.");
            yield break;
        }
        Queue<Vector2Int> temp = new Queue<Vector2Int>(this.path);

        while (temp.Count > 0)
        {
            Vector2Int targetPos = temp.Dequeue();

            player.transform.position = new Vector3(targetPos.x, 0.5f, targetPos.y);

            yield return new WaitForSeconds(delayTime);
        }

        playerMoving = false;
    }

    void Initialize()
    {
        Destroy(levelParent);
    }

    List<Vector2Int> Dijkstra(int[,] map, Vector2Int start, Vector2Int goal)
    {
        int w = map.GetLength(0);
        int h = map.GetLength(1);

        int[,] dist = new int[w, h];                       //지금까지 온 최소 비용
        bool[,] visited = new bool[w, h];                  //확정 여부
        Vector2Int?[,] parent = new Vector2Int?[w, h];     //경로 복원용

        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
                dist[x, y] = int.MaxValue;

        dist[start.x, start.y] = 0;

        Vector2Int[] dirs =
        {
            new Vector2Int(1,0),
            new Vector2Int(-1,0),
            new Vector2Int(0,1),
            new Vector2Int(0,-1),
        };

        Test6_PriorityQueue<Vector2Int> openQue = new Test6_PriorityQueue<Vector2Int>();

        openQue.Enqueue(start, 0);

        while (openQue.Count > 0)
        {
            Vector2Int cur = openQue.Dequeue();

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

                int moveCost = TileCost(map[nx, ny]);  //cur -> (nx, my) 비용
                if (moveCost == int.MaxValue) continue;

                int newDist = dist[cur.x, cur.y] + moveCost;

                // 더 싼 길 발견
                if (newDist < dist[nx, ny])
                {
                    dist[nx, ny] = newDist;
                    parent[nx, ny] = cur;

                    if (!visited[nx, ny] && !openQue.Contains(new Vector2Int(nx, ny)))
                        openQue.Enqueue(new Vector2Int(nx, ny), newDist);
                }
            }
        }

        return null;
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
