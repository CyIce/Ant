using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainDirecionController : MonoBehaviour {

    private GameMapController gameMapController;

    private AntMovingController antController;

    private MoveLight moveLight;

    public Transform rainTra;

    public Transform aimQua;

    public Vector3 direction;

    private bool change = false;

    public iVector2 moveMap;

    public Vector3 rainDirection;

    private void Awake()
    {
        gameMapController = GameObject.Find("Main Camera").GetComponent<GameMapController>();
        moveLight = GameObject.Find("Directional Light").GetComponent<MoveLight>();
        antController = GameObject.Find("Ant").GetComponent<AntMovingController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Ant" && change == false)
        {
            rainTra.rotation = aimQua.rotation;
            change = true;
            gameMapController.moveGameMap(moveMap);
            gameMapController.rainRay = rainDirection;

            if (rainDirection.x == rainDirection.z)
            {
                moveLight.setChange(2);
            }
            else if (rainDirection.z < 0)
            {
                moveLight.setChange(1);
            }

            antController.removeRoad();
        }
    }
}
