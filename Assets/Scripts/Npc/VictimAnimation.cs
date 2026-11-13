using UnityEngine;

public class VictimAnimation : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OnHooked()
    {
        animator.SetBool("IsHooked", true);

        NPCRunner runner = GetComponent<NPCRunner>();
        if (runner != null) runner.StopRunning();
    }

    public void OnReleased()
    {
        animator.SetBool("IsHooked", false);
    }
}