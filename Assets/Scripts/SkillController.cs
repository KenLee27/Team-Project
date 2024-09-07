using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillController : MonoBehaviour
{
    public GameObject playerSkillPrefab;  // �÷��̾� ��ų ������
    private Transform firePoint;           // ��ų�� �߻�� ��ġ
    public float skillSpeed = 20f;        // ��ų �ӵ�
    public float skillRange = 9f;         // ��ų ��Ÿ�

    void Start()
    {
        // �ʱ�ȭ �۾� �ʿ�� ���⿡ �߰�
    }

    // �÷��̾� ��ų�� �߻��ϴ� �Լ�
    public void FireSkill()
    {
        // ��ų �ν��Ͻ��� ����
        GameObject skillInstance = Instantiate(playerSkillPrefab, firePoint.position, Quaternion.identity);

        // ��ų�� ���ư� ���� ����
        Vector3 targetDirection = transform.forward;

        // ��ų�� ȸ���� ����
        skillInstance.transform.rotation = Quaternion.LookRotation(targetDirection);

        // ��ų�� �̵��� �Ҹ��� ó���ϴ� ��ũ��Ʈ�� ����
        skillInstance.GetComponent<PlayerSkill>().Initialize(targetDirection, skillSpeed, skillRange);
    }

    public void SetFirePoint(Transform newFirePoint)
    {
        firePoint = newFirePoint;
    }

}
