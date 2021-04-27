using UnityEngine;

public class EndGame : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (MonoBehaviour c in animator.gameObject.transform.parent.gameObject.GetComponents<MonoBehaviour>())
        {
            c.enabled = false;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        MenuFunctions.Instance.ShowMenu("GameOver");
        animator.gameObject.transform.parent.gameObject.SetActive(false);
        Debug.Log("Game Over");
    }
}