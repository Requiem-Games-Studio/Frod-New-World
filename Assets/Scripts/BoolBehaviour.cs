using UnityEngine;

public class BoolBehaviour : StateMachineBehaviour
{
    public string boolState;
    
    // Se llama cuando la animación comienza a reproducirse
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(boolState, true);
    }

    // Se llama cuando la animación termina o se interrumpe
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(boolState, false);
    }
}
