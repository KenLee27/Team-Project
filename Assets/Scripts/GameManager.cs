using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public SuperPlayerController playerController;
    public ImgsFillDynamic hpBar;

    public GameObject hpSliderPrefab;     // HP 슬라이더 프리팹
    public GameObject crabHPSliderPrefab;     // HP 슬라이더 프리팹

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
        InitializeMonsters();
    }

    public void InitializeMonsters()
    {
        EnemyController2[] monsters = FindObjectsOfType<EnemyController2>(); // 모든 몬스터 탐색
        EnemyControllerClabKing[] crabKing = FindObjectsOfType<EnemyControllerClabKing>();

        foreach (var monster in monsters)
        {
            monster.InitializeHPBar(hpSliderPrefab); // 각 몬스터에 슬라이더 초기
        }
        foreach (var king in crabKing)
        {
            king.InitializeHPBar(crabHPSliderPrefab); // 각 보스에 슬라이더 초기화
        }
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
