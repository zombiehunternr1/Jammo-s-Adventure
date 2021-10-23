using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using UnityEngine.InputSystem;
using System;

public class PlayerScript : MonoBehaviour
{
    public float    Acceleration = 70f,
                    SprintingAcceleration = 140.0f,
                    AirAcceleration = 18f,
                    Deceleration = 7.6f,
                    AirDeceleration = 1.1f,
                    RotateSpeed = 0.7f,
                    AirRotateSpeed = 0.4f,
                    SlideSpeed = 35.0f,
                    Gravity = 150.0f,
                    MaxSpeed = 9.0f,
                    MovingPlatformFriction = 7.7f,
                    Average = .5f,
                    CurrentAcceleration,
                    CurrentDeceleration,
                    CurrentRotationSpeed,
                    DistanceToTarget,
                    CurrentGravity,
                    GroundDistance = 1.1f,
                    GroundLength,
                    JumpForce = 8.0f,
                    GroundPoundForce = 0.1f,
                    DoubleJumpForce = 8.0f,
                    DoubleJumpHeight = 1f,
                    SmallBounce = 1.2f,
                    BigBounce = 1.5f,
                    GroundPoundDistance = 3f,
                    BounceForce,
                    StartPunchTime = 0,
                    PunchCoolDown = 0,
                    GoalPunchTime = 1.5f,
                    HUDDisplayTime,
                    ResetTime = 3f,
                    IdleDanceMarker = 10f;

    public Vector3  Direction,
                    MoveDirection,
                    ScreenMovementForward,
                    ScreenMovementRight,
                    MovingObjSpeed,
                    CurrentSpeed,
                    PlayerSpawnPosition,
                    Offset,
                    PlayerCheckPointPosition;

    public bool     CanMove,
                    IsRunning,
                    IsRunPressed,
                    IsAttacking,
                    IsBodyslam,
                    IsBodyslamPerforming,
                    IsSlideAttack,
                    IsSlideAttackPerforming,
                    IsHoldJump,
                    IsBounce,
                    ApplyGroundPoundGravity = false,
                    IsSidescroller = false,
                    Grounded,
                    HasJumped = false,
                    HasDoubleJumped = false,
                    CanDoubleJump = false,
                    CanPunch = true,
                    SwitchPunch,
                    HasExploded,
                    IsDisplayingHUD,
                    IsDisplayHUDPerforming,
                    IsDancing;

    private float IdleTimer;
    private Quaternion ScreenMovementSpace;
    private Quaternion PlayerOriginalRotation;

    [Header("Components")]
    public GameObject ExplosionModel;
    public Mesh PlayerGroundMesh;
    public Animator HUDAnimator;
    public AnimatorController AnimController;
    public Transform MainCamera;
    public GameObject GroundPoundDust;
    [HideInInspector]
    public GameObject PlayerModel;
    [HideInInspector]
    public PlayerInput playerInput;
    [HideInInspector]
    public Rigidbody RB;
    [HideInInspector]
    public CharacterSkinController SkinController;
    [HideInInspector]
    public Animator playerAnimator;
    private Collider Cap;
    private BlendTree LongIdleTree;

    //Player movement
    Vector2 CurrentMovementInput;
    Vector3 CurrentMovement;

    //Attack settings
    Vector3 SmallHitBox = new Vector3(2, 0.5f, 2);
    Vector3 BigHitBox = new Vector3(2, 2f, 2);
    Vector3 GroundPoundHitBox = new Vector3(2.5f, 0.1f, 2.5f);
    Vector3 SlideAttackHitBox = new Vector3(1f, 0.5f, 1f);

    //Hit detection
    int SideHitValue;
    Collider[] hitColliders;
    enum HitPlayerDirection { None, Top, Bottom, Forward, Back, Left, Right, Attack, Bodyslam, Slide }

    private void Start()
    {
        PlayerModel = GetComponentInChildren<Transform>().GetChild(0).gameObject;
        playerAnimator = GetComponent<Animator>();
        RB = GetComponent<Rigidbody>();
        Cap = GetComponent<Collider>();
        SkinController = GetComponent<CharacterSkinController>();
        playerInput = GetComponent<PlayerInput>();
        PlayerSpawnPosition = transform.position;
        PlayerOriginalRotation = transform.rotation;
        AnimatorStateMachine RootStateMachine = AnimController.layers[0].stateMachine;
        AnimatorState StateWithBlendTree = RootStateMachine.states[RootStateMachine.states.Length - 2].state;
        LongIdleTree = (BlendTree)StateWithBlendTree.motion;
    }

