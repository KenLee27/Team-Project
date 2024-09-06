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
    private float timeSinceLastDive;


    public float moveSpeed = 4f;                   // 이동 속도
    public float jumpForce = 5f;                   // 점프 힘
    public float resetPhaseDelay = 1.2f;             // 공격 리셋 시간
    public float DiveDelay = 1.1f;                 // 다이브 쿨타임
    public float PlayerHP = 0f;
    public float PlayerMaxHP = 100f;
    public float PlayerStamina = 0f;
    public float PlayerMaxStamina = 100f;
    public float StaminaRegenTime = 1f;          //스테미나 회복을 위해 스테미나 소모를 멈추고 기다려야 하는 시간
    public float StaminaRegenSpeed = 20f;          //스테미나 초당 회복 수치


    private Vector2 velocity = Vector2.zero;



    public float PlayerDamage = 3f;                //플레이어 데미지

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
    public bool FirstStaminaCheck = false;

    private bool firstDropDie = true;


    private Coroutine resetPhaseCoroutine;
    private Coroutine resetS_PhaseCoroutine;


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
    private int s_attackPhase = 0;

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
        isinvincibility = false;
        firstDropDie = true;

        PlayerHP = PlayerMaxHP;
        PlayerStamina = PlayerMaxStamina;
        GameManager.Instance.UpdatePlayerHP(PlayerHP);
        GameManager.Instance.UpdatePlayerST(PlayerStamina);
        timeSinceLastDive = StaminaRegenTime;
    }

    void Update()
    {
        GameManager.Instance.UpdatePlayerST(PlayerStamina);

        //플레이어 스테미나 컨트롤러
        if ( PlayerStamina < 30f )
        {
            canDive = false;
            FirstStaminaCheck = true;
        }
        else if( PlayerStamina >=40 && FirstStaminaCheck)
        {
            canDive = true;
            FirstStaminaCheck = false;
        }

        //회피 시전 후 1초가 지나야 스테미나가 회복 가능.
        if (timeSinceLastDive >= StaminaRegenTime)
        {
            RecoverStamina();
        }
        else
        {
            timeSinceLastDive += Time.deltaTime;
        }

        //사망 컨트롤러
        if (PlayerHP <= 0)
        {
            HandleDead();
        }

        //서있을 때와 앉아 있을 때의 속도 컨트롤러
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

        //락온 상태일때 적 바라보기 기능
        if (cameraController.IsLockedOn && cameraController.LockedTarget != null)
        {
            RotateTowardsEnemy();
        }

        HandleState();

        //점프 컨트롤러
        if (Input.GetButtonDown("Jump") && isGround &&!isAttacking&&!isDive)
        {
            currentState = State.JUMP;
        }

        //버튼 클릭 & 공격 컨트롤러
        if (Input.GetMouseButtonDown(0) && canAttack && (currentState == State.IDLE || currentState == State.MOVE)&&isGround&&!isDive)
        {
            HandleAttack();
        }

        if (Input.GetMouseButtonDown(1) && canAttack && (currentState == State.IDLE || currentState == State.MOVE) && isGround && !isDive)
        {
            HandleSpecialAttack();
        }
        //앉기
        if (canCrouched && Input.GetKeyDown(KeyCode.LeftControl) && isGround && !isAttacking && !isDive)
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
                if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetAxisRaw("Horizontal") != 0 && !isAttacking && isGround)
                {
                    HandleDive_Forward();
                    Debug.Log("구른다!");
                }
                if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") != 0 && !isAttacking && isGround)
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

    void RecoverStamina()
    {
        PlayerStamina += StaminaRegenSpeed * Time.deltaTime;
        if (PlayerStamina > PlayerMaxStamina)
        {
            PlayerStamina = PlayerMaxStamina;
        }
    }


    private void HandleDead()
    {
        isinvincibility = true;
        canAttack = false;
        canDive = false;

        //GameManager.Instance.UpdatePlayerHP(PlayerHP);
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
        PlayerStamina -= 20f;
        isStand = true;
        float startTime = Time.time;
        bool invincibilityCheck = true;
        while (Time.time < startTime + 0.7f)                                //애니메이션 시간
        {
            transform.Translate(Vector3.back * 3f * Time.deltaTime);

            //벡스탭 실행 중에 스테미나 회복 대기 시간 0초 고정
            timeSinceLastDive = 0f;
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
        yield return new WaitForSeconds(DiveDelay);
        Debug.Log("다이브 쿨타임 끝!");
        canDive = true;
        currentState = State.IDLE;
    }

    public void TriggerDive()
    {
        isinvincibility = true;
    }

    public void EndDive()
    {
        isinvincibility = false;
    }

    private IEnumerator DiveDirection()
    {
        PlayerStamina -= 30f;
        bool invincibilityCheck = true;
        isStand = true;
        float attackAnimationDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        float startTime = Time.time;
        while (Time.time < startTime + 1.1f)
        {

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

            //다이브 애니메이션이 실행 중일 때 스테미나 회복 대기 시간 0초 고정
            timeSinceLastDive = 0f;
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
                break;
            case State.DIVE:
                break;
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

        animator.SetBool("isMoving", isMoving);

        if (cameraController.IsLockedOn && cameraController.LockedTarget != null)
        {

            animator.SetBool("isLockOn", true);

            Vector3 toTarget = (cameraController.LockedTarget.position - transform.position).normalized;
            float targetRotation = Mathf.Atan2(toTarget.x, toTarget.z) * Mathf.Rad2Deg;
            float newRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, smoothRotationTime);
            transform.eulerAngles = Vector3.up * newRotation;

            Vector3 moveDir = transform.forward * inputDir.y + transform.right * inputDir.x;
            targetSpeed = moveSpeed * moveDir.magnitude;
            currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedVelocity, smoothMoveTime);
            transform.Translate(moveDir.normalized * currentSpeed * Time.deltaTime, Space.World);

            Vector3 localMoveDir = transform.InverseTransformDirection(moveDir);
            float smoothX = Mathf.SmoothDamp(animator.GetFloat("Xaxis"), localMoveDir.x, ref velocity.x, 0.1f);
            float smoothY = Mathf.SmoothDamp(animator.GetFloat("Yaxis"), localMoveDir.z, ref velocity.y, 0.1f);

            animator.SetFloat("Xaxis", smoothX);
            animator.SetFloat("Yaxis", smoothY);
        }
        else
        {
            animator.SetBool("isLockOn", false);
            if (isMoving)
            {
                float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
                float newRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, smoothRotationTime);
                transform.eulerAngles = Vector3.up * newRotation;
            }

            targetSpeed = moveSpeed * inputDir.magnitude;
            currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedVelocity, smoothMoveTime);
            transform.Translate(transform.forward * currentSpeed * Time.deltaTime, Space.World);

            animator.SetBool("isLockOn", false);
        }

        

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
                animator.CrossFade("SwordAttack_2", 0.1f);
                StartCoroutine(PerformAttackMovement());
                break;
            case 3:
                animator.CrossFade("SwordAttack_3", 0.1f);
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

    private void HandleSpecialAttack()
    {
        currentState = State.ATTACK;
        isAttacking = true;
        canAttack = false; // 공격 가능 플래그를 false로 설정
        s_attackPhase++; // 공격 단계 증가

        switch (s_attackPhase)
        {
            case 1:
                animator.CrossFade("SwordSpecialAttack_1", 0.1f);
                StartCoroutine(PerformS_AttackMovement());
                break;
            case 2:
                animator.CrossFade("SwordSpecialAttack_2", 0.1f);
                StartCoroutine(PerformS_AttackMovement());
                break;
            default:
                return;
        }
        StartCoroutine(EnableNextS_AttackAfterDelay()); // 공격 가능 대기

        if (resetS_PhaseCoroutine != null)
        {
            StopCoroutine(resetS_PhaseCoroutine);
        }

        resetS_PhaseCoroutine = StartCoroutine(ResetS_AttackPhaseAfterDelay());
    }


    private IEnumerator PerformAttackMovement()
    {
        // 애니메이션의 특정 시간 동안 컨트롤
        float attackAnimationDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        float startTime = 0f;

        while (startTime < attackAnimationDuration)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("SwordAttack_1"))
            {
                PlayerDamage = 3f;
            }
            // 공격 애니메이션이 실행 중일 때 이동
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("SwordAttack_2"))
            {
                PlayerDamage = 4f;
            }

            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("SwordAttack_3"))
            {
                PlayerDamage = 6f;
            }

            startTime += Time.deltaTime;
            yield return null;
        }
    }
    private IEnumerator PerformS_AttackMovement()
    {
        // 애니메이션의 특정 시간 동안 컨트롤
        float attackAnimationDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        float startTime = 0f;

        while (startTime < attackAnimationDuration)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("SwordSpecialAttack_1"))
            {
                PlayerDamage = 10f;
            }
            // 공격 애니메이션이 실행 중일 때 이동
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("SwordSpecialAttack_2"))
            {
                PlayerDamage = 20f;
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
            attackDelay = 1.44f;
        }
        else
        {
            attackDelay = 0.9f;
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
    private IEnumerator EnableNextS_AttackAfterDelay()
    {
        //공격 단수 별 딜레이 조정
        
        attackDelay = 2f;
        

        yield return new WaitForSeconds(attackDelay);
        isStand = true;
        animator.SetBool("isCrouching", !isStand);
        isAttacking = false;
        canAttack = true; // 다시 공격 가능해짐

        if (s_attackPhase >= 2) // 공격이 완료된 경우
        {
            s_attackPhase = 0; // 공격 단계 초기화
            currentState = State.IDLE; // IDLE로 돌아감
        }
        currentState = State.IDLE;
    }

    private IEnumerator ResetAttackPhaseAfterDelay()
    {
        yield return new WaitForSeconds(resetPhaseDelay); // 공격하지 않은 동안 대기
        if (attackPhase > 0) // 공격 단계가 0이 아니면
        {
            attackPhase = 0; // 공격 단계 초기화
            Debug.Log("공격초기화!");
        }
    }

    private IEnumerator ResetS_AttackPhaseAfterDelay()
    {
        yield return new WaitForSeconds(2.2f); // 공격하지 않은 동안 대기
        if (s_attackPhase > 0) // 공격 단계가 0이 아니면
        {
            s_attackPhase = 0; // 공격 단계 초기화
            Debug.Log("공격초기화!");
        }
    }

    public void TriggerAttack()
    {
        isAttackHit = true;
    }

    public void EndAttack()
    {
        isAttackHit = false;
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

        if (firstDropDie&&collision.gameObject.CompareTag("DeathZone"))
        {
            PlayerHP -= PlayerMaxHP;
            GameManager.Instance.UpdatePlayerHP(PlayerHP);
            firstDropDie = false;
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
                Debug.Log("맞았다!");
                isAttacked = true;
                HandleHit(other);
            }
        }
    }

    private void HandleHit(Collider other)
    {
        Ienemy enemy = other.GetComponentInParent<Ienemy>();
        float GetDamage = enemy.Damage;
        Debug.Log("데미지!");
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
        canDive = false;
        isGround = false;
        canCrouched = false;
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
        canDive = true;
        isGround = true;
        canCrouched = true;

        animator.SetBool("isAttacked", isAttacked);

        animator.SetBool("isCrouching", !isStand);

        currentState = State.IDLE;
    }

}