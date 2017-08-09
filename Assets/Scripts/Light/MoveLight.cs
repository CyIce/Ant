using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLight : MonoBehaviour {

    private Quaternion initQua;
    private Quaternion endQua;
    public Transform northTra;
    private int change = 0;

    private float startTime;


    void Start ()
    {
        initQua = gameObject.transform.rotation;
        endQua = northTra.rotation;
	}
	
	void Update ()
    {
        if (change != 0)
        {

            if (change == 1)
            {
                float distCovered = (Time.time - startTime) * 1;
                float fracJourney = distCovered / 1;
                gameObject.transform.rotation = Quaternion.Lerp(initQua, endQua, fracJourney);
            }
            else if (change == 2)
            {
                float distCovered = (Time.time - startTime) * 1;
                float fracJourney = distCovered / 1;
                gameObject.transform.rotation = Quaternion.Lerp(endQua, initQua, fracJourney);
            }
        }

        if (Time.time - startTime > 1f)
        {
            change = 0;
        }
	}

    public void setChange(int _change)
    {
        change = _change;
        startTime = Time.time;
    }
}
