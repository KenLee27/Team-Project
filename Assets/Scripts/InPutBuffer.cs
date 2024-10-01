using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InPutBuffer : MonoBehaviour
{
    private Queue<(KeyCode key, float time)> inputBuffer = new Queue<(KeyCode, float)>();
    public float bufferTime = 0.2f; // ���Է��� ����� �ð�

    void Update()
    {
        // ���� �ð� �ʰ��� �Է� ����
        while (inputBuffer.Count > 0 && Time.time - inputBuffer.Peek().time > bufferTime)
        {
            inputBuffer.Dequeue();
            Debug.Log("���� �ð� �ى��~");
        }
    }

    // ���� �Է��� �����ϰ� ���ο� �Է��� ���ۿ� �߰��ϴ� �޼���
    public void ReplaceBufferedInput(KeyCode key)
    {
        // ���� �ʱ�ȭ (���� �Է� ����)
        inputBuffer.Clear();

        // ���ο� �Է��� ���ۿ� �߰�
        inputBuffer.Enqueue((key, Time.time));
        Debug.Log("ReplaceBufferedInput ȣ���, ����� Ű: " + key);
    }

    // ���ۿ��� �Է��� �������� �޼���
    public bool GetBufferedInput(out KeyCode key)
    {
        if (inputBuffer.Count > 0)
        {
            key = inputBuffer.Dequeue().key;
            Debug.Log("GetBufferedInput - ��ȿ�� ���� �Է� ��ȯ: " + key);
            return true;
        }
        else
        {

            key = KeyCode.None;
            return false;
        }
    }
}
