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

    public float totalHPRecovery = 40f; // �� ȸ���� ü��
    private float recoveryPerSecond;
    public float totalManaRecovery = 40f; // �� ȸ���� ü��
    private float recoveryManaPerSecond;
    public float nowPosion = 0;
    public float maxPosion = 2f;
    public float nowMpPosion = 0;
    public float maxMpPosion = 2f;

    public InPutBuffer inputBuffer;

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
    private bool isRunning = false;  // �÷��̾ ���� �޸��� ������ üũ�ϴ� ����
    public bool isAttackHit = false;
    public bool FirstStaminaCheck = false;
    private bool CanSave = false;

    private bool firstDropDie = true;


    private Coroutine resetPhaseCoroutine;
    private Coroutine resetS_PhaseCoroutine;

    private float keyHoldTime = 0.0f;  // LeftControl Ű�� ���� �ð��� ������ ����
    public float crouchHoldThreshold = 1.0f;  // �޸���� ��ũ������ ���� �ð� (1��)
    public Transform myHand;


    private enum State
    {
        IDLE,
        MOVE,
        JUMP,
        ATTACK,
        DIVE,
        HIT,
        DIE,
        DRINK,
        SAVE
    }

    private State currentState = State.IDLE;
    private Transform cameraTransform; // ī�޶� Transform ���� �߰�

    private int attackPhase = -1;
    private int s_attackPhase = 0;
    KeyCode bufferedInput;
    public bool canAttack = true; // ���� ���� ����
    public bool canDive = true;
    public bool canCrouched = true;
    public bool CanDrink = true;

    private float attackDelay = 1f; // �� ���� ������ ������
    private SkinnedMeshRenderer[] skinnedMeshRenderers;

    public void SetTransparency(float alpha)
    {
        foreach (var renderer in skinnedMeshRenderers)
        {
            foreach (var material in renderer.materials // �� Skinned Mesh�� ��� Material�� ��ȸ
            )
            {
                // URP/Lit���� Ȯ��
                if (alpha <= 0f)
                {
                    // Surface Type�� Transparent�� ����
                    material.SetFloat("_Surface", 1.0f);
                }
                else
                {
                    // Surface Type�� Opaque�� ����
                    material.SetFloat("_Surface", 0f);
                }

                // ���� �� ����
                Color color = material.color;
                color.a = alpha; // ���ϴ� ���� ������ ����
                material.color = color;

                // ���� ��� ����
                if (alpha < 1f) // ������ ���
                {
                    material.SetFloat("_Blend", 0.0f); // 0 = Alpha
                    material.SetInt("_ZWrite", 0); // ���� ������ ���� ZWrite ��Ȱ��ȭ
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent; // ���� ť ����
                }
                else // �������� ���
                {
                    material.SetFloat("_Blend", 1.0f); // ������ ���������� ����
                    material.SetInt("_ZWrite", 1); // ������ ������ ���� ZWrite Ȱ��ȭ
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.EnableKeyword("_SURFACE_TYPE_OPAQUE");
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry; // ������ ť ���� (2000)
                }
            }
        }
    }


    void Start()
    {
        skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        transform.position = GameManager.Instance.LoadPosition();        //���ӸŴ����� ����� ���� ��ġ�� ��Ȱ
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
        nowPosion = maxPosion;
        nowMpPosion = maxMpPosion;

        GameManager.Instance.UpdatePlayerHP(PlayerHP);
        GameManager.Instance.UpdatePlayerST(PlayerStamina);
        GameManager.Instance.UpdatePlayerMana(PlayerMana);
        
        isAttackHit = false;
        timeSinceLastDive = StaminaRegenTime;
        recoveryPerSecond = totalHPRecovery / 1.7f;
        recoveryManaPerSecond = totalManaRecovery / 1.7f;

        StandUpTime();
    }

    private void HandleEmptyDrink()
    {
        animator.Play("No_Item_Drink", 0, 0);
        CanDrink = false;
        canDive = false;
        canCrouched = false;
        canAttack = false;
        StartCoroutine(EmptyDrinkDelay());
    }

    private IEnumerator EmptyDrinkDelay()
    {
        float startTime = Time.time;
        while (Time.time < startTime + 3.6f)                                //�ִϸ��̼� �ð�
        {
            if (currentState == State.HIT)
            {
                yield break;
            }
            yield return null;
        }
        CanDrink = true;
        canDive = true;
        canCrouched = true;
        canAttack = true;
        currentState = State.IDLE;

        yield return null;
    }

    private void HandleHPDrink()
    {
        animator.Play("Item_Drink", 0, 0);
        CanDrink = false;
        canDive = false;
        canCrouched = false;
        canAttack = false;
        StartCoroutine(HPDrinkDelay());
    }

    private IEnumerator HPDrinkDelay()
    {
        float startTime = Time.time;
        while (Time.time < startTime + 1.7f)                                //�ִϸ��̼� �ð�
        {
            if (currentState == State.HIT)
            {
                yield break;
            }

            PlayerHP += recoveryPerSecond * Time.deltaTime; // �� �����Ӹ��� ü���� ������Ŵ
            PlayerHP = Mathf.Clamp(PlayerHP, 0, PlayerMaxHP); // ü���� �ִ밪���� Ŭ����

            GameManager.Instance.UpdatePlayerHP(PlayerHP);
            yield return null;
        }
        CanDrink = true;
        canDive = true;
        canCrouched = true;
        canAttack = true;
        currentState = State.IDLE;

        yield return null;
    }

    private void HandleMPDrink()
    {
        animator.Play("Item_Drink", 0, 0);
        CanDrink = false;
        canDive = false;
        canCrouched = false;
        canAttack = false;
        StartCoroutine(MPDrinkDelay());
    }

    private IEnumerator MPDrinkDelay()
    {
        float startTime = Time.time;
        while (Time.time < startTime + 1.7f)                                //�ִϸ��̼� �ð�
        {
            if (currentState == State.HIT)
            {
                yield break;
            }

            PlayerMana += recoveryManaPerSecond * Time.deltaTime; // �� �����Ӹ��� ü���� ������Ŵ
            PlayerMana = Mathf.Clamp(PlayerMana, 0, PlayerMaxMana); // ü���� �ִ밪���� Ŭ����

            GameManager.Instance.UpdatePlayerMana(PlayerMana);
            yield return null;
        }
        CanDrink = true;
        canDive = true;
        canCrouched = true;
        canAttack = true;
        currentState = State.IDLE;

        yield return null;
    }

    void Update()
    {
        if(!isGround)
        {
            timeSinceLastDive = 0;
        }
        HandleBuffer();

        GameManager.Instance.UpdatePlayerST(PlayerStamina);

        if (Input.GetKeyDown(KeyCode.P))                                //PŰ �Է½� �������� �ʱ�ȭ �׽�Ʈ ���� ��ũ��Ʈ
        {
            PlayerPrefs.DeleteAll(); // ��� PlayerPrefs ������ ����
            PlayerPrefs.Save(); // ��� ����
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && CanDrink && (currentState == State.IDLE || currentState == State.MOVE) && isStand)                                //FŰ �Է½� ���ӸŴ��� SavePosition�� ����
        {
            if (nowPosion <= 0)
            {
                currentState = State.DRINK;

                HandleEmptyDrink();
            }
            else
            {
                nowPosion -= 1;

                currentState = State.DRINK;

                HandleHPDrink();
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && CanDrink && (currentState == State.IDLE || currentState == State.MOVE) && isStand)                                //FŰ �Է½� ���ӸŴ��� SavePosition�� ����
        {
            if (nowMpPosion <= 0)
            {
                currentState = State.DRINK;

                HandleEmptyDrink();
            }
            else
            {
                nowMpPosion -= 1;

                currentState = State.DRINK;

                HandleMPDrink();
            }
        }

        if (Input.GetKeyDown(KeyCode.F) && CanSave && (currentState == State.IDLE || currentState == State.MOVE))                                //FŰ �Է½� ���ӸŴ��� SavePosition�� ����
        {
            Vector3 directionToSavePoint = (collider.transform.position - transform.position).normalized; // ���̺� ����Ʈ ����
            directionToSavePoint.y = 0; // Y�� ȸ�� ����
            Quaternion targetRotation = Quaternion.LookRotation(directionToSavePoint);
            transform.rotation = targetRotation; // �÷��̾� ȸ��

            GameManager.Instance.SavePosition(transform.position);
            Debug.Log("save!");

            string savePointName = collider.name; // ���̺� ����Ʈ �̸� ��������
            UIManager.Instance.ActivateSaveButton(savePointName);


            HandleSave();
            // ��ư Ȱ��ȭ ���� ����
            if(PlayerPrefs.GetInt(collider.name, 0) == 0)
            {
                PlayerPrefs.SetInt(savePointName, 1);  // 1�� ��ư�� Ȱ��ȭ�Ǿ����� �ǹ�
                PlayerPrefs.Save();
            }
        }

        //���� ����Ī Ȯ��
        if (currentWeaponName == "Falchion" || currentWeaponName == "Longsword")
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
        if(isStand && !isRunning)
        {
            moveSpeed = 4f;
        }
        else if(isStand && isRunning)
        {
            moveSpeed = 5.6f;
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
        if (Input.GetButtonDown("Jump") && isGround && !isAttacking && !isDive && currentState != State.HIT && currentState != State.DRINK)
        {
            currentState = State.JUMP;
        }

        //��ư Ŭ�� & ���� ��Ʈ�ѷ�
        if ((currentState == State.DIVE || currentState == State.ATTACK || currentState == State.IDLE || currentState == State.MOVE)&& isGround )
        {
            if (canDive && canAttack && inputBuffer.GetBufferedInput(out bufferedInput) && PlayerStamina >= 5f)
            {
                if (bufferedInput == KeyCode.Mouse0)
                {
                    HandleAttack();
                    Debug.Log(currentWeaponName + " �ߵ�");
                }
                else if (bufferedInput == KeyCode.Mouse1 && PlayerMana >= 10)
                {
                    HandleSpecialAttack();
                    Debug.Log(currentWeaponName + " �ߵ�");
                }

                else if (cameraController.IsLockedOn && cameraController.LockedTarget != null && PlayerStamina >= 15f)
                {
                    if (Input.GetAxisRaw("Horizontal") < 0 && !isAttacking && isGround)
                    {
                        if (bufferedInput == KeyCode.LeftShift)
                        {
                            HandleDive_left();
                            Debug.Log("�·� ������!");
                        }
                    }

                    if (Input.GetAxisRaw("Horizontal") > 0 && !isAttacking && isGround)
                    {
                        if (bufferedInput == KeyCode.LeftShift)
                        {
                            HandleDive_Right();
                            Debug.Log("��� ������!");
                        }
                    }

                    if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") > 0 && !isAttacking && isGround)
                    {
                        if (bufferedInput == KeyCode.LeftShift)
                        {
                            HandleDive_Forward();
                            Debug.Log("�շ� ������!");
                        }
                    }

                    if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") < 0 && !isAttacking && isGround)
                    {
                        if (bufferedInput == KeyCode.LeftShift)
                        {
                            HandleDive_Back();
                            Debug.Log("�ڷ� ������!");
                        }
                    }

                    if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0 && !isAttacking && isGround)
                    {
                        if (bufferedInput == KeyCode.LeftShift)
                        {
                            HandleStep_Back();
                            Debug.Log("�齺��!");
                        }
                    }
                }
                else if(PlayerStamina >= 15f)
                {
                    if (Input.GetAxisRaw("Horizontal") != 0 && !isAttacking && isGround)
                    {
                        if (bufferedInput == KeyCode.LeftShift)
                        {
                            HandleDive_Forward();
                            Debug.Log("������!");
                        }
                    }
                    if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") != 0 && !isAttacking && isGround)
                    {
                        if (bufferedInput == KeyCode.LeftShift)
                        {
                            HandleDive_Forward();
                            Debug.Log("������!");
                        }
                    }
                    if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0 && !isAttacking && isGround)
                    {
                        if (bufferedInput == KeyCode.LeftShift)
                        {
                            HandleStep_Back();
                            Debug.Log("�齺��!");
                        }
                    }
                }
            }
        }

        //�ɱ�
        if (!isMoving || cameraController.IsLockedOn)
        {
            keyHoldTime = 0.0f;

        }

        // LeftControl Ű�� ������ �� �ð� ����
        if (Input.GetKey(KeyCode.LeftControl))
        {
            keyHoldTime += Time.deltaTime;  // ���� �ð� ����
        }

        // LeftControl Ű�� ���� �� ���� ó��
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            // �޸��⸦ �����ϰ� isRunning ������ false�� ����
            if (isRunning)
            {
                animator.SetBool("isRunning", false);
                isRunning = false;
            }
            else if (keyHoldTime < crouchHoldThreshold && canCrouched && isGround && !isAttacking && !isDive)
            {
                // 1�� �̸����� ������ �� ��ũ���� ����
                HandleCrouched();
            }

            // Ű�� �� �� �ð� �ʱ�ȭ
            keyHoldTime = 0.0f;
        }

        // 1�� �̻� ������ �ְ�, isMoving�� true�̸� ���� �޸��� ���� ���� ��
        if (keyHoldTime >= crouchHoldThreshold && isMoving && !isRunning && isGround)
        {
            // �޸��� ���� ����
            animator.SetBool("isRunning", true);
            isRunning = true;
        }

        // �޸��� ���� ��, Ű�� ��� ������ �ִ��� Ȯ���Ͽ� ����
        if (isRunning && !Input.GetKey(KeyCode.LeftControl))
        {
            // Ű�� ������ ��� �޸��� ���߱�
            animator.SetBool("isRunning", false);
            isRunning = false;
        }

        if (isRunning && cameraController.IsLockedOn)
        {
            // Ű�� ������ ��� �޸��� ���߱�
            animator.SetBool("isRunning", false);
            isRunning = false;
        }

        if (isRunning)
        {
            timeSinceLastDive = 0;
            PlayerStamina -= 5f * Time.deltaTime;
            if (PlayerStamina <= 0)
            {
                PlayerStamina = 0f;
                animator.SetBool("isRunning", false);
                isRunning = false;
            }
        }

        /*
        if (isRunning &&!IsCurrentAnimation("Run"))
        {
            animator.SetBool("isRunning", false);  // �ִϸ������� isRunning�� false�� ����
            isRunning = false;  // �޸��� ���µ� false�� �ʱ�ȭ
        }*/
        HideWeapon();
    }

    bool IsCurrentAnimation(string animationName)
    {
        // �ִϸ������� ���� �ִϸ��̼� ���� ��������
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);  // ���̾� 0 ���
        return currentState.IsName(animationName);
    }

    private void HandleBuffer()
    {
        if(Input.GetMouseButtonDown(0))
        {
            inputBuffer.ReplaceBufferedInput(KeyCode.Mouse0);
        }
        if (Input.GetMouseButtonDown(1))
        {
            inputBuffer.ReplaceBufferedInput(KeyCode.Mouse1);
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            inputBuffer.ReplaceBufferedInput(KeyCode.LeftShift);
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
        if(PlayerPrefs.GetInt(collider.name, 0) == 0)
        {
            animator.Play("Save", 0, 0);

            currentState = State.SAVE;
            StartCoroutine(SaveDelay());
        }
        else
        {
            animator.Play("rest", 0, 0);

            nowPosion = maxPosion;
            nowMpPosion = maxMpPosion;
            PlayerHP = PlayerMaxHP;
            PlayerMana = PlayerMaxMana;
            PlayerStamina = PlayerMaxStamina;

            GameManager.Instance.UpdatePlayerHP(PlayerHP);
            GameManager.Instance.UpdatePlayerMana(PlayerMana);

            currentState = State.SAVE;
            StartCoroutine(RestDelay());
        }
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

    private IEnumerator RestDelay()
    {
        yield return new WaitForSeconds(2f);

        UIManager.Instance.OpenTeleport();
        UIManager.Instance.LoadSaveButtonState();

        while (true)
        {
            if (UIManager.Instance.currentState == UIManager.UIState.Game)        //�� �κ��� ���� ��������.
            {
                animator.Play("rest_end", 0, 0);
                StartCoroutine(StandUpTime());
                break;
            }

            yield return null;
        }
        yield return null;
    }

    private IEnumerator StandUpTime()
    {
        currentState = State.SAVE;
        yield return new WaitForSeconds(2.1f);
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
                CanDrink = true;
                break;
            case State.MOVE:
                HandleMove();
                CanDrink = true;
                break;
            case State.JUMP:
                HandleJump();

                HandleMove();
                break;
            case State.ATTACK:
                currentSpeed = 0;
                isMoving = false;
                break;
            case State.DIVE:
                currentSpeed = 0;
                isMoving = false;
                break;
            case State.SAVE:
                currentSpeed = 0;
                isMoving = false;
                break;
            case State.HIT:
                currentSpeed = 0;
                isMoving = false;
                break;
            case State.DRINK:
                currentSpeed = 0;
                isMoving = false;
                break;
        }
    }

    private void HideWeapon()
    {
        Transform nowWeapon = myHand.transform.Find(currentWeapon + "_Instance");
        if(nowWeapon == null)
        {
            return;
        }
        bool FirstCheck = true;
        if (currentState == State.DRINK)
        {
            nowWeapon.gameObject.SetActive(false);
            FirstCheck = true;
        }
        else if(FirstCheck == true)
        {
            FirstCheck = false;
            nowWeapon.gameObject.SetActive(true);
        }
    }

    private void HandleMove()
    {
        //�������� ���ϴ� ���� ����
        if (currentState == State.SAVE || currentState == State.DIVE || currentState == State.ATTACK || currentState == State.HIT || currentState == State.DIE)
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
        if (!isStand)
        {
            isStand = true;
            animator.SetBool("isCrouching", !isStand);
            currentState = State.IDLE;

            return;
        }

        if (isGround && PlayerStamina > 10f && currentState != State.DRINK)
        {
            PlayerStamina -= 10f;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGround = false;
            animator.SetBool("isJumping", !isGround);
            animator.CrossFade("Jump", 0.1f, 0, 0);
        }

    }


    private void HandleAttack()
    {
        PlayerStamina -= 5f;
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
                else if(currentWeaponName == "Longsword")
                {
                    animator.CrossFade(currentWeaponName + "Attack_0", 0.1f);
                    PlayerDamage = (PlayerAtk / 3) * 4;
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
                else if (currentWeaponName == "Longsword")
                {
                    PlayerDamage = (PlayerAtk / 3) * 4;
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
                else if (currentWeaponName == "Longsword")
                {
                    PlayerDamage = (PlayerAtk / 3) * 6;
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
                else if (currentWeaponName == "Longsword")
                {
                    PlayerDamage = (PlayerAtk / 3) * 6;
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
        PlayerStamina -= 5f;
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
                else if (currentWeaponSkill == "Elite")
                {
                    PlayerDamage = (PlayerAtk / 3) * 15;
                    PlayerMana -= 20; // 1Ÿ ���� �Ҹ�
                }
                else if (currentWeaponSkill == "Blood")
                {
                    PlayerDamage = (PlayerAtk / 3) * 12;
                    PlayerMana -= 12; // 1Ÿ ���� �Ҹ�
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
                else if (currentWeaponSkill == "Elite")
                {
                    PlayerDamage = (PlayerAtk / 3) * 15;
                    PlayerMana -= 20; // 1Ÿ ���� �Ҹ�
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
        else if (currentWeaponName == "Longsword")
        {
            if (attackPhase == 3)
            {
                attackDelay = 1.8f;
            }
            else
            {
                attackDelay = 1f;
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
        else if (currentWeaponSkill == "Elite")
        {
            attackDelay = 1.9f;
        }
        else if (currentWeaponSkill == "Blood")
        {
            attackDelay = 2.4f;
        }
        float startTime = Time.time;
        while (Time.time < startTime + attackDelay)     //����(�޺� ������)
        {
            if (currentWeaponSkill == "Rot")
            {
                transform.Translate(Vector3.forward * 2f * Time.deltaTime);
            }

            bool firstcheck = false;
            if (currentWeaponSkill == "Elite" && s_attackPhase == 2 && Time.time < startTime + 0.333f)
            {
                Transform Weapon_ = myHand.transform.Find(currentWeapon + "_Instance");

                SetTransparency(0f);
                transform.Translate(Vector3.forward * 9f * Time.deltaTime);
                Weapon_.gameObject.SetActive(false);
                isinvincibility = true;
            }
            else if(!firstcheck && s_attackPhase == 2)
            {
                firstcheck = true;
                Transform Weapon_ = myHand.transform.Find(currentWeapon + "_Instance");
                SetTransparency(1f);
                Weapon_.gameObject.SetActive(true);
                isinvincibility = false;
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
        else if (currentWeaponName == "Longsword")
        {
            resetPhaseDelay = 1.4f;
        }
        float startTime = Time.time;
        while (Time.time < startTime + resetPhaseDelay)     //����(�޺� ������)
        {
            timeSinceLastDive = 0f;

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
        else if (currentWeaponSkill == "Elite")
        {
            resetPhaseDelay = 2.4f;
        }
        else if (currentWeaponSkill == "Blood")
        {
            resetPhaseDelay = 1f;
        }
        float startTime = Time.time;
        while (Time.time < startTime + resetPhaseDelay)     //����(�޺� ������)
        {
            timeSinceLastDive = 0f;

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
    public void StartCanMove()
    {
        isinvincibility = false;
        currentState = State.IDLE;
    }
    public void StartCantMove()
    {
        isinvincibility = true;
        currentState = State.SAVE;
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
            collider = other;
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
        GameManager.Instance.UpdatePlayerHP(PlayerHP);      //������ UIǥ��

        if (GetDamage < 29)
        {
            isAttackHit = false;                                //�������� ���

            animator.CrossFade("Hit", 0.1f, 0, 0);
            attackPhase = -1;                                   //���������� �ʱ�ȭ
            s_attackPhase = 0;

            StartCoroutine(AttackedMotionDelay(0.8f, 0.7f));
        }

        else if (other_hit.gameObject.name == "SquakeSkill(Clone)")
        {
            isAttackHit = false;                                //�������� ���

            animator.CrossFade("Hit_Up", 0.1f, 0, 0);
            attackPhase = -1;                                   //���������� �ʱ�ȭ
            s_attackPhase = 0;

            StartCoroutine(AttackedMotionDelay(3.5f, 2f));
        }

        else
        {
            Vector3 hitDirection = other_hit.transform.position - transform.position;

            if (hitDirection.z > 0) // �տ��� ���� ���
            {
                isAttackHit = false;                                //�������� ���

                animator.CrossFade("Hit_F", 0.1f, 0, 0);
                attackPhase = -1;                                   //���������� �ʱ�ȭ
                s_attackPhase = 0;

                StartCoroutine(AttackedMotionDelay(3f, 1.4f));
            }
            else
            {
                isAttackHit = false;                                //�������� ���

                animator.CrossFade("Hit_B", 0.1f, 0, 0);
                attackPhase = -1;                                   //���������� �ʱ�ȭ
                s_attackPhase = 0;

                StartCoroutine(AttackedMotionDelay(3f, 1.4f));
            }
        }

    }

    private IEnumerator AttackedMotionDelay(float stunTime, float stunTime_)
    {
        isAttacked = false;
        float attackAnimationDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        float startTime = Time.time;
        bool firstcheck = false;

        animator.SetBool("isAttacked", isAttacked);
        while (Time.time < startTime + stunTime)
        {
            currentSpeed = 0f;

            if (isAttacked || currentState == State.DIVE || currentState == State.ATTACK || currentState == State.JUMP)
            {
                yield break;
            }
            isAttackHit = false;
            //�ǰ� ��������
            if (Time.time <= startTime + stunTime_)
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
                if (!firstcheck)
                {
                    firstcheck = true;

                    isinvincibility = false;
                    currentState = State.IDLE;

                    canAttack = true;
                    canDive = true;
                }
            }

            yield return null;
        }


        isGround = true;
        canCrouched = true;


        animator.SetBool("isCrouching", !isStand);


    }
}