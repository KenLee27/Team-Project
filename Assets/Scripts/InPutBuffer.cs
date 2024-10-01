using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InPutBuffer : MonoBehaviour
{
    private Queue<(KeyCode key, float time)> inputBuffer = new Queue<(KeyCode, float)>();
    public float bufferTime = 0.2f; // 선입력을 기억할 시간

    void Update()
    {
        // 버퍼 시간 초과한 입력 제거
        while (inputBuffer.Count > 0 && Time.time - inputBuffer.Peek().time > bufferTime)
        {
            inputBuffer.Dequeue();
            Debug.Log("버퍼 시간 다됬어~");
        }
    }

    // 기존 입력을 제거하고 새로운 입력을 버퍼에 추가하는 메서드
    public void ReplaceBufferedInput(KeyCode key)
    {
        // 버퍼 초기화 (기존 입력 제거)
        inputBuffer.Clear();

        // 새로운 입력을 버퍼에 추가
        inputBuffer.Enqueue((key, Time.time));
        Debug.Log("ReplaceBufferedInput 호출됨, 저장된 키: " + key);
    }

    // 버퍼에서 입력을 가져오는 메서드
    public bool GetBufferedInput(out KeyCode key)
    {
        if (inputBuffer.Count > 0)
        {
            key = inputBuffer.Dequeue().key;
            Debug.Log("GetBufferedInput - 유효한 버퍼 입력 반환: " + key);
            return true;
        }
        else
        {

            key = KeyCode.None;
            return false;
        }
    }
}
