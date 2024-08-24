using System.Collections;
using UnityEngine;

public class SuperPlayerController : MonoBehaviour
{
    private float smoothMoveTime = 0.15f;
    private float speedVelocity;
    private float currentSpeed;
    private float targetSpeed;
    private float rotationVelocity;
    private float smoothRotationTime = 0.3f;

    public float moveSpeed = 5f;                   // 이동 속도
    public float jumpForce = 3f;                   // 점프 힘
    public float resetPhaseDelay = 0.5f;             // 공격 리셋 시간

    public SuperCameraController cameraController; // SuperCameraController 참조
    public Animator animator;                      // 애니메이터 참조
    private Rigidbody rb;

    public bool isGround = true;
    public bool isMoving = false;
    public bool isAttacking = false;

    private Coroutine resetPhaseCoroutine;


    private enum State
    {
        IDLE,
        MOVE,
        JUMP,
        ATTACK
    }

    private State currentState = State.IDLE;
    private Transform cameraTransform; // 카메라 Transform 변수 추가

    private int attackPhase = 0;
    public bool canAttack = true; // 공격 가능 여부
    private float attackDelay = 1f; // 각 공격 사이의 딜레이

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        cameraTransform = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (cameraController.IsLockedOn && cameraController.LockedTarget != null)
        {
            RotateTowardsEnemy();
        }

        HandleState();

        if (Input.GetButtonDown("Jump") && isGround)
        {
            currentState = State.JUMP;
        }

        // Mouse Button Click Handling
        if (Input.GetMouseButtonDown(0) && canAttack && (currentState == State.IDLE || currentState == State.MOVE))
        {
            HandleAttack();
        }
    }

    private void HandleState()
    {
        switch (currentState)
        {
            case State.IDLE:
                HandleMove();
                break;
            case State.MOVE:
                HandleMove();
                break;
            case State.JUMP:
                HandleJump();
                break;
            case State.ATTACK:
                // 공격 중일 때는 추가 로직 필요 없음
                break;
        }
    }

    private void HandleMove()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 inputDir = input.normalized;
        isMoving = inputDir != Vector2.zero;

        if (cameraController.IsLockedOn && cameraController.LockedTarget != null)
        {
            Vector3 toTarget = (cameraController.LockedTarget.position - transform.position).normalized;
            float targetRotation = Mathf.Atan2(toTarget.x, toTarget.z) * Mathf.Rad2Deg;
            float newRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, smoothRotationTime);
            transform.eulerAngles = Vector3.up * newRotation;

            Vector3 moveDir = transform.forward * inputDir.y + transform.right * inputDir.x;
            targetSpeed = moveSpeed * moveDir.magnitude;
            currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedVelocity, smoothMoveTime);
            transform.Translate(moveDir.normalized * currentSpeed * Time.deltaTime, Space.World);
        }
        else
        {
            if (isMoving)
            {
                float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
                float newRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, smoothRotationTime);
                transform.eulerAngles = Vector3.up * newRotation;
            }

            targetSpeed = moveSpeed * inputDir.magnitude;
            currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedVelocity, smoothMoveTime);
            transform.Translate(transform.forward * currentSpeed * Time.deltaTime, Space.World);
        }

        animator.SetBool("isMoving", isMoving);

        if (!isMoving && currentState == State.MOVE)
        {
            currentState = State.IDLE;
        }
        else if (isMoving && currentState == State.IDLE)
        {
            currentState = State.MOVE;
        }
    }

    private void HandleJump()
    {
        if (isGround)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGround = false;
            animator.SetBool("isJumping", true); // 점프 애니메이션
            currentState = State.IDLE;
        }
    }

    private void HandleAttack()
    {
        isAttacking = true;
        canAttack = false; // 공격 가능 플래그를 false로 설정
        attackPhase++; // 공격 단계 증가

        switch (attackPhase)
        {
            case 1:
                animator.CrossFade("SwordAttack_1", 0.1f);
                break;
            case 2:
                animator.CrossFade("SwordAttack_2",0.1f);
                break;
            case 3:
                animator.CrossFade("SwordAttack_3",0.1f);
                StartCoroutine(PerformAttackMovement());
                break;
            default:
                return;
        }
        StartCoroutine(EnableNextAttackAfterDelay()); // 공격 가능 대기

        if (resetPhaseCoroutine != null)
        {
            StopCoroutine(resetPhaseCoroutine);
        }

        resetPhaseCoroutine = StartCoroutine(ResetAttackPhaseAfterDelay());
    }

    private IEnumerator PerformAttackMovement()
    {
        // 애니메이션의 특정 시간 동안 이동
        float attackAnimationDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        float startTime = Time.time;

        while (Time.time < startTime + attackAnimationDuration)
        {
            // 공격 애니메이션이 실행 중일 때 이동
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("SwordAttack_3"))
            {
                transform.Translate(Vector3.forward * 0f * Time.deltaTime);
            }
            yield return null;
        }
    }

    private IEnumerator EnableNextAttackAfterDelay()
    {
        //공격 단수 별 딜레이 조정
        if (attackPhase == 3)
        {
            attackDelay = 2f;
        }
        else
        {
            attackDelay = 1f;
        }

        moveSpeed = 0; //공격중 속도 제어
        yield return new WaitForSeconds(attackDelay);
        canAttack = true; // 다시 공격 가능해짐
        isAttacking = false;
        moveSpeed = 5;

        if (attackPhase >= 3) // 공격이 완료된 경우
        {
            attackPhase = 0; // 공격 단계 초기화
            currentState = State.IDLE; // IDLE로 돌아감
        }
    }

    private IEnumerator ResetAttackPhaseAfterDelay()
    {
        yield return new WaitForSeconds(1.5f); // 공격하지 않은 동안 대기
        if (attackPhase > 0) // 공격 단계가 0이 아니면
        {
            attackPhase = 0; // 공격 단계 초기화
            Debug.Log("공격초기화!");
            currentState = State.IDLE; // IDLE 상태로 변경
        }
    }

    private void RotateTowardsEnemy()
    {
        Vector3 direction = cameraController.LockedTarget.position - transform.position;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
            animator.SetBool("isJumping", false);
        }
    }
}