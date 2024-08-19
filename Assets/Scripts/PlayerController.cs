using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float smoothRotationTime = 0.3f;    //�÷��̾� ȸ���ӵ� ����ȭ ���
    private float smoothMoveTime = 0.15f;       //�÷��̾� �̵��ӵ� ����ȭ ���
    private float moveSpeed = 5;                //�÷��̾� �̵��ӵ�

    private float rotationVelocity;     //�÷��̾� ȸ���ӵ�
    private float speedVelocity;        //�÷��̾� �̵��ӵ�
    private float currentSpeed;         //�÷��̾� ����ӵ�
    private float targetSpeed;          //�÷��̾� ��ǥ�ӵ�

    private float jumpForce = 5.0f;     //�÷��̾� ������
    private bool isGround = true;

    public bool isDead = false;
    public bool isMoving = false;
    public bool isJumping = false;

    private Transform cameraTransform;
    private Rigidbody playerRigidbody;
    public Animator animator;

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

    private void Move()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 inputDir = input.normalized;

        if (inputDir != Vector2.zero)      //�÷��̾� ȸ�� �� �������
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
    }
}
