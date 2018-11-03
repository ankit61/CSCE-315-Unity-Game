using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public bool punch = false;
    public bool jump = false;

	// Use this for initialization
	void Start () {

    }

    // Update is called once per frame
    void Update () {
        if (punch)
        {
            punch = false;
            gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(10000.0f, 0.0f));
        }
        if (jump)
        {
            jump = false;
            gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0.0f, 10000.0f));
        }
    }
}
