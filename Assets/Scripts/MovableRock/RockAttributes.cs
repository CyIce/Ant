using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockAttributes : MonoBehaviour
{

    private GameMapController gameMapController;

    public iVector2 rockSize;

    private Vector3 initPosition;

    public int rockType;

    private void Awake()
    {
        gameMapController = GameObject.Find("Main Camera").GetComponent<GameMapController>();
    }

    void Start()
    {
        markingGameMap(gameObject.transform.position, true);
        initPosition = gameObject.transform.position;
    }

    public void markingGameMap(Vector3 pos, bool isMark)
    {
        int x, z;
        int mark = 0;

        if (isMark == true)
        {
            mark = getRockType(gameObject.transform.position);
        }
        pos.x += (rockSize.x - 1) * 0.5f;
        pos.z += (rockSize.z - 1) * 0.5f;
        x = -(int)pos.x + 1;
        z = -(int)pos.z + 1;

        int xLenght = rockSize.x + x;
        int zLenght = rockSize.z + z;


        for (int i = x; i < xLenght; i++)
        {
            for (int j = z; j < zLenght; j++)
            {
                gameMapController.gameMap[i+ (int)(gameMapController.rainRay.x / 2), j+(int)(gameMapController.rainRay.z/2)] = mark;
            }
        }
    }

    public int getRockType(Vector3 pos)
    {
        if (rockType == 11)
        {
            if (Mathf.Abs(pos.x - initPosition.x) <= 0.05f &&
                Mathf.Abs(pos.z - initPosition.z) <= 0.05f)
            {
                return 11;
            }
            else if (Mathf.Abs(pos.x - initPosition.x) <= 0.05f)
            {
                return 13;
            }
            else
            {
                return 12;
            }
        }
        else
        {
            return rockType;
        }
    }

    public void toStandardPosition()
    {
        float i = 0.5f, j = 0.5f;

        if (rockSize.x % 2 == 0)
        {
            i = Mathf.Abs(gameObject.transform.position.x - (int)gameObject.transform.position.x) < 0.5f ? 0 : 1;
        }
        if (rockSize.z % 2 == 0)
        {
            j = Mathf.Abs(gameObject.transform.position.z - (int)gameObject.transform.position.z) < 0.5f ? 0 : 1;
        }

        Vector3 vec;
        vec.x = (int)gameObject.transform.position.x - i;
        vec.y = gameObject.transform.position.y;
        vec.z = (int)gameObject.transform.position.z - j;

        gameObject.transform.position = vec;
        markingGameMap(gameObject.transform.position, true);
    }
}

   