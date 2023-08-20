using UnityEngine;

public class Chip : MonoBehaviour
{
    [SerializeField] private Player up;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Flip()
    {
        if (up == Player.Black)
        {
            animator.Play("BlackToWhite");
            up = Player.White;
        }
        else
        {
            animator.Play("WhiteToBlack");
            up = Player.Black;
        }
    }
}
