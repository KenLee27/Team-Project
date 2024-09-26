using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttachment : MonoBehaviour
{
    private Transform handTransform;
    private Vector3 positionOffset = new Vector3(0.0213f, 0.0227f, -0.0396f);
    private Vector3 rotationOffset = new Vector3(-35.123f, 172.116f, 52.211f);

    void Start()
    {
        SetHandTransform();
        AttachWeapon();
    }

    void SetHandTransform()
    {
        handTransform = GameObject.Find("Player/Knight_Errant_B/root/root.x/spine_01.x/spine_02.x/spine_03.x/shoulder.r/arm_stretch.r/forearm_stretch.r/hand.r/c_index1.r").transform; //캐릭터 변경시 필수 확인

        if (handTransform == null)
        {
            Debug.LogError("Hand Transform could not be found. Please check the hierarchy path.");
        }
    }

    void AttachWeapon()
    {
        if (handTransform != null)
        {
            transform.SetParent(handTransform);
            transform.localPosition = positionOffset;
            transform.localRotation = Quaternion.Euler(rotationOffset);
        }
        else
        {
            Debug.LogWarning("Hand Transform is not assigned in WeaponAttachment.");
        }
    }
}
