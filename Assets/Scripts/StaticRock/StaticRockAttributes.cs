using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticRockAttributes : MonoBehaviour {

    private GameMapController gameMapController;

    public iVector2 rockSize;

    void Start ()
    {
        gameMapController = GameObject.Find("Main Camera").GetComponent<GameMapController>();
    }

    public void markingGameMap()
    {
        int x, z;
        Vector3 pos = gameObject.transform.position;

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
                gameMapController.gameMap[i + (int)(gameMapController.rainRay.x / 2), j + (int)(gameMapController.rainRay.z / 2)] = 1;
            }
        }
    }


}
