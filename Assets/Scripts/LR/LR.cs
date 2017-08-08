using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 从相机向鼠标发射一条射线
/// </summary>
/// 
public class LR : MonoBehaviour {

    private LineRenderer line;

    private void Awake()
    {
        line = gameObject.GetComponent<LineRenderer>();
    }

	void Update ()
    {
        emitRay();
    }

    void emitRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log(hit.point);
            line.SetPosition(0, gameObject.transform.position);
            line.SetPosition(1, hit.point);
        }
    }
}
