using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using UnityEngine.InputSystem;
using System;

public class PlayerScript : MonoBehaviour
{
    //Player components
    public AnimatorController AnimController;
    public GameObject GroundPoundDust;
    PlayerInput PlayerInput;
    CharacterController CharController;
    Animator Animator;
    BlendTree LongIdleTree;

    //Animation values
    int IsRunningHash;
    int IsJumpingHash;
    int IsBigJumpHash;
    int IsMovingHash;
    int IsSmallAttackHash;
    int IsBigAttackHash;
    int IsLongIdleHash;
    int RandomLongIdleHash;
    int IsBodyslamHash;
    int IsSlideAttackHash;
    int IsRandomHookshotHash;
    int PlayerDied;
    int Victory;

    //General setup values
    int Zero = 0;
    int Devider = 2;
    float Average = .5f;
    float CurrentPlayerSpeed;
    bool CanMove;
    
    //Attack settings
    bool IsAttacking;
    bool IsBodyslam;
    bool IsSlideAttack;
    bool IsSlideAttackPerforming;
    bool IsBodyslamPerforming;
    float BodySlamDistance = 1;
    float BodySlamHeightMultiplier = 5.5f;
    public float SlideMultiplier = 5;
    Vector3 SmallHitBox = new Vector3(2, 0.5f, 2);
    Vector3 BigHitBox = new Vector3(2, 2f, 2);
    Vector3 GroundPoundHitBox = new Vector3(2.5f, 0.1f, 2.5f);
    Vector3 SlideAttackHitBox = new Vector3(1f, 0.5f, 1f);

    //Gravity settings
    [SerializeField]
    LayerMask GroundMask;
    float GroundedGravity = -.05f;
    float Gravity;
    float FallMultiplier = 2;
    float BodySlamMultiplier = 5;
    float LandingDelay;
    float Velocity = -1.2f;
    bool IsFalling;

    //Player movement
    Vector2 CurrentMovementInput;
    Vector3 CurrentMovement;
    Vector3 CurrentRunMovement;
    Vector3 LastPosition;
    bool IsMovementPressed;
    bool IsRunPressed;
    float RotationFactorPerFrame = 15;
    float WalkMultiplier = 2.5f;
    float RunMultiplier = 3.5f;

    //Player jumping
    public float SetFlipHeight;
    [HideInInspector]
    public bool IsJumpPressed = false;
    bool IsJumpAnimating = false;
    float InitialJumpVelocity;
    float NextHeight;
    float MinJumpHeight = 2;
    float ConstantJumpForce;
    float ConstantJumpForceTarget = 7.5f;
    float ConstantJumpForceDecrease = 5;
    float MaxJumpTime = 0.75f;
    float SmallBounce = 1.2f;
    float BigBounce = 1.5f;
    [HideInInspector]
    public bool IsJumping = false;
    [HideInInspector]
    public bool IsBounce = false;

    //Idle dancing
    public float IdleDanceMarker;
    private float IdleTimer;
    private bool IsDancing;

    //Hit detection
    int SideHitValue;
    bool Attack;
    Collider[] hitColliders;
    enum HitPlayerDirection { None, Top, Bottom, Forward, Back, Left, Right, Spin, Invincibility }

    private void Awake()
    {
        InitialSetup();
        AnimatorSetup();
        PlayerControllerSetup();
        JumpVariablesSetup();
    }

    private void InitialSetup()
    {
        PlayerInput = new PlayerInput();
        CharController = GetComponent<CharacterController>();
        Animator = GetComponent<Animator>();
        CanMove = true;
    }

    private void AnimatorSetup()
    {
        IsRunningHash = Animator.StringToHash("IsRunning");
        IsJumpingHash = Animator.StringToHash("IsJumping");
        IsBigJumpHash = Animator.StringToHash("IsBigJump");
        IsMovingHash = Animator.StringToHash("IsMoving");
        IsSmallAttackHash = Animator.StringToHash("IsSmallAttack");
        IsBigAttackHash = Animator.StringToHash("IsBigAttack");
        IsLongIdleHash = Animator.StringToHash("IsLongIdle");
        RandomLongIdleHash = Animator.StringToHash("LongIdleAnimPicked");
        IsBodyslamHash = Animator.StringToHash("IsBodyslam");
        IsSlideAttackHash = Animator.StringToHash("IsSlideAttack");
        IsRandomHookshotHash = Animator.StringToHash("RandomHookshot");
        PlayerDied = Animator.StringToHash("IsDead");
        Victory = Animator.StringToHash("IsVictory");
        GetIdleBlendTree();
    }
    private void GetIdleBlendTree()
    {
        AnimatorStateMachine RootStateMachine = AnimController.layers[Zero].stateMachine;
        AnimatorState StateWithBlendTree = RootStateMachine.states[5].state;
        LongIdleTree = (BlendTree)StateWithBlendTree.motion;
    }

    private void PlayerControllerSetup()
    {
        PlayerInput.CharacterControls.Move.started += OnMovementInput;
        PlayerInput.CharacterControls.Move.canceled += OnMovementInput;
        PlayerInput.CharacterControls.Move.performed += OnMovementInput;
        PlayerInput.CharacterControls.Run.started += OnRun;
        PlayerInput.CharacterControls.Run.canceled += OnRun;
        PlayerInput.CharacterControls.Jump.started += OnJump;
        PlayerInput.CharacterControls.Jump.performed += OnJump;
        PlayerInput.CharacterControls.Jump.canceled += OnJump;
        PlayerInput.CharacterControls.Attack.started += OnAttack;
        PlayerInput.CharacterControls.Attack.canceled += OnAttack;
        PlayerInput.CharacterControls.SpecialAttack.started += OnBodySlam;
        PlayerInput.CharacterControls.SpecialAttack.canceled += OnBodySlam;
        PlayerInput.CharacterControls.SpecialAttack.started += OnSlideAttack;
        PlayerInput.CharacterControls.SpecialAttack.canceled += OnSlideAttack;
        PlayerInput.CharacterControls.DeathTest.started += OnDeathTest;
        PlayerInput.CharacterControls.DeathTest.canceled += OnDeathTest;
        PlayerInput.CharacterControls.VictoryTest.started += OnVictoryTest;
        PlayerInput.CharacterControls.VictoryTest.canceled += OnVictoryTest;
    }
    
    private void JumpVariablesSetup()
    {
        float TimeToApex = MaxJumpTime / Devider;
        Gravity = (-Devider * MinJumpHeight) / Mathf.Pow(TimeToApex, Devider);
        InitialJumpVelocity = (Devider * MinJumpHeight) / TimeToApex;
    }

    private bool IsGrounded()
    {
        return CharController.isGrounded;
    }

    private void FixedUpdate()
    {
        HandleAnimation();
        if (CanMove)
        {
            HandleRotation();
            CheckAttackstyle();
            HandleJump();
        }
        CheckIfRunning();
        HandleGravity();
    }

    private void HandleRotation()
    {
        Vector3 PositionToLookAt;
        PositionToLookAt.x = CurrentMovement.x;
        PositionToLookAt.y = Zero;
        PositionToLookAt.z = CurrentMovement.z;
        Quaternion CurrentRotation = transform.rotation;

        if (IsMovementPressed)
        {
            Quaternion TargetRotation = Quaternion.LookRotation(PositionToLookAt);
            transform.rotation = Quaternion.Slerp(CurrentRotation, TargetRotation, RotationFactorPerFrame * Time.deltaTime);
        }
    }

    private void HandleAnimation()
    {
        CheckLongIdle();
        bool IsRunning = Animator.GetBool(IsRunningHash);
        CurrentPlayerSpeed = CharController.velocity.magnitude;
        Animator.SetFloat(IsMovingHash, CurrentPlayerSpeed);
        CheckPlayingAttackAnimationStates();

        if ((IsMovementPressed && IsRunPressed) && !IsRunning)
        {
            Animator.SetBool(IsRunningHash, true);
        }
        else if ((!IsMovementPressed || !IsRunPressed) && IsRunning)
        {
            Animator.SetBool(IsRunningHash, false);
        }
    }

    private void CheckLongIdle()
    {
        if (transform.position == LastPosition && !IsAttacking)
        {
            if (!IsDancing)
            {
                IdleTimer += Time.deltaTime;
                if (IdleTimer >= IdleDanceMarker)
                {
                    IsDancing = true;
                    IdleTimer = Zero;
                    int RandomLongIdleChosen = UnityEngine.Random.Range(Zero, LongIdleTree.children.Length);
                    Animator.SetFloat(RandomLongIdleHash, RandomLongIdleChosen);
                    Animator.SetTrigger(IsLongIdleHash);
                }
            }
            else if (Animator.GetCurrentAnimatorStateInfo(Zero).normalizedTime > 1f)
            {
                IsDancing = false;
                IdleTimer = Zero;
                Animator.ResetTrigger(IsLongIdleHash);
                Animator.SetTrigger(IsLongIdleHash);
            }
        }
        else
        {
            if (IsDancing)
            {
                Animator.ResetTrigger(IsLongIdleHash);
                Animator.SetTrigger(IsLongIdleHash);
                IsDancing = false;
                IdleTimer = Zero;
            }
            IdleTimer = Zero;
        }
    }

