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
    }

    public void OnReleased()
    {
        animator.SetBool("IsHooked", false);
    }
}