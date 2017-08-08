using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 控制游戏中隐藏的岩石
/// </summary>

public class HideRockController : MonoBehaviour {

    public GameObject hideRockContainer;

    public float speed = 1.0f;
    private float startTime;
    private Vector3 initPosition;
    private Vector3 endPosition;
    private float journeyLength = 1;

    private bool isHide = true;

    void Start ()
    {
        initPosition = hideRockContainer.transform.position;
        endPosition = initPosition + new Vector3(0, 1, 0);
	}

    void Update()
    {
        if (isHide == false)
        {
            unhide();
            if ((Time.time - startTime) > 1.6f)
            {
                Destroy(gameObject.GetComponent<HideRockController>(), 1);
            }
        }
    }

    /// <summary>
    /// 取消隐藏
    /// </summary>
    void unhide()
    {
        float distCovered;
        float fracJourney;

        distCovered = (Time.time - startTime) * speed;
        fracJourney = distCovered / journeyLength;
        hideRockContainer.transform.position = Vector3.Lerp(initPosition, endPosition, fracJourney);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Ant" && isHide == true)
        {
            isHide = false;
            startTime = Time.time;
            if(hideRockContainer.tag=="StaticRock")
            {
                hideRockContainer.GetComponent<StaticRockAttributes>().markingGameMap();
            }
            else if(hideRockContainer.tag == "HideAndMovableRock")
            {
                hideRockContainer.GetComponent<RockAttributes>().markingGameMap(hideRockContainer.transform.position, true);
            }
        }
    }

}