    private void Update()
    {
        CurrentGravity = ApplyGroundPoundGravity ? Gravity / 8 : Gravity;

        if (IsRunning && !IsSlideAttackPerforming)
        {
            CurrentAcceleration = (Grounded) ? SprintingAcceleration : AirAcceleration;
        }
        else
        {
            if (IsSlideAttackPerforming)
            {
                CurrentAcceleration = (Grounded) ? SlideSpeed : AirAcceleration;
            }
            else
            {
                CurrentAcceleration = (Grounded) ? Acceleration : AirAcceleration;
            }
        }
        CurrentDeceleration = (Grounded) ? Deceleration : AirDeceleration;
        CurrentRotationSpeed = (Grounded) ? RotateSpeed : AirRotateSpeed;

        ScreenMovementSpace = Quaternion.Euler(0, MainCamera.eulerAngles.y, 0);
        ScreenMovementForward = ScreenMovementSpace * Vector3.forward;
        ScreenMovementRight = ScreenMovementSpace * Vector3.right;

        float H = CurrentMovementInput.x;
        float V = CurrentMovementInput.y;

        if (!IsSidescroller)
        {
            if (IsSlideAttackPerforming)
            {
                Direction = transform.forward;
            }
            else
            {
                Direction = (ScreenMovementForward * V) + (ScreenMovementRight * H);
            }
        }
        else
        {
            Direction = Vector3.right * H;
        }
        if (IsSlideAttackPerforming)
        {
            hitColliders = Physics.OverlapBox(Cap.bounds.center / 2, SlideAttackHitBox);
            CheckAttackHit();
        }
        MoveDirection = transform.position + Direction;
        HandleMovementAnimation();
    }

    private void FixedUpdate()
    {
        Grounded = IsGrounded();
        if (HasJumped)
        {
            Jump(JumpForce);
        }

        if (HasDoubleJumped)
        {
            DoubleJump(JumpForce);
        }

        if (CanMove)
        {
            MoveTo(MoveDirection, CurrentAcceleration, 0.4f, true);
        }
        if (RotateSpeed != 0 && Direction.magnitude != 0)
        {
            if (CanMove && !IsSlideAttackPerforming)
            {
                RotateToDirection(MoveDirection, CurrentRotationSpeed * 5, true);
            }
        }
        ManageSpeed(CurrentDeceleration, MaxSpeed + MovingObjSpeed.magnitude, true);
        if (!IsBodyslamPerforming)
        {
            RB.AddForce(new Vector3(0, -CurrentGravity, 0), ForceMode.Force);
        }
    }

    public void ResetGameoverPosition()
    {
        transform.position = PlayerSpawnPosition;
        transform.rotation = PlayerOriginalRotation;
    }

    public void SetCheckPoint(Checkpoint Checkpoint)
    {
        PlayerCheckPointPosition = Checkpoint.gameObject.transform.position;
    }

    public void ResetCheckpointPosition()
    {
        if (PlayerCheckPointPosition != new Vector3(0, 0, 0))
        {
            transform.position = PlayerCheckPointPosition;
        }
        else
        {
            transform.position = PlayerSpawnPosition;
        }
        transform.rotation = PlayerOriginalRotation;
    }

    private bool IsGrounded()
    {
        RaycastHit? Hit = GetGroundHit();
        if(Hit != null)
        {
            if (IsBodyslamPerforming)
            {
                Instantiate(GroundPoundDust, new Vector3(transform.position.x, Hit.Value.point.y, transform.position.z), transform.rotation);
                hitColliders = Physics.OverlapBox(Hit.Value.point, GroundPoundHitBox);
                foreach(Collider Col in hitColliders)
                {
                    ICrateBase CrateType = (ICrateBase)Col.gameObject.transform.GetComponent(typeof(ICrateBase));
                    if (CrateType != null)
                    {
                        CrateType.Break((int)ReturnDirection(gameObject, Col.gameObject));
                    }
                }
                GroundPoundEnd();
            }
        }
        return Hit != null;
    }
    public void ResetPlayerMovement()
    {
        RB.velocity = Vector3.zero;
        CurrentMovementInput = Vector2.zero;
        RB.MovePosition(Vector3.zero);
        IsSlideAttackPerforming = false;
        DistanceToTarget = 0;
        playerAnimator.SetFloat("IsMoving", 0);
        playerAnimator.SetBool("IsRunning", false);
        playerAnimator.Rebind();
    }

    private void HandleMovementAnimation()
    {
        playerAnimator.SetFloat("IsMoving", DistanceToTarget);
        playerAnimator.SetBool("IsGrounded", Grounded);
        if (GameManager.Booleans.CanMove)
        {
            CheckLongIdle();
        }
    }

