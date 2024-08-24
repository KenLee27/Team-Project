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

        
        Slash_Attack();
        if (!isAttacking) 
        {
            Move();
            Jump();
        }
    }

    private void Slash_Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(ComboAction());
        }

        animator.SetBool("isAttacking", isAttacking);
        animator.SetBool("isComboAttacking", isComboAttacking);
    }
    private IEnumerator ComboAction()
    {
        isAttacking = true;
        animator.SetTrigger("SwordAttack1");
        yield return new WaitForSeconds(1.2f);
        
        isAttacking = false;
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
