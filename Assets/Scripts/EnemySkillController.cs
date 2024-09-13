using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkillController : MonoBehaviour
{
    public GameObject SkillPrefab;  // ��ų ������
    public GameObject quakeSkillPrefab;
    public GameObject S_quakeSkillPrefab;
    public GameObject Scratch_skill;
    public GameObject enemy;
    public Transform Player;        // �÷��̾� ��ġ
    private Transform firePoint;    // ��ų�� �߻�� ��ġ
    public float skillSpeed = 20f;  // ��ų �ӵ�
    public float skillRange = 14f;  // ��ų ��Ÿ�
    private Vector3 targetDirection;

    void Start()
    {
        // ��ų �߻� ��ġ�� �� ������Ʈ���� ã��
        firePoint = enemy.transform.Find("skill_start_position");

        if (firePoint == null)
        {
            Debug.LogWarning("no start point!");
        }
    }

    // ȭ�� ��ų�� �߻��ϴ� �Լ�
    public void arrowSkill()
    {
        if (Player == null)
        {
            Debug.LogWarning("Player transform is not assigned!");
            return;
        }

        // �÷��̾��� �Ӹ� �� ��ǥ ���� ����
        Vector3 playerHeadPosition = Player.position + Vector3.up * 1f;  // �÷��̾� �Ӹ� ���̷� ������ ����

        // �÷��̾� �������� ��ų�� ���ư����� ����
        targetDirection = (playerHeadPosition - firePoint.position).normalized;

        // ��ų �ν��Ͻ��� ����
        GameObject skillInstance = Instantiate(SkillPrefab, firePoint.position, Quaternion.identity);

        // ��ų�� ȸ���� ���� (Ÿ�� ������ �ٶ󺸵��� ȸ��)
        skillInstance.transform.rotation = Quaternion.LookRotation(targetDirection);

        // ��ų�� �̵��� �Ҹ��� ó���ϴ� ��ũ��Ʈ�� ����
        skillInstance.GetComponent<EnemySkill>().Initialize(targetDirection, skillSpeed, skillRange);
    }

    public void quakeSkill()
    {
        if (Player == null)
        {
            Debug.LogWarning("Player transform is not assigned!");
            return;
        }

        // �÷��̾��� �Ӹ� �� ��ǥ ���� ����
        Vector3 playerHeadPosition = Player.position + Vector3.up * 1f;  // �÷��̾� �Ӹ� ���̷� ������ ����

        // �÷��̾� �������� ��ų�� ���ư����� ����
        targetDirection = (playerHeadPosition - transform.position).normalized;

        // ��ų �ν��Ͻ��� ����
        GameObject skillInstance = Instantiate(quakeSkillPrefab, transform.position, Quaternion.identity);


        // ��ų�� �̵��� �Ҹ��� ó���ϴ� ��ũ��Ʈ�� ����
        skillInstance.GetComponent<quakeSkill>().Initialize(targetDirection, 0, 0);
    }

    public void S_quakeSkill()
    {
        if (Player == null)
        {
            Debug.LogWarning("Player transform is not assigned!");
            return;
        }

        // �÷��̾��� �Ӹ� �� ��ǥ ���� ����
        Vector3 playerHeadPosition = Player.position + Vector3.up * 1f;  // �÷��̾� �Ӹ� ���̷� ������ ����

        // �÷��̾� �������� ��ų�� ���ư����� ����
        targetDirection = (playerHeadPosition - transform.position).normalized;

        // ��ų �ν��Ͻ��� ����
        GameObject skillInstance = Instantiate(S_quakeSkillPrefab, transform.position, Quaternion.identity);


        // ��ų�� �̵��� �Ҹ��� ó���ϴ� ��ũ��Ʈ�� ����
        skillInstance.GetComponent<quakeSkill>().Initialize(targetDirection, 0, 0);
    }

    public void scratchSkill()
    {
        if (Player == null)
        {
            Debug.LogWarning("Player transform is not assigned!");
            return;
        }

        // �÷��̾��� �Ӹ� �� ��ǥ ���� ����
        Vector3 playerHeadPosition = Player.position + Vector3.up * 1f;  // �÷��̾� �Ӹ� ���̷� ������ ����

        // �÷��̾� �������� ��ų�� ���ư����� ����
        targetDirection = (playerHeadPosition - firePoint.position).normalized;

        // ��ų �ν��Ͻ��� ����
        GameObject skillInstance = Instantiate(Scratch_skill, firePoint.position, Quaternion.identity);

        // ��ų�� ȸ���� ���� (Ÿ�� ������ �ٶ󺸵��� ȸ��)
        skillInstance.transform.rotation = Quaternion.LookRotation(targetDirection);

        // ��ų�� �̵��� �Ҹ��� ó���ϴ� ��ũ��Ʈ�� ����
        skillInstance.GetComponent<scratchSkill>().Initialize(targetDirection, 20f, 5f);
    }

}
