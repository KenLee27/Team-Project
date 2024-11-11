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
            DontDestroyOnLoad(gameObject); // 씬이 변경되어도 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재할 경우 중복 방지
        }
    }

    public static void PauseGame()
    {
        if (!instance.isGamePaused)
        {
            Time.timeScale = 0f; // 게임 일시 정지
            instance.isGamePaused = true;
            Debug.Log("게임이 일시 정지되었습니다.");
        }
    }

    public static void ResumeGame()
    {
        if (instance.isGamePaused)
        {
            Time.timeScale = 1f; // 게임 재개
            instance.isGamePaused = false;
            Debug.Log("게임이 재개되었습니다.");
        }
    }

    public static bool IsGamePaused()
    {
        return instance.isGamePaused; // 현재 게임 일시 정지 상태 반환
    }
}