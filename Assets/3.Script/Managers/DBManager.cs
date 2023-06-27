using LitJson;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    // DBName
    [SerializeField] private string tableName = "user_info";

    // config json path
    private string DBPath = string.Empty;

    // MySql 관련
    MySqlDataReader reader;
    private string serverConnInfo;

    public UserInfo info;

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

        DBPath = Application.dataPath + "/Database";
        serverConnInfo = Server_set(DBPath);
        if (serverConnInfo == null)
        {
            Debug.Log("SQL Server is null");
            return;
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

        using (MySqlConnection conn = new MySqlConnection(serverConnInfo))
        {
            conn.Open();

            string sqlCommand = string.Format
                ($@"SELECT id, pw FROM {tableName}
                WHERE id='{id}' AND pw='{pw}';");

            MySqlCommand cmd = new MySqlCommand(sqlCommand, conn);
            try
            {
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string readData_ID = reader.IsDBNull(0) ? string.Empty : reader["id"].ToString();
                        string readData_Pw = reader.IsDBNull(1) ? string.Empty : reader["pw"].ToString();

                        if (!readData_ID.Equals(string.Empty) && !readData_Pw.Equals(string.Empty))
                        {
                            // 로그인 성공
                            info = new UserInfo(readData_ID, readData_Pw);
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
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }

        return success;
    }
    public bool CheckDuplicationID(string id)
    {
        bool isDuplicated = false;

        using (MySqlConnection conn = new MySqlConnection(serverConnInfo))
        {
            conn.Open();
            string sqlCommand = $"Select id from {tableName} where id = '{id}'";

            MySqlCommand cmd = new MySqlCommand(sqlCommand, conn);

            try
            {
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string readData_ID = reader.IsDBNull(0) ? string.Empty : reader["id"].ToString();

                        if (!readData_ID.Equals(string.Empty))
                        {
                            // ID가 이미 존재함
                            if (!reader.IsClosed)
                            {
                                reader.Close();
                                isDuplicated = true;
                                break;
                            }
                        }
                        else
                        {
                            // ID가 중복되지 않음
                            isDuplicated = false;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }

        return isDuplicated;
    }
    public bool CheckValidationQueryString(string str)
    {
        bool isValid = true;

        foreach (char ch in str)
        {
            if (!(
                (ch >= 'A' && ch <= 'Z') ||
                (ch >= 'a' && ch <= 'z') ||
                (ch >= '0' && ch <= '9'))
                )
            {
                isValid = false;
                break;
            }
        }

        return isValid;
    }
    public bool RegisterUserInfo(string id, string pw)
    {
        bool success = false;

        using (MySqlConnection conn = new MySqlConnection(serverConnInfo))
        {
            conn.Open();
            string sqlCommand = $"insert into {tableName} Values ('{id}', '{pw}')";

            MySqlCommand cmd = new MySqlCommand(sqlCommand, conn);

            try
            {
                cmd.ExecuteNonQuery();
                success = true;
            }
            catch(Exception ex)
            {
                Debug.Log(ex.Message);
                success = false;
            }
        }
        
        return success;
    }
}
