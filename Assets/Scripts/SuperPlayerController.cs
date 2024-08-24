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

    public SuperCameraController cameraController; // SuperCameraController ����
    public Animator animator;                      // �ִϸ����� ����
    private Rigidbody rb;

    public bool isGround = true;
    public bool isMoving = false;

    private enum State
    {
        IDLE,
        MOVE,
        JUMP,
        ATTACK
    }

    private State currentState = State.IDLE;
    private Transform cameraTransform; // ī�޶� Transform ���� �߰�

    private int attackPhase = 0;
    public bool canAttack = true; // ���� ���� ����
    private float attackDelay = 0.3f; // �� ���� ������ ������

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
                // ���� ���� ���� �߰� ���� �ʿ� ����
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
            animator.SetBool("isJumping", true); // ���� �ִϸ��̼�
            currentState = State.IDLE;
        }
    }

    private void HandleAttack()
    {
        canAttack = false; // ���� ���� �÷��׸� false�� ����
        attackPhase++; // ���� �ܰ� ����

        switch (attackPhase)
        {
            case 1:
                animator.SetTrigger("SwordAttack1");
                break;
            case 2:
                animator.SetTrigger("SwordAttack2");
                break;
            case 3:
                animator.SetTrigger("SwordAttack3");
                break;
            default:
                attackPhase = 0; // ���� �ܰ� ����
                currentState = State.IDLE; // IDLE ���·� ���ư�
                StartCoroutine(ResetAttackPhaseAfterDelay());
                return;
        }

        StartCoroutine(EnableNextAttackAfterDelay()); // ���� ���� ���
    }

    private IEnumerator EnableNextAttackAfterDelay()
    {
        yield return new WaitForSeconds(attackDelay);
        canAttack = true; // �ٽ� ���� ��������

        if (attackPhase >= 3) // ������ �Ϸ�� ���
        {
            attackPhase = 0; // ���� �ܰ� �ʱ�ȭ
            currentState = State.IDLE; // IDLE�� ���ư�
        }
    }

    private IEnumerator ResetAttackPhaseAfterDelay()
    {
        yield return new WaitForSeconds(attackDelay);
        attackPhase = 0; // ���� �ܰ踦 ����
        currentState = State.IDLE; // IDLE ���·� ���ư�
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




/*
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

    public SuperCameraController cameraController; // SuperCameraController ����
    public Animator animator;                      // �ִϸ����� ����
    private Rigidbody rb;

    public bool isGround = true;
    public bool isMoving = false;

    private enum State
    {
        IDLE,
        MOVE,
        JUMP,
        ATTACK
    }

    private State currentState = State.IDLE;
    private Transform cameraTransform; // ī�޶� Transform ���� �߰�

    private int attackPhase = 0;
    public bool canAttack = true; // ���� ���� ����
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
                // ���� ���� ���� �߰� ���� �ʿ� ����
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
            animator.SetBool("isJumping", true); // ���� �ִϸ��̼�
            currentState = State.IDLE;
        }
    }

    private void HandleAttack()
    {
        canAttack = false; // ���� ���� �÷��׸� false�� ����
        attackPhase++; // ���� �ܰ� ����

        switch (attackPhase)
        {
            case 1:
                animator.SetTrigger("SwordAttack1");
                break;
            case 2:
                animator.SetTrigger("SwordAttack2");
                break;
            case 3:
                animator.SetTrigger("SwordAttack3");
                break;
            default:
                attackPhase = 0; // ���� �ܰ� ����
                currentState = State.IDLE; // IDLE ���·� ���ư�
                StartCoroutine(ResetAttackPhaseAfterDelay());
                return;
        }

        StartCoroutine(EnableNextAttackAfterDelay()); // ���� ���� ���
    }

    private IEnumerator EnableNextAttackAfterDelay()
    {
        yield return new WaitForSeconds(attackDelay);
        canAttack = true; // �ٽ� ���� ��������

        if (attackPhase >= 3) // ������ �Ϸ�� ���
        {
            attackPhase = 0; // ���� �ܰ� �ʱ�ȭ
            currentState = State.IDLE; // IDLE�� ���ư�
        }
    }

    private IEnumerator ResetAttackPhaseAfterDelay()
    {
        yield return new WaitForSeconds(attackDelay);
        attackPhase = 0; // ���� �ܰ踦 ����
        currentState = State.IDLE; // IDLE ���·� ���ư�
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
*/