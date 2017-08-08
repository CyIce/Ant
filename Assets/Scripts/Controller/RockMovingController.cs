using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用于控制岩石的移动
/// </summary>

public class RockMovingController : MonoBehaviour {

    //储存鼠标点击的目标
    private GameObject clickObj = null;
    //记录上一次鼠标有效点击的位置
    private Vector3 lastClickPosition;
    //记录当前鼠标与点击点的位置差值
    private Vector3 posDis;
    //
    private GameMapController gameMapController;

    private RockAttributes rockAttributes;

    private void Awake()
    {
        gameMapController = gameObject.GetComponent<GameMapController>();
    }

    void Start ()
    {
		
	}
	
	void Update ()
    {
        rockMoving();

    }

    void rockMoving()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 clickPosition = emitRay();
            if (clickObj != null)
            {
                rockAttributes = clickObj.GetComponent<RockAttributes>();
                posDis = clickObj.transform.position - clickPosition;
                lastClickPosition = clickPosition;
            }
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 clickPosition = emitRay();
            if (clickObj != null)
            {
                movingDirection(clickPosition);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (clickObj != null)
            {
                clickObj.GetComponent<RockAttributes>().toStandardPosition();
            }
            else
            {
                clickObj = null;
            }
            
        }
    }

    
    /// <summary>
    /// 通过玩家滑动的方向判断岩石移动的方向
    /// </summary>
    /// <param name="clickPosition"></param>
    void movingDirection(Vector3 clickPosition)
    {
        Vector3 nowPosition = posDis + clickPosition;
        Vector3 moveDirction = Vector3.zero;
        //true==X,false==Y
        bool dir = true;

        lastClickPosition -= clickPosition;

        nowPosition.y = clickObj.transform.position.y;

        //判断岩石移动的方向
        int moveType = rockAttributes.getRockType(clickObj.transform.position);

        if (moveType == 11)
        {
            dir = Mathf.Abs(lastClickPosition.x) >= Mathf.Abs(lastClickPosition.z) ? true : false;
        }
        else if (moveType == 12)
        {
            dir = true;
        }
        else if (moveType == 13)
        {
            dir = false;
        }
        //确定岩石移动的方向
        float nestStep = 0.5f;

        if (dir == true) 
        {
            nestStep *= rockAttributes.rockSize.x;
            nowPosition.z = clickObj.transform.position.z;
            moveDirction = lastClickPosition.x > 0 ? new Vector3(-nestStep, 0, 0) : new Vector3(nestStep, 0, 0);
        }
        else
        {
            nestStep *= rockAttributes.rockSize.z;
            nowPosition.x = clickObj.transform.position.x;
            moveDirction = lastClickPosition.z > 0 ? new Vector3(0, 0, -nestStep) : new Vector3(0, 0, nestStep);
        }

        lastClickPosition = clickPosition;

        if (gameMapController.isMovable(nowPosition + moveDirction) == true) 
        {
            rockAttributes.markingGameMap(clickObj.transform.position, false);
            clickObj.transform.position = nowPosition;
        }
        else
        {
            posDis = clickObj.transform.position - clickPosition;
        }

    }

    /// <summary>
    /// 从相机发射一条射线，传出鼠标点击的位置
    /// </summary>
    /// <returns></returns>
    Vector3 emitRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (clickObj != null && clickObj != hit.collider.gameObject) 
            {
                clickObj.GetComponent<RockAttributes>().toStandardPosition();
                clickObj = null;
            }
            else if (hit.collider.gameObject.tag == "MovableRock")
            {
                clickObj = hit.collider.gameObject;
                return hit.point;
            }
            else if (hit.collider.gameObject.tag == "HideAndMovableRock")
            {
                if (hit.point.y > 1)
                {
                    clickObj = hit.collider.gameObject;
                    return hit.point;
                }
            }
            else
            {
                clickObj = null;
            }
        }
        else
        {
            clickObj = null;
        }

        return Vector3.zero;
    }

}
