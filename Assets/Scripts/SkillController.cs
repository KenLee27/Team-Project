using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillController : MonoBehaviour
{
    public GameObject playerSkillPrefab;  // 플레이어 스킬 프리팹
    private Transform firePoint;           // 스킬이 발사될 위치
    public float skillSpeed = 20f;        // 스킬 속도
    public float skillRange = 9f;         // 스킬 사거리

    void Start()
    {
        // 초기화 작업 필요시 여기에 추가
    }

    // 플레이어 스킬을 발사하는 함수
    public void FireSkill()
    {
        // 스킬 인스턴스를 생성
        GameObject skillInstance = Instantiate(playerSkillPrefab, firePoint.position, Quaternion.identity);

        // 스킬이 날아갈 방향 설정
        Vector3 targetDirection = transform.forward;

        // 스킬의 회전값 설정
        skillInstance.transform.rotation = Quaternion.LookRotation(targetDirection);

        // 스킬에 이동과 소멸을 처리하는 스크립트를 붙임
        skillInstance.GetComponent<PlayerSkill>().Initialize(targetDirection, skillSpeed, skillRange);
    }

    public void SetFirePoint(Transform newFirePoint)
    {
        firePoint = newFirePoint;
    }

}
