using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public SuperPlayerController playerController;
    public ImgsFillDynamic hpBar;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // 게임 오브젝트가 씬 로드 시 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject);  // 이미 인스턴스가 존재하면 새로 생성된 객체를 파괴
        }

        InitializePlayerHP();
    }

    public void UpdatePlayerHP(float currentHP)
    {
        float hpRatio = currentHP / 100f;
        hpBar.SetValue(hpRatio);
    }

    private void InitializePlayerHP()
    {
        if (playerController != null && hpBar != null)
        {
            float initialHPRatio = playerController.PlayerHP / 100f;
            hpBar.SetValue(initialHPRatio, true); // 체력을 직접 설정
        }
    }
}
