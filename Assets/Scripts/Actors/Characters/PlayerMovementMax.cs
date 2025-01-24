using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Animator animator;

    private Vector3 movement;

    void Update()
    {
        // Détecter les mouvements du joueur
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        movement = new Vector3(horizontal, 0, vertical).normalized;

        // Appliquer les animations
        if (movement.magnitude > 0.1f)
        {
            animator.SetBool("isMoving", true);
            animator.SetFloat("Horizontal", horizontal);
            animator.SetFloat("Vertical", vertical);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }

    /*void FixedUpdate()
    {
        // Déplacement du joueur
        if (movement.magnitude > 0.1f)
        {
            transform.position += movement * moveSpeed * Time.deltaTime;

            // Rotation vers la direction du mouvement
            Quaternion targetRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.1f);
        }
    }*/
}


