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
    private Vector3 lastMovement;                  //�ֱ� �̵� ����


    public float moveSpeed = 4f;                   // �̵� �ӵ�
    public float jumpForce = 3f;                   // ���� ��
    public float resetPhaseDelay = 0.5f;             // ���� ���� �ð�
    public float DiveDelay = 1.2f;                 // ���̺� ��Ÿ��
    public float PlayerHP = 100f;

    public float GetDamage = 10f;

    public SuperCameraController cameraController; // SuperCameraController ����
    public Animator animator;                      // �ִϸ����� ����
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
    private Transform cameraTransform; // ī�޶� Transform ���� �߰�

    private int attackPhase = 0;
    public bool canAttack = true; // ���� ���� ����
    public bool canDive = true;
    public bool canCrouched = true;

    private float attackDelay = 1f; // �� ���� ������ ������

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

        //���̺� ������ ������ ����
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
        //�ɱ�
        if(canCrouched && Input.GetKeyDown(KeyCode.LeftControl) && isGround && !isAttacking && !isDive)
        {
            HandleCrouched();
        }

        if(canDive)
        {
            //ī�޶� �����϶��� �ƴҶ��� ������ ����
            if (cameraController.IsLockedOn && cameraController.LockedTarget != null)
            {
                if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetAxisRaw("Horizontal") < 0 && !isAttacking && isGround)
                {
                    HandleDive_left();
                    Debug.Log("�·� ������!");
                }

                if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetAxisRaw("Horizontal") > 0 && !isAttacking && isGround)
                {
                    HandleDive_Right();
                    Debug.Log("��� ������!");
                }

                if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") > 0 && !isAttacking && isGround)
                {
                    HandleDive_Forward();
                    Debug.Log("�շ� ������!");
                }

                if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") < 0 && !isAttacking && isGround)
                {
                    HandleDive_Back();
                    Debug.Log("�ڷ� ������!");
                }

                if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0 && !isAttacking && isGround)
                {
                    HandleStep_Back();
                    Debug.Log("�齺��!");
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetAxisRaw("Horizontal") < 0 && !isAttacking && isGround)
                {
                    HandleDive_Forward();
                    Debug.Log("������!");
                }

                if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetAxisRaw("Horizontal") > 0 && !isAttacking && isGround)
                {
                    HandleDive_Forward();
                    Debug.Log("������!");
                }

                if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") > 0 && !isAttacking && isGround)
                {
                    HandleDive_Forward();
                    Debug.Log("������!");
                }

                if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") < 0 && !isAttacking && isGround)
                {
                    HandleDive_Forward();
                    Debug.Log("������!");
                }

                if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0 && !isAttacking && isGround)
                {
                    HandleStep_Back();
                    Debug.Log("�齺��!");
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
            Debug.Log("�ɾƾ���");
            isStand = false;
        }
        else if(isStand == false)
        {
            Debug.Log("�Ͼ����");
            isStand = true;
        }

        animator.SetBool("isCrouching",!isStand);

        StartCoroutine(EnableNextCrouchedAfterDelay());
    }

    private IEnumerator EnableNextCrouchedAfterDelay()
    {
        canCrouched = false;
        Debug.Log("�ɱ� ��Ÿ��");
        yield return new WaitForSeconds(0.2f);
        Debug.Log("�ɱ� ��Ÿ�� ��!");
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

        Debug.Log("�齺�� ��Ÿ��");
        yield return new WaitForSeconds(0.7f);
        Debug.Log("�齺�� ��Ÿ�� ��!");
        canDive = true;
        currentState = State.IDLE;
    }

    private IEnumerator BackStepDirection()
    {
        isStand = true;
        float startTime = Time.time;
        while (Time.time < startTime + 0.7f)                                //�ִϸ��̼� �ð�
        {
            //������ �����ð� ����
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
        Debug.Log("�ִϸ��̼� ��!");
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

        Debug.Log("���̺� ��Ÿ��");
        yield return new WaitForSeconds(1.3f);
        Debug.Log("���̺� ��Ÿ�� ��!");
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
            
            //���̺� �����ð�
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


            // ���̺� �ִϸ��̼��� ���� ���� �� �̵�
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
        Debug.Log("�ִϸ��̼� ��!");
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
                // ���� ���� ���� �߰� ���� �ʿ� ����
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
        //�������� ���ϴ� ���� ����
        if (currentState == State.JUMP || currentState == State.DIVE || currentState ==State.ATTACK || currentState == State.HIT || currentState == State.DIE)
        {
            return;
        }

        //������ ����
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
            animator.SetBool("isJumping", true); // ���� �ִϸ��̼�
            currentState = State.IDLE;
        }
        
    }

    private void HandleAttack()
    {
        currentState = State.ATTACK;
        isAttacking = true;
        canAttack = false; // ���� ���� �÷��׸� false�� ����
        attackPhase++; // ���� �ܰ� ����

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
        StartCoroutine(EnableNextAttackAfterDelay()); // ���� ���� ���

        if (resetPhaseCoroutine != null)
        {
            StopCoroutine(resetPhaseCoroutine);
        }

        resetPhaseCoroutine = StartCoroutine(ResetAttackPhaseAfterDelay());
    }

    private IEnumerator PerformAttackMovement()
    {
        // �ִϸ��̼��� Ư�� �ð� ���� �̵�
        float attackAnimationDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        float startTime = 0f;

        while (startTime < attackAnimationDuration)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("SwordAttack_1"))
            {
                if(startTime > 0.5f && startTime < 0.8f)                                        //���� ���� �ð�
                {
                    isAttackHit = true;
                }

                else
                {
                    isAttackHit = false;
                }
            }
            // ���� �ִϸ��̼��� ���� ���� �� �̵�
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("SwordAttack_2"))
            {
                transform.Translate(Vector3.forward * 0.3f * Time.deltaTime);

                if (startTime > 0.666f && startTime < 1f)                                        //���� ���� �ð�
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

                if (startTime > 0.666f && startTime < 1f)                                        //���� ���� �ð�
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
        //���� �ܼ� �� ������ ����
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
        canAttack = true; // �ٽ� ���� ��������

        if (attackPhase >= 3) // ������ �Ϸ�� ���
        {
            attackPhase = 0; // ���� �ܰ� �ʱ�ȭ
            currentState = State.IDLE; // IDLE�� ���ư�
        }
        currentState = State.IDLE;
    }

    private IEnumerator ResetAttackPhaseAfterDelay()
    {
        yield return new WaitForSeconds(1.7f); // �������� ���� ���� ���
        if (attackPhase > 0) // ���� �ܰ谡 0�� �ƴϸ�
        {
            attackPhase = 0; // ���� �ܰ� �ʱ�ȭ
            Debug.Log("�����ʱ�ȭ!");
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
                Debug.Log("���￡�� �¾Ҵ�!");
                isAttacked = true;
                HandleHit();
            }
        }
    }

    private void HandleHit()
    {
        Debug.Log("10������!");
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

            //�ǰ� �����ð�
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