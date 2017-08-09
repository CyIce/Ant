using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 控制角色的移动
/// </summary>

public class AntMovingController : MonoBehaviour {

    private struct Line
    {
        public int x;
        public int z;
        public int last;
        public int position;
    };

    private GameMapController gameMapController;
    private RockAttributes attributes;
    //角色每一步的始点和终点
    private Vector3 beginPos;
    private Vector3 endPos;
    //角色移动的速度
    public float speed = 1.0f;

    private float startTime;

    private float journeyLength;
    //记录角色移动的路线
    private List<iVector2> road = new List<iVector2>();
    //保存移动的四个方向
    private int[,] direction = new int[4, 2];
    //记录表搜索过的节点
    private int[,] mark = new int[11, 31];
    //记录角色上方的岩石
    private GameObject topRock = null;
    //角色上方岩石的标记
    private int topRockMark = 0;
    //装载可移动岩石的父物体
    public GameObject movableRockContainer;

    private void Awake()
    {
        gameMapController = GameObject.Find("Main Camera").GetComponent<GameMapController>();
    }

    void Start ()
    {
        gameMapController.gameState = GameMapController.GameState.GAME;
        initDirection();
        beginPos = gameObject.transform.position;
        endPos = beginPos;
        startTime = Time.time;
        journeyLength = 1;
    }


    void Update ()
    {
        if (gameMapController.gameState == GameMapController.GameState.GAME)
        {
            mouseControl();
        }
        else if (gameMapController.gameState == GameMapController.GameState.MOVING)
        {
            antMoving();
        }
    }

