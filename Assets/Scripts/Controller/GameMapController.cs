﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 控制与游戏地图相关的代码
/// </summary>
/// 
//int类型的verctor2
[System.Serializable]
public struct iVector2
{
     public int x;
     public int z;
 };

public class GameMapController : MonoBehaviour {

    //游戏状态枚举
    public enum GameState
    {
        START,
        PAUSE,
        GAME,
        MOVING,
        SWITCH,
        END,
        Win
    }

    //游戏地图的大小
    public iVector2 mapSize;
    //用于标记游戏中各个位置的信息
    public int[,] gameMap = new int[12, 32];
    //用于记录游戏中固定的岩石的位置和大小
    public Vector4[] staticRock;
    //记录当前游戏状态
    public GameState gameState;

    public Vector3 rainRay;

    private void Awake()
    {
        //initGameMap();
    }

    void Start ()
    { 
        markStaticRock();
        rainRay = Vector3.up;

    }

    /// <summary>
    /// 将实际的坐标转化为地图的下标
    /// </summary>
    /// <param name="_vec"></param>
    /// <returns></returns>
    public iVector2 toIvector2(Vector3 _vec)
    {
        iVector2 iVec;

        iVec.x = -(int)_vec.x + 1;
        iVec.z = -(int)_vec.z + 1;

        return iVec;
    }
    
    /// <summary>
    /// 标记固定的岩石的位置
    /// </summary>
    void markStaticRock()
    {
        iVector2 vec;
        int i, j, k;

        for (i = 0; i < staticRock.Length; i++) 
        {
            vec = toIvector2(new Vector3(staticRock[i].x, 0, staticRock[i].y));

            for (j = 0; j < staticRock[i].z; j++) 
            {
                for (k = 0; k < staticRock[i].w; k++) 
                {
                    //标记地图上的位置为可行区域
                    gameMap[vec.x + j, vec.z + k] = 1;
                }
            }
        }
    }

    /// <summary>
    /// 判断传入的坐标是否可以移动
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public bool isMovable(Vector3 pos)
    {
        iVector2 vec;
        pos.x -= rainRay.x / 2;
        pos.z -= rainRay.z / 2;

        vec.x = -(int)pos.x + 1;
        vec.z = -(int)pos.z + 1;

        if (vec.x < 0 || vec.x >= mapSize.x || vec.z < 0 || vec.z >= mapSize.z)
        {
            return false;
        }

        if (gameMap[vec.x, vec.z] != 0)
        {
            return false;
        }
        else
        {
            return true;
        }

    }

    public void moveGameMap(iVector2 moveMap)
    {
        int i, j;
        if (moveMap.x != 0)
        {
            if (moveMap.x == 1)
            {
                for (i = mapSize.x; i > 0; i--)
                {
                    for (j = 0; j <= mapSize.z; j++)
                    {
                        gameMap[i, j] = gameMap[i - 1, j];
                    }
                }
            }
            else if (moveMap.x == -1) 
            {
                for (i = 0; i < mapSize.z; i++)
                {
                    for (j = 0; j <= mapSize.z; j++)
                    {
                        gameMap[i, j] = gameMap[i + 1, j];
                    }
                }
            }
        }
        else if(moveMap.z != 0)
        {
            if (moveMap.z == 1)
            {
                for (i = mapSize.z; i > 0; i--)
                {
                    for (j = 0; j <= mapSize.x; j++)
                    {
                        gameMap[j, i] = gameMap[j, i-1];
                    }
                }
            }
            else if (moveMap.z == -1)
            {
                for (i = 0; i < mapSize.z; i++)
                {
                    for (j = 0; j <= mapSize.x; j++)
                    {
                        gameMap[j, i] = gameMap[j, i + 1];
                    }
                }
            }
        }
    }

}
