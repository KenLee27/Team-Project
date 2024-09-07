using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttachment : MonoBehaviour
{
    private Transform handTransform;
    private Vector3 positionOffset = new Vector3(0.144f, -0.043f, 0.044f);
    private Vector3 rotationOffset = new Vector3(90f, -50f, -105);

    void Start()
    {
        SetHandTransform();
        AttachWeapon();
    }

    void SetHandTransform()
    {
        handTransform = GameObject.Find("Player/root/pelvis/spine_01/spine_02/spine_03/clavicle_r/upperarm_r/lowerarm_r/hand_r").transform; //캐릭터 변경시 필수 확인

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
