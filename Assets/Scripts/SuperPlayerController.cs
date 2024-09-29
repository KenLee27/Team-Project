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
    private float timeSinceLastDive;

    public string currentWeapon;                  //���⸦ ����
    public string currentWeaponName;              //������ �̸��� ����
    public string currentWeaponSkill;             //������ ��ų�� ����
    public void SetCurrentWeaponType(string weaponType)
    {
        currentWeapon = weaponType;

        // ����ٷ� �����Ͽ� ���� �̸��� ���� ��ų ����
        string[] parts = currentWeapon.Split('_'); // ����ٷ� ���ڿ� �и�

        if (parts.Length == 2) // �� �κ��� �ִ��� Ȯ��
        {
            currentWeaponName = parts[0];        // ù ��° �κ��� ���� �̸�
            currentWeaponSkill = parts[1];       // �� ��° �κ��� ���� ��ų
        }
        else
        {
            // ���� ó��: ��ȿ���� ���� ����
            currentWeaponName = string.Empty;
            currentWeaponSkill = string.Empty;
            Debug.Log("Invalid weapon type format.");
        }
    }

    public float moveSpeed = 4f;                   // �̵� �ӵ�
    public float jumpForce = 5f;                   // ���� ��
    public float resetPhaseDelay = 1.2f;             // ���� ���� �ð�
    public float DiveDelay = 0.8f;                 // ���̺� ��Ÿ��
    public float PlayerHP = 0f;
    public float PlayerAtk = 3f;    
    public float PlayerMaxHP = 100f;
    public float PlayerStamina = 0f;
    public float PlayerMaxStamina = 100f;
    public float StaminaRegenTime = 0.1f;          //���׹̳� ȸ���� ���� ���׹̳� �Ҹ� ���߰� ��ٷ��� �ϴ� �ð�
    public float StaminaRegenSpeed = 20f;          //���׹̳� �ʴ� ȸ�� ��ġ
    public float PlayerMana = 0f; // ���� ����
    public float PlayerMaxMana = 100f; // �ִ� ����
    public float playerSoul = 0f;


    private Vector2 velocity = Vector2.zero;

    public float PlayerDamage = 3f;                //�÷��̾� ������

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
    public bool FirstStaminaCheck = false;
    private bool CanSave = false;

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
        DIE,
        SAVE
    }

    private State currentState = State.IDLE;
    private Transform cameraTransform; // ī�޶� Transform ���� �߰�

    private int attackPhase = -1;
    private int s_attackPhase = 0;

    public bool canAttack = true; // ���� ���� ����
    public bool canDive = true;
    public bool canCrouched = true;

    private float attackDelay = 1f; // �� ���� ������ ������


    void Start()
    {
        transform.position = GameManager.Instance.spawnPosition;        //���ӸŴ����� ����� ���� ��ġ�� ��Ȱ

        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        cameraTransform = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isinvincibility = false;
        firstDropDie = true;

        PlayerMana = PlayerMaxMana;
        PlayerHP = PlayerMaxHP;
        PlayerStamina = PlayerMaxStamina;

        GameManager.Instance.UpdatePlayerHP(PlayerHP);
        GameManager.Instance.UpdatePlayerST(PlayerStamina);
        GameManager.Instance.UpdatePlayerMana(PlayerMana);
        
        isAttackHit = false;
        timeSinceLastDive = StaminaRegenTime;
    }

    void Update()
    {
        GameManager.Instance.UpdatePlayerST(PlayerStamina);

        if (Input.GetKeyDown(KeyCode.P))                                //PŰ �Է½� �������� �ʱ�ȭ �׽�Ʈ ���� ��ũ��Ʈ
        {
            PlayerPrefs.DeleteAll(); // ��� PlayerPrefs ������ ����
            PlayerPrefs.Save(); // ��� ����
        }

        if (Input.GetKeyDown(KeyCode.F) && CanSave && (currentState == State.IDLE || currentState == State.MOVE))                                //FŰ �Է½� ���ӸŴ��� SavePosition�� ����
        {
            Vector3 directionToSavePoint = (collider.transform.position - transform.position).normalized; // ���̺� ����Ʈ ����
            directionToSavePoint.y = 0; // Y�� ȸ�� ����
            Quaternion targetRotation = Quaternion.LookRotation(directionToSavePoint);
            transform.rotation = targetRotation; // �÷��̾� ȸ��

            GameManager.Instance.SavePosition(transform.position);
            Debug.Log("save!");

            HandleSave();
        }

        //���� ����Ī Ȯ��
        if (currentWeaponName == "Falchion")
        {
            animator.SetFloat("Blend", 0f);
        }
        else if (currentWeaponName == "Axe")
        {
            animator.SetFloat("Blend", 0.5f);
        }
        else if (currentWeaponName == "Dagger")
        {
            animator.SetFloat ("Blend", 1f);
        }

        //�÷��̾� ���׹̳� ��Ʈ�ѷ�
        if ( PlayerStamina < 15f )
        {
            canDive = false;
            FirstStaminaCheck = true;
        }
        else if( PlayerStamina >=15 && FirstStaminaCheck)
        {
            canDive = true;
            FirstStaminaCheck = false;
        }

        //ȸ�� ���� �� 1�ʰ� ������ ���׹̳��� ȸ�� ����.
        if (timeSinceLastDive >= StaminaRegenTime)
        {
            RecoverStamina();
        }
        else
        {
            timeSinceLastDive += Time.deltaTime;
        }

        //��� ��Ʈ�ѷ�
        if (PlayerHP <= 0)
        {
            HandleDead();
        }

        //������ ���� �ɾ� ���� ���� �ӵ� ��Ʈ�ѷ�
        if(isStand)
        {

        }
        else if(!isStand)
        {
            moveSpeed = 2.5f;
        }

        //���� �����϶� �� �ٶ󺸱� ���
        if (cameraController.IsLockedOn && cameraController.LockedTarget != null)
        {
            RotateTowardsEnemy();
        }

        HandleState();

        //���� ��Ʈ�ѷ�
        if (Input.GetButtonDown("Jump") && isGround &&!isAttacking&&!isDive&& currentState != State.HIT)
        {
            currentState = State.JUMP;
        }

        //��ư Ŭ�� & ���� ��Ʈ�ѷ�
        if (Input.GetMouseButtonDown(0) && canAttack && (currentState == State.ATTACK || currentState == State.IDLE || currentState == State.MOVE)&&!isDive && isGround)
        {
            HandleAttack();
            Debug.Log(currentWeaponName + "�ߵ�");
        }

        if (Input.GetMouseButtonDown(1) && canAttack && (currentState == State.ATTACK || currentState == State.IDLE || currentState == State.MOVE) && !isDive && isGround&&PlayerMana >= 10)
        {
            HandleSpecialAttack();
            Debug.Log(currentWeaponName + "�ߵ�");
        }
        //�ɱ�
        if (canCrouched && Input.GetKeyDown(KeyCode.LeftControl) && isGround && !isAttacking && !isDive)
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
                if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetAxisRaw("Horizontal") != 0 && !isAttacking && isGround)
                {
                    HandleDive_Forward();
                    Debug.Log("������!");
                }
                if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") != 0 && !isAttacking && isGround)
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

    private void HandleSave()
    {
        isinvincibility = true;
        canAttack = false;
        canDive = false;
        canCrouched = false;
        CanSave = false;
        //GameManager.Instance.UpdatePlayerHP(PlayerHP);
        animator.Play("Save",0,0);


        currentState = State.SAVE;
        StartCoroutine(SaveDelay());
    }

    private IEnumerator SaveDelay()
    {
        yield return new WaitForSeconds(4f);
        isinvincibility = false;
        canAttack = true;
        canDive = true;
        canCrouched = true;
        CanSave = true;
        currentState = State.IDLE;
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
        PlayerStamina -= 15f;
        isStand = true;
        float startTime = Time.time;
        while (Time.time < startTime + 0.7f)                                //�ִϸ��̼� �ð�
        {
            transform.Translate(Vector3.back * 3f * Time.deltaTime);

            //������ ���� �߿� ���׹̳� ȸ�� ��� �ð� 0�� ����
            timeSinceLastDive = 0f;
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
        yield return new WaitForSeconds(DiveDelay);
        Debug.Log("���̺� ��Ÿ�� ��!");
        canDive = true;
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
        PlayerStamina -= 15f;
        isStand = true;
        canAttack = false;

        float attackAnimationDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        float startTime = Time.time;
        while (Time.time < startTime + 0.7f)        //���̺� �ð�
        {
            //���̺� �ִϸ��̼��� ���� ���� �� ���׹̳� ȸ�� ��� �ð� 0�� ����
            timeSinceLastDive = 0f;
            yield return null;
        }
        animator.SetBool("isCrouching", !isStand);
        currentState = State.IDLE;
        isDive = false;
        isMoving = true;
        canAttack = true;
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

                HandleMove();
                break;
            case State.ATTACK:
                currentSpeed = 0;
                break;
            case State.DIVE:
                currentSpeed = 0;
                break;
            case State.SAVE:
                currentSpeed = 0;
                break;
        }
    }

    private void HandleMove()
    {
        //�������� ���ϴ� ���� ����
        if (currentState == State.SAVE ||currentState == State.DIVE || currentState ==State.ATTACK || currentState == State.HIT || currentState == State.DIE)
        {
            return;
        }

        //������ ����
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
            animator.SetBool("isJumping", !isGround);
            animator.CrossFade("Jump", 0.1f, 0, 0);
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
            case 0:
                if(currentWeaponName == "Axe")
                {
                    animator.CrossFade(currentWeaponName + "Attack_0", 0.1f);
                    PlayerDamage = (PlayerAtk / 3) * 5;
                    break;
                }
                else
                {
                    attackPhase++;
                }
                goto case 1;
            case 1:
                animator.CrossFade(currentWeaponName+"Attack_1", 0.1f);
                if (currentWeaponName == "Dagger")
                {
                    PlayerDamage = (PlayerAtk / 3) * 2;
                }
                else if (currentWeaponName == "Axe")
                {
                    PlayerDamage = (PlayerAtk / 3) * 5;
                }
                else if (currentWeaponName == "Falchion")
                {
                    PlayerDamage = PlayerAtk;
                }
                break;
            case 2:
                animator.CrossFade(currentWeaponName + "Attack_2", 0.1f);
                if (currentWeaponName == "Dagger")
                {
                    PlayerDamage = PlayerAtk;
                }
                else if (currentWeaponName == "Axe")
                {
                    PlayerDamage = PlayerAtk * 2;
                }
                else if (currentWeaponName == "Falchion")
                {
                    PlayerDamage = (PlayerAtk / 3) * 4;
                }
                break;
            case 3:
                animator.CrossFade(currentWeaponName + "Attack_3", 0.1f);
                if (currentWeaponName == "Dagger")
                {
                    PlayerDamage = (PlayerAtk / 3) * 5;
                }
                else if (currentWeaponName == "Axe")
                {
                    PlayerDamage = PlayerAtk * 3;
                }
                else if (currentWeaponName == "Falchion")
                {
                    PlayerDamage = PlayerAtk * 2;
                }
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
    private void HandleSpecialAttack()
    {
        currentState = State.ATTACK;
        isAttacking = true;
        canAttack = false; // ���� ���� �÷��׸� false�� ����

        isStand = true;
        animator.SetBool("isCrouching", !isStand); //���� �� ���ִ� �ڼ�


        s_attackPhase++; // ���� �ܰ� ����

        switch (s_attackPhase)
        {
            case 1:
                animator.CrossFade(currentWeaponSkill + "SpecialAttack_1", 0.1f,0,0);
                
                if (currentWeaponSkill == "Red")
                {
                    PlayerDamage = PlayerAtk * 2;
                    PlayerMana -= 10; // 1Ÿ ���� �Ҹ�
                }
                else if (currentWeaponSkill == "Exe")
                {
                    PlayerDamage = PlayerAtk * 4;
                    PlayerMana -= 10; // 1Ÿ ���� �Ҹ�
                }
                else if (currentWeaponSkill == "Cross")
                {
                    PlayerDamage = (PlayerAtk / 3) * 8;
                    PlayerMana -= 25; // 1Ÿ ���� �Ҹ�
                }
                else if (currentWeaponSkill == "Rot")
                {
                    PlayerDamage = (PlayerAtk / 3) * 3;
                    PlayerMana -= 30; // 1Ÿ ���� �Ҹ�
                }
                GameManager.Instance.UpdatePlayerMana(PlayerMana); // ���� UI ������Ʈ
                break;
            case 2:
                animator.CrossFade(currentWeaponSkill + "SpecialAttack_2", 0.1f,0,0);
                
                if (currentWeaponSkill == "Red")
                {
                    PlayerDamage = PlayerAtk * 2;
                    PlayerMana -= 10; // 2Ÿ ���� �Ҹ�
                    Debug.Log(PlayerDamage + " ������!");
                }
                else if (currentWeaponSkill == "Exe")
                {
                    PlayerDamage = PlayerAtk * 4;
                    PlayerMana -= 15; // 2Ÿ ���� �Ҹ�
                    Debug.Log(PlayerDamage + " ������!");
                }
                else if (currentWeaponSkill == "Cross")
                {
                    PlayerDamage = (PlayerAtk / 3) * 11;
                    PlayerMana -= 15; // 2Ÿ ���� �Ҹ�
                    Debug.Log(PlayerDamage + " ������!");
                }
                GameManager.Instance.UpdatePlayerMana(PlayerMana); // ���� UI ������Ʈ
                break;
            default:
                return;
        }
        StartCoroutine(EnableNextS_AttackAfterDelay()); // ���� ���� ���

        if (resetS_PhaseCoroutine != null)
        {
            StopCoroutine(resetS_PhaseCoroutine);
        }

        resetS_PhaseCoroutine = StartCoroutine(ResetS_AttackPhaseAfterDelay());
    }

    private IEnumerator EnableNextAttackAfterDelay()
    {
        //���� �� ���� �ܼ� �� ������ ����
        if(currentWeaponName == "Falchion")
        {
            //���� �ܼ� �� ������ ����
            if (attackPhase == 3)
            {
                attackDelay = 1.8f;
            }
            else
            {
                attackDelay = 0.9f;
            }
        }
        else if (currentWeaponName == "Axe")
        {
            if (attackPhase == 3)
            {
                attackDelay = 1.8f;
            }
            else
            {
                attackDelay = 1.3f;
            }
        }
        else if (currentWeaponName == "Dagger")
        {
            //���� �ܼ� �� ������ ����
            if (attackPhase == 3)
            {
                attackDelay = 1f;
            }
            else if (attackPhase == 2)
            {
                attackDelay = 0.4f;
            }
            else if (attackPhase == 1)
            {
                attackDelay = 0.6f;
            }
        }
        float startTime = Time.time;
        while (Time.time < startTime + attackDelay)     //����(�޺� ������)
        {
            if(currentState ==State.HIT)
            {
                yield break;                            //���� ��� �ڷ�ƾ ����
            }
            yield return null;
        }


        isStand = true;
        animator.SetBool("isCrouching", !isStand); //���� �� ���ִ� �ڼ�


        Debug.Log("Can Attack!");
        canAttack = true; // �ٽ� ���� ��������
        isAttacking = false;

        if (attackPhase >= 3) // ������ �Ϸ�� ���
        {
            attackPhase = -1; // ���� �ܰ� �ʱ�ȭ
        }
    }
    private IEnumerator EnableNextS_AttackAfterDelay()
    {
        //���� �ܼ� �� ������ ����

        if (currentWeaponSkill == "Cross")
        {
            if (s_attackPhase == 2)
            {
                attackDelay = 1.8f;
            }
            else
            {
                attackDelay = 1.2f;
            }
        }
        else if (currentWeaponSkill == "Exe")
        {
            attackDelay = 2.7f;
        }
        else if (currentWeaponSkill == "Red")
        {
            attackDelay = 1.3f;
        }
        else if (currentWeaponSkill == "Rot")
        {
            attackDelay = 3.1f;
        }
        float startTime = Time.time;
        while (Time.time < startTime + attackDelay)     //����(�޺� ������)
        {
            if(currentWeaponSkill == "Rot")
            {
                transform.Translate(Vector3.forward * 2f * Time.deltaTime);
            }
            if (currentState == State.HIT)
            {
                yield break;                            //���� ��� �ڷ�ƾ ����
            }
            yield return null;
        }


        Debug.Log("you can attack!");
        isStand = true;
        animator.SetBool("isCrouching", !isStand);
        isAttacking = false;
        canAttack = true; // �ٽ� ���� ��������

        if (s_attackPhase >= 2) // ������ �Ϸ�� ���
        {
            s_attackPhase = 0; // ���� �ܰ� �ʱ�ȭ
        }
    }

    private IEnumerator ResetAttackPhaseAfterDelay()
    {
        //���� �� ���� �ʱ�ȭ �ð� ����
        if (currentWeaponName == "Falchion")
        {
            resetPhaseDelay = 1.2f;
        }
        else if (currentWeaponName == "Axe")
        {
            resetPhaseDelay = 2f;
        }
        else if (currentWeaponName == "Dagger")
        {
            resetPhaseDelay = 0.9f;
        }
        float startTime = Time.time;
        while (Time.time < startTime + resetPhaseDelay)     //����(�޺� ������)
        {
            if (currentState == State.HIT)
            {
                yield break;                            //���� ��� �ڷ�ƾ ����
            }
            yield return null;
        }


        if (attackPhase > -1) // ���� �ܰ谡 0�� �ƴϸ�
        {
            attackPhase = -1; // ���� �ܰ� �ʱ�ȭ
            Debug.Log("�����ʱ�ȭ!");
        }
    }
    private IEnumerator ResetS_AttackPhaseAfterDelay()
    {

        if (currentWeaponSkill == "Cross")
        {
            resetPhaseDelay = 2.2f;
        }
        else if (currentWeaponSkill == "Exe")
        {
            resetPhaseDelay = 2.2f;
        }
        else if (currentWeaponSkill == "Red")
        {
            resetPhaseDelay = 1.7f;
        }
        else if (currentWeaponSkill == "Rot")
        {
            resetPhaseDelay = 1.7f;
        }
        float startTime = Time.time;
        while (Time.time < startTime + resetPhaseDelay)     //����(�޺� ������)
        {
            if (currentState == State.HIT)
            {
                yield break;                            //���� ��� �ڷ�ƾ ����
            }
            yield return null;
        }


        if (s_attackPhase > 0) // ���� �ܰ谡 0�� �ƴϸ�
        {
            s_attackPhase = 0; // ���� �ܰ� �ʱ�ȭ
            Debug.Log("�����ʱ�ȭ!");
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

    public void CanMove()
    {
        currentState = State.IDLE;
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
            if(currentState == State.JUMP)
            {
                currentState = State.IDLE;
            }
        }

        if (firstDropDie&&collision.gameObject.CompareTag("DeathZone"))
        {
            PlayerHP -= PlayerMaxHP;
            GameManager.Instance.UpdatePlayerHP(PlayerHP);
            firstDropDie = false;
        }

    }
    private Collider collider;
    void OnTriggerEnter(Collider other)
    {
        collider = other;   
        if (other.CompareTag("EnemyAttack") && !isinvincibility)
        {
            isinvincibility = true;                    //�ǰ� ���� Ȱ��ȭ

            isAttacking = false;
            currentState = State.HIT;
            Debug.Log("�¾Ҵ�!");
            isAttacked = true;
            HandleHit(other);

        }

        if (other.CompareTag("SavePoint")) // �÷��̾���� �浹 ���� Ȯ��
        {
            CanSave = true;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        collider = other;

        if (other.CompareTag("SavePoint")) // �÷��̾ �ݶ��̴��� ������ ��
        {
            CanSave = false;
        }
    }


    private void HandleHit(Collider other_hit)
    {
        currentState = State.HIT;

        Ienemy enemy = other_hit.GetComponentInParent<Ienemy>();
        float GetDamage = enemy.Damage;                     //������ ���

        PlayerHP = PlayerHP - GetDamage;
        GameManager.Instance.UpdatePlayerHP(PlayerHP);      //UI ������Ʈ

        isAttackHit = false;                                //�������� ���

        animator.CrossFade("Hit", 0.1f,0,0);
        attackPhase = -1;                                   //���������� �ʱ�ȭ
        s_attackPhase = 0;

        StartCoroutine(AttackedMotionDelay());
        
    }

    private IEnumerator AttackedMotionDelay()
    {
        isAttacked = false;
        float attackAnimationDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        float startTime = Time.time;

        animator.SetBool("isAttacked", isAttacked);
        while (Time.time < startTime + 0.8f)
        {
            if (isAttacked)
            {
                yield break;
            }
            isAttackHit = false;
            //�ǰ� ��������
            if (Time.time <= startTime + 0.7f)
            {
                isinvincibility = true;
                currentState = State.HIT;
                isStand = true;
                canAttack = false;
                canDive = false;
                canCrouched = false;
            }
            else
            {
                isinvincibility = false;
            }
            
            yield return null;
        }
        
        canDive = true;
        isGround = true;
        canAttack = true;
        canCrouched = true;


        animator.SetBool("isCrouching", !isStand);

        currentState = State.IDLE;
    }

}