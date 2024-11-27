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
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void OnClickNewGame()
    {
        PlayClickSound();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        StartCoroutine(StartNewGame()); // ���ο� ���� ���� �ڷ�ƾ
    }

    public void OnClickLoad()
    {
        PlayClickSound();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        StartCoroutine(StartLoadGame()); // ���� �ҷ����� �ڷ�ƾ
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





    private IEnumerator StartNewGame()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        StartCoroutine(LoadSceneAsync("Calios"));
    }

    private IEnumerator StartLoadGame()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        StartCoroutine(LoadSceneAsync("Calios"));
    }



    

    public IEnumerator LoadSceneAsync(string nextSceneName)
    {
        LoadingManager.tips = tips; // �ε� �� ���� �� �ΰ����� ������ ����
        LoadingManager.nextSceneName = nextSceneName;

        SceneManager.LoadScene("LoadingScene");
        yield return null; // �ε� �� �ε� �� ���� ������ ���
    }

    private string[] tips = new string[]
    {
        "��Ȯ�� Ÿ�ֿ̹� ���� ������ ���ݿ� ���� �ʽ��ϴ�",
        "���δ� �� ���ڱ� �������� ���� �������� ���캸�°� ���� ���� �ֽ��ϴ�",
        "�������� ��¥�� Į�� �Ųٷ� ��� �ֽ��ϴ�",
        "��Ȯ�� ���� Ÿ�̹��� �˸� ������ �������ϴ�",
        "������ �����⸸ �ϸ� ���׹̳� ������ �ȵǼ� �������� �����Դϴ�"
    };

    private IEnumerator LoadScene(string sceneName) //�ۼ��� ������
    {
        yield return new WaitForSeconds(1f); // 1�� ��� (�ε� ȿ��)

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single); //�� �ε�(?)

        while (!asyncLoad.isDone)
        {
            Debug.Log("�ε� ��...");
            yield return null;
        }

        Debug.Log("�� �ε� �Ϸ�");
    }






    private void PlayClickSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound); // ��� ���� ���
        }
    }
}