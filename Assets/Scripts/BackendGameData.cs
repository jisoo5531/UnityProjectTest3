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
            result.AppendLine($"| {itemKey} : {inventory[itemKey]}��");
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
        // ���� ���� ����
        if (userData == null)
        {
            userData = new UserData(1, 3.5f, "ģ�ߴ� ������ ȯ���Դϴ�.");
        }

        userData.equipment = new List<string>
        {
            "������ ����",
            "��ö ����",
            "�츣�޽��� ��ȭ"
        };

        userData.inventory = new Dictionary<string, int>
        {
            { "���� ����", 1 },
            { "�Ķ� ����", 1 },
            { "�Ͼ� ����", 1 }
        };

        Debug.Log("�ڳ� ������Ʈ ��Ͽ� �ش� �����͵��� �߰��մϴ�.");
        Param param = new Param
        {
            { "level", userData.level },
            { "atk", userData.atk },
            { "info", userData.info },
            { "equipment", userData.equipment },
            { "inventory", userData.inventory }
        };

        Debug.Log("���� ���� ������ ������ ��û�մϴ�.");
        var bro = Backend.GameData.Insert("USER_DATA", param);

        if (bro.IsSuccess())
        {
            Debug.Log($"���� ���� ������ ���Կ� �����߽��ϴ�. {bro}");

            // ������ ���� ������ ������
            gameDataRowInDate = bro.GetInDate();
        }
        else
        {
            Debug.LogError($"���� ���� ������ ���Կ� �����߽��ϴ�. {bro}");
        }

        Debug.Log("������ �ʱ�ȭ");
    }

    public void GameDataGet()
    {
        // ���� ���� �ҷ�����
        Debug.Log("���� ���� ��ȸ �Լ��� ȣ���մϴ�.");

        var bro = Backend.GameData.GetMyData("USER_DATA", new Where());

        if (bro.IsSuccess())
        {
            Debug.Log($"���� ���� ��ȸ�� �����߽��ϴ�. : {bro}"); 

            LitJson.JsonData gameDataJson = bro.FlattenRows();  // Json���� ���ϵ� �����͸� �޾ƿ´�.

            // �޾ƿ� �������� ������ 0�̶�� �����Ͱ� �������� �ʴ� ��
            if (gameDataJson.Count <= 0)
            {
                Debug.LogError("�����Ͱ� �������� �ʽ��ϴ�.");
            }
            else
            {
                gameDataRowInDate = gameDataJson[0]["inDate"].ToString();   // �ҷ��� ���� ������ ������

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
            Debug.LogError($"���� ���� ��ȸ�� �����߽��ϴ�. : {bro}");
        }
    }

    public void LevelUp()
    {
        // ���� ���� ����
        Debug.Log("������ 1 ������ŵ�ϴ�.");
        userData.level += 1;
        userData.atk += 3.5f;
        userData.info = "������ �����մϴ�.";
    }

    public void GameDataUpdate()
    {
        // ���� ���� ����
        if (userData == null)
        {
            Debug.LogError("�������� �ٿ�ްų� ���� ������ �����Ͱ� �������� �ʽ��ϴ�. Insert Ȥ�� Get�� ���� �����͸� �������ּ���");
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
            Debug.Log("�� ���� �ֽ� ���� ���� ������ ������ ��û�մϴ�.");

            bro = Backend.GameData.Update("USER_DATA", new Where(), param);
        }
        else
        {
            Debug.Log($"{gameDataRowInDate}�� ���� ���� ������ ������ ��û�մϴ�.");

            bro = Backend.GameData.UpdateV2("USER_DATA", gameDataRowInDate, Backend.UserInDate, param);
        }

        if (bro.IsSuccess())
        {
            Debug.Log($"���� ���� ������ ������ �����߽��ϴ�. {bro}");
        }
        else
        {
            Debug.LogError($"���� ���� ������ ������ �����߽��ϴ�. {bro}");
        }

    }
}
