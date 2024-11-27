using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossCController : MonoBehaviour, Ienemy
{
    public Animator anim; //�ִϸ�����
    UnityEngine.AI.NavMeshAgent nmAgent; //navmeshagent �߰�

    public GameObject player_1;

    public Transform player; //�÷��̾� Ÿ��
    public LayerMask playerLayer;
    public GameObject weaponAttackRange;         //���ݹ���


    private float HP = 0; //�� ü�� ���� �� �ʱ�ȭ
    public float MaxHP = 200;
    private float detectingRange = 29f;         //�� Ž�� �Ÿ�
    private float checkRange = 6f;             //��������Ÿ�
    private float attackRange = 5f;           //�� ���� ��Ÿ�
    private float smoothRotationSpeed = 15f;     //�� ����ȭ ȸ�� �ӵ�
    private float stunMax = 80f;
    private float stunGauge = 0f;
    private float alertSpeed = 1.8f;
    private float dashSpeed = 11f;
    private float chaseSpeed = 6f;
    public float Damage = 30f;

    private Slider hpSlider;                     // ���� HP �����̴�
    private Text bossName;
    private GameObject hpSliderObject;          // �����̴� UI ������Ʈ

    float Ienemy.Damage => Damage;

    public bool isHit = false;                  //���� ����
    public bool isAttack = false;
    private bool isAlreadyHit = false;          //�ǰ� ���� ����
    public bool is2Phase = false;
    public bool is3Phase = false;

    Vector3 directionToPlayer;
    Vector3 directionToBase;

    float distanceToPlayer = 100f;
    float distanceToBase = 0f;
    private float attackCount = 0f;
    private float attackMaxCount = 4f;


    private Quaternion targetRotation;
    private float currentSpeed;


    private bool moveRight;  // �¿� �̵� ���� ��� ���� ����
    private bool directionInitialized = false;  // ������ �ʱ�ȭ�Ǿ����� ���θ� Ȯ���ϴ� ����
    private float timeSinceLastCheck = 0f;
    private float checkInterval = 2f; // 2�ʸ��� üũ
    public float enemySoul = 150f;
    public bool isDead = false;
    private SuperPlayerController playerScript;
    Vector3 initialPoint; //�� ��ġ ��ġ ���� ����

    public Transform FinalForcedTeleporter;
    public Transform hiddenMessage1;
    public Transform hiddenMessage2;
    public Transform hiddenMessage3;
    public Transform message3;
    public Transform hiddenDoor1;
    public Transform hiddenDoor2;
    public Transform hiddenBoss;
    public Transform removeBox;


    enum State
    {
        IDLE, //�⺻
        CHASE, //�߰�
        ATTACK_1, //��Ÿ1
        ATTACK_2, //��Ÿ2 
        ATTACK_3, //��Ÿ3
        ATTACK_4, //��Ÿ4
        ATTACK_5, //��Ÿ5
        ATTACK_6, //��Ÿ6
        ATTACK_7, //��Ÿ7
        COMBO_1, //�޺�1
        COMBO_2, //�޺�2
        COMBO_3, //�޺�3
        DIVE,  //������
        STEP,  //���̵彺��
        BACK,  //��ȯ
        CHECK,  //��Ȳ����
        DIE,  //���
    }

    State state;

    public AudioSource audioSource;
    public AudioClip[] footSound;
    public AudioClip[] diveSound;
    public AudioClip[] dashSound;
    public AudioClip[] slashSound;
    public AudioClip[] airSound;
    public AudioClip[] crashSound;
    public AudioClip[] attackedSound;
    public AudioClip[] clearSound;

    void Awake()
    {
        if (PlayerPrefs.GetInt("Final", 0) == 1)
        {
            Destroy(gameObject);
            FinalForcedTeleporter.gameObject.SetActive(true);
            hiddenMessage1.gameObject.SetActive(true);
            hiddenMessage2.gameObject.SetActive(true);
            hiddenMessage3.gameObject.SetActive(true);
            message3.gameObject.SetActive(false);
            hiddenDoor1.gameObject.SetActive(false);
            hiddenDoor2.gameObject.SetActive(false);
            hiddenBoss.gameObject.SetActive(true);
            removeBox.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        if (player_1 != null)
        {
            playerScript = player_1.GetComponent<SuperPlayerController>();
        }

        initialPoint = new Vector3(transform.position.x, transform.position.y, transform.position.z);   //�� ��ġ ��ġ �ʱ�ȭ
        HP = MaxHP; //ü�� �ʱ�ȭ
        anim = GetComponent<Animator>();
        nmAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        state = State.IDLE;
        StartCoroutine(StateMachine());

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.volume = 1.0f;
        audioSource.spatialBlend = 1.0f;
        audioSource.minDistance = 1.0f;
        audioSource.maxDistance = 10.0f;
    }


    IEnumerator StateMachine()
    {
        while (true)
        {
            yield return StartCoroutine(state.ToString());
        }

    }

    //���� �� ����
    IEnumerator CHECK()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (moveRight)
        {
            anim.CrossFade("step_r", 0.1f, 0, 0);
        }
        else
        {
            anim.CrossFade("step_l", 0.1f, 0, 0);
        }
        float checkTime = 2f;
        if (!is2Phase)
        {
            checkTime = 2f;                         //���� ���� �ð�
        }
        else
        {
            checkTime = 0.5f;                         //���� ���� �ð�
        }

        float elapsedTime = 0f;

        bool firstDiveCheck = false;
        while (elapsedTime < checkTime)
        {

            //�������
            if (HP <= 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            if (distanceToPlayer <= attackRange && playerScript.currentState == SuperPlayerController.State.ATTACK && !firstDiveCheck)
            {
                firstDiveCheck = true;
                float randomDive = UnityEngine.Random.Range(0f, 10f); //�̹� ���� ������ ����
                if (randomDive < 4f)  //40% Ȯ��, ������ �󵵸� �ø��� �ʹٸ� ���� Ȯ���� ����.
                {
                    yield return new WaitForSecondsRealtime(0.2f); //������ ���� ��ٸ� �ð�;
                    ChangeState(State.DIVE); //������.
                    yield break;
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ��ǥ���� �Ÿ��� �����Ÿ� ���� �־��� ���
        if (attackRange < distanceToPlayer)
        {
            ChangeState(State.CHASE);                       //�߰�
            yield break;  // �ڷ�ƾ ����
        }

        //��Ÿ ��������
        if (attackCount >= attackMaxCount)
        {
            float randomValue = UnityEngine.Random.Range(0f, 6f);

            if (randomValue < 2)
            {
                ChangeState(State.COMBO_1);
                yield break;  // �ڷ�ƾ ����
            }
            else if (randomValue < 4)
            {
                ChangeState(State.COMBO_2);
                yield break;  // �ڷ�ƾ ����
            }
            else if (randomValue < 6)
            {
                ChangeState(State.COMBO_3);
                yield break;  // �ڷ�ƾ ����
            }
        }
        if (distanceToPlayer <= attackRange)
        {
            if(!is2Phase && !is3Phase)  //2����� �ƴϰ� 3����� �ƴ϶�� ��Ÿ��� 5���� ���� ����
            {
                float randomValue = UnityEngine.Random.Range(0f, 5f);

                if (randomValue < 1)
                {
                    ChangeState(State.ATTACK_1);
                    yield break;  // �ڷ�ƾ ����
                }
                else if (randomValue < 2)
                {
                    ChangeState(State.ATTACK_2);
                    yield break;  // �ڷ�ƾ ����
                }
                else if (randomValue < 3)
                {
                    ChangeState(State.ATTACK_3);
                    yield break;  // �ڷ�ƾ ����
                }
                else if (randomValue < 4)
                {
                    ChangeState(State.ATTACK_4);
                    yield break;  // �ڷ�ƾ ����
                }
                else if (randomValue < 5)
                {
                    ChangeState(State.ATTACK_5);
                    yield break;  // �ڷ�ƾ ����
                }
            }
            else //2������ Ȥ�� 3��������
            {
                float randomValue = UnityEngine.Random.Range(0f, 7f);

                if (randomValue < 1)
                {
                    ChangeState(State.ATTACK_1);
                    yield break;  // �ڷ�ƾ ����
                }
                else if (randomValue < 2)
                {
                    ChangeState(State.ATTACK_2);
                    yield break;  // �ڷ�ƾ ����
                }
                else if (randomValue < 3)
                {
                    ChangeState(State.ATTACK_3);
                    yield break;  // �ڷ�ƾ ����
                }
                else if (randomValue < 4)
                {
                    ChangeState(State.ATTACK_4);
                    yield break;  // �ڷ�ƾ ����
                }
                else if (randomValue < 5)
                {
                    ChangeState(State.ATTACK_5);
                    yield break;  // �ڷ�ƾ ����
                }

                else if (randomValue < 6)
                {
                    ChangeState(State.ATTACK_6);
                    yield break;  // �ڷ�ƾ ����
                }

                else if (randomValue < 7)
                {
                    ChangeState(State.ATTACK_7);
                    yield break;  // �ڷ�ƾ ����
                }
            }

        }

        //100f �̻� �Ÿ��� ��������
        if (100f < distanceToPlayer)
        {
            // StateMachine�� BACK���� ����
            ChangeState(State.BACK);
            yield break;            // �ڷ�ƾ ����
        }

        ChangeState(State.CHECK);
        yield break;

    }

    IEnumerator DIE()
    {
        if (isDead)
        {
            yield break;
        }
        isDead = true;

        // �ִϸ������� ���� �ִϸ��̼� ���� ���� ��������
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // �浹 ���� ���� (Collider ��Ȱ��ȭ)
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        // ��� �ִϸ��̼� ���
        anim.Play("dead");

        // �ִϸ��̼��� normalizedTime�� 1.0�� ������� ������ ���
        while (true)
        {
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

            // �ִϸ��̼��� normalizedTime�� 1.0 �̻��� �� �ִϸ��̼��� �Ϸ�� ������ ����
            if (curAnimStateInfo.IsName("dead") && curAnimStateInfo.normalizedTime >= 1.0f)
            {
                break;
            }

            yield return null; // �� ������ ���
        }

        GameManager.Instance.UpdatePlayerSOUL(enemySoul);
        PlayerPrefs.SetInt("Final", 1);
        PlayerPrefs.Save();

        // �ִϸ��̼��� ���� �� ������Ʈ�� ����
        GameManager.Instance.DisplayDemonSlainedText();
        Destroy(gameObject);
        Destroy(hpSliderObject);
        GameManager.Instance.StopBackgroundMusic();
        GameManager.Instance.VolumeDown();
        GameManager.Instance.PlayBackgroundMusic();
        FinalForcedTeleporter.gameObject.SetActive(true);
        hiddenMessage1.gameObject.SetActive(true);
        hiddenMessage2.gameObject.SetActive(true);
        hiddenMessage3.gameObject.SetActive(true);
        message3.gameObject.SetActive(false);
        hiddenDoor1.gameObject.SetActive(false);
        hiddenDoor2.gameObject.SetActive(false);
        hiddenBoss.gameObject.SetActive(true);
        removeBox.gameObject.SetActive(false);
    }

    IEnumerator IDLE()
    {
        // ���� animator �������� ���
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // �ִϸ��̼� �̸��� Idle �� �ƴϸ� Play
        if (curAnimStateInfo.IsName("idle") == false)
            anim.Play("idle", 0, 0);

        if (distanceToPlayer <= detectingRange) //Ž�� ���� �ȿ� �÷��̾ ������
        {
            hpSliderObject.SetActive(true);
            GameManager.Instance.StopBackgroundMusic();
            GameManager.Instance.VolumeUp();
            GameManager.Instance.PlaySelectedBackgroundMusic(7);
            // StateMachine �� ��÷� ����
            ChangeState(State.CHECK);
            yield break;
        }

        yield return null;

        if (HP < 0)
        {
            ChangeState(State.DIE);
            yield break;
        }
    }

    IEnumerator CHASE()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        //�ִϸ��̼� ��ȯ ���
        if (curAnimStateInfo.IsName("run") == false)
        {
            anim.CrossFade("run", 0.1f, 0, 0);
            // SetDestination �� ���� �� frame�� �ѱ������ �ڵ�
            yield return new WaitForSeconds(0.1f);
        }

        //�ִϸ��̼� ��ȯ üũ
        curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        while (!curAnimStateInfo.IsName("run"))
        {
            yield return null;
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        //�������
        if (HP <= 0)
        {
            ChangeState(State.DIE);
            yield break;
        }
        //������ ���� ����

        if (attackCount >= attackMaxCount)
        {
            float randomValue = UnityEngine.Random.Range(0f, 6f);

            if (randomValue < 2)
            {
                ChangeState(State.COMBO_1);
                yield break;  // �ڷ�ƾ ����
            }
            else if (randomValue < 4)
            {
                ChangeState(State.COMBO_2);
                yield break;  // �ڷ�ƾ ����
            }
            else if (randomValue < 6)
            {
                ChangeState(State.COMBO_3);
                yield break;  // �ڷ�ƾ ����
            }
        }
        if (distanceToPlayer <= attackRange)
        {
            if (!is2Phase && !is3Phase)  //2����� �ƴϰ� 3����� �ƴ϶�� ��Ÿ��� 5���� ���� ����
            {
                float randomValue = UnityEngine.Random.Range(0f, 5f);

                if (randomValue < 1)
                {
                    ChangeState(State.ATTACK_1);
                    yield break;  // �ڷ�ƾ ����
                }
                else if (randomValue < 2)
                {
                    ChangeState(State.ATTACK_2);
                    yield break;  // �ڷ�ƾ ����
                }
                else if (randomValue < 3)
                {
                    ChangeState(State.ATTACK_3);
                    yield break;  // �ڷ�ƾ ����
                }
                else if (randomValue < 4)
                {
                    ChangeState(State.ATTACK_4);
                    yield break;  // �ڷ�ƾ ����
                }
                else if (randomValue < 5)
                {
                    ChangeState(State.ATTACK_5);
                    yield break;  // �ڷ�ƾ ����
                }
            }
            else //2������ Ȥ�� 3��������
            {
                float randomValue = UnityEngine.Random.Range(0f, 7f);

                if (randomValue < 1)
                {
                    ChangeState(State.ATTACK_1);
                    yield break;  // �ڷ�ƾ ����
                }
                else if (randomValue < 2)
                {
                    ChangeState(State.ATTACK_2);
                    yield break;  // �ڷ�ƾ ����
                }
                else if (randomValue < 3)
                {
                    ChangeState(State.ATTACK_3);
                    yield break;  // �ڷ�ƾ ����
                }
                else if (randomValue < 4)
                {
                    ChangeState(State.ATTACK_4);
                    yield break;  // �ڷ�ƾ ����
                }
                else if (randomValue < 5)
                {
                    ChangeState(State.ATTACK_5);
                    yield break;  // �ڷ�ƾ ����
                }

                else if (randomValue < 6)
                {
                    ChangeState(State.ATTACK_6);
                    yield break;  // �ڷ�ƾ ����
                }

                else if (randomValue < 7)
                {
                    ChangeState(State.ATTACK_7);
                    yield break;  // �ڷ�ƾ ����
                }
            }
        }

        if (distanceToBase >= 100f) //�� �Ÿ� 100f �־����� ����
        {
            hpSliderObject.SetActive(false);
            GameManager.Instance.StopBackgroundMusic();
            GameManager.Instance.VolumeDown();
            GameManager.Instance.PlayBackgroundMusic();
            ChangeState(State.BACK);
            yield break;
        }

        yield return null;
    }

    IEnumerator DIVE()
    {
        //�¿� ���� ������
        float randomDive = UnityEngine.Random.Range(0f, 1f);
        if (randomDive < 0.5f)
        {
            anim.CrossFade("roll_r", 0.1f, 0, 0);
        }
        else
        {
            anim.CrossFade("roll_l", 0.1f, 0, 0);
        }

        float attackDuration = 0.81f;  // ���� �ִϸ��̼��� ���� �ð�
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            if (HP <= 0)
            {
                ChangeState(State.DIE);
                yield break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (distanceToPlayer < attackRange)   //������ �÷��̾ ������ �ݰ�.
        {
            if (attackCount >= attackMaxCount)
            {
                float randomValue = UnityEngine.Random.Range(0f, 6f);

                if (randomValue < 2)
                {
                    ChangeState(State.COMBO_1);
                    yield break;  // �ڷ�ƾ ����
                }
                else if (randomValue < 4)
                {
                    ChangeState(State.COMBO_2);
                    yield break;  // �ڷ�ƾ ����
                }
                else if (randomValue < 6)
                {
                    ChangeState(State.COMBO_3);
                    yield break;  // �ڷ�ƾ ����
                }
            }
            if (distanceToPlayer <= attackRange)
            {
                if (!is2Phase && !is3Phase)  //2����� �ƴϰ� 3����� �ƴ϶�� ��Ÿ��� 5���� ���� ����
                {
                    float randomValue = UnityEngine.Random.Range(0f, 5f);

                    if (randomValue < 1)
                    {
                        ChangeState(State.ATTACK_1);
                        yield break;  // �ڷ�ƾ ����
                    }
                    else if (randomValue < 2)
                    {
                        ChangeState(State.ATTACK_2);
                        yield break;  // �ڷ�ƾ ����
                    }
                    else if (randomValue < 3)
                    {
                        ChangeState(State.ATTACK_3);
                        yield break;  // �ڷ�ƾ ����
                    }
                    else if (randomValue < 4)
                    {
                        ChangeState(State.ATTACK_4);
                        yield break;  // �ڷ�ƾ ����
                    }
                    else if (randomValue < 5)
                    {
                        ChangeState(State.ATTACK_5);
                        yield break;  // �ڷ�ƾ ����
                    }
                }
                else //2������ Ȥ�� 3��������
                {
                    float randomValue = UnityEngine.Random.Range(0f, 7f);

                    if (randomValue < 1)
                    {
                        ChangeState(State.ATTACK_1);
                        yield break;  // �ڷ�ƾ ����
                    }
                    else if (randomValue < 2)
                    {
                        ChangeState(State.ATTACK_2);
                        yield break;  // �ڷ�ƾ ����
                    }
                    else if (randomValue < 3)
                    {
                        ChangeState(State.ATTACK_3);
                        yield break;  // �ڷ�ƾ ����
                    }
                    else if (randomValue < 4)
                    {
                        ChangeState(State.ATTACK_4);
                        yield break;  // �ڷ�ƾ ����
                    }
                    else if (randomValue < 5)
                    {
                        ChangeState(State.ATTACK_5);
                        yield break;  // �ڷ�ƾ ����
                    }

                    else if (randomValue < 6)
                    {
                        ChangeState(State.ATTACK_6);
                        yield break;  // �ڷ�ƾ ����
                    }

                    else if (randomValue < 7)
                    {
                        ChangeState(State.ATTACK_7);
                        yield break;  // �ڷ�ƾ ����
                    }
                }
            }
        }

        ChangeState(State.CHECK);
        yield break;
    }

    IEnumerator STEP()
    { 
        //�¿� ���� ���̵�
        float randomDive = UnityEngine.Random.Range(0f, 1f);
        if (randomDive < 0.5f)
        {
            anim.CrossFade("side_r", 0.1f, 0, 0);
        }
        else
        {
            anim.CrossFade("side_l", 0.1f, 0, 0);
        }

        float attackDuration = 0.5f;  // ���� �ִϸ��̼��� ���� �ð�
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            if (HP <= 0)
            {
                ChangeState(State.DIE);
                yield break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (distanceToPlayer < attackRange)   // �÷��̾ ������ �ݰ�.
        {
            if (attackCount >= attackMaxCount)
            {
                float randomValue = UnityEngine.Random.Range(0f, 6f);

                if (randomValue < 2)
                {
                    ChangeState(State.COMBO_1);
                    yield break;  // �ڷ�ƾ ����
                }
                else if (randomValue < 4)
                {
                    ChangeState(State.COMBO_2);
                    yield break;  // �ڷ�ƾ ����
                }
                else if (randomValue < 6)
                {
                    ChangeState(State.COMBO_3);
                    yield break;  // �ڷ�ƾ ����
                }
            }
            if (distanceToPlayer <= attackRange)
            {
                if (!is2Phase && !is3Phase)  //2����� �ƴϰ� 3����� �ƴ϶�� ��Ÿ��� 5���� ���� ����
                {
                    float randomValue = UnityEngine.Random.Range(0f, 5f);

                    if (randomValue < 1)
                    {
                        ChangeState(State.ATTACK_1);
                        yield break;  // �ڷ�ƾ ����
                    }
                    else if (randomValue < 2)
                    {
                        ChangeState(State.ATTACK_2);
                        yield break;  // �ڷ�ƾ ����
                    }
                    else if (randomValue < 3)
                    {
                        ChangeState(State.ATTACK_3);
                        yield break;  // �ڷ�ƾ ����
                    }
                    else if (randomValue < 4)
                    {
                        ChangeState(State.ATTACK_4);
                        yield break;  // �ڷ�ƾ ����
                    }
                    else if (randomValue < 5)
                    {
                        ChangeState(State.ATTACK_5);
                        yield break;  // �ڷ�ƾ ����
                    }
                }
                else //2������ Ȥ�� 3��������
                {
                    float randomValue = UnityEngine.Random.Range(0f, 7f);

                    if (randomValue < 1)
                    {
                        ChangeState(State.ATTACK_1);
                        yield break;  // �ڷ�ƾ ����
                    }
                    else if (randomValue < 2)
                    {
                        ChangeState(State.ATTACK_2);
                        yield break;  // �ڷ�ƾ ����
                    }
                    else if (randomValue < 3)
                    {
                        ChangeState(State.ATTACK_3);
                        yield break;  // �ڷ�ƾ ����
                    }
                    else if (randomValue < 4)
                    {
                        ChangeState(State.ATTACK_4);
                        yield break;  // �ڷ�ƾ ����
                    }
                    else if (randomValue < 5)
                    {
                        ChangeState(State.ATTACK_5);
                        yield break;  // �ڷ�ƾ ����
                    }

                    else if (randomValue < 6)
                    {
                        ChangeState(State.ATTACK_6);
                        yield break;  // �ڷ�ƾ ����
                    }

                    else if (randomValue < 7)
                    {
                        ChangeState(State.ATTACK_7);
                        yield break;  // �ڷ�ƾ ����
                    }
                }
            }
        }

        ChangeState(State.CHECK);
        yield break;
    }

    //�������ϵ�

    IEnumerator ATTACK_1()
    {
        if (is2Phase)
        {
            attackCount++;
        }
        anim.CrossFade("attack_1", 0.1f, 0, 0);
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = 3.05f; // ���� �ִϸ��̼��� ���� �ð�

        float elapsedTime = 0f;

        while (elapsedTime < attackDuration)
        {
            //�������
            if (HP <= 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        float randomStep = UnityEngine.Random.Range(0f, 1f);
        if (randomStep < 0.3f) //���� �� 30% Ȯ���� ���� ��� �ٽ� ����.
        {
            ChangeState(State.STEP);
            yield break;
        }

        if(is3Phase) //3�������� ���� ���� �ð� ���� �ٷ� �߰�.
        {
            ChangeState(State.CHASE);
            yield break;
        }

        // StateMachine�� ������� ����
        ChangeState(State.CHECK);
        yield return null;
    }

    IEnumerator ATTACK_2()
    {
        if (is2Phase)
        {
            attackCount++;
        }
        anim.CrossFade("attack_2", 0.1f, 0, 0);
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = 3.05f; // ���� �ִϸ��̼��� ���� �ð�

        float elapsedTime = 0f;

        while (elapsedTime < attackDuration)
        {
            //�������
            if (HP <= 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        float randomStep = UnityEngine.Random.Range(0f, 1f);
        if (randomStep < 0.3f) //���� �� 30% Ȯ���� ���� ��� �ٽ� ����.
        {
            ChangeState(State.STEP);
            yield break;
        }

        if (is3Phase) //3�������� ���� ���� �ð� ���� �ٷ� �߰�.
        {
            ChangeState(State.CHASE);
            yield break;
        }

        // StateMachine�� ������� ����
        ChangeState(State.CHECK);
        yield return null;
    }

    IEnumerator ATTACK_3()
    {
        if (is2Phase)
        {
            attackCount++;
        }
        anim.CrossFade("attack_3", 0.1f, 0, 0);
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = 3.2f;  // ���� �ִϸ��̼��� ���� �ð�

        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            //�������
            if (HP <= 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        float randomStep = UnityEngine.Random.Range(0f, 1f);
        if (randomStep < 0.3f) //���� �� 30% Ȯ���� ���� ��� �ٽ� ����.
        {
            ChangeState(State.STEP);
            yield break;
        }

        if (is3Phase) //3�������� ���� ���� �ð� ���� �ٷ� �߰�.
        {
            ChangeState(State.CHASE);
            yield break;
        }

        // StateMachine�� ������� ����
        ChangeState(State.CHECK);
        yield return null;
    }

    IEnumerator ATTACK_4()
    {
        if (is2Phase)
        {
            attackCount++;
        }
        anim.CrossFade("attack_4", 0.1f, 0, 0);
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = 2.6f;  // ���� �ִϸ��̼��� ���� �ð�
        float elapsedTime = 0f;

        while (elapsedTime < attackDuration)
        {
            //�������
            if (HP <= 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        float randomStep = UnityEngine.Random.Range(0f, 1f);
        if (randomStep < 0.3f) //���� �� 30% Ȯ���� ���� ��� �ٽ� ����.
        {
            ChangeState(State.STEP);
            yield break;
        }

        if (is3Phase) //3�������� ���� ���� �ð� ���� �ٷ� �߰�.
        {
            ChangeState(State.CHASE);
            yield break;
        }
        // StateMachine�� ������� ����
        ChangeState(State.CHECK);
        yield return null;
    }

    IEnumerator ATTACK_5()
    {
        if (is2Phase)
        {
            attackCount++;
        }
        anim.CrossFade("attack_5", 0.1f, 0, 0);
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = 2.2f;  // ���� �ִϸ��̼��� ���� �ð�
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            //�������
            if (HP <= 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        float randomStep = UnityEngine.Random.Range(0f, 1f);
        if (randomStep < 0.3f) //���� �� 30% Ȯ���� ���� ��� �ٽ� ����.
        {
            ChangeState(State.STEP);
            yield break;
        }

        if (is3Phase) //3�������� ���� ���� �ð� ���� �ٷ� �߰�.
        {
            ChangeState(State.CHASE);
            yield break;
        }
        // StateMachine�� ������� ����
        ChangeState(State.CHECK);
        yield return null;
    }

    IEnumerator ATTACK_6()
    {
        if (is2Phase)
        {
            attackCount++;
        }
        anim.CrossFade("attack_6", 0.1f, 0, 0);
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = 3.4f;  // ���� �ִϸ��̼��� ���� �ð�
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            //�������
            if (HP <= 0)
            {
                ChangeState(State.DIE);
                yield break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        float randomStep = UnityEngine.Random.Range(0f, 1f);
        if (randomStep < 0.3f) //���� �� 30% Ȯ���� ���� ��� �ٽ� ����.
        {
            ChangeState(State.STEP);
            yield break;
        }

        if (is3Phase) //3�������� ���� ���� �ð� ���� �ٷ� �߰�.
        {
            ChangeState(State.CHASE);
            yield break;
        }
        // StateMachine�� ������� ����

        ChangeState(State.CHECK);
        yield return null;
    }

    IEnumerator ATTACK_7()
    {
        if (is2Phase)
        {
            attackCount++;
        }
        anim.CrossFade("attack_7", 0.1f, 0, 0);
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float attackDuration = 3.9f;  // ���� �ִϸ��̼��� ���� �ð�
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            //�������
            if (HP <= 0)
            {
                ChangeState(State.DIE);
                yield break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        float randomStep = UnityEngine.Random.Range(0f, 1f);
        if (randomStep < 0.3f) //���� �� 30% Ȯ���� ���� ��� �ٽ� ����.
        {
            ChangeState(State.STEP);
            yield break;
        }

        if (is3Phase) //3�������� ���� ���� �ð� ���� �ٷ� �߰�.
        {
            ChangeState(State.CHASE);
            yield break;
        }
        // StateMachine�� ������� ����
        ChangeState(State.CHECK);
        yield return null;
    }


    IEnumerator COMBO_1()
    {
        if (is2Phase)
        {
            attackCount = 0;
        }

        anim.CrossFade("combo_1", 0.1f, 0, 0);
        float attackDuration = 6.3f;  // ���� �ִϸ��̼��� ���� �ð�

        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            //�������
            if (HP <= 0)
            {
                ChangeState(State.DIE);
                yield break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        float randomStep = UnityEngine.Random.Range(0f, 1f);
        if (randomStep < 0.3f) //���� �� 30% Ȯ���� ���� ��� �ٽ� ����.
        {
            ChangeState(State.STEP);
            yield break;
        }

        if (is3Phase) //3�������� ���� ���� �ð� ���� �ٷ� �߰�.
        {
            ChangeState(State.CHASE);
            yield break;
        }

        // StateMachine�� ������� ����
        ChangeState(State.CHECK);
        yield return null;
    }

    IEnumerator COMBO_2()
    {
        if (is2Phase)
        {
            attackCount = 0;
        }

        anim.CrossFade("combo_2", 0.1f, 0, 0);
        float attackDuration = 4.1f;  // ���� �ִϸ��̼��� ���� �ð�

        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            //�������
            if (HP <= 0)
            {
                ChangeState(State.DIE);
                yield break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        float randomStep = UnityEngine.Random.Range(0f, 1f);
        if (randomStep < 0.3f) //���� �� 30% Ȯ���� ���� ��� �ٽ� ����.
        {
            ChangeState(State.STEP);
            yield break;
        }

        if (is3Phase) //3�������� ���� ���� �ð� ���� �ٷ� �߰�.
        {
            ChangeState(State.CHASE);
            yield break;
        }

        // StateMachine�� ������� ����
        ChangeState(State.CHECK);
        yield return null;
    }

    IEnumerator COMBO_3()
    {
        if (is2Phase)
        {
            attackCount = 0;
        }

        anim.CrossFade("combo_3", 0.1f, 0, 0);
        float attackDuration = 4.23f;  // ���� �ִϸ��̼��� ���� �ð�

        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            //�������
            if (HP <= 0)
            {
                ChangeState(State.DIE);
                yield break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        float randomStep = UnityEngine.Random.Range(0f, 1f);
        if (randomStep < 0.3f) //���� �� 30% Ȯ���� ���� ��� �ٽ� ����.
        {
            ChangeState(State.STEP);
            yield break;
        }

        if (is3Phase) //3�������� ���� ���� �ð� ���� �ٷ� �߰�.
        {
            ChangeState(State.CHASE);
            yield break;
        }

        // StateMachine�� ������� ����
        ChangeState(State.CHECK);
        yield return null;
    }





    void Update()
    {
        UpdateHPBar();

        directionToPlayer = new Vector3(player.position.x - transform.position.x,
            0f, player.position.z - transform.position.z);    //�÷��̾���� ��ġ����
        distanceToPlayer = directionToPlayer.magnitude;                                       //�÷��̾���� ��ġ���� ��ġȭ

        directionToBase = new Vector3(initialPoint.x - transform.position.x,
            0f, initialPoint.z - transform.position.z);
        distanceToBase = directionToBase.magnitude;

        if (HP < 150 && HP > 60)//������ ��ȭ üũ, ������ ������ ���⼭
        {
            is2Phase = true;
        }
        else if (HP <= 60)
        {
            is3Phase = true;
            attackMaxCount = 2;
        }

        if (state != State.BACK && state != State.DIE && state != State.IDLE && state != State.DIVE )
        {
            LookPlayer(directionToPlayer);
        }

        if (state == State.CHASE)
        {
            ChasePlayer();
        }

        if (state == State.DIE || state == State.ATTACK_1 || state == State.ATTACK_2 || state == State.ATTACK_3 || state == State.ATTACK_4 || state == State.ATTACK_5 || state == State.ATTACK_6 || state == State.ATTACK_7
            || state == State.COMBO_1 || state == State.COMBO_2 || state == State.COMBO_3 || state ==  State.DIVE || state == State.STEP )
        {
            Stopnow();
        }

        if (state == State.BACK)
        {
            ReturnToBase();
        }

        if (state == State.CHECK)
        {
            CheckPlayer();
        }

        if (isAttack)
        {
            // ���� ������ �� �ݶ��̴��� Ȱ��ȭ�մϴ�.
            EnableAttackColliders(true);
        }
        else
        {
            // ���� ���°� �ƴ� �� �ݶ��̴��� ��Ȱ��ȭ�մϴ�.
            EnableAttackColliders(false);
        }

        // �ִϸ��̼ǿ��� ��Ʈ ����� ����Ͽ� �̵�
        Vector3 movement = anim.deltaPosition; // �ִϸ��̼��� ��ȭ��
        transform.position += movement; // ���� ��ġ ������Ʈ

    }

    public void TriggerAttack()
    {
        isAttack = true;
    }

    public void EndAttack()
    {
        isAttack = false;
    }

    public void CantRot()
    {
        smoothRotationSpeed = 0.1f;
    }

    public void CanRot()
    {
        smoothRotationSpeed = 3f;
    }

    public void FastRot()
    {
        smoothRotationSpeed = 15f;
    }

    public void InitializeHPBar(GameObject hpSliderPrefab)
    {
        hpSliderObject = Instantiate(hpSliderPrefab, GameObject.Find("Canvas").transform); // �����̴� ���� �� �θ� Canvas�� ����
        hpSlider = hpSliderObject.GetComponent<Slider>();
        bossName = hpSliderObject.GetComponent<Text>();
        UpdateHPBar();
        bossName.text = "������ ��� ����";
    }

    private void UpdateHPBar()
    {
        if (hpSlider != null)
        {
            hpSlider.value = HP / MaxHP; // �����̴� �� ������Ʈ
        }
    }

    void ChangeState(State newState)
    {
        state = newState;
    }

    void ChasePlayer()                      //�߰� ���
    {
        nmAgent.speed = chaseSpeed;
        nmAgent.SetDestination(player.position);
    }


    void LookPlayer(Vector3 direction)      //��� ���
    {

        Vector3 flatDirection = new Vector3(direction.x, 0f, direction.z);
        if (flatDirection != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(flatDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, smoothRotationSpeed * Time.deltaTime);
        }
    }

    void ReturnToBase()  //��ȯ ���
    {
        nmAgent.speed = 4f;
        if (distanceToBase > 6f)  // �Ÿ��� 3 �̻��� ��쿡�� �̵�
        {
            nmAgent.SetDestination(initialPoint);
        }
        else
        {
            Debug.Log("������");
            HP = MaxHP;
            ChangeState(State.IDLE); //idle ���·� ����
        }
    }

    void CheckPlayer()
    {
        nmAgent.speed = alertSpeed;  //��� �»��϶��� ������
        // ������ �ʱ�ȭ���� �ʾ����� 50% Ȯ���� �¿� �̵� ������ ����
        if (!directionInitialized)
        {
            moveRight = UnityEngine.Random.Range(0f, 1f) >= 0.5f;
            directionInitialized = true;  // ������ �ʱ�ȭ�Ǿ����� ǥ��
        }

        timeSinceLastCheck += Time.deltaTime;

        if (timeSinceLastCheck >= checkInterval)
        {
            timeSinceLastCheck = 0f;

            // 50% Ȯ���� �¿� �̵� ���� ����
            if (UnityEngine.Random.Range(0f, 1f) < 0.5f)
            {
                moveRight = !moveRight;
            }
        }

        // �÷��̾���� ���� ���� ���
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        // �÷��̾���� ���� �Ÿ�
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // �¿�� �̵��� ������ ��� (���� Ȥ�� �������� 90�� ȸ��)
        Vector3 moveDirection;
        if (moveRight)
        {
            moveDirection = Quaternion.Euler(0, 90, 0) * directionToPlayer;  // �������� ȸ��
        }
        else
        {
            moveDirection = Quaternion.Euler(0, -90, 0) * directionToPlayer; // �������� ȸ��
        }

        // ��ǥ ��ġ ���
        Vector3 targetPosition = player.position - directionToPlayer * checkRange + moveDirection * 10f;

        // NavMeshAgent�� �������� ����
        nmAgent.SetDestination(targetPosition);
    }

    void Stopnow()
    {
        nmAgent.SetDestination(transform.position);
    }


    //���� ��Ȱ��ȭ or Ȱ��ȭ ���
    void EnableAttackColliders(bool enable)
    {
        // attackRangeL�� attackRangeR�� �ݶ��̴��� Ȱ��ȭ �Ǵ� ��Ȱ��ȭ
        if (weaponAttackRange != null)
        {
            weaponAttackRange.GetComponent<Collider>().enabled = enable;
        }

    }
    //������ ����
    void OnTriggerEnter(Collider other)
    {
        if (isAlreadyHit) { return; }
        if (other.CompareTag("Weapon") && player_1.GetComponent<SuperPlayerController>().isAttackHit)
        {
            HP = HP - player_1.GetComponent<SuperPlayerController>().PlayerDamage;
            Debug.Log("������!");
            isHit = true;
            PRSAttacked();

            isAlreadyHit = true; // Ÿ�ݵǾ����� ǥ��
            StartCoroutine(ResetHit()); // ���� �ð� �� �÷��� �ʱ�ȭ

        }

        if (other.CompareTag("magic"))
        {
            HP = HP - player_1.GetComponent<SuperPlayerController>().PlayerDamage;
            Debug.Log("������!");
            isHit = true;

            isAlreadyHit = true; // Ÿ�ݵǾ����� ǥ��
            StartCoroutine(ResetHit()); // ���� �ð� �� �÷��� �ʱ�ȭ

        }

    }

    IEnumerator ResetHit()
    {
        yield return new WaitForSeconds(0.3f); // 0.3�� �� �ʱ�ȭ, �ʿ信 ���� ���� ����
        isAlreadyHit = false;
    }

    public void PRSFoot()
    {
        int randomIndex = UnityEngine.Random.RandomRange(0, footSound.Length);
        AudioClip selectedClip = footSound[randomIndex];
        audioSource.PlayOneShot(selectedClip);
    }

    public void PRSDive()
    {
        int randomIndex = UnityEngine.Random.RandomRange(0, diveSound.Length);
        AudioClip selectedClip = diveSound[randomIndex];
        audioSource.PlayOneShot(selectedClip);
    }

    public void PRSDash()
    {
        int randomIndex = UnityEngine.Random.RandomRange(0, dashSound.Length);
        AudioClip selectedClip = dashSound[randomIndex];
        audioSource.PlayOneShot(selectedClip);
    }

    public void PRSSlash()
    {
        int randomIndex = UnityEngine.Random.RandomRange(0, slashSound.Length);
        AudioClip selectedClip = slashSound[randomIndex];
        audioSource.PlayOneShot(selectedClip);
    }

    public void PRSAir()
    {
        int randomIndex = UnityEngine.Random.RandomRange(0, airSound.Length);
        AudioClip selectedClip = airSound[randomIndex];
        audioSource.PlayOneShot(selectedClip);
    }

    public void PRSClash()
    {
        int randomIndex = UnityEngine.Random.RandomRange(0, crashSound.Length);
        AudioClip selectedClip = crashSound[randomIndex];
        audioSource.PlayOneShot(selectedClip);
    }

    public void PRSAttacked()
    {
        int randomIndex = UnityEngine.Random.RandomRange(0, attackedSound.Length);
        AudioClip selectedClip = attackedSound[randomIndex];
        audioSource.PlayOneShot(selectedClip);
    }
}