    private void CheckPlayingAttackAnimationStates()
    {
        if (Animator.GetCurrentAnimatorStateInfo(Zero).IsName("Small attack") && Animator.GetCurrentAnimatorStateInfo(Zero).normalizedTime > 1f)
        {
            IsAttacking = false;
            Animator.SetBool(IsSmallAttackHash, IsAttacking);
        }
        else if (Animator.GetCurrentAnimatorStateInfo(1).IsName("Big attack Left") && Animator.GetCurrentAnimatorStateInfo(1).normalizedTime > 1f)
        {
            IsAttacking = false;
            Animator.SetBool(IsBigAttackHash, IsAttacking);
        }
        else if (Animator.GetCurrentAnimatorStateInfo(1).IsName("Big attack Right") && Animator.GetCurrentAnimatorStateInfo(1).normalizedTime > 1f)
        {
            IsAttacking = false;
            Animator.SetBool(IsBigAttackHash, IsAttacking);
        }
        else if (Animator.GetCurrentAnimatorStateInfo(Zero).IsName("Mid-air attack") && Animator.GetCurrentAnimatorStateInfo(Zero).normalizedTime > 1f)
        {
            IsAttacking = false;
            Animator.SetBool(IsSmallAttackHash, IsAttacking);
            Animator.SetBool(IsBigAttackHash, IsAttacking);
        }
        else if (Animator.GetCurrentAnimatorStateInfo(Zero).IsName("Slide attack") && Animator.GetCurrentAnimatorStateInfo(Zero).normalizedTime > 1f)
        {
            IsSlideAttackPerforming = false;
            Animator.SetBool(IsSlideAttackHash, IsSlideAttackPerforming);
        }
    }

    private void CheckIfRunning()
    {
        if (IsRunPressed)
        {
            LastPosition = CharController.transform.position;
            CharController.Move(CurrentRunMovement * Time.deltaTime);
        }
        else
        {
            LastPosition = CharController.transform.position;
            CharController.Move(CurrentMovement * Time.deltaTime);
        }
    }

