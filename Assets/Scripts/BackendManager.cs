using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;

public class BackendManager : MonoBehaviour
{
    private void Start()
    {
        var bro = Backend.Initialize(); // �ڳ� �ʱ�ȭ

        // �ڳ� �ʱ�ȭ�� ���� ���䰪
        if (bro.IsSuccess())
        {
            Debug.Log($"�ʱ�ȭ ���� : {bro}");       // ������ ��� statusCod 204 Success
        }
        else
        {
            Debug.LogError($"�ʱ�ȭ ���� : {bro}");  // ������ ��� statucCode 400�� ���� �߻�
        }

        Test();
    }

    private void Test()
    {
        //BackendLogin.Instance.CustomSignUp("user1", "1234");     // �ڳ� ȸ������
        BackendLogin.Instance.CustomLogin("user1", "1234");      // �ڳ� �α���
        //BackendLogin.Instance.UpdateNickname("¥�ھ�");          // �ڳ� �г��� ����

        //BackendGameData.Instance.GameDataInsert();      // ������ ����
        BackendGameData.Instance.GameDataGet();             // ������ �ҷ�����

        if (BackendGameData.userData == null)
        {
            BackendGameData.Instance.GameDataInsert();
        }

        BackendGameData.Instance.LevelUp();             // ���ÿ� ����� �����͸� ����

        BackendGameData.Instance.GameDataUpdate();      // ������ ����� �����͸� ����� (����� �κи�)

        Debug.Log("�׽�Ʈ�� �����մϴ�.");
    }
}
