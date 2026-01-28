using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    Animator animator;

    public float horizontal;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        animator.SetFloat("Horizontal", horizontal);
    }
}
