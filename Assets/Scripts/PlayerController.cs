using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float smoothRotationTime = 0.3f;
    private float smoothMoveTime = 0.15f;
    private float moveSpeed = 5;

    private float rotationVelocity;
    private float speedVelocity;
    private float currentSpeed;
    private float targetSpeed;

    private float jumpForce = 5.0f;

    public bool isGround = true;
    public bool isDead = false;
    public bool isMoving = false;
    public bool isJumping = false;
    public bool isAttacking = false;
    public bool isComboAttacking = false;

    private Transform cameraTransform;
    private Rigidbody playerRigidbody;
    public Animator animator;
    public SuperCameraController cameraController;

    private int attackPhase = 0;
    private bool canAttack = true; // 공격 가능 여부
    private float attackDelay = 1f; // 각 공격 사이의 딜레이

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        cameraTransform = Camera.main.transform;
        playerRigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead)
        {
            return;
        }

        HandleComboAttack();

        if (!isAttacking) // 공격 중일 때 Move와 Jump를 막음
        {
            Move();
            Jump();
        }
    }

    private void HandleComboAttack()
    {
        if (Input.GetMouseButtonDown(0) && canAttack) // canAttack이 true여야 입력을 받음
        {
            isAttacking = true;
            canAttack = false; // 공격 입력 후 비활성화
            attackPhase++;

            if (attackPhase == 1)
            {
                animator.SetTrigger("SwordAttack1");
            }
            else if (attackPhase == 2)
            {
                animator.SetTrigger("SwordAttack2");
            }
            else if (attackPhase == 3)
            {
                animator.SetTrigger("SwordAttack3");
            }

            attackPhase = Mathf.Min(attackPhase, 3);

            // 코루틴을 시작하여 일정 시간 후에 다시 입력을 받을 수 있도록 함
            StartCoroutine(ResetComboIfNoAdditionalInput());
            StartCoroutine(EnableNextAttackAfterDelay());
        }
    }

    private IEnumerator ResetComboIfNoAdditionalInput()
    {
        // 공격 후 일정시간 동안 추가 입력이 없으면 초기화
        yield return new WaitForSeconds(1);

        if (attackPhase == 1 || attackPhase == 2 || attackPhase == 3)
        {
            isAttacking = false;
            attackPhase = 0;
        }
    }

    private IEnumerator EnableNextAttackAfterDelay()
    {
        yield return new WaitForSeconds(attackDelay);
        canAttack = true; // 딜레이가 지나면 다시 공격 가능
    }

    private void Move()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 inputDir = input.normalized;

        if (cameraController.IsLockedOn && cameraController.LockedTarget != null)
        {
            Vector3 toTarget = (cameraController.LockedTarget.position - transform.position).normalized;

            float targetRotation = Mathf.Atan2(toTarget.x, toTarget.z) * Mathf.Rad2Deg;
            float newRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, smoothRotationTime);
            transform.eulerAngles = Vector3.up * newRotation;

            Vector3 moveDir = transform.forward * inputDir.y + transform.right * inputDir.x;
            targetSpeed = moveSpeed * moveDir.magnitude;

            isMoving = inputDir != Vector2.zero;

            currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedVelocity, smoothMoveTime);
            transform.Translate(moveDir.normalized * currentSpeed * Time.deltaTime, Space.World);
        }
        else
        {
            isMoving = inputDir != Vector2.zero;

            if (inputDir != Vector2.zero)
            {
                float rotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
                transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, rotation, ref rotationVelocity, smoothRotationTime);
            }

            targetSpeed = moveSpeed * inputDir.magnitude;
            currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedVelocity, smoothMoveTime);
            transform.Translate(transform.forward * currentSpeed * Time.deltaTime, Space.World);
        }

        animator.SetBool("isMoving", isMoving);
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGround)
        {
            playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGround = false;
            isJumping = true;
        }

        animator.SetBool("isJumping", isJumping);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
            isJumping = false;
        }
        else if (collision.gameObject.CompareTag("DeathZone"))
        {
            isDead = true;
            isJumping = false;
            animator.SetBool("isDead", isDead);
        }
    }
}




/*
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float smoothRotationTime = 0.3f;    //플레이어 회전속도 최적화 계수
    private float smoothMoveTime = 0.15f;       //플레이어 이동속도 최적화 계수
    private float moveSpeed = 5;                //플레이어 이동속도

    private float rotationVelocity;     //플레이어 회전속도
    private float speedVelocity;        //플레이어 이동속도
    private float currentSpeed;         //플레이어 현재속도
    private float targetSpeed;          //플레이어 목표속도

    private float jumpForce = 5.0f;     //플레이어 점프력

    public bool isGround = true;
    public bool isDead = false;
    public bool isMoving = false;
    public bool isJumping = false;
    public bool isAttacking = false;
    public bool isComboAttacking = false;

    private Transform cameraTransform;
    private Rigidbody playerRigidbody;
    public Animator animator;
    public SuperCameraController cameraController;

    void Start()
    {
        Cursor.visible = false;                     //커서 비활성화
        Cursor.lockState = CursorLockMode.Locked;   //플레이 모드에서 커서를 중앙에 고정

        cameraTransform = Camera.main.transform;
        playerRigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (isDead)
        {
            return;
        }
        Move();
        Jump();
    }

    private void Move()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 inputDir = input.normalized;

        if (cameraController.IsLockedOn && cameraController.LockedTarget != null)
        {
            Vector3 toTarget = (cameraController.LockedTarget.position - transform.position).normalized;

            float targetRotation = Mathf.Atan2(toTarget.x, toTarget.z) * Mathf.Rad2Deg;
            float newRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, smoothRotationTime);
            transform.eulerAngles = Vector3.up * newRotation;

            Vector3 moveDir = transform.forward * inputDir.y + transform.right * inputDir.x;
            targetSpeed = moveSpeed * moveDir.magnitude;

            if (inputDir != Vector2.zero)
            {
                isMoving = true;
                
            }
            else
            {
                isMoving = false;
            }

            currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedVelocity, smoothMoveTime);
            transform.Translate(moveDir.normalized * currentSpeed * Time.deltaTime, Space.World);
        }
        else
        {
            if (inputDir != Vector2.zero)
            {
                isMoving = true;
                float rotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
                transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, rotation, ref rotationVelocity, smoothRotationTime);
            }
            else
            {
                isMoving = false;
            }

            targetSpeed = moveSpeed * inputDir.magnitude;
            currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedVelocity, smoothMoveTime);
            transform.Translate(transform.forward * currentSpeed * Time.deltaTime, Space.World);
        }

        animator.SetBool("isMoving", isMoving);
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGround)
        {
            playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGround = false;
            isJumping = true;
        }

        animator.SetBool("isJumping", isJumping);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
            isJumping = false;
        }
        else if (collision.gameObject.CompareTag("DeathZone"))
        {
            isDead = true;
            isJumping = false;
            animator.SetBool("isDead", isDead);
        }
    }
}
*/
