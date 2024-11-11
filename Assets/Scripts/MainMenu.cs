using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuCanvas;  // ���� �޴� ĵ����
    private bool isMenuActive = true;  // �޴� Ȱ��ȭ ���� �ʱ�ȭ (���� �� Ȱ��ȭ)
    public AudioClip buttonClickSound; 
    private AudioSource audioSource; 

    void Start()
    {
        Debug.Log("MainMenu Start �����"); // Start ���� Ȯ��
        mainMenuCanvas.SetActive(true); // ���� �޴� Ȱ��ȭ
        Time.timeScale = 0f; // ���� �Ͻ� ����
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void OnClickNewGame()
    {
        PlayClickSound(); // Ŭ�� ���� ���
        Debug.Log("�� ���� ����");
        PlayerPrefs.DeleteAll(); // PlayerPrefs �ʱ�ȭ
        StartCoroutine(StartNewGame()); // ���ο� ���� ���� �ڷ�ƾ
    }

    public void OnClickLoad()
    {
        PlayClickSound(); // Ŭ�� ���� ���
        Debug.Log("���� �ҷ�����");
        StartCoroutine(StartLoadGame()); // ���� �ҷ����� �ڷ�ƾ
    }

    private IEnumerator StartNewGame()
    {
        yield return new WaitForSecondsRealtime(0.2f); // ���� ��� �� ��� ���
        StartCoroutine(LoadScene("Old_Dock"));
    }

    private IEnumerator StartLoadGame()
    {
        yield return new WaitForSecondsRealtime(0.2f); // ���� ��� �� ��� ���
        StartCoroutine(LoadScene("Old_Dock"));
    }

    private IEnumerator LoadScene(string sceneName)
    {
        Debug.Log($"�� '{sceneName}' �ε� ����");
        mainMenuCanvas.SetActive(false); // ���� �޴� ��Ȱ��ȭ
        Time.timeScale = 1f; // ���� �簳
        yield return new WaitForSeconds(1f); // 1�� ��� (�ε� ȿ��)

        // �� �ε�
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

        // �ε尡 �Ϸ�� ������ ���
        while (!asyncLoad.isDone)
        {
            Debug.Log("�ε� ��..."); 
            yield return null; // ���� �����ӱ��� ���
        }

        Debug.Log("�� �ε� �Ϸ�"); 
    }

    public void OnClickQuit()
    {
        PlayClickSound(); // Ŭ�� ���� ���
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ��ư Ŭ�� ���� ��� �޼���
    private void PlayClickSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound); // ��� ���� ���
        }
    }


    public void ToggleMenu()
    {
        isMenuActive = true;
        mainMenuCanvas.SetActive(true);
        Time.timeScale = 0; // ���� �Ͻ� ����
    }
}