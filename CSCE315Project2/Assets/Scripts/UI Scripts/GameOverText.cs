using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverText : MonoBehaviour {

    public float alphaIncrease = 0.01f;

    private Text curText;
    private Color curColor;

    private bool blinking = false;

	// Use this for initialization
	void Start () {
        curText = gameObject.GetComponent<Text>();
        curColor = curText.color;
        curColor.a = 0f;
        curText.color = curColor;
    }
	
	// Update is called once per frame
	void Update () {
        if(blinking){
            return;
        }
        if(System.Math.Abs(curText.color.a - 1f) < 0.000001f)
        {
            blinking = true;
            //StartCoroutine(StartBlinking());
            return;
        }
        float curAlpha = curText.color.a;
        curColor.a = Mathf.Min(1f, curAlpha + alphaIncrease);
        curText.color = curColor;
    }

    IEnumerator StartBlinking(){
        while(true){
            curColor.a = 0f;
            curText.color = curColor;
            yield return new WaitForSeconds(0.5f);

            curColor.a = 1f;
            curText.color = curColor;
            yield return new WaitForSeconds(1f);
        }
    }
}
