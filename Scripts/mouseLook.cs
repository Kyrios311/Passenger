using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouseLook : MonoBehaviour {

    public bool paused = false; //pauser
    public bool inventory = false;

    Vector2 mLook;
    Vector2 smoothV; //Smooth vector
    public float sens = 5.0f;
    public float smoothing = 2.0f;
    int invert = -1; // -1=Default 1=Inverted

    GameObject player;

	// Use this for initialization
	void Start () {
        player = this.transform.parent.gameObject;
        Cursor.lockState = CursorLockMode.Locked;
    }
	
	// Update is called once per frame
	void Update () {
        if (!paused && !inventory)
        {
            Cursor.lockState = CursorLockMode.Locked;
            var delta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            smoothV.x = Mathf.Lerp(smoothV.x, delta.x, 1f / smoothing);
            smoothV.y = Mathf.Lerp(smoothV.y, delta.y, 1f / smoothing);
            mLook += smoothV;
            mLook.y = Mathf.Clamp(mLook.y, -90f, 90f); //Clamps look values so player doesn't flip

            transform.localRotation = Quaternion.AngleAxis(invert * mLook.y, Vector3.right);
            player.transform.localRotation = Quaternion.AngleAxis(mLook.x, player.transform.up);
        }
        else
        {
            transform.localRotation = Quaternion.AngleAxis(invert * mLook.y, Vector3.right);
            player.transform.localRotation = Quaternion.AngleAxis(mLook.x, player.transform.up);
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void invertY ()
    {
        invert = invert * -1; 
    }
}
