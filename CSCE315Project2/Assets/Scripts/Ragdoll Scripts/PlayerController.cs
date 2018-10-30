using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public GameObject playerTorso;
    public GameObject playerHandBottomR;
    public GameObject playerHandBottomL;
    public GameObject playerLegBottomR;
    public GameObject playerLegBottomL;

    // Use this for initialization
    void Start () {
		if(!(playerTorso & playerHandBottomR & playerHandBottomL & playerLegBottomR & playerLegBottomL))
        {
            Debug.Log("Player Controller is not initialized correctly");
        }
	}

    void HandleMovement()
    {
        if (Input.GetKey("d"))
        {
            //gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(5000.0f, 0.0f));
            Vector2 curVelocity = playerTorso.GetComponent<Rigidbody2D>().velocity;
            playerTorso.GetComponent<Rigidbody2D>().velocity = new Vector2(10.0f, curVelocity.y);
        }
        else if (Input.GetKey("a"))
        {
            //gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(-40.0f, 0.0f));
            Vector2 curVelocity = playerTorso.GetComponent<Rigidbody2D>().velocity;
            playerTorso.GetComponent<Rigidbody2D>().velocity = new Vector2(-10.0f, curVelocity.y);
        }

        if (Input.GetKeyDown("space"))
        {
            Debug.Log("Jumping");
            playerTorso.GetComponent<Rigidbody2D>().AddForce(new Vector2(0.0f, 90000.0f));
            //gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        }

        if (Input.GetKeyDown("r"))
        {
            Debug.Log("Ragdolling");
            playerTorso.GetComponent<Rigidbody2D>().AddForce(new Vector2(-20000.0f, 200.0f));
            playerTorso.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        }
    }
	
    void HandleActions()
    {
        if (Input.GetKeyDown("j")) // Jump
        {
            playerHandBottomR.GetComponent<Rigidbody2D>().AddForce(new Vector2(10000.0f, 0.0f));
        }
        if (Input.GetKeyDown("k")) // Kick
        {
            playerLegBottomR.GetComponent<Rigidbody2D>().AddForce(new Vector2(100000.0f, 0.0f));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision Detected!!!!!");
    }

    // Update is called once per frame
    void Update () {
        HandleActions();
        HandleMovement();
    }
}