    /// <summary>
    /// 检测鼠标点击
    /// </summary>
    void mouseControl()
    {
        int inputType = 0;
        if (Input.GetMouseButtonDown(0))
        {
            inputType = 1;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            inputType = 2;
        }
        if (inputType != 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if ((hit.transform.tag == "MovableRock" && inputType == 1) ||
                    hit.transform.tag != "MovableRock" && inputType == 2)
                {
                    return;
                }

                Vector3 hitPos = hit.point;
                if (hitPos.x < 0 && hitPos.x >= -11 && hitPos.z < 0 && hitPos.z >= -31)
                {
                    hitPos.x = (-hitPos.x) + 1;
                    hitPos.z = (-hitPos.z) + 1;

                    Bfs((int)hitPos.x, (int)hitPos.z);

                    gameMapController.gameState = GameMapController.GameState.MOVING;

                    startTime = Time.time;
                }
            }
        }
    }

    /// <summary>
    /// 角色移动
    /// </summary>
    void antMoving()
    {
        if (road.Count > 1)
        {
            if(topRock!=null&& gameMapController.gameState == GameMapController.GameState.GAME)
            {
                topRock.GetComponent<RockAttributes>().markingGameMap(topRock.transform.position, false);
            }
            gameMapController.gameState = GameMapController.GameState.MOVING;

            beginPos.x = -road[0].x + 0.5f;
            beginPos.z = -road[0].z + 0.5f;
            endPos.x = -road[1].x + 0.5f;
            endPos.z = -road[1].z + 0.5f;

            if (topRock != null && gameObject.transform.position == beginPos) 
            {
                //如果岩石无法向前移动，则切断角色和岩石的联系
                if (isConllider(endPos - beginPos) == true) 
                {
                    topRock.transform.parent = movableRockContainer.transform;
                    topRock.GetComponent<RockAttributes>().markingGameMap(topRock.transform.position, true);
                    topRock = null;
                }
                //如果岩石可以继续向前移动，则清除岩石在当前位置的标记信息
                else 
                {
                    topRock.GetComponent<RockAttributes>().markingGameMap(topRock.transform.position, false);
                }
            }

            //控制角色匀速移动
            float distCovered = (Time.time - startTime) * speed;
            float fracJourney = distCovered / journeyLength;
            gameObject.transform.position = Vector3.Lerp(beginPos, endPos, fracJourney);

            if (gameObject.transform.position == endPos)
            {
                if (topRock == null && gameMapController.gameMap[road[1].x, road[1].z] > 1)
                {
                    topRockMark = gameMapController.gameMap[road[1].x, road[1].z];
                    getTopRock(new Vector3(endPos.x, 1, endPos.z));
                }
                
                startTime = Time.time;
                road.RemoveAt(0);
            }
        }
        else
        {
            gameMapController.gameState= GameMapController.GameState.GAME;
            if (topRock != null)
            {
                topRock.GetComponent<RockAttributes>().markingGameMap(topRock.transform.position, true);
            }
        }
    }

    /// <summary>
    /// 通过玩家点击的位置搜索可移动的路径
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    public void Bfs(int x, int z)
    {
        road.Clear();
        initMark();

        Line[] line = new Line[2000];
        int length;
        int endIndex = 0;

        line[0].x = -(int)gameObject.transform.position.x + 1;
        line[0].z = -(int)gameObject.transform.position.z + 1;
        line[0].last = -1;
        line[0].position = gameMapController.gameMap[line[0].x, line[0].z];
        mark[line[0].x, line[0].z] = 1;

        length = 1;

        int i, j;
        int nowX, nowZ;

        for (i = 0; i < length; i++)
        {
            for (j = 0; j < 4; j++)
            {
                if (x == line[i].x && z == line[i].z)
                {
                    endIndex = i;
                    break;
                }

                nowX = line[i].x + direction[j, 0];
                nowZ = line[i].z + direction[j, 1];

                if ((nowX < 0 || nowX >= gameMapController.mapSize.x) || (nowZ < 0 || nowZ >= gameMapController.mapSize.z) || (mark[nowX, nowZ] == 1))
                {
                    continue;
                }

                if (((line[i].position == 11) ||
                   (line[i].position == 12 && j <= 1) ||
                   (line[i].position == 13 && j > 1)) &&
                   gameMapController.gameMap[nowX, nowZ] == 0)
                {
                    mark[nowX, nowZ] = 1;
                    line[length].x = nowX;
                    line[length].z = nowZ;
                    line[length].last = i;
                    if (line[i].position == 11)
                    {
                        if (j <= 1)
                        {
                            line[length].position = 12;
                        }
                        else
                        {
                            line[length].position = 13;
                        }
                    }
                    else
                    {
                        line[length].position = line[i].position;
                    }

                    length++;
                }
                else if (gameMapController.gameMap[nowX, nowZ] != 0)
                {
                    mark[nowX, nowZ] = 1;
                    line[length].x = nowX;
                    line[length].z = nowZ;
                    line[length].last = i;
                    line[length].position = gameMapController.gameMap[nowX, nowZ];
                    length++;
                }

            }

            if (endIndex != 0)
            {
                break;
            }
        }

        iVector2 tmp;
        while (endIndex != -1)
        {
            tmp.x = line[endIndex].x;
            tmp.z = line[endIndex].z;
            road.Add(tmp);
            endIndex = line[endIndex].last;
        }

        road.Reverse();

        Vector2 v;

        for (i = 0; i < road.Count; i++)
        {
            v.x = road[i].x;
            v.y = road[i].z;
        }
    }

    /// <summary>
    /// 初始化方向数组
    /// </summary>
    void initDirection()
    {
        direction[0, 0] = -1;
        direction[0, 1] = 0;

        direction[1, 0] = 1;
        direction[1, 1] = 0;

        direction[2, 0] = -0;
        direction[2, 1] = -1;

        direction[3, 0] = 0;
        direction[3, 1] = 1;
    }

    /// <summary>
    /// 初始化标记数组
    /// </summary>
    void initMark()
    {
        for (int i = 0; i < gameMapController.mapSize.x; i++)
        {
            for (int j = 0; j < gameMapController.mapSize.z; j++)
            {
                mark[i, j] = 0;
            }
        }
    }

    void getTopRock(Vector3 pos)
    {
        RaycastHit hit;
        if (Physics.Raycast(pos, gameMapController.rainRay, out hit, 1))
        {
            topRock = hit.collider.gameObject;
            topRock.transform.parent = gameObject.transform.transform;
            attributes = topRock.GetComponent<RockAttributes>();
            attributes.markingGameMap(topRock.transform.position, false);
        }
    }

    bool isConllider(Vector3 direction)
    {
        Vector3 step = Vector3.zero;
        Vector3 newPosition = topRock.transform.position;

        int count = 0;
        direction /= 1.9f;
        
        if (direction.x != 0)
        {
            direction *= attributes.rockSize.x;
            count = attributes.rockSize.z;
            newPosition.z += (count - 1) * 0.5f;
            step = Vector3.back;
        }
        else if(direction.z != 0)
        {
            direction *= attributes.rockSize.z;
            count = attributes.rockSize.x;
            newPosition.x += (count - 1) * 0.5f;
            step = Vector3.left;
        }

        for(int i=1;i<=count;i++)
        {
            if(gameMapController.isMovable(newPosition+direction)==false)
            {
                return true;
            }
            newPosition += step;
        }

        return false;
    }

    public void removeRoad()
    {
        if(road.Count>1)
        {
            road.RemoveRange(2, road.Count-2);
        }
    }
}
