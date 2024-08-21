using BackEnd;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class UserData
{
    public int level = 1;
    public float atk = 3.5f;
    public string info = string.Empty;
    public List<string> equipment = new List<string>();
    public Dictionary<string, int> inventory = new Dictionary<string, int>();

    public UserData() { }    
    public UserData(int level, float atk, string info)
    {
        this.level = level;
        this.atk = atk;
        this.info = info;
    }

    public override string ToString()
    {
        StringBuilder result = new StringBuilder();
        result.AppendLine($"level : {level}");
        result.AppendLine($"atk : {atk}");
        result.AppendLine($"info : {info}");

        result.AppendLine($"inventory");
        foreach (var itemKey in inventory.Keys)
        {
            result.AppendLine($"| {itemKey} : {inventory[itemKey]}개");
        }

        result.AppendLine($"equipment");
        foreach (var equip in equipment)
        {
            result.AppendLine($"| {equip}");
        }        

        return result.ToString();
    }
}

public class BackendGameData : MonoBehaviour
{
    private static BackendGameData instance = null;

    public static BackendGameData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new BackendGameData();
            }
            return instance;
        }
    }

    public static UserData userData;

    private string gameDataRowInDate = string.Empty;

    public void GameDataInsert()
    {
        // 게임 정보 삽입
        if (userData == null)
        {
            userData = new UserData(1, 3.5f, "친추는 언제나 환영입니다.");
        }

        userData.equipment = new List<string>
        {
            "전사의 투구",
            "강철 갑옷",
            "헤르메스의 군화"
        };

        userData.inventory = new Dictionary<string, int>
        {
            { "빨간 포션", 1 },
            { "파란 포션", 1 },
            { "하얀 포션", 1 }
        };

        Debug.Log("뒤끝 업데이트 목록에 해당 데이터들을 추가합니다.");
        Param param = new Param
        {
            { "level", userData.level },
            { "atk", userData.atk },
            { "info", userData.info },
            { "equipment", userData.equipment },
            { "inventory", userData.inventory }
        };

        Debug.Log("게임 정보 데이터 삽입을 요청합니다.");
        var bro = Backend.GameData.Insert("USER_DATA", param);

        if (bro.IsSuccess())
        {
            Debug.Log($"게임 정보 데이터 삽입에 성공했습니다. {bro}");

            // 삽입한 게임 정보의 고유값
            gameDataRowInDate = bro.GetInDate();
        }
        else
        {
            Debug.LogError($"게임 정보 데이터 삽입에 실패했습니다. {bro}");
        }

        Debug.Log("데이터 초기화");
    }

    public void GameDataGet()
    {
        // 게임 정보 불러오기
        Debug.Log("게임 정보 조회 함수를 호출합니다.");

        var bro = Backend.GameData.GetMyData("USER_DATA", new Where());

        if (bro.IsSuccess())
        {
            Debug.Log($"게임 정보 조회에 성공했습니다. : {bro}"); 

            LitJson.JsonData gameDataJson = bro.FlattenRows();  // Json으로 리턴된 데이터를 받아온다.

            // 받아온 데이터의 개수가 0이라면 데이터가 존재하지 않는 것
            if (gameDataJson.Count <= 0)
            {
                Debug.LogError("데이터가 존재하지 않습니다.");
            }
            else
            {
                gameDataRowInDate = gameDataJson[0]["inDate"].ToString();   // 불러온 게임 정보의 고유값

                userData = new UserData
                    (
                        int.Parse(gameDataJson[0]["level"].ToString()),
                        float.Parse(gameDataJson[0]["atk"].ToString()),
                        gameDataJson[0]["info"].ToString()
                    );

                foreach (string itemKey in gameDataJson[0]["inventory"].Keys)
                {
                    userData.inventory.Add(itemKey, int.Parse(gameDataJson[0]["inventory"][itemKey].ToString()));
                }

                foreach (LitJson.JsonData equip in gameDataJson[0]["equipment"])
                {
                    userData.equipment.Add(equip.ToString());
                }

                Debug.Log(userData.ToString());
            }
        }
        else
        {
            Debug.LogError($"게임 정보 조회에 실패했습니다. : {bro}");
        }
    }

    public void LevelUp()
    {
        // 게임 정보 수정
        Debug.Log("레벨을 1 증가시킵니다.");
        userData.level += 1;
        userData.atk += 3.5f;
        userData.info = "내용을 변경합니다.";
    }

    public void GameDataUpdate()
    {
        // 게임 정보 수정
        if (userData == null)
        {
            Debug.LogError("서버에서 다운받거나 새로 삽입한 데이터가 존재하지 않습니다. Insert 혹은 Get을 통해 데이터를 생성해주세요");
            return;
        }

        Param param = new Param
        {
            { "level", userData.level },
            { "atk", userData.atk },
            { "info", userData.info },
            { "equipment", userData.equipment},
            { "inventory", userData.inventory }
        };

        BackendReturnObject bro = null;

        if (string.IsNullOrEmpty(gameDataRowInDate))
        {
            Debug.Log("내 제일 최신 게임 정보 데이터 수정을 요청합니다.");

            bro = Backend.GameData.Update("USER_DATA", new Where(), param);
        }
        else
        {
            Debug.Log($"{gameDataRowInDate}의 게임 정보 데이터 수정을 요청합니다.");

            bro = Backend.GameData.UpdateV2("USER_DATA", gameDataRowInDate, Backend.UserInDate, param);
        }

        if (bro.IsSuccess())
        {
            Debug.Log($"게임 정보 데이터 수정에 성공했습니다. {bro}");
        }
        else
        {
            Debug.LogError($"게임 정보 데이터 수정에 실패했습니다. {bro}");
        }

    }
}
