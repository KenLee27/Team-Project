using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    private static GameStateManager instance;
    private bool isGamePaused = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // ���� ����Ǿ �ı����� �ʵ��� ����
        }
        else
        {
            Destroy(gameObject); // �̹� �ν��Ͻ��� ������ ��� �ߺ� ����
        }
    }

    public static void PauseGame()
    {
        if (!instance.isGamePaused)
        {
            Time.timeScale = 0f; // ���� �Ͻ� ����
            instance.isGamePaused = true;
            Debug.Log("������ �Ͻ� �����Ǿ����ϴ�.");
        }
    }

    public static void ResumeGame()
    {
        if (instance.isGamePaused)
        {
            Time.timeScale = 1f; // ���� �簳
            instance.isGamePaused = false;
            Debug.Log("������ �簳�Ǿ����ϴ�.");
        }
    }

    public static bool IsGamePaused()
    {
        return instance.isGamePaused; // ���� ���� �Ͻ� ���� ���� ��ȯ
    }
}