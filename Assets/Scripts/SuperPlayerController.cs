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

    public float moveSpeed = 5f;                   // �̵� �ӵ�
    public float jumpForce = 3f;                   // ���� ��
    public float resetPhaseDelay = 0.5f;             // ���� ���� �ð�
    public float DiveDelay = 2.0f;

    public SuperCameraController cameraController; // SuperCameraController ����
    public Animator animator;                      // �ִϸ����� ����
    private Rigidbody rb;

    public bool isGround = true;
    public bool isMoving = false;
    public bool isAttacking = false;
    public bool isDive = false;

    private Coroutine resetPhaseCoroutine;


    private enum State
    {
        IDLE,
        MOVE,
        JUMP,
        ATTACK,
        DIVE
    }

    private State currentState = State.IDLE;
    private Transform cameraTransform; // ī�޶� Transform ���� �߰�

    private int attackPhase = 0;
    public bool canAttack = true; // ���� ���� ����
    public bool canDive = true;
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
        if(canDive)
        {
            //ī�޶� �����϶��� �ƴҶ��� ������ ����
            if (cameraController.IsLockedOn)
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
            }
        }
        
        

    }

    private void HandleDive_left()
    {
        currentState= State.DIVE;
        isDive = true;
        animator.CrossFade("LeftDive",0.1f);
        StartCoroutine(DiveDirection());
        StartCoroutine(EnableNextDiveAfterDelay());
    }

    private void HandleDive_Right()
    {
        currentState = State.DIVE;
        isDive = true;
        animator.CrossFade("RightDive",0.1f);
        StartCoroutine(DiveDirection());
        StartCoroutine(EnableNextDiveAfterDelay());
    }

    private void HandleDive_Forward()
    {
        currentState = State.DIVE;
        isDive = true;
        animator.CrossFade("ForwardDive", 0.1f);
        StartCoroutine(DiveDirection());
        StartCoroutine(EnableNextDiveAfterDelay());
    }

    private void HandleDive_Back()
    {
        currentState = State.DIVE;
        isDive = true;
        animator.CrossFade("BackDive", 0.1f);
        StartCoroutine(DiveDirection());
        StartCoroutine(EnableNextDiveAfterDelay());
    }

    private IEnumerator EnableNextDiveAfterDelay()
    {
        Debug.Log("���̺� ��Ÿ��");
        canDive = false;
        yield return new WaitForSeconds(DiveDelay);
        canDive = true; //�������� �ƴ�.
        Debug.Log("���̺� ��Ÿ�� ��!");
        currentState = State.IDLE;
    }

    private IEnumerator DiveDirection()
    {
        float attackAnimationDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        float startTime = Time.time;
        while (Time.time < startTime + attackAnimationDuration)
        {
            // ���� �ִϸ��̼��� ���� ���� �� �̵�
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("LeftDive"))
            {
                transform.Translate(Vector3.left * 5f * Time.deltaTime);
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("RightDive"))
            {
                transform.Translate(Vector3.right * 5f * Time.deltaTime);
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
        isDive = false;

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
        }
    }

    private void HandleMove()
    {

        if (currentState == State.JUMP || currentState == State.DIVE || currentState ==State.ATTACK)
        {
            return;
        }


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
        float startTime = Time.time;

        while (Time.time < startTime + attackAnimationDuration)
        {
            // ���� �ִϸ��̼��� ���� ���� �� �̵�
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("SwordAttack_2"))
            {
                transform.Translate(Vector3.forward * 0.5f * Time.deltaTime);
            }

            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("SwordAttack_3"))
            {
                transform.Translate(Vector3.forward * 1.2f * Time.deltaTime);
            }
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

        moveSpeed = 0; //������ �ӵ� ����
        yield return new WaitForSeconds(attackDelay);
        canAttack = true; // �ٽ� ���� ��������
        isAttacking = false; //�������� �ƴ�.
        moveSpeed = 5;

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
}