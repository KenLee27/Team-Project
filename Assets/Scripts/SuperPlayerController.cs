using System.Collections;
using UnityEngine;

public class SuperPlayerController : MonoBehaviour
{
    private float smoothMoveTime = 0.15f;
    private float speedVelocity;
    private float currentSpeed;
    private float targetSpeed;
    private float rotationVelocity;
    private float smoothRotationTime = 0.15f;
    private Vector3 lastMovement;                  //최근 이동 방향


    public float moveSpeed = 4f;                   // 이동 속도
    public float jumpForce = 3f;                   // 점프 힘
    public float resetPhaseDelay = 0.5f;             // 공격 리셋 시간
    public float DiveDelay = 1.2f;                 // 다이브 쿨타임
    public float PlayerHP = 100f;

    public float GetDamage = 10f;

    public SuperCameraController cameraController; // SuperCameraController 참조
    public Animator animator;                      // 애니메이터 참조
    private Rigidbody rb;

    public bool isGround = true;
    public bool isMoving = false;
    public bool isAttacking = false;
    public bool isDive = false;
    public bool isStand = true;
    public bool isAttacked = false;
    public bool isinvincibility = false;
    public bool isDie = false;
    public bool isAttackHit = false;


    private Coroutine resetPhaseCoroutine;


    private enum State
    {
        IDLE,
        MOVE,
        JUMP,
        ATTACK,
        DIVE,
        HIT,
        DIE
    }

    private State currentState = State.IDLE;
    private Transform cameraTransform; // 카메라 Transform 변수 추가

    private int attackPhase = 0;
    public bool canAttack = true; // 공격 가능 여부
    public bool canDive = true;
    public bool canCrouched = true;

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


        if (PlayerHP <= 0)
        {
            HandleDead();
        }

        if(isStand)
        {
            moveSpeed = 4f;
        }
        else if(!isStand)
        {
            moveSpeed = 2.5f;
        }

        //다이브 전까지 방향을 저장
        if (!isDive)
        {
            lastMovement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        }

        if (cameraController.IsLockedOn && cameraController.LockedTarget != null)
        {
            RotateTowardsEnemy();
        }

        HandleState();

        if (Input.GetButtonDown("Jump") && isGround &&!isAttacking&&!isDive)
        {
            currentState = State.JUMP;
        }

        // Mouse Button Click Handling
        if (Input.GetMouseButtonDown(0) && canAttack && (currentState == State.IDLE || currentState == State.MOVE)&&isGround&&!isDive)
        {
            HandleAttack();
        }
        //앉기
        if(canCrouched && Input.GetKeyDown(KeyCode.LeftControl) && isGround && !isAttacking && !isDive)
        {
            HandleCrouched();
        }

        if(canDive)
        {
            //카메라가 락온일때와 아닐때의 구르기 차이
            if (cameraController.IsLockedOn && cameraController.LockedTarget != null)
            {
                if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetAxisRaw("Horizontal") < 0 && !isAttacking && isGround)
                {
                    HandleDive_left();
                    Debug.Log("좌로 구른다!");
                }

                if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetAxisRaw("Horizontal") > 0 && !isAttacking && isGround)
                {
                    HandleDive_Right();
                    Debug.Log("우로 구른다!");
                }

                if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") > 0 && !isAttacking && isGround)
                {
                    HandleDive_Forward();
                    Debug.Log("앞로 구른다!");
                }

                if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") < 0 && !isAttacking && isGround)
                {
                    HandleDive_Back();
                    Debug.Log("뒤로 구른다!");
                }

                if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0 && !isAttacking && isGround)
                {
                    HandleStep_Back();
                    Debug.Log("백스탭!");
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetAxisRaw("Horizontal") < 0 && !isAttacking && isGround)
                {
                    HandleDive_Forward();
                    Debug.Log("구른다!");
                }

                if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetAxisRaw("Horizontal") > 0 && !isAttacking && isGround)
                {
                    HandleDive_Forward();
                    Debug.Log("구른다!");
                }

                if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") > 0 && !isAttacking && isGround)
                {
                    HandleDive_Forward();
                    Debug.Log("구른다!");
                }

                if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") < 0 && !isAttacking && isGround)
                {
                    HandleDive_Forward();
                    Debug.Log("구른다!");
                }

                if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0 && !isAttacking && isGround)
                {
                    HandleStep_Back();
                    Debug.Log("백스탭!");
                }
            }
        }
    }

    private void HandleDead()
    {
        animator.SetBool("isDead", true);


        currentState = State.DIE;
        isDie = true;
    }

    private void HandleCrouched()
    {
        if(isStand == true)
        {
            Debug.Log("앉아야지");
            isStand = false;
        }
        else if(isStand == false)
        {
            Debug.Log("일어나야지");
            isStand = true;
        }

        animator.SetBool("isCrouching",!isStand);

        StartCoroutine(EnableNextCrouchedAfterDelay());
    }

    private IEnumerator EnableNextCrouchedAfterDelay()
    {
        canCrouched = false;
        Debug.Log("앉기 쿨타임");
        yield return new WaitForSeconds(0.2f);
        Debug.Log("앉기 쿨타임 끝!");
        canCrouched = true;
    }

    private void HandleStep_Back()
    {
        canDive = false;
        currentState = State.DIVE;
        isDive = true;
        animator.CrossFade("BackStep", 0.1f, 0, 0);
        StartCoroutine(BackStepDirection());
        StartCoroutine(EnableNextBackStepAfterDelay());
    }
        

    private IEnumerator EnableNextBackStepAfterDelay()
    {

        Debug.Log("백스탭 쿨타임");
        yield return new WaitForSeconds(0.7f);
        Debug.Log("백스탭 쿨타임 끝!");
        canDive = true;
        currentState = State.IDLE;
    }

    private IEnumerator BackStepDirection()
    {
        isStand = true;
        float startTime = Time.time;
        while (Time.time < startTime + 0.7f)                                //애니메이션 시간
        {
            //벡스탭 무적시간 설정
            if (Time.time >= startTime + 0.2f && Time.time <= startTime + 0.5f)
            {
                isinvincibility = true;
            }
            else
            {
                isinvincibility = false;
            }
            transform.Translate(Vector3.back * 3f * Time.deltaTime);

            yield return null;
        }
        Debug.Log("애니메이션 끝!");
        animator.SetBool("isCrouching", !isStand);
        isDive = false;
        currentState = State.IDLE;
    }

    private void HandleDive_left()
    {
        canDive = false;
        currentState = State.DIVE;
        isDive = true;
        animator.CrossFade("LeftDive",0.1f,0,0);
        StartCoroutine(DiveDirection());
        StartCoroutine(EnableNextDiveAfterDelay());
    }

    private void HandleDive_Right()
    {
        canDive = false;
        currentState = State.DIVE;
        isDive = true;
        animator.CrossFade("RightDive",0.1f,0,0);
        StartCoroutine(DiveDirection());
        StartCoroutine(EnableNextDiveAfterDelay());
    }

    private void HandleDive_Forward()
    {
        canDive = false;
        currentState = State.DIVE;
        isDive = true;
        animator.CrossFade("ForwardDive", 0.1f,0,0);
        StartCoroutine(DiveDirection());
        StartCoroutine(EnableNextDiveAfterDelay());
    }

    private void HandleDive_Back()
    {
        canDive = false;
        currentState = State.DIVE;
        isDive = true;
        animator.CrossFade("BackDive", 0.1f,0,0);
        StartCoroutine(DiveDirection());
        StartCoroutine(EnableNextDiveAfterDelay());
    }

    private IEnumerator EnableNextDiveAfterDelay()
    {

        Debug.Log("다이브 쿨타임");
        yield return new WaitForSeconds(1.3f);
        Debug.Log("다이브 쿨타임 끝!");
        canDive = true;
        currentState = State.IDLE;
    }

    private IEnumerator DiveDirection()
    {
        bool invincibilityCheck = true;
        isStand = true;
        float attackAnimationDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        float startTime = Time.time;
        while (Time.time < startTime + 1.3f)
        {
            
            //다이브 무적시간
            if (Time.time >= startTime + 0.1f && Time.time <= startTime + 1.0f)
            {
                isinvincibility = true;
            }
            else if(invincibilityCheck && !(Time.time <= startTime + 1.0f))
            {
                isinvincibility = false;
                invincibilityCheck = false;
            }
            else if(!(Time.time >= startTime + 0.1f))
            {
                isinvincibility = false;
            }


            // 다이브 애니메이션이 실행 중일 때 이동
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("LeftDive"))
            {
                transform.Translate(lastMovement.normalized * 5f * Time.deltaTime);
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("RightDive"))
            {
                transform.Translate(lastMovement.normalized * 5f * Time.deltaTime);
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("ForwardDive"))
            {
                transform.Translate(Vector3.forward * 5f * Time.deltaTime);
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("BackDive"))
            {
                transform.Translate(Vector3.back * 5f * Time.deltaTime);
            }
            yield return null;
        }
        Debug.Log("애니메이션 끝!");
        animator.SetBool("isCrouching", !isStand);
        isDive = false;
        isMoving = true;
        currentState = State.IDLE;
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
            case State.DIVE:
                break;
            /*case State.HIT:
                HandleHit();
                break;
            case State.DIE:
                break;*/
        }
    }

    private void HandleMove()
    {
        //움직이지 못하는 상태 구현
        if (currentState == State.JUMP || currentState == State.DIVE || currentState ==State.ATTACK || currentState == State.HIT || currentState == State.DIE)
        {
            return;
        }

        //움직임 구현
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
        if(!isStand)
        {
            isStand = true;
            animator.SetBool("isCrouching", !isStand);
            currentState = State.IDLE;

            return;
        }

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
        currentState = State.ATTACK;
        isAttacking = true;
        canAttack = false; // 공격 가능 플래그를 false로 설정
        attackPhase++; // 공격 단계 증가

        switch (attackPhase)
        {
            case 1:
                animator.CrossFade("SwordAttack_1", 0.1f);
                StartCoroutine(PerformAttackMovement());
                break;
            case 2:
                animator.CrossFade("SwordAttack_2",0.1f);
                StartCoroutine(PerformAttackMovement());
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
        float startTime = 0f;

        while (startTime < attackAnimationDuration)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("SwordAttack_1"))
            {
                if(startTime > 0.5f && startTime < 0.8f)                                        //공격 판정 시간
                {
                    isAttackHit = true;
                }

                else
                {
                    isAttackHit = false;
                }
            }
            // 공격 애니메이션이 실행 중일 때 이동
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("SwordAttack_2"))
            {
                transform.Translate(Vector3.forward * 0.3f * Time.deltaTime);

                if (startTime > 0.666f && startTime < 1f)                                        //공격 판정 시간
                {
                    isAttackHit = true;
                }

                else
                {
                    isAttackHit = false;
                }

            }

            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("SwordAttack_3"))
            {
                transform.Translate(Vector3.forward * 1.2f * Time.deltaTime);

                if (startTime > 0.666f && startTime < 1f)                                        //공격 판정 시간
                {
                    isAttackHit = true;
                }

                else
                {
                    isAttackHit = false;
                }

            }

            startTime += Time.deltaTime;
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
            attackDelay = 1.5f;
        }

        yield return new WaitForSeconds(attackDelay);
        isStand = true;
        animator.SetBool("isCrouching", !isStand);
        isAttacking = false;
        canAttack = true; // 다시 공격 가능해짐

        if (attackPhase >= 3) // 공격이 완료된 경우
        {
            attackPhase = 0; // 공격 단계 초기화
            currentState = State.IDLE; // IDLE로 돌아감
        }
        currentState = State.IDLE;
    }

    private IEnumerator ResetAttackPhaseAfterDelay()
    {
        yield return new WaitForSeconds(1.7f); // 공격하지 않은 동안 대기
        if (attackPhase > 0) // 공격 단계가 0이 아니면
        {
            attackPhase = 0; // 공격 단계 초기화
            Debug.Log("공격초기화!");
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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyAttack"))
        {
            if (!isinvincibility)
            {
                isinvincibility = true;
                currentState = State.HIT;
                Debug.Log("구울에게 맞았다!");
                isAttacked = true;
                HandleHit();
            }
        }
    }

    private void HandleHit()
    {
        Debug.Log("10데미지!");
        PlayerHP = PlayerHP - GetDamage;
        GameManager.Instance.UpdatePlayerHP(PlayerHP);
        isAttackHit = false;
        animator.CrossFade("Hit", 0.1f,0,0);
        attackPhase = 0;


        StartCoroutine(AttackedMotionDelay());
        
    }

    private IEnumerator AttackedMotionDelay()
    {
        isAttacked = false;
        isStand = true;
        float attackAnimationDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        float startTime = Time.time;
        while (Time.time < startTime + 0.8f)
        {

            //피격 무적시간
            if (Time.time >= startTime + 0.0f && Time.time <= startTime + 0.7f)
            {
                
            }
            else
            {
                isinvincibility = false;
            }

            yield return null;
        }
        animator.SetBool("isAttacked", isAttacked);

        animator.SetBool("isCrouching", !isStand);

        currentState = State.IDLE;
    }

}