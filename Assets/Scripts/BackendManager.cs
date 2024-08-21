using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;

public class BackendManager : MonoBehaviour
{
    private void Start()
    {
        var bro = Backend.Initialize(); // 뒤끝 초기화

        // 뒤끝 초기화에 대한 응답값
        if (bro.IsSuccess())
        {
            Debug.Log($"초기화 성공 : {bro}");       // 성공일 경우 statusCod 204 Success
        }
        else
        {
            Debug.LogError($"초기화 실패 : {bro}");  // 실패일 경우 statucCode 400대 에러 발생
        }

        Test();
    }

    private void Test()
    {
        //BackendLogin.Instance.CustomSignUp("user1", "1234");     // 뒤끝 회원가입
        BackendLogin.Instance.CustomLogin("user1", "1234");      // 뒤끝 로그인
        //BackendLogin.Instance.UpdateNickname("짜자안");          // 뒤끝 닉네임 변경

        //BackendGameData.Instance.GameDataInsert();      // 데이터 삽입
        BackendGameData.Instance.GameDataGet();             // 데이터 불러오기

        if (BackendGameData.userData == null)
        {
            BackendGameData.Instance.GameDataInsert();
        }

        BackendGameData.Instance.LevelUp();             // 로컬에 저장된 데이터를 변경

        BackendGameData.Instance.GameDataUpdate();      // 서버에 저장된 데이터를 덮어쓰기 (변경된 부분만)

        Debug.Log("테스트를 종료합니다.");
    }
}
