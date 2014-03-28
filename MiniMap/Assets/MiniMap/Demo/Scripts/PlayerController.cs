using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    bool grounded = false;
    byte currentState = 0;

    Rigidbody body;
    CapsuleCollider capsule;

    float runSpeed = 4f;
    float jumpForce = 5f;
    float groundedTime = 0f;

    [SerializeField]
    LayerMask walkable = 0;
    
    void Start()
    {
        capsule = collider as CapsuleCollider;

        // We will rotate ourselves and we dont want any drag
        body = rigidbody;
        body.drag = 0f;
        body.freezeRotation = true;

        // Set camera to target this transform
        RPGThirdPersonCamera.Instance.Target = transform;
    }

    void changeMovementState(byte state)
    {
        if (currentState != state)
        {
            currentState = state;
            GetComponent<PlayerAnimator>().OnPlayerMovement(state);
        }
    }

    void FixedUpdate()
    {
        if (body != null && capsule != null)
        {
            bool wasGrounded = grounded;

            grounded = Physics.Raycast(transform.position + Vector3.up, Vector3.down, 1.05f, walkable);

            if (grounded && !wasGrounded)
            {
                if (Time.time - groundedTime > 0.4f)
                {
                    changeMovementState(PlayerMovementEvent.Land);
                }
            }

            if (grounded)
            {
                groundedTime = Time.time;
            }

            if (Input.GetMouseButton(1))
            {
                transform.Rotate(Vector3.up, Input.GetAxisRaw("Mouse X") * 200f * Time.fixedDeltaTime);
            }

            if (grounded)
            {
                Vector3 movement =
                    Input.GetAxisRaw("Horizontal") * Vector3.right +
                    Input.GetAxisRaw("Vertical") * Vector3.forward;

                if (movement != Vector3.zero)
                {
                    movement.Normalize();
                    movement = transform.rotation * movement * runSpeed;

                    var a = RPGControllerUtils.SignedAngle(transform.forward, movement.normalized, Vector3.up);
                    var r = a > 1;

                    switch (Mathf.RoundToInt(Mathf.Abs(a)))
                    {
                        case 0:
                            changeMovementState(PlayerMovementEvent.Forward);
                            break;

                        case 45:
                            changeMovementState(r ? PlayerMovementEvent.ForwardRight : PlayerMovementEvent.ForwardLeft);
                            break;

                        case 90:
                            changeMovementState(r ? PlayerMovementEvent.Right : PlayerMovementEvent.Left);
                            break;

                        case 135:
                            changeMovementState(r ? PlayerMovementEvent.BackwardRight : PlayerMovementEvent.BackwardLeft);
                            break;

                        case 180:
                            changeMovementState(PlayerMovementEvent.Backward);
                            break;
                    }

                    if (Mathf.Abs(a) > 91)
                    {
                        movement *= 0.5f;
                    }

                    body.velocity = movement;
                }
                else
                {
                    changeMovementState(PlayerMovementEvent.Idle);
                    body.velocity = new Vector3(0, body.velocity.y, 0);
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    grounded = false;
                    body.velocity += Vector3.up * jumpForce;
                    changeMovementState(PlayerMovementEvent.Jump);
                }
            }
            else
            {
                if (Time.time - groundedTime > 0.4f)
                {
                    changeMovementState(PlayerMovementEvent.Fall);
                }
            }
        }
    }
}
