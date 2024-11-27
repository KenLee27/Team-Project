using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject playerUI;
    public GameObject weaponUI;
    public GameObject stateUI;
    public GameObject statusUI;
    public GameObject equipmentUI;
    public GameObject teleportUI;
    public GameObject messageUI;
    public GameObject confirmUI;
    public GameObject warningUI;
    public GameObject quitUI;

    private static UIManager _instance;

    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<UIManager>();
            return _instance;
        }
    }

    private SuperPlayerController playerController;
    private EquipmentManager equipmentManager;

    public enum UIState { Game, Menu, Status, Equipment, Teleport, Message, Confirm, Warning, Quit }
    public UIState currentState = UIState.Game;

    private Vector3 burningGround = new Vector3(476.948f, 20.861f, 486.508f);
    private Vector3 caliosBridge = new Vector3(331.77f, 83.362f, 535.92f);
    private Vector3 outerBattleField = new Vector3(169.007f, 83.23f, 463.072f);
    private Vector3 caliosChurch = new Vector3(239.09f, 152.152f, 672.217f);
    private Vector3 topOfCastle = new Vector3(270.86f, 149.491f, 681.799f);
    private Vector3 middleOfCastle = new Vector3(264.142f, 122.55f, 577.977f);
    private Vector3 mageBossRoom = new Vector3(708.77f, 17.154f, 361.02f);
    private Vector3 mageForcedTeleporter = new Vector3(318.297f, 144.348f, 664.796f);
    private Vector3 finalBossRoom = new Vector3(546.727f, 17.691f, 685.254f);

    public float[] necessitySoul;
    public float[] hpLevel;
    public float[] mnLevel;
    public float[] stLevel;
    public float[] strLevel;

    public bool canHPLevelUp = true;
    public bool canMNLevelUP = true;
    public bool canSTLevelUp = true;
    public bool canSTRLevelUp = true;

    public AudioSource audioSource;
    public AudioClip clickSound;
    public AudioClip keyClickSound;
    public AudioClip warningSound;

    private void Awake()
    {
        necessitySoul = new float[121] //������ �� �ʿ��� ��
        {
              100f,   200f,   300f,   400f,   500f,   600f,   700f,   800f,   900f,  1000f,
             1100f,  1200f,  1300f,  1400f,  1500f,  1600f,  1700f,  1800f,  1900f,  2000f,
             2100f,  2200f,  2300f,  2400f,  2500f,  2600f,  2700f,  2800f,  2900f,  3000f,
             3100f,  3200f,  3300f,  3400f,  3500f,  3600f,  3700f,  3800f,  3900f,  4000f,
             4100f,  4200f,  4300f,  4400f,  4500f,  4600f,  4700f,  4800f,  4900f,  5000f,
             5100f,  5200f,  5300f,  5400f,  5500f,  5600f,  5700f,  5800f,  5900f,  6000f,
             6100f,  6200f,  6300f,  6400f,  6500f,  6600f,  6700f,  6800f,  6900f,  7000f,
             7100f,  7200f,  7300f,  7400f,  7500f,  7600f,  7700f,  7800f,  7900f,  8000f,
             8100f,  8200f,  8300f,  8400f,  8500f,  8600f,  8700f,  8800f,  8900f,  9000f,
             9100f,  9200f,  9300f,  9400f,  9500f,  9600f,  9700f,  9800f,  9900f, 10000f,
            10100f, 10200f, 10300f, 10400f, 10500f, 10600f, 10700f, 10800f, 10900f, 11000f,
            11100f, 11200f, 11300f, 11400f, 11500f, 11600f, 11700f, 11800f, 11900f, 12000f, 13000f
        };

        hpLevel = new float[31] //������ �� HP ��ȭ��
        {
      100f, 104f, 108f, 112f, 116f, 120f, 124f, 128f, 132f, 136f, 140f,
            142f, 144f, 146f, 148f, 150f, 152f, 154f, 156f, 158f, 160f,
            161f, 162f, 163f, 164f, 165f, 166f, 167f, 168f, 169f, 180f
        };

        mnLevel = new float[31]  //������ �� MN ��ȭ��
        {
      100f, 103f, 106f, 109f, 112f, 115f, 118f, 121f, 124f, 127f, 130f,
            132f, 134f, 136f, 138f, 140f, 142f, 144f, 146f, 148f, 150f,
            151f, 152f, 153f, 154f, 155f, 156f, 157f, 158f, 159f, 175f
        };

        stLevel = new float[31] //������ �� ST ��ȭ��
        {
      100f, 102f, 104f, 106f, 108f, 110f, 112f, 114f, 116f, 118f, 120f,
            122f, 124f, 126f, 128f, 130f, 132f, 134f, 136f, 138f, 140f,
            140f, 141f, 142f, 143f, 144f, 145f, 146f, 150f, 155f, 160f
        };

        strLevel = new float[31] //������ �� ATK ��ȭ��
        {
      3.0f, 3.1f, 3.2f, 3.3f, 3.4f, 3.5f, 3.6f, 3.7f, 3.8f, 3.9f, 4.0f,
            4.1f, 4.2f, 4.3f, 4.4f, 4.5f, 4.6f, 4.7f, 4.8f, 4.9f, 5.0f,
            5.1f, 5.2f, 5.3f, 5.4f, 5.5f, 5.6f, 5.7f, 5.8f, 5.9f, 6.5f
        };
    }

    private void Start()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // �̱��� �ν��Ͻ� ����
        _instance = this;

        AccessController();
        InitializeButtonClickEvents();
        SetInitialUIState();
        LoadSaveButtonState();
        UpdateStatusText();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.volume = 0.3f;
    }

    

    public void IncreaseLife()
    {
        float currentSoul = PlayerPrefs.GetFloat("playerSoul");
        float playerLv = PlayerPrefs.GetFloat("playerLevel");
        int myLv = (int)playerLv - 1;

        if (currentSoul >= necessitySoul[myLv] && canHPLevelUp)
        {
            GameManager.Instance.UpdateUsePlayerSOUL(necessitySoul[myLv]);

            float currentLife = PlayerPrefs.GetFloat("playerLife");
            currentLife++;
            PlayerPrefs.SetFloat("playerLife", currentLife);

            float currentLevel = PlayerPrefs.GetFloat("playerLevel");
            currentLevel++;
            PlayerPrefs.SetFloat("playerLevel", currentLevel);

            PlayerPrefs.Save();
            UpdateStatusText();
            playerController.UpdateCurrentPlayerStat();

            if (audioSource != null && clickSound != null) { audioSource.PlayOneShot(clickSound); }
            else { Debug.LogWarning("AudioSource or ClickSound is not assigned."); }
        }
        else
        {
            if (audioSource != null && warningSound != null) { audioSource.PlayOneShot(warningSound); }
            else { Debug.LogWarning("AudioSource or WarningSound is not assigned."); }
        }
    }

    public void IncreaseMental()
    {
        float currentSoul = PlayerPrefs.GetFloat("playerSoul");
        float playerLv = PlayerPrefs.GetFloat("playerLevel");
        int myLv = (int)playerLv - 1;

        if (currentSoul >= necessitySoul[myLv] && canMNLevelUP)
        {
            GameManager.Instance.UpdateUsePlayerSOUL(necessitySoul[myLv]);

            float currentMental = PlayerPrefs.GetFloat("playerMental");
            currentMental++;
            PlayerPrefs.SetFloat("playerMental", currentMental);

            float currentLevel = PlayerPrefs.GetFloat("playerLevel");
            currentLevel++;
            PlayerPrefs.SetFloat("playerLevel", currentLevel);

            PlayerPrefs.Save();
            UpdateStatusText();
            playerController.UpdateCurrentPlayerStat();

            if (audioSource != null && clickSound != null) { audioSource.PlayOneShot(clickSound); }
            else { Debug.LogWarning("AudioSource or ClickSound is not assigned."); }
        }
        else
        {
            if (audioSource != null && warningSound != null) { audioSource.PlayOneShot(warningSound); }
            else { Debug.LogWarning("AudioSource or WarningSound is not assigned."); }
        }
    }

    public void IncreaseEndurance()
    {
        float currentSoul = PlayerPrefs.GetFloat("playerSoul");
        float playerLv = PlayerPrefs.GetFloat("playerLevel");
        int myLv = (int)playerLv - 1;

        if (currentSoul >= necessitySoul[myLv] && canSTLevelUp)
        {
            GameManager.Instance.UpdateUsePlayerSOUL(necessitySoul[myLv]);

            float currentEndurance = PlayerPrefs.GetFloat("playerEndurance");
            currentEndurance++;
            PlayerPrefs.SetFloat("playerEndurance", currentEndurance);

            float currentLevel = PlayerPrefs.GetFloat("playerLevel");
            currentLevel++;
            PlayerPrefs.SetFloat("playerLevel", currentLevel);

            PlayerPrefs.Save();
            UpdateStatusText();
            playerController.UpdateCurrentPlayerStat();

            if (audioSource != null && clickSound != null) { audioSource.PlayOneShot(clickSound); }
            else { Debug.LogWarning("AudioSource or ClickSound is not assigned."); }
        }
        else
        {
            if (audioSource != null && warningSound != null) { audioSource.PlayOneShot(warningSound); }
            else { Debug.LogWarning("AudioSource or WarningSound is not assigned."); }
        }
    }

    public void IncreaseForce()
    {
        float currentSoul = PlayerPrefs.GetFloat("playerSoul");
        float playerLv = PlayerPrefs.GetFloat("playerLevel");
        int myLv = (int)playerLv - 1;

        if (currentSoul >= necessitySoul[myLv] && canSTRLevelUp)
        {
            GameManager.Instance.UpdateUsePlayerSOUL(necessitySoul[myLv]);

            float currentForce = PlayerPrefs.GetFloat("playerForce");
            currentForce++;
            PlayerPrefs.SetFloat("playerForce", currentForce);

            float currentLevel = PlayerPrefs.GetFloat("playerLevel");
            currentLevel++;
            PlayerPrefs.SetFloat("playerLevel", currentLevel);

            PlayerPrefs.Save();
            UpdateStatusText();
            playerController.UpdateCurrentPlayerStat();

            if (audioSource != null && clickSound != null) { audioSource.PlayOneShot(clickSound); }
            else { Debug.LogWarning("AudioSource or ClickSound is not assigned."); }
        }
        else
        {
            if (audioSource != null && warningSound != null) { audioSource.PlayOneShot(warningSound); }
            else { Debug.LogWarning("AudioSource or WarningSound is not assigned."); }
        }
    }

    private void teleportBurningGround()
    {
        GameManager.Instance.changeDisplayName("��Ÿ�� ����");
        GameManager.Instance.SavePosition(burningGround);
        GoBack();
        GameManager.Instance.StartScene();
    }
    private void teleportCaliosBridge()
    {
        GameManager.Instance.changeDisplayName("Į������ ���� ����");
        GameManager.Instance.SavePosition(caliosBridge);
        GoBack();
        GameManager.Instance.StartScene();
    }
    private void teleportOuterBattleField()
    {
        GameManager.Instance.displayName = "�ܰ� ������";
        GameManager.Instance.SavePosition(outerBattleField);
        GoBack();
        GameManager.Instance.StartScene();
    }
    private void teleportCaliosChurch()
    {
        GameManager.Instance.changeDisplayName("Į������ �뼺��");
        GameManager.Instance.SavePosition(caliosChurch);
        GoBack();
        GameManager.Instance.StartScene();
    }
    private void teleportTopOfCastle()
    {
        GameManager.Instance.changeDisplayName("���� ����");
        GameManager.Instance.SavePosition(topOfCastle);
        GoBack();
        GameManager.Instance.StartScene();
    }
    private void teleportMiddleOfCastle()
    {
        GameManager.Instance.changeDisplayName("���� ����");
        GameManager.Instance.SavePosition(middleOfCastle);
        GoBack();
        GameManager.Instance.StartScene();
    }

    private void teleportMageBossRoom()
    {
        GameManager.Instance.changeDisplayName("Į������ ȭ��");
        GameManager.Instance.SavePosition(mageBossRoom);
        GoBack();
        GameManager.Instance.StartScene();
    }

    private void teleportMageForcedTeleporter()
    {
        GameManager.Instance.changeDisplayName("���� ����");
        GameManager.Instance.SavePosition(mageForcedTeleporter);
        GoBack();
        GameManager.Instance.StartScene();
    }

    private void teleportFinalBossRoom()
    {
        GameManager.Instance.changeDisplayName("������ ������");
        GameManager.Instance.SavePosition(finalBossRoom);
        GoBack();
        GameManager.Instance.StartScene();
    }

    public void ActivateSaveButton(string savePointName)
    {
        Transform buttonTransform = teleportUI.transform.Find(savePointName);

        if (buttonTransform != null)
        {
            Button button = buttonTransform.GetComponent<Button>();
            if (button != null)
            {
                button.gameObject.SetActive(true); // ��ư Ȱ��ȭ
            }
            else
            {
                Debug.LogWarning($"Button component not found on {savePointName}!");
            }
        }
        else
        {
            Debug.LogWarning($"Button for save point {savePointName} not found in teleportUI!");
        }
    }

    public void RefreshMessage(string messagePointName)
    {
        Text messageText = messageUI.transform.Find("Message").GetComponent<Text>();

        switch (messagePointName)
        {
            case "M1":
                messageText.text = "�Ǹ����� �����޿� ���� ���ѱ� Į������ ������ ��ã�� ������ ������ ��ã�ƶ�.";
                break;
            case "M2":
                messageText.text = "��Ǹ� ī���� ������ Į������ �������� ���ϴ� ������ ���θ��� �ִ�.";
                break;
            case "M3":
                messageText.text = "���� ��ġ�� ���鿡 ������ �Ҹ��� ��ġ �ϴÿ� ��������� �췿�Ҹ��͵� ���Ҵ�.";
                break;
            case "M4":
                messageText.text = "�������� ������ �Ǿ���. �Ǹ����� ���簡 �ѷ��ϰ� ��������.";
                break;
            case "M5":
                messageText.text = "��𼱰� ������ ������ Ŀ�ٶ� ���� ��������.";
                break;
            case "M6":
                messageText.text = "��𼱰� �귯������ ���� ���� ������ ���ư� ���� ����.";
                break;
            case "M7":
                messageText.text = "�ᱹ ������� �͹��ȱ���. \r\n������ �����忡�� �ʸ� ��ٸ��� �־�. \r\n- ���� -";
                break;
            case "M8":
                messageText.text = "��򰡿��� ��������� ���簨�� �������� �� ����.";
                break;
            case "M9":
                messageText.text = "���ʿ��� ��������� ������ ��������. �� �ȿ��� ��ħ�� ���� �����Ѵ�.";
                break;
            case "M10":
                messageText.text = "���� ������ �� �ӿ� ���� �Ҹ��� ��ġ ��׸��� �� ��� �Ҹ��͵� ���Ҵ�.";
                break;
        }
    }

    public void RefreshConfirmMessage(string confirmPointName)
    {
        Text messageText = confirmUI.transform.Find("Message").GetComponent<Text>();

        switch (confirmPointName) 
        {
            case "MageIn":
                messageText.text = "�������� ���� ��� ���εǾ� �ֽ��ϴ�. \r\n�ٸ� ������ �̵��Ͻðڽ��ϱ�?";
                break;
            case "MageOut":
                messageText.text = "���� ������ �����ϰ� ���ư��ðڽ��ϱ�?";
                break;
        }
    }

    public void RefreshWarningMessage(string warningPointName)
    {
        Text messageText = warningUI.transform.Find("Message").GetComponent<Text>();

        switch (warningPointName)
        {
            case "FinalIn":
                messageText.text = "�������� ���� ����� ����ϱ� �����մϴ�.\r\n�ٸ� ������ �̵��Ͻðڽ��ϱ�?\r\n��� : ���� Ŭ���� ������ �̰����� �ٽ� ���ƿ� �� �����ϴ�.";
                break;
            case "FinalOut":
                messageText.text = "���� �ִ� ������ ���ư��ðڽ��ϱ�?";
                break;
        }
    }

    public void QuitGame()
    {
        StartCoroutine(GameManager.Instance.LoadSceneAsync("MainMenu"));
    }


    // ���� ���� �� ��ư Ȱ��ȭ ���� ���� �Լ�
    public void LoadSaveButtonState()
    {
        foreach (Transform buttonTransform in teleportUI.transform)
        {
            if (buttonTransform.TryGetComponent(out Button button))
            {
                string savePointName = buttonTransform.name;
                if (PlayerPrefs.GetInt(savePointName, 0) == 1)  // 1�̸� Ȱ��ȭ�� ����
                {
                    button.gameObject.SetActive(true);
                }
                else
                {
                    button.gameObject.SetActive(false);
                }
            }
        }
    }

    public void UpdatePosionDisplay(float nHPposion, float nMNposion)
    {
        Transform HPposion = playerUI.transform.Find("HPPdisplay");
        Transform MNposion = playerUI.transform.Find("MNPdisplay");

        if (HPposion != null)
        {
            float displayH = nHPposion;
            Text[] statTexts = HPposion.GetComponentsInChildren<Text>();
            foreach (Text t in statTexts) { t.text = displayH.ToString(); }
        }
        if (MNposion != null)
        {
            float displayM = nMNposion;
            Text[] statTexts = MNposion.GetComponentsInChildren<Text>();
            foreach (Text t in statTexts) { t.text = displayM.ToString(); }
        }
        Debug.Log("���� ���� �Ϸ�");
    }

    public void UpdateStatusText()
    {
        Transform levelTextUI = statusUI.transform.Find("LevelValue");
        Transform soulTextUI = statusUI.transform.Find("SoulValue");
        Transform necessityTextUI = statusUI.transform.Find("NecessityValue");
        Transform hpTextUI = statusUI.transform.Find("HPDisplay");
        Transform mnTextUI = statusUI.transform.Find("MNDisplay");
        Transform stTextUI = statusUI.transform.Find("STDisplay");
        Transform strTextUI = statusUI.transform.Find("STRDisplay");
        Transform hpValueTextUI = statusUI.transform.Find("HPValue");
        Transform mnValueTextUI = statusUI.transform.Find("MNValue");
        Transform stValueTextUI = statusUI.transform.Find("STValue");
        Transform atkValueTextUI = statusUI.transform.Find("ATKDisplay");

        if (hpValueTextUI != null)
        {
            float currentLife = PlayerPrefs.GetFloat("playerLife");
            int iCurrentLife = (int)currentLife - 10;

            Text[] statTexts = hpValueTextUI.GetComponentsInChildren<Text>();
            foreach (Text t in statTexts)
            {
                t.text = hpLevel[iCurrentLife].ToString();
            }
        }

        if (mnValueTextUI != null)
        {
            float currentMental = PlayerPrefs.GetFloat("playerMental");
            int iCurrentMental = (int)currentMental - 10;

            Text[] statTexts = mnValueTextUI.GetComponentsInChildren<Text>();
            foreach (Text t in statTexts)
            {
                t.text = mnLevel[iCurrentMental].ToString();
            }
        }

        if (stValueTextUI != null)
        {
            float currentEndurance = PlayerPrefs.GetFloat("playerEndurance");
            int iCurrentEndurance = (int)currentEndurance - 10;

            Text[] statTexts = stValueTextUI.GetComponentsInChildren<Text>();
            foreach (Text t in statTexts)
            {
                t.text = stLevel[iCurrentEndurance].ToString();
            }
        }

        if (atkValueTextUI != null)
        {
            float currentForce = PlayerPrefs.GetFloat("playerForce");
            int iCurrentForce = (int)currentForce - 10;

            Text[] statTexts = atkValueTextUI.GetComponentsInChildren<Text>();
            foreach (Text t in statTexts)
            {
                t.text = strLevel[iCurrentForce].ToString();
            }
        }

        if (levelTextUI != null)
        {
            float currentLevel = PlayerPrefs.GetFloat("playerLevel");
            Text[] statTexts = levelTextUI.GetComponentsInChildren<Text>();
            foreach(Text t in statTexts)
            {
                t.text = currentLevel.ToString();
            }
        }

        if (soulTextUI != null)
        {
            float currentSoul = PlayerPrefs.GetFloat("playerSoul");
            Text[] statTexts = soulTextUI.GetComponentsInChildren<Text>();
            foreach (Text t in statTexts)
            {
                t.text = currentSoul.ToString();
            }
        }

        if (necessityTextUI != null)
        {
            float playerLv = PlayerPrefs.GetFloat("playerLevel");
            int myLv = (int)playerLv - 1;
            Debug.Log(myLv);

            Text[] statTexts = necessityTextUI.GetComponentsInChildren<Text>();
            foreach (Text t in statTexts)
            {
                t.text = necessitySoul[myLv].ToString();
            }
        }

        if (hpTextUI != null)
        {
            float currentHP = PlayerPrefs.GetFloat("playerLife");
            Text[] statTexts = hpTextUI.GetComponentsInChildren<Text>();
            foreach (Text t in statTexts)
            {
                t.text = currentHP.ToString();
            }
        }

        if (mnTextUI != null)
        {
            float currentMN = PlayerPrefs.GetFloat("playerMental");
            Text[] statTexts = mnTextUI.GetComponentsInChildren<Text>();
            foreach (Text t in statTexts)
            {
                t.text = currentMN.ToString();
            }
        }

        if (strTextUI != null)
        {
            float currentSTR = PlayerPrefs.GetFloat("playerForce");
            Text[] statTexts = strTextUI.GetComponentsInChildren<Text>();
            foreach (Text t in statTexts)
            {
                t.text = currentSTR.ToString();
            }
        }

        if (stTextUI != null)
        {
            float currentST = PlayerPrefs.GetFloat("playerEndurance");
            Text[] statTexts = stTextUI.GetComponentsInChildren<Text>();
            foreach (Text t in statTexts)
            {
                t.text = currentST.ToString();
            }
        }
    }

    public void InitializeButtonClickEvents()
    {
        Transform statusMenuTransform = stateUI.transform.Find("StatusMenu");
        Transform equipmentMenuTransform = stateUI.transform.Find("EquipmentMenu");
        Transform teleportBurningGroundUI = teleportUI.transform.Find("BurningGround");
        Transform teleportCaliosBridgeUI = teleportUI.transform.Find("CaliosBridge");
        Transform teleportOuterBattleFieldUI = teleportUI.transform.Find("OuterBattleField");
        Transform teleportCaliosChurchUI = teleportUI.transform.Find("CaliosChurch");
        Transform teleportTopOfCastleUI = teleportUI.transform.Find("TopOfCastle");
        Transform teleportMiddleOfCastleUI = teleportUI.transform.Find("MiddleOfCastle");
        Transform teleportMageBossRoomUI = confirmUI.transform.Find("AcceptButton");
        Transform teleportCancelUI = confirmUI.transform.Find("RefuseButton");
        Transform teleportFinalBossRoomUI = warningUI.transform.Find("FAcceptButton");
        Transform teleportFCancelUI = warningUI.transform.Find("FRefuseButton");
        Transform quitConfirnUI = quitUI.transform.Find("AcceptQuitButton");
        Transform quitRefuseUI = quitUI.transform.Find("RefuseQuitButton");
        Transform hpButton = statusUI.transform.Find("HPButton");
        Transform mnButton = statusUI.transform.Find("MNButton");
        Transform stButton = statusUI.transform.Find("STButton");
        Transform strButton = statusUI.transform.Find("STRButton");

        if (quitConfirnUI != null)
        {
            Button[] statusButtons = quitConfirnUI.GetComponentsInChildren<Button>(); // StatusMenu�� ��� ��ư ��������
            foreach (Button button in statusButtons)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(QuitGame); // ShowStatusUI �޼��� ����
            }
        }
        else
        {
            Debug.LogWarning("quitConfirnUI not found in StateUI!");
        }

        if (quitRefuseUI != null)
        {
            Button[] statusButtons = quitRefuseUI.GetComponentsInChildren<Button>(); // StatusMenu�� ��� ��ư ��������
            foreach (Button button in statusButtons)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(GoBack); // ShowStatusUI �޼��� ����
            }
        }
        else
        {
            Debug.LogWarning("quitConfirnUI not found in StateUI!");
        }

        if (teleportMageBossRoomUI != null)
        {
            int toggleIndex = PlayerPrefs.GetInt("toggleTeleporter");

            Button[] statusButtons = teleportMageBossRoomUI.GetComponentsInChildren<Button>(); // StatusMenu�� ��� ��ư ��������
            foreach (Button button in statusButtons)
            {
                if (toggleIndex == 0)
                {
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(teleportMageBossRoom); // ShowStatusUI �޼��� ����
                    PlayerPrefs.SetInt("toggleTeleporter", 1);
                    PlayerPrefs.Save();
                }
                else
                {
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(teleportMageForcedTeleporter);
                    PlayerPrefs.SetInt("toggleTeleporter", 0);
                    PlayerPrefs.Save();
                }
            }
        }
        else
        {
            Debug.LogWarning("AcceptButton not found in StateUI!");
        }

        if (teleportFinalBossRoomUI != null)
        {
            int toggleIndex = PlayerPrefs.GetInt("toggleFTeleporter");

            Button[] statusButtons = teleportFinalBossRoomUI.GetComponentsInChildren<Button>(); // StatusMenu�� ��� ��ư ��������
            foreach (Button button in statusButtons)
            {
                if (toggleIndex == 0)
                {
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(teleportFinalBossRoom); // ShowStatusUI �޼��� ����
                    PlayerPrefs.SetInt("toggleFTeleporter", 1);
                    PlayerPrefs.Save();
                }
                else
                {
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(teleportCaliosChurch);
                    PlayerPrefs.SetInt("toggleFTeleporter", 0);
                    PlayerPrefs.Save();
                }
            }
        }
        else
        {
            Debug.LogWarning("AcceptButton not found in StateUI!");
        }

        if (teleportCancelUI != null)
        {
            Button[] statusButtons = teleportCancelUI.GetComponentsInChildren<Button>(); // StatusMenu�� ��� ��ư ��������
            foreach (Button button in statusButtons)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(GoBack); // ShowStatusUI �޼��� ����
            }
        }
        else
        {
            Debug.LogWarning("RefuseButton not found in StateUI!");
        }

        if (teleportFCancelUI != null)
        {
            Button[] statusButtons = teleportFCancelUI.GetComponentsInChildren<Button>(); // StatusMenu�� ��� ��ư ��������
            foreach (Button button in statusButtons)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(GoBack); // ShowStatusUI �޼��� ����
            }
        }
        else
        {
            Debug.LogWarning("RefuseButton not found in StateUI!");
        }

        if (hpButton != null)
        {
            Button[] statusButtons = hpButton.GetComponentsInChildren<Button>(); // StatusMenu�� ��� ��ư ��������
            foreach (Button button in statusButtons)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(IncreaseLife); // ShowStatusUI �޼��� ����
            }
        }
        else
        {
            Debug.LogWarning("StatusMenu not found in StateUI!");
        }

        if (mnButton != null)
        {
            Button[] statusButtons = mnButton.GetComponentsInChildren<Button>(); // StatusMenu�� ��� ��ư ��������
            foreach (Button button in statusButtons)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(IncreaseMental); // ShowStatusUI �޼��� ����
            }
        }
        else
        {
            Debug.LogWarning("StatusMenu not found in StateUI!");
        }

        if (stButton != null)
        {
            Button[] statusButtons = stButton.GetComponentsInChildren<Button>(); // StatusMenu�� ��� ��ư ��������
            foreach (Button button in statusButtons)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(IncreaseEndurance); // ShowStatusUI �޼��� ����
            }
        }
        else
        {
            Debug.LogWarning("StatusMenu not found in StateUI!");
        }

        if (strButton != null)
        {
            Button[] statusButtons = strButton.GetComponentsInChildren<Button>(); // StatusMenu�� ��� ��ư ��������
            foreach (Button button in statusButtons)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(IncreaseForce); // ShowStatusUI �޼��� ����
            }
        }
        else
        {
            Debug.LogWarning("StatusMenu not found in StateUI!");
        }

        if (teleportBurningGroundUI != null)
        {
            Button[] statusButtons = teleportBurningGroundUI.GetComponentsInChildren<Button>(); // StatusMenu�� ��� ��ư ��������
            foreach (Button button in statusButtons)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(teleportBurningGround); // ShowStatusUI �޼��� ����
            }
        }
        else
        {
            Debug.LogWarning("StatusMenu not found in StateUI!");
        }

        if (teleportCaliosBridgeUI != null)
        {
            Button[] statusButtons = teleportCaliosBridgeUI.GetComponentsInChildren<Button>(); // StatusMenu�� ��� ��ư ��������
            foreach (Button button in statusButtons)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(teleportCaliosBridge); // ShowStatusUI �޼��� ����
            }
        }
        else
        {
            Debug.LogWarning("StatusMenu not found in StateUI!");
        }

        if (teleportOuterBattleFieldUI != null)
        {
            Button[] statusButtons = teleportOuterBattleFieldUI.GetComponentsInChildren<Button>(); // StatusMenu�� ��� ��ư ��������
            foreach (Button button in statusButtons)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(teleportOuterBattleField); // ShowStatusUI �޼��� ����
            }
        }
        else
        {
            Debug.LogWarning("StatusMenu not found in StateUI!");
        }

        if (teleportCaliosChurchUI != null)
        {
            Button[] statusButtons = teleportCaliosChurchUI.GetComponentsInChildren<Button>(); // StatusMenu�� ��� ��ư ��������
            foreach (Button button in statusButtons)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(teleportCaliosChurch); // ShowStatusUI �޼��� ����
            }
        }
        else
        {
            Debug.LogWarning("StatusMenu not found in StateUI!");
        }

        if (teleportTopOfCastleUI != null)
        {
            Button[] statusButtons = teleportTopOfCastleUI.GetComponentsInChildren<Button>(); // StatusMenu�� ��� ��ư ��������
            foreach (Button button in statusButtons)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(teleportTopOfCastle); // ShowStatusUI �޼��� ����
            }
        }
        else
        {
            Debug.LogWarning("StatusMenu not found in StateUI!");
        }

        if (teleportMiddleOfCastleUI != null)
        {
            Button[] statusButtons = teleportMiddleOfCastleUI.GetComponentsInChildren<Button>(); // StatusMenu�� ��� ��ư ��������
            foreach (Button button in statusButtons)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(teleportMiddleOfCastle); // ShowStatusUI �޼��� ����
            }
        }
        else
        {
            Debug.LogWarning("StatusMenu not found in StateUI!");
        }

        if (statusMenuTransform != null)
        {
            Button[] statusButtons = statusMenuTransform.GetComponentsInChildren<Button>(); // StatusMenu�� ��� ��ư ��������
            foreach (Button button in statusButtons)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(ShowStatusUI); // ShowStatusUI �޼��� ����
            }
        }
        else
        {
            Debug.LogWarning("StatusMenu not found in StateUI!");
        }

        if (equipmentMenuTransform != null)
        {
            Button[] equipmentButtons = equipmentMenuTransform.GetComponentsInChildren<Button>(); // EquipmentMenu�� ��� ��ư ��������
            foreach (Button button in equipmentButtons)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(ShowEquipmentUI); // ShowEquipmentUI �޼��� ����
                Debug.Log($"Button {button.name} �̺�Ʈ ��ϵ�.");
            }
        }
        else
        {
            Debug.LogWarning("EquipmentMenu not found in StateUI!");
        }
    }

    public void AccessController()
    {
        playerController = FindObjectOfType<SuperPlayerController>();
        equipmentManager = FindObjectOfType<EquipmentManager>();

        if (playerController == null)
        {
            Debug.LogError("PlayerController not found in the scene.");
        }
        if (equipmentManager == null)
        {
            Debug.LogError("EquipmentManager not found in the scene.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleMenu();
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            GoBack();
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            OpenTeleport();
            LoadSaveButtonState();
        }
    }

    public void FindUIElements()
    {
        playerUI = GameObject.Find("PlayerUI"); //����ȭ�� UI �÷��̾� �⺻ �������ͽ�
        weaponUI = GameObject.Find("WeaponUI"); //����ȭ�� UI �÷��̾� ���� ���÷���
        stateUI = GameObject.Find("StateUI"); //�޴� UI �������ͽ�UI�� ���âUI���� ����
        statusUI = GameObject.Find("StatusUI"); //�������ͽ�â UI
        equipmentUI = GameObject.Find("EquipmentUI"); //���â UI
        teleportUI = GameObject.Find("TeleportUI"); //�ڷ���Ʈ UI
        messageUI = GameObject.Find("MessageUI"); //���� �޼��� UI
        confirmUI = GameObject.Find("ConfirmUI"); //���� Ȯ�� �޼��� UI
        warningUI = GameObject.Find("WarningUI"); //���� ��� �޼��� UI
        quitUI = GameObject.Find("QuitUI"); //���� ���� UI

        if (playerUI == null) Debug.LogWarning("PlayerUI not found!");
        if (weaponUI == null) Debug.LogWarning("WeaponUI not found!");
        if (stateUI == null) Debug.LogWarning("StateUI not found!");
        if (statusUI == null) Debug.LogWarning("StatusUI not found!");
        if (equipmentUI == null) Debug.LogWarning("EquipmentUI not found!");
        if (teleportUI == null) Debug.LogWarning("TeleportUI not found!");
        if (messageUI == null) Debug.LogWarning("MessageUI not found!");
        if (confirmUI == null) Debug.LogWarning("ConfirmUI not found!");
        if (warningUI == null) Debug.LogWarning("WarningU not found!");
        if (quitUI == null) Debug.LogWarning("quitUI not found!");
    }

    public void SetInitialUIState()
    {
        if (playerUI != null) playerUI.SetActive(true);
        if (weaponUI != null) weaponUI.SetActive(true);
        if (stateUI != null) stateUI.SetActive(false);
        if (statusUI != null) statusUI.SetActive(false);
        if (equipmentUI != null) equipmentUI.SetActive(false);
        if (teleportUI != null) teleportUI.SetActive(false);
        if (messageUI != null) messageUI.SetActive(false);
        if (confirmUI != null) confirmUI.SetActive(false);
        if (warningUI != null) warningUI.SetActive(false);
        if (quitUI != null) quitUI.SetActive(false);
    }

    private void ToggleMenu()
    {
        switch (currentState)
        {
            case UIState.Game:
                currentState = UIState.Menu;
                OpenMenu();
                break;
            case UIState.Menu:
            case UIState.Status:
            case UIState.Equipment:
            case UIState.Teleport:
            case UIState.Message:
            case UIState.Confirm:
            case UIState.Warning:
            case UIState.Quit:
                break;
        }
    }

    public void OpenTeleport()
    {
        currentState = UIState.Teleport;

        if (playerController != null) playerController.canAttack = false;

        if (playerUI != null) playerUI.SetActive(false);
        if (weaponUI != null) weaponUI.SetActive(false);
        if (stateUI != null) stateUI.SetActive(false);
        if (statusUI != null) statusUI.SetActive(false);
        if (equipmentUI != null) equipmentUI.SetActive(false);
        if (teleportUI != null) teleportUI.SetActive(true);
        if (messageUI != null) messageUI.SetActive(false);
        if (confirmUI != null) confirmUI.SetActive(false);
        if (warningUI != null) warningUI.SetActive(false);
        if (quitUI != null) quitUI.SetActive(false);

        if (audioSource != null && keyClickSound != null) { audioSource.PlayOneShot(keyClickSound); }
        else { Debug.LogWarning("AudioSource or KeyClickSound is not assigned."); }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OpenMessage()
    {
        currentState = UIState.Message;

        if (playerUI != null) playerUI.SetActive(false);
        if (weaponUI != null) weaponUI.SetActive(false);
        if (stateUI != null) stateUI.SetActive(false);
        if (statusUI != null) statusUI.SetActive(false);
        if (equipmentUI != null) equipmentUI.SetActive(false);
        if (teleportUI != null) teleportUI.SetActive(false);
        if (messageUI != null) messageUI.SetActive(true);
        if (confirmUI != null) confirmUI.SetActive(false);
        if (warningUI != null) warningUI.SetActive(false);
        if (quitUI != null) quitUI.SetActive(false);

        if (audioSource != null && keyClickSound != null) { audioSource.PlayOneShot(keyClickSound); }
        else { Debug.LogWarning("AudioSource or KeyClickSound is not assigned."); }
    }

    public void OpenConfirm()
    {
        currentState = UIState.Confirm;

        if (playerUI != null) playerUI.SetActive(false);
        if (weaponUI != null) weaponUI.SetActive(false);
        if (stateUI != null) stateUI.SetActive(false);
        if (statusUI != null) statusUI.SetActive(false);
        if (equipmentUI != null) equipmentUI.SetActive(false);
        if (teleportUI != null) teleportUI.SetActive(false);
        if (messageUI != null) messageUI.SetActive(false);
        if (confirmUI!= null) confirmUI.SetActive(true);
        if (warningUI != null) warningUI.SetActive(false);
        if (quitUI != null) quitUI.SetActive(false);

        if (audioSource != null && keyClickSound != null) { audioSource.PlayOneShot(keyClickSound); }
        else { Debug.LogWarning("AudioSource or KeyClickSound is not assigned."); }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OpenWarning()
    {
        currentState = UIState.Warning;

        if (playerUI != null) playerUI.SetActive(false);
        if (weaponUI != null) weaponUI.SetActive(false);
        if (stateUI != null) stateUI.SetActive(false);
        if (statusUI != null) statusUI.SetActive(false);
        if (equipmentUI != null) equipmentUI.SetActive(false);
        if (teleportUI != null) teleportUI.SetActive(false);
        if (messageUI != null) messageUI.SetActive(false);
        if (confirmUI != null) confirmUI.SetActive(false);
        if (warningUI != null) warningUI.SetActive(true);
        if (quitUI != null) quitUI.SetActive(false);

        if (audioSource != null && keyClickSound != null) { audioSource.PlayOneShot(keyClickSound); }
        else { Debug.LogWarning("AudioSource or KeyClickSound is not assigned."); }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OpenQuit()
    {
        currentState = UIState.Quit;

        if (playerUI != null) playerUI.SetActive(false);
        if (weaponUI != null) weaponUI.SetActive(false);
        if (stateUI != null) stateUI.SetActive(false);
        if (statusUI != null) statusUI.SetActive(false);
        if (equipmentUI != null) equipmentUI.SetActive(false);
        if (teleportUI != null) teleportUI.SetActive(false);
        if (messageUI != null) messageUI.SetActive(false);
        if (confirmUI != null) confirmUI.SetActive(false);
        if (warningUI != null) warningUI.SetActive(false);
        if (quitUI != null) quitUI.SetActive(true);

        if (audioSource != null && keyClickSound != null) { audioSource.PlayOneShot(keyClickSound); }
        else { Debug.LogWarning("AudioSource or KeyClickSound is not assigned."); }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OpenMenu() // E��ư : �޴�â Ȱ��ȭ, ������ ���� ��Ȱ��ȭ, ���콺 Ŀ�� Ȱ��ȭ
    {
        if (playerController != null) playerController.canAttack = false;

        if (playerUI != null) playerUI.SetActive(false);
        if (weaponUI != null) weaponUI.SetActive(false);
        if (stateUI != null) stateUI.SetActive(true);
        if (statusUI != null) statusUI.SetActive(false);
        if (equipmentUI != null) equipmentUI.SetActive(false);
        if (equipmentUI != null) teleportUI.SetActive(false);
        if (messageUI != null) messageUI.SetActive(false);
        if (confirmUI != null) confirmUI.SetActive(false);
        if (warningUI != null) warningUI.SetActive(false);
        if (quitUI != null) quitUI.SetActive(false);

        if (audioSource != null && keyClickSound != null) { audioSource.PlayOneShot(keyClickSound); }
        else { Debug.LogWarning("AudioSource or KeyClickSound is not assigned."); }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ShowStatusUI() // �������ͽ�â Ȱ��ȭ, �޴�â ��Ȱ��ȭ
    {
        Debug.Log("�������ͽ� ����");
        stateUI.SetActive(false);
        statusUI.SetActive(true);

        if (audioSource != null && clickSound != null) { audioSource.PlayOneShot(clickSound); }
        else { Debug.LogWarning("AudioSource or ClickSound is not assigned."); }

        currentState = UIState.Status;
    }

    public void ShowEquipmentUI() // ���â Ȱ��ȭ, �޴�â ��Ȱ��ȭ
    {
        Debug.Log("��� ����");
        stateUI.SetActive(false);
        equipmentUI.SetActive(true);

        if (audioSource != null && clickSound != null) { audioSource.PlayOneShot(clickSound); }
        else { Debug.LogWarning("AudioSource or ClickSound is not assigned."); }

        currentState = UIState.Equipment;

        if (equipmentManager != null)
        {
            equipmentManager.ToggleEquipmentMenu();
        }
    }

    private void GoBack() // Q��ư : �ڷΰ���
    {
        switch (currentState)
        {
            case UIState.Menu:
                currentState = UIState.Game;
                CloseMenu();
                break;
            case UIState.Status:
                currentState = UIState.Menu;
                OpenMenu();
                break;
            case UIState.Equipment:
                currentState = UIState.Menu;
                OpenMenu();
                break;
            case UIState.Teleport:
                currentState = UIState.Game;
                CloseMenu();
                break;
            case UIState.Message:
                currentState = UIState.Game;
                CloseMenu();
                break;
            case UIState.Confirm:
                currentState = UIState.Game;
                CloseMenu();
                break;
            case UIState.Warning:
                currentState = UIState.Game;
                CloseMenu();
                break;
            case UIState.Quit:
                currentState = UIState.Game;
                CloseMenu();
                break;
        }
    }

    private void CloseMenu()
    {
        if (playerController != null) playerController.canAttack = true;

        if (playerUI != null) playerUI.SetActive(true);
        if (weaponUI != null) weaponUI.SetActive(true);
        if (stateUI != null) stateUI.SetActive(false);
        if (statusUI != null) statusUI.SetActive(false);
        if (equipmentUI != null) equipmentUI.SetActive(false);
        if (equipmentUI != null) teleportUI.SetActive(false);
        if (messageUI != null) messageUI.SetActive(false);
        if (confirmUI != null) confirmUI.SetActive(false);
        if (warningUI != null) warningUI.SetActive(false);
        if (quitUI != null) quitUI.SetActive(false);

        if (audioSource != null && clickSound != null) { audioSource.PlayOneShot(clickSound); }
        else { Debug.LogWarning("AudioSource or ClickSound is not assigned."); }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void InitializeCanLevelUp()
    {
        canHPLevelUp = true;
        canMNLevelUP = true;
        canSTLevelUp = true;
        canSTRLevelUp = true;
    }
}