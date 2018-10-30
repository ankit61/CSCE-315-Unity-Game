using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtomicAnimation : StateMachineBehaviour {

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // When leaving the special move state, stop the particles.
        //Debug.Log("Finishing Punch");
        animator.SetInteger("Animation State", 0);
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       //Debug.Log("Entering Animation");

    }
}
