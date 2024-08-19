using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float Yaxis;
    public float Xaxis;

    public Transform player;

    private float rotationSensitive = 3f;   //ī�޶� ȸ�� ����
    private float distance = 8f;            //�÷��̾���� �Ÿ�
    private float rotationMin = 5f;         //ī�޶� ���Ͽ����� �ּ� ����
    private float rotationMax = 80f;        //ī�޶� ���Ͽ����� �ִ� ����
    private float smoothTime = 0.12f;       //ī�޶� ȸ�� ���� ����ȭ ���

    private Vector3 targetRotation;
    private Vector3 currentVel;

    void LateUpdate()
    {
        Yaxis += Input.GetAxis("Mouse X") * rotationSensitive;  //���콺 �¿������ : ī�޶��� Y�� ȸ��
        Xaxis -= Input.GetAxis("Mouse Y") * rotationSensitive;  //���콺 ���Ͽ����� : ī�޶��� X�� ȸ��
        Xaxis = Mathf.Clamp(Xaxis, rotationMin, rotationMax);   //���콺 ���Ͽ����� ���� ����

        targetRotation = Vector3.SmoothDamp(targetRotation, new Vector3(Xaxis, Yaxis), ref currentVel, smoothTime);
        this.transform.eulerAngles = targetRotation;

        transform.position = player.position - transform.forward * distance;    //�÷��̾���� �Ÿ� ����
    }
}
