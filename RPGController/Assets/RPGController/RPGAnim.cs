using System;
using UnityEngine;

public class RPGAnim : MonoBehaviour
{
    RPGInput input = null;
    RPGMotor motor = null;
    Quaternion rotation = Quaternion.identity;

    [SerializeField]
    Animation target;

    [SerializeField]
    AnimationClip idle = null;

    [SerializeField]
    AnimationClip walking = null;

    [SerializeField]
    AnimationClip running = null;

    [SerializeField]
    AnimationClip strafeLeft = null;

    [SerializeField]
    AnimationClip strafeRight = null;

    [SerializeField]
    AnimationClip backpedaling = null;

    [SerializeField]
    AnimationClip sliding = null;

    [SerializeField]
    AnimationClip jumping = null;

    [SerializeField]
    AnimationClip falling = null;

    [SerializeField]
    AnimationClip landing = null;

    [SerializeField]
    bool rotateOnStrafe = true;

    [SerializeField]
    float rotationSpeed = 20f;

    [SerializeField]
    float forwardRunAnimationSpeed = 2f;

    [SerializeField]
    float forwardWalkAnimationSpeed = 1.5f;

    [SerializeField]
    float backwardRunAnimationSpeed = -1.5f;

    [SerializeField]
    float backwardWalkAnimationSpeed = -0.5f;

    [SerializeField]
    float strafeRunAnimationSpeed = -1.5f;

    [SerializeField]
    float strafeWalkAnimationSpeed = -0.5f;

    void Start()
    {
        if (target == null && animation != null)
        {
            target = animation;
        }

        if (transform.parent != null)
        {
            Transform t = transform.parent;

            while (t != null && (motor == null || input == null))
            {
                if (motor == null)
                {
                    motor = t.GetComponent<RPGMotor>();
                }

                if (input == null)
                {
                    input = t.GetComponent<RPGInput>();
                }

                t = t.parent;
            }

            if (motor != null)
            {
                motor.OnJump = new Action(OnJump);
                motor.OnLand = new Action(OnLand);
                motor.OnFall = new Action(OnFall);
                motor.OnIdle = new Action(OnIdle);
                motor.OnMove = new Action(OnMove);
                motor.OnSlide = new Action(OnSlide);
            }
        }

        if (target != null)
        {
            target.wrapMode = WrapMode.Loop;
            target.Stop();

            SetupPrioAnimation(landing);
            SetupPrioAnimation(jumping);
        }

        if (motor != null)
        {
            SetRotation(motor.transform.forward);
        }
    }

    void Update()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
    }

    void OnMove()
    {
        bool movingForward = motor.MovementInput.z > 0;
        bool movingBackward = motor.MovementInput.z < 0;
        bool movingLeft = motor.MovementInput.x < 0;
        bool movingRight = motor.MovementInput.x > 0;

        if (movingForward)
        {
            if (movingLeft)
            {
                SetRotation((-motor.transform.right + motor.transform.forward).normalized);
            }
            else if (movingRight)
            {
                SetRotation((motor.transform.right + motor.transform.forward).normalized);
            }
            else
            {
                SetRotation(motor.transform.forward);
            }

            PlayAnimation("Forward", input.IsRunning ? running : walking, input.IsRunning ? forwardRunAnimationSpeed : forwardWalkAnimationSpeed);
        }

        else if (movingBackward)
        {
            if (movingLeft)
            {
                SetRotation((motor.transform.right + motor.transform.forward).normalized);
            }
            else if (movingRight)
            {
                SetRotation((-motor.transform.right + motor.transform.forward).normalized);
            }
            else
            {
                SetRotation(motor.transform.forward);
            }

            PlayAnimation("Backpedal", backpedaling, input.IsRunning ? backwardRunAnimationSpeed : backwardWalkAnimationSpeed);
        }

        // Strafing
        else
        {
            if (rotateOnStrafe)
            {
                if (movingLeft)
                {
                    SetRotation(-motor.transform.right);
                }
                else
                {
                    SetRotation(motor.transform.right);
                }

                PlayAnimation("Forward", input.IsRunning ? running : walking, input.IsRunning ? forwardRunAnimationSpeed : forwardWalkAnimationSpeed);
            }
            else
            {
                SetRotation(motor.transform.forward);

                if (movingLeft)
                {
                    PlayAnimation("StrafeLeft", strafeLeft, input.IsRunning ? strafeRunAnimationSpeed : strafeWalkAnimationSpeed);
                }
                else
                {
                    PlayAnimation("StrafeRight", strafeRight, input.IsRunning ? strafeRunAnimationSpeed : strafeWalkAnimationSpeed);
                }
            }
        }
    }

    void OnIdle()
    {
        PlayAnimation("Idle", idle);

        if (!motor.IsJumping)
        {
            SetRotation(motor.transform.forward);
        }
    }

    void OnFall()
    {
        if (motor.GroundDelta > 0.3f)
        {
            PlayAnimation("Falling", falling);
        }

        if (Input.GetAxisRaw("Mouse X") != 0f)
        {
            SetRotation(motor.transform.forward);
        }
    }

    void OnSlide()
    {
        PlayAnimation("Sliding", sliding);

        if (Input.GetAxisRaw("Mouse X") != 0f)
        {
            SetRotation(motor.transform.forward);
        }
    }

    void OnJump()
    {
        PlayAnimation("Jumping", jumping);

        if (Input.GetAxisRaw("Mouse X") != 0f)
        {
            SetRotation(motor.transform.forward);
        }
    }

    void OnLand()
    {
        if (motor.GroundDelta > 0.3f)
        {
            PlayAnimation("Landing", landing);
        }
    }

    void SetupPrioAnimation(AnimationClip clip)
    {
        if (clip != null)
        {
            target[clip.name].wrapMode = WrapMode.Once;
            target[clip.name].layer = 1;
        }
    }

    void PlayAnimation(string name, AnimationClip clip)
    {
        PlayAnimation(name, clip, 1f);
    }

    void PlayAnimation(string name, AnimationClip clip, float speed)
    {
        if (clip != null)
        {
            if (target != null)
            {
                target[clip.name].speed = speed;
                target.CrossFade(clip.name);
            }
            else
            {
                Debug.LogError("[RPGAnim] The target is not set");
            }
        }
        else
        {
            Debug.LogWarning("[RPGAnim] " + name + " animation missing");
        }
    }

    void SetRotation(Vector3 towards)
    {
        rotation = Quaternion.LookRotation(towards);
    }
}