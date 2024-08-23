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
    private bool isGround = true;

    public bool isDead = false;
    public bool isMoving = false;
    public bool isJumping = false;

    private Transform cameraTransform;
    private Rigidbody playerRigidbody;
    public Animator animator;
    public BetterCameraController cameraController;

    void Start()
    {
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

    private void Slash_Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
        }
    }

    private void Move()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 inputDir = input.normalized;

        if (cameraController.IsLockedOn && cameraController.LockedTarget != null)
        {
            // Lock On 상태일 때, 플레이어가 적을 향하도록 방향 설정
            Vector3 toTarget = (cameraController.LockedTarget.position - transform.position).normalized;

            float targetRotation = Mathf.Atan2(toTarget.x, toTarget.z) * Mathf.Rad2Deg;
            float newRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, smoothRotationTime);
            transform.eulerAngles = Vector3.up * newRotation;

            // 플레이어가 양옆, 앞뒤로도 움직일 수 있도록 함
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
            // 기본적인 자유 이동
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

    /*
    private void Move()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 inputDir = input.normalized;

        if (inputDir != Vector2.zero)      //플레이어 회전 후 방향고정
        {
            isMoving = true;

            float rotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, rotation, ref rotationVelocity, smoothRotationTime);
        }
        else
        {
            isMoving = false;
        }

        animator.SetBool("isMoving", isMoving);
        
        targetSpeed = moveSpeed * inputDir.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedVelocity, smoothMoveTime);
        transform.Translate(transform.forward * currentSpeed * Time.deltaTime, Space.World);
    }
    */

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
