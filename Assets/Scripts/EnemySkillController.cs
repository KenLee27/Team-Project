using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkillController : MonoBehaviour
{
    public GameObject SkillPrefab;  // 스킬 프리팹
    public GameObject quakeSkillPrefab;
    public GameObject S_quakeSkillPrefab;
    public GameObject Scratch_skill;
    public GameObject enemy;
    public Transform Player;        // 플레이어 위치
    private Transform firePoint;    // 스킬이 발사될 위치
    public float skillSpeed = 20f;  // 스킬 속도
    public float skillRange = 14f;  // 스킬 사거리
    private Vector3 targetDirection;

    void Start()
    {
        // 스킬 발사 위치를 적 오브젝트에서 찾음
        firePoint = enemy.transform.Find("skill_start_position");

        if (firePoint == null)
        {
            Debug.LogWarning("no start point!");
        }
    }

    // 화살 스킬을 발사하는 함수
    public void arrowSkill()
    {
        if (Player == null)
        {
            Debug.LogWarning("Player transform is not assigned!");
            return;
        }

        // 플레이어의 머리 위 목표 지점 설정
        Vector3 playerHeadPosition = Player.position + Vector3.up * 1f;  // 플레이어 머리 높이로 오프셋 설정

        // 플레이어 방향으로 스킬이 날아가도록 설정
        targetDirection = (playerHeadPosition - firePoint.position).normalized;

        // 스킬 인스턴스를 생성
        GameObject skillInstance = Instantiate(SkillPrefab, firePoint.position, Quaternion.identity);

        // 스킬의 회전값 설정 (타겟 방향을 바라보도록 회전)
        skillInstance.transform.rotation = Quaternion.LookRotation(targetDirection);

        // 스킬에 이동과 소멸을 처리하는 스크립트를 붙임
        skillInstance.GetComponent<EnemySkill>().Initialize(targetDirection, skillSpeed, skillRange);
    }

    public void quakeSkill()
    {
        if (Player == null)
        {
            Debug.LogWarning("Player transform is not assigned!");
            return;
        }

        // 플레이어의 머리 위 목표 지점 설정
        Vector3 playerHeadPosition = Player.position + Vector3.up * 1f;  // 플레이어 머리 높이로 오프셋 설정

        // 플레이어 방향으로 스킬이 날아가도록 설정
        targetDirection = (playerHeadPosition - transform.position).normalized;

        // 스킬 인스턴스를 생성
        GameObject skillInstance = Instantiate(quakeSkillPrefab, transform.position, Quaternion.identity);


        // 스킬에 이동과 소멸을 처리하는 스크립트를 붙임
        skillInstance.GetComponent<quakeSkill>().Initialize(targetDirection, 0, 0);
    }

    public void S_quakeSkill()
    {
        if (Player == null)
        {
            Debug.LogWarning("Player transform is not assigned!");
            return;
        }

        // 플레이어의 머리 위 목표 지점 설정
        Vector3 playerHeadPosition = Player.position + Vector3.up * 1f;  // 플레이어 머리 높이로 오프셋 설정

        // 플레이어 방향으로 스킬이 날아가도록 설정
        targetDirection = (playerHeadPosition - transform.position).normalized;

        // 스킬 인스턴스를 생성
        GameObject skillInstance = Instantiate(S_quakeSkillPrefab, transform.position, Quaternion.identity);


        // 스킬에 이동과 소멸을 처리하는 스크립트를 붙임
        skillInstance.GetComponent<quakeSkill>().Initialize(targetDirection, 0, 0);
    }

    public void scratchSkill()
    {
        if (Player == null)
        {
            Debug.LogWarning("Player transform is not assigned!");
            return;
        }

        // 플레이어의 머리 위 목표 지점 설정
        Vector3 playerHeadPosition = Player.position + Vector3.up * 1f;  // 플레이어 머리 높이로 오프셋 설정

        // 플레이어 방향으로 스킬이 날아가도록 설정
        targetDirection = (playerHeadPosition - firePoint.position).normalized;

        // 스킬 인스턴스를 생성
        GameObject skillInstance = Instantiate(Scratch_skill, firePoint.position, Quaternion.identity);

        // 스킬의 회전값 설정 (타겟 방향을 바라보도록 회전)
        skillInstance.transform.rotation = Quaternion.LookRotation(targetDirection);

        // 스킬에 이동과 소멸을 처리하는 스크립트를 붙임
        skillInstance.GetComponent<scratchSkill>().Initialize(targetDirection, 20f, 5f);
    }

}
