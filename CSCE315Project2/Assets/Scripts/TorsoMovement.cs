using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorsoMovement : MonoBehaviour {

    public AudioSource swampSound;

    // Use this for initialization
    void Start () {
        //var audioClip = Resources.Load<AudioClip>("/Audio/swamp.mp3");  //Load the AudioClip from the Resources Folder
        //swampSound.clip = audioClip;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey("d"))
        {
            //gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(5000.0f, 0.0f));
            Vector2 curVelocity = gameObject.GetComponent<Rigidbody2D>().velocity;
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(10.0f, curVelocity.y);
        }
        else if (Input.GetKey("a"))
        {
            //gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(-40.0f, 0.0f));
            Vector2 curVelocity = gameObject.GetComponent<Rigidbody2D>().velocity;
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(-10.0f, curVelocity.y);
        }

        if (Input.GetKeyDown("space"))
        {
            Debug.Log("Jumping");
            gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0.0f, 20000.0f));
            //gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            //swampSound.Play();
        }

        if (Input.GetKeyDown("c"))
        {
            Debug.Log("Ragdolling");
            gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(-20000.0f, 200.0f));
            gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            //swampSound.Play();
        }
    }
}