    private void CheckLongIdle()
    {
        if (Direction == Vector3.zero && !IsAttacking)
        {
            if (!IsDancing)
            {
                IdleTimer += Time.deltaTime;
                if (IdleTimer >= IdleDanceMarker)
                {
                    IsDancing = true;
                    IdleTimer = 0;
                    int RandomLongIdleChosen = UnityEngine.Random.Range(0, LongIdleTree.children.Length);
                    playerAnimator.SetFloat("RandomLongIdle", RandomLongIdleChosen);
                    playerAnimator.Play("Long Idles");
                }
            }
            else if (playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f)
            {
                IsDancing = false;
                IdleTimer = 0;
                playerAnimator.Play("Movement");
                SkinController.ReturnToNormalEvent();
            }
        }
        else
        {
            if (IsDancing)
            {
                IsDancing = false;
                IdleTimer = 0;
                playerAnimator.Play("Movement");
                SkinController.ReturnToNormalEvent();
            }
            IdleTimer = 0;
        }
    }

    private float DistanceToGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(Cap.bounds.center, Vector3.down, out hit, 200))
        {
            if (hit.transform)
            {
                var distance = Vector3.Distance(Cap.bounds.center, hit.point);
                return distance;
            }
        }
        return 0;
    }

    #region Player Actions

    public void Jump(float jumpForce)
    {
        RB.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        playerAnimator.Play("Jump");
        HasJumped = false;
        CanDoubleJump = true;
    }

    public void DoubleJump(float jumpForce)
    {
        RB.velocity = Vector3.zero;
        RB.AddForce(Vector3.up * DoubleJumpForce, ForceMode.Impulse);
        playerAnimator.Play("Flip");
        HasDoubleJumped = false;
        CanDoubleJump = false;
    }
    private void GroundPoundStart()
    {
        IsBodyslamPerforming = true;
        ApplyGroundPoundGravity = true;
        RB.velocity = Vector3.zero;
        CanMove = false;
        playerAnimator.Play("Groundpound");
        StartCoroutine(DelayFalling(0.2f));
    }

    private void GroundPoundEnd()
    {
        //CameraShake.Instance.DoShake();
        IsBodyslamPerforming = false;
        ApplyGroundPoundGravity = false;
        StartCoroutine(EnableMovement(0.3f));
    }

    #endregion

    #region Player Collision methods

    private void OnCollisionEnter(Collision collision)
    {
        ICrateBase CrateType = (ICrateBase)collision.gameObject.GetComponent(typeof(ICrateBase));
        if(CrateType != null)
        {
            int Direction = (int)ReturnDirection(gameObject, collision.collider.gameObject);
            CrateType.Break(Direction);
            if(Direction == 1)
            {
                if(CrateType is TNT)
                {
                    TNT Tnt = collision.gameObject.GetComponent<TNT>();
                    if (!Tnt.HasBounced && !IsBodyslamPerforming)
                    {
                        float Bounceforce = IsHoldJump ? BigBounce : SmallBounce;
                        Jump(Bounceforce);
                        Tnt.HasBounced = true;
                    }
                }
                else
                {
                    if (!IsBodyslamPerforming)
                    {
                        float Bounceforce = IsHoldJump ? BigBounce : SmallBounce;
                        Jump(Bounceforce);
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        ICollectable Collectable = (ICollectable)collider.gameObject.GetComponent(typeof(ICollectable));
        if (Collectable != null)
        {
            Collectable.Collect();
        }
    }

    private void CheckAttackHit()
    {
        if (hitColliders != null)
        {
            foreach (Collider hitCollider in hitColliders)
            {
                Vector3 forward = (hitCollider.transform.position - transform.position).normalized;

                if (Vector3.Dot(forward, transform.forward) > 0.7f)
                {
                    ICrateBase CrateType = (ICrateBase)hitCollider.gameObject.GetComponent(typeof(ICrateBase));
                    if (CrateType != null)
                    {
                        CrateType.Break((int)ReturnDirection(gameObject, hitCollider.gameObject));
                    }
                }
            }
        }
    }

    #endregion

    #region Hit Detection Converter

    //Detects if hitting an object or not
    public RaycastHit? GetGroundHit()
    {
        foreach (var x in PlayerGroundMesh.vertices)
        {
            RaycastHit hit;
            var point = transform.TransformPoint(x);
            Debug.DrawRay(point, Vector3.down * GroundLength, Color.blue);
            if (Physics.Raycast(point, Vector3.down, out hit, GroundLength))
            {
                if (hit.transform)
                {
                    return hit;
                }
            }
        }
        return null;
    }

    //Converts type of interaction into an integer depending on enum value
    private HitPlayerDirection ReturnDirection(GameObject Object, GameObject ObjectHit)
    {
        HitPlayerDirection HitDirection = HitPlayerDirection.None;
        if (IsBodyslamPerforming)
        {
            return HitPlayerDirection.Bodyslam;
        }

        else if (!CanPunch)
        {
            return HitPlayerDirection.Attack;
        }
        else if (IsSlideAttackPerforming)
        {
            return HitPlayerDirection.Slide;
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
    #endregion

    #region Coroutines

    public IEnumerator DownwardsForce()
    {
        while (transform.position.y > 0 && !IsGrounded())
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - Average * Time.deltaTime, transform.position.z);
            yield return transform.position; 
        }
        StopCoroutine(DownwardsForce());
    }

    IEnumerator DelayFalling(float time)
    {
        yield return new WaitForSeconds(time);
        RB.AddForce(Vector3.down * JumpForce * GroundPoundForce, ForceMode.Impulse);
    }

    IEnumerator EnableMovement(float time)
    {
        yield return new WaitForSeconds(time);
        CanMove = true;
    }
    IEnumerator DisplayHUD()
    {
        if (IsDisplayHUDPerforming)
        {
            HUDAnimator.SetBool("IsDisplayingHUD", IsDisplayHUDPerforming);
            while (HUDDisplayTime < ResetTime)
            {
                HUDDisplayTime += Time.deltaTime;
                yield return HUDDisplayTime;
            }
            HUDDisplayTime = 0;
            IsDisplayHUDPerforming = false;
            HUDAnimator.SetBool("IsDisplayingHUD", IsDisplayHUDPerforming);
        }
    }
    #endregion

    #region Animation Events
    private void SmallAttackEvent()
    {
        hitColliders = Physics.OverlapBox(Cap.bounds.center / 2, SmallHitBox);
        CheckAttackHit();
    }

    private void BigAttackEvent()
    {
        hitColliders = Physics.OverlapBox(Cap.bounds.center, BigHitBox);
        CheckAttackHit();
    }

    private void ResetPunchEvent()
    {
        CanPunch = true;
        playerAnimator.SetInteger("Swap Punch", 0);
    }

    private void SlideAttackEvent()
    {
        Cap.GetComponent<CapsuleCollider>().center = new Vector3(0, 0.5f, 0);
        Cap.GetComponent<CapsuleCollider>().height = 0.9f;
    }

    private void SlideAttackEndEvent()
    {
        IsSlideAttackPerforming = false;
        Cap.GetComponent<CapsuleCollider>().center = new Vector3(0, 0.9f, 0);
        Cap.GetComponent<CapsuleCollider>().height = 1.8f;
    }
    #endregion

    #region RigidBody Controls

    #region (boolean) MoveTo(Vector3 Destination, float Acceleration, float StoppingDistance, bool IgnoreY)
    public bool MoveTo(Vector3 Destination, float Acceleration, float StoppingDistance, bool IgnoreY)
    {
        Vector3 RelativePosition = (Destination - transform.position);
        if (IgnoreY)
        { RelativePosition.y = 0; }

        DistanceToTarget = RelativePosition.magnitude;
        if (DistanceToTarget <= StoppingDistance)
        { return true; }
        else
        {
            RB.AddForce(RelativePosition.normalized * Acceleration * Time.deltaTime, ForceMode.VelocityChange);
            return false;
        }
    }
    #endregion //Move To Position.

    #region RotateToVelocity(float TurnSpeed, bool IgnoreY)
    public void RotateToVelocity(float TurnSpeed, bool IgnoreY)
    {
        Vector3 Direction;
        if (IgnoreY)
            Direction = new Vector3(RB.velocity.x, 0f, RB.velocity.z);
        else
            Direction = RB.velocity;

        if (Direction.magnitude > 0.1)
        {
            Quaternion dir = Quaternion.LookRotation(Direction);
            Quaternion Slerp = Quaternion.Slerp(transform.rotation, dir, Direction.magnitude * TurnSpeed * Time.deltaTime);
            RB.MoveRotation(Slerp);
        }
    }
    #endregion //Rotate Towards Velocity Direction.

    #region RotateToDirection(Vector3 LookDirection, float TurnSpeed, bool IgnoreY)
    public void RotateToDirection(Vector3 LookDirection, float TurnSpeed, bool IgnoreY)
    {
        Vector3 CharacterPosition = transform.position;
        if (IgnoreY)
        {
            CharacterPosition.y = 0;
            LookDirection.y = 0;
        }

        Vector3 NewDirection = LookDirection - CharacterPosition;
        Quaternion Direction = Quaternion.LookRotation(NewDirection);
        Quaternion Slerp = Quaternion.Slerp(transform.rotation, Direction, TurnSpeed * Time.deltaTime);
        RB.MoveRotation(Slerp);
    }
    #endregion //Rotate Towards Said Direction.

    #region ManageSpeed(float Deceleration, float MaxSpeed, bool IgnoreY)
    public void ManageSpeed(float Deceleration, float MaxSpeed, bool IgnoreY)
    {
        CurrentSpeed = RB.velocity;
        if (IgnoreY)
            CurrentSpeed.y = 0;

        if (CurrentSpeed.magnitude > 0)
        {
            RB.AddForce((CurrentSpeed * -1) * Deceleration * Time.deltaTime, ForceMode.VelocityChange);
            if (RB.velocity.magnitude > MaxSpeed)
                RB.AddForce((CurrentSpeed * -1) * Deceleration * Time.deltaTime, ForceMode.VelocityChange);
        }
    }
    #endregion //Manage Our Speed.

    #endregion

    #region InputSystem

    public void OnMove(InputAction.CallbackContext context)
    {
        if (GameManager.Booleans.CanMove && !IsSlideAttackPerforming)
        {
            if (!IsRunPressed)
            {
                playerAnimator.SetBool("IsRunning", false);
            }
            CurrentMovementInput = context.ReadValue<Vector2>();
            CurrentMovement = new Vector3(CurrentMovementInput.x, 0, CurrentMovementInput.y);
        }
        else
        {
            if (IsSlideAttackPerforming)
            {
                CurrentMovement = CurrentSpeed * SlideSpeed;
            }
            else
            {
                CurrentMovement = Vector3.zero;
                RB.velocity = Vector3.zero;
            }
        }
    }
    public void OnRun(InputAction.CallbackContext context)
    {
        if (GameManager.Booleans.CanMove)
        {
            IsRunPressed = context.ReadValueAsButton();
            IsRunning = IsRunPressed;
            playerAnimator.SetBool("IsRunning", IsRunning);
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        SlideAttackEndEvent();
        if (GameManager.Booleans.CanMove)
        {
            if (context.ReadValueAsButton() == true)
            {
                IsHoldJump = true;
                if (!HasJumped)
                {
                    if (Grounded)
                    {
                        HasJumped = true;
                    }
                }
                var distance = DistanceToGround();
                if (CanDoubleJump && !HasDoubleJumped && distance > DoubleJumpHeight)
                    HasDoubleJumped = true;
            }
            else
            {
                IsHoldJump = false;
            }
        }  
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (GameManager.Booleans.CanMove)
        {
            if (IsAttacking = context.ReadValueAsButton() == true)
            {
                if (Grounded)
                {
                    if (DistanceToTarget == 0)
                    {
                        playerAnimator.Play("Small attack");
                    }
                    else
                    {
                        if (CanPunch)
                        {
                            CanPunch = false;
                            int Result = new System.Random().Next(1, 100);
                            int Value = (Result > 50) ? 1 : 2;
                            playerAnimator.SetInteger("Swap Punch", Value);
                            playerAnimator.Play("Attack");
                        }
                    }
                }
                else
                {
                    playerAnimator.Play("Mid-air attack");
                }
            }
        }
    }

    public void OnBodySlam(InputAction.CallbackContext context)
    {
        IsBodyslam = context.ReadValueAsButton();
        if (IsBodyslam && !Grounded)
        {
            if (!IsBodyslamPerforming)
            {
                var distance = DistanceToGround();
                if (distance >= GroundPoundDistance)
                {
                    GroundPoundStart();
                }
            }
        }
    }

    public void OnSlideAttack(InputAction.CallbackContext context)
    {
        if (GameManager.Booleans.CanMove)
        {
            IsSlideAttack = context.ReadValueAsButton();
            if (IsSlideAttack && Grounded && DistanceToTarget != 0)
            {
                playerAnimator.Play("Slide");
                IsSlideAttack = true;
                IsSlideAttackPerforming = IsSlideAttack;
            }
        }
    }

    public void OnHUDDisplay(InputAction.CallbackContext context)
    {
        if (GameManager.Booleans.CanMove)
        {
            if (!IsDisplayHUDPerforming)
            {
                IsDisplayingHUD = context.ReadValueAsButton();
                IsDisplayHUDPerforming = IsDisplayingHUD;
                StartCoroutine(DisplayHUD());
            }
        }
    }
    #endregion
}
