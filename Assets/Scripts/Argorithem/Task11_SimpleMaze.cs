using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Task11_SimpleMaze : MonoBehaviour
{
    [Header("미로 생성 변수")]
    [SerializeField] int mazeSize = 9;
    [SerializeField][Range(0, 1)] float roadSpawnRate = 0.75f;

    [Header("비주얼 오브젝트")]
    [SerializeField] GameObject wall;
    [SerializeField] GameObject road;
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
    Vector2Int?[,] parent;
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

            //최단 거리 탐색
            FindPathBFS();
        }
    }

    bool SearchMaze(int x, int y)
    {
        //범위/벽/재방문 체크
        if (x < 0 || y < 0 || x >= map.GetLength(0) || y >= map.GetLength(1)) return false;
        if (map[x, y] == 1 || visited[x, y]) return false;

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

    void FindPathBFS()
    {
        int w = map.GetLength(0);
        int h = map.GetLength(1);
        visited = new bool[w, h];
        parent = new Vector2Int?[w, h];
        Queue<Vector2Int> q = new Queue<Vector2Int>();
        q.Enqueue(start);
        visited[start.x, start.y] = true;

        //추가 과제
        int currentDepth = 0;
        int count1 = 0;
        int count2 = 0;
        while (q.Count > 0)
        {
            Vector2Int cur = q.Dequeue();

            //목표 도착
            if (cur == goal)
            {
                Debug.Log("BSF: GOAL 도착!");
                path = ReconstructPath();
            }

            //네 방향 이웃 탐색
            foreach (var d in dirs)
            {
                int nx = cur.x + d.x;
                int ny = cur.y + d.y;

                if (!InBounds(nx, ny)) continue;  //전체 바운더리
                if (map[nx, ny] == 1) continue;   //벽
                if (visited[nx, ny]) continue;    //이미 방문

                visited[nx, ny] = true;
                parent[nx, ny] = cur;             //경로 복원용 부모
                q.Enqueue(new Vector2Int(nx, ny));
            }
        }
    }

    bool InBounds(int x, int y)
    {
        return x >= 0 && y >= 0 &&
               x < map.GetLength(0) &&
               y < map.GetLength(1);
    }

    List<Vector2Int> ReconstructPath()
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int? cur = goal;

        //goal -> start 방향으로 parent 따라가기
        while (cur.HasValue)
        {
            path.Add(cur.Value);
            cur = parent[cur.Value.x, cur.Value.y];
        }

        path.Reverse();   //start -> goal 순서로 반전
        Debug.Log($"경로 길이: {path.Count}");
        foreach (var p in path)
        {
            Debug.Log(p);
        }
        return path;
    }

    void GenerateMaxPos()
    {

    }
}
