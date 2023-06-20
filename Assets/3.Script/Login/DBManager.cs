using LitJson;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UserInfo
{
    public string id { get; private set; }
    public string pw { get; private set; }
    public UserInfo(string id, string pw)
    {
        this.id = id;
        this.pw = pw;
    }
}

public class DBManager : MonoBehaviour
{
    public static DBManager instance = null;

    // config json path
    public string DBPath = string.Empty;

    // MySql 변수들
    MySqlConnection connection;
    MySqlDataReader reader;

    UserInfo info;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        try
        {
            DBPath = Application.dataPath + "/Database";
            string serverInfo = Server_set(DBPath);
            if (serverInfo == null)
            {
                Debug.Log("SQL Server is null");
                return;
            }
            connection = new MySqlConnection(serverInfo);
            connection.Open();
            Debug.Log("MySQL Server is Open");
        }
        catch (Exception ex)
        {

        }
    }
    private string Server_set(string path)
    {
        if (!File.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        string jsonString = File.ReadAllText(path + "/DBConnectionConfig.json");
        
        JsonData itemData = JsonMapper.ToObject(jsonString);
        string serverInfo =
            $"Server= {itemData[0]["IP"]}; " +
            $"Database={itemData[0]["TableName"]}; " +
            $"Uid={itemData[0]["ID"]}; " +
            $"Pwd={itemData[0]["PW"]}; " +
            $"Port={itemData[0]["PORT"]};" +
            $"CharSet=utf8;";

        return serverInfo;
    }

    public bool Login(string id, string pw)
    {
        bool success = false;

        string sqlCommand = string.Format
                ($@"SELECT id, pw FROM user_info
                WHERE id='{id}' AND pw='{pw}';");

        using(MySqlCommand cmd = new MySqlCommand(sqlCommand, connection))
        {
            try
            {
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string name = reader.IsDBNull(0) ? string.Empty : reader["id"].ToString();
                        string pass = reader.IsDBNull(1) ? string.Empty : reader["pw"].ToString();

                        if (!name.Equals(string.Empty) && !pass.Equals(string.Empty))
                        {
                            // 로그인 성공
                            info = new UserInfo(name, pass);
                            if (!reader.IsClosed)
                            {
                                reader.Close();
                                success = true;
                                break;
                            }
                        }
                        else
                        {
                            // 로그인 실패
                            break;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                
            }
        }
        return success;
    }
}