    private void CheckAttackstyle()
    {
        RaycastHit Hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out Hit, 20))
        {
            if (Hit.transform)
            {
                var Distance = Vector3.Distance(transform.position, Hit.point);

                if (!IsGrounded() && Distance > BodySlamDistance && IsBodyslam && !IsBodyslamPerforming && !IsSlideAttackPerforming)
                {
                    IsBodyslamPerforming = IsBodyslam;
                    CanMove = false;
                    Animator.SetBool(IsBodyslamHash, IsBodyslamPerforming);
                    CharController.Move(Vector3.up * BodySlamHeightMultiplier * Time.fixedDeltaTime);
                    LandingDelay = 0.7f;
                }
            }
        }
        if (transform.position == LastPosition && IsAttacking)
        {
            Animator.SetBool(IsSmallAttackHash, IsAttacking);
        }
        else if (transform.position != LastPosition && IsAttacking)
        {
            Animator.SetLayerWeight(1, 1);
            Animator.SetInteger(IsRandomHookshotHash, UnityEngine.Random.Range(Zero, 100));
            Animator.SetBool(IsBigAttackHash, IsAttacking);
        }
        else if(IsGrounded() && transform.position != LastPosition && IsSlideAttack && !IsSlideAttackPerforming && !IsBodyslamPerforming)
        {
            IsSlideAttackPerforming = IsSlideAttack;
            Animator.SetBool(IsSlideAttackHash, IsSlideAttackPerforming);
            if (CanMove)
            {
                CharController.Move(Vector3.forward * SlideMultiplier * Time.fixedDeltaTime);
            }
        }
    }

    private void CheckAttackHit()
    {
        if (hitColliders != null)
        {
            foreach (var hitCollider in hitColliders)
            {
                Vector3 forward = (hitCollider.transform.position - transform.position).normalized;

                if (Vector3.Dot(forward, transform.forward) > 0.7f)
                {
                    Debug.Log("Facing the object");
                }
            }
        }
    }

    private void HandleGravity()
    {
        IsFalling = CurrentMovement.y < Velocity;
        if(LandingDelay > Zero)
        {
            LandingDelay -= Time.deltaTime;
        }
        if(IsGrounded() && IsBodyslamPerforming)
        {
            IsBodyslamPerforming = false;

            Instantiate(GroundPoundDust, transform.position, transform.rotation);
            hitColliders = Physics.OverlapBox(CharController.bounds.center, GroundPoundHitBox);
            CheckAttackHit();
            StartCoroutine("ResetMovement");
        }
        if (IsGrounded() && LandingDelay <= Zero)
        {
            Animator.SetBool(IsBodyslamHash, IsBodyslamPerforming);
            CheckPlayingAttackAnimationStates();
            if (IsJumpAnimating)
            {
                IsJumpAnimating = false;
                Animator.SetBool(IsJumpingHash, IsJumpAnimating);
            }
            Animator.SetBool(IsJumpingHash, IsJumpAnimating);
            CurrentMovement.y = GroundedGravity;
            CurrentRunMovement.y = GroundedGravity;
            return;
        }
        if (IsFalling)
        {
            float PreviousYVelocity = CurrentMovement.y;
            float NewYVelocity;
            float NextYVelocity;
            if (IsBodyslamPerforming)
            {
                NewYVelocity = CurrentMovement.y + (Gravity * BodySlamMultiplier * Time.deltaTime);
                NextYVelocity = (PreviousYVelocity + NewYVelocity) * Average;
                CurrentMovement.y = NextYVelocity;
                CurrentRunMovement.y = NextYVelocity;
                NextHeight = transform.position.y;
                return;
            }
            NewYVelocity = CurrentMovement.y + (Gravity * FallMultiplier * Time.deltaTime);
            NextYVelocity = (PreviousYVelocity + NewYVelocity) * Average;
            CurrentMovement.y = NextYVelocity;
            CurrentRunMovement.y = NextYVelocity;
            NextHeight = transform.position.y;
            if (SetFlipHeight < NextHeight)
            {
                Animator.SetTrigger(IsBigJumpHash);
            }
            else
            {
                Animator.SetBool(IsJumpingHash, false);
            }
        }
        else
        {
            float PreviousYVelocity = CurrentMovement.y;
            float NewYVelocity = CurrentMovement.y + (Gravity * Time.deltaTime);
            float NextYVelocity = (PreviousYVelocity + NewYVelocity) * Average;
            CurrentMovement.y = NextYVelocity;
            CurrentRunMovement.y = NextYVelocity;
        }
    }

    public void HandleJump()
    {
        if (!IsJumping && IsGrounded() && IsJumpPressed)
        {
            LandingDelay = 0.2f;
            RegularJump();
        }
        else if(IsBounce && IsGrounded() && !IsJumpPressed)
        {
            LandingDelay = 0.2f;
            SmallBounceJump();
        }
        else if (IsBounce && IsGrounded() && IsJumpPressed)
        {
            LandingDelay = 0.2f;
            BigBounceJump();
        }
        else if (!IsJumpPressed && IsJumping && IsGrounded())
        {
            IsJumping = false;
        }
        if (IsJumpPressed)
        {
            CurrentMovement.y += ConstantJumpForce * Time.deltaTime;
            CurrentRunMovement.y += ConstantJumpForce * Time.deltaTime;
            if(ConstantJumpForce > 0)
            {
                ConstantJumpForce -= ConstantJumpForceDecrease * Time.deltaTime;
            }
        }
        else if(IsGrounded())
        {
            ConstantJumpForce = ConstantJumpForceTarget;
        }
    }

    //Different jump types
    private void RegularJump()
    {
        Animator.SetBool(IsJumpingHash, true);
        IsJumpAnimating = true;
        IsJumping = true;
        CurrentMovement.y = InitialJumpVelocity * Average;
        CurrentRunMovement.y = InitialJumpVelocity * Average;
    }

    private void SmallBounceJump()
    {
        Animator.SetBool(IsJumpingHash, true);
        IsJumpAnimating = true;
        IsBounce = false;
        CurrentMovement.y = InitialJumpVelocity * Average * SmallBounce;
        CurrentRunMovement.y = InitialJumpVelocity * Average * SmallBounce;
    }

    private void BigBounceJump()
    {
        Animator.SetBool(IsJumpingHash, true);
        IsJumpAnimating = true;
        IsBounce = false;
        CurrentMovement.y = InitialJumpVelocity * Average * BigBounce;
        CurrentRunMovement.y = InitialJumpVelocity * Average * BigBounce;
    }

    //Hit detection method when colliding with an Interface type
    private void OnControllerColliderHit(ControllerColliderHit collision)
    {
        ICollectable Collectable = (ICollectable)collision.gameObject.GetComponent(typeof(ICollectable));
        if (Collectable != null)
        {
            Collectable.Collect();
        }
    }

    //Converts type of interaction into an integer depending on enum value
    private HitPlayerDirection ReturnDirection(GameObject Object, GameObject ObjectHit)
    {
        HitPlayerDirection HitDirection = HitPlayerDirection.None;
        /*
                if (PlayerManager.IsInvinsible)
                {
                    return HitPlayerDirection.Invincibility;
                }
        */
        if (Attack)
        {
            return HitPlayerDirection.Spin;
        }
        RaycastHit MyRayHit;
        Vector3 direction = (ObjectHit.transform.position - Object.transform.position).normalized;
        Ray MyRay = new Ray(Object.transform.position, direction);

        if (Physics.Raycast(MyRay, out MyRayHit))
        {
            if (MyRayHit.collider != null)
            {
                Vector3 MyNormal = MyRayHit.normal;
                MyNormal = MyRayHit.transform.TransformDirection(MyNormal);

                if (MyNormal == MyRayHit.transform.up)
                {
                    HitDirection = HitPlayerDirection.Top;
                    SideHitValue = Convert.ToInt32(HitDirection);
                }
                if (MyNormal == -MyRayHit.transform.up)
                {
                    HitDirection = HitPlayerDirection.Bottom;
                    SideHitValue = Convert.ToInt32(HitDirection);
                }
                if (MyNormal == MyRayHit.transform.forward)
                {
                    HitDirection = HitPlayerDirection.Forward;
                    SideHitValue = Convert.ToInt32(HitDirection);
                }
                if (MyNormal == -MyRayHit.transform.forward)
                {
                    HitDirection = HitPlayerDirection.Back;
                    SideHitValue = Convert.ToInt32(HitDirection);
                }
                if (MyNormal == MyRayHit.transform.right)
                {
                    HitDirection = HitPlayerDirection.Right;
                    SideHitValue = Convert.ToInt32(HitDirection);
                }
                if (MyNormal == -MyRayHit.transform.right)
                {
                    HitDirection = HitPlayerDirection.Left;
                    SideHitValue = Convert.ToInt32(HitDirection);
                }
            }
        }
        return HitDirection;
    }

    //Coroutines
    IEnumerator ResetMovement()
    {
        yield return new WaitForSeconds(.5f);
        CanMove = true;
    }

    //Animation events
    private void SmallAttackEvent()
    {
        hitColliders = Physics.OverlapBox(CharController.bounds.center / 2, SmallHitBox);
        CheckAttackHit();
    }

    private void BigAttackEvent()
    {
        hitColliders = Physics.OverlapBox(CharController.bounds.center, BigHitBox);
        CheckAttackHit();
    }

    private void SlideAttackEvent()
    {
        hitColliders = Physics.OverlapBox(CharController.bounds.center / 2, SlideAttackHitBox);
    }

    private void RebindAnimationsEvent()
    {
        Animator.Rebind();
    }

    private void DisableMovementEvent()
    {
        CanMove = false;
    }

    private void EnableMovementEvent()
    {
        CanMove = true;
    }

    //Player input system
    private void OnEnable()
    {
        PlayerInput.CharacterControls.Enable();
    }

    private void OnDisable()
    {
        PlayerInput.CharacterControls.Disable();
    }

    private void OnMovementInput(InputAction.CallbackContext context)
    {
        if (CanMove)
        {
            CurrentMovementInput = context.ReadValue<Vector2>();
        }
        else
        {
            CurrentMovementInput = Vector2.zero;
        }
        CurrentMovement.x = CurrentMovementInput.x * WalkMultiplier;
        CurrentMovement.z = CurrentMovementInput.y * WalkMultiplier;
        CurrentRunMovement.x = CurrentMovementInput.x * RunMultiplier;
        CurrentRunMovement.z = CurrentMovementInput.y * RunMultiplier;
        IsMovementPressed = CurrentMovementInput.x != Zero || CurrentMovementInput.y != Zero;
    }

    private void OnRun(InputAction.CallbackContext context)
    {
        IsRunPressed = context.ReadValueAsButton();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        IsJumpPressed = context.ReadValueAsButton();
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        IsAttacking = context.ReadValueAsButton();
    }

    private void OnBodySlam(InputAction.CallbackContext context)
    {
        IsBodyslam = context.ReadValueAsButton();
    }

    private void OnSlideAttack(InputAction.CallbackContext context)
    {
        IsSlideAttack = context.ReadValueAsButton();
    }

    private void OnDeathTest(InputAction.CallbackContext context)
    {
        Animator.SetTrigger(PlayerDied);
    }

    private void OnVictoryTest(InputAction.CallbackContext context)
    {
        Animator.SetTrigger(Victory);
    }
}
