using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour {

    public Transform antTra;

	void Start ()
    {
		
	}
	
	void Update ()
    {
        if (gameObject.transform.position.x == antTra.position.x && gameObject.transform.position.z == antTra.position.z)
        {
            SceneManager.LoadScene("Level 01");
        }
	}
}
