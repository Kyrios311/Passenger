using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatorY : MonoBehaviour {
    public int speed = 2;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(0, speed * Time.deltaTime, 0);
    }
}
