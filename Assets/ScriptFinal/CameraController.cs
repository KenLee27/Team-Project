using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float Yaxis;
    public float Xaxis;

    public Transform player;

    private float rotationSensitive = 3f;   //카메라 회전 감도
    private float distance = 8f;            //플레이어와의 거리
    private float rotationMin = 5f;         //카메라 상하움직임 최소 각도
    private float rotationMax = 80f;        //카메라 상하움직임 최대 각도
    private float smoothTime = 0.12f;       //카메라 회전 감도 최적화 계수

    private Vector3 targetRotation;
    private Vector3 currentVel;

    void LateUpdate()
    {
        Yaxis += Input.GetAxis("Mouse X") * rotationSensitive;  //마우스 좌우움직임 : 카메라의 Y축 회전
        Xaxis -= Input.GetAxis("Mouse Y") * rotationSensitive;  //마우스 상하움직임 : 카메라의 X축 회전
        Xaxis = Mathf.Clamp(Xaxis, rotationMin, rotationMax);   //마우스 상하움직임 각도 제한

        targetRotation = Vector3.SmoothDamp(targetRotation, new Vector3(Xaxis, Yaxis), ref currentVel, smoothTime);
        this.transform.eulerAngles = targetRotation;

        transform.position = player.position - transform.forward * distance;    //플레이어와의 거리 설정
    }
}
