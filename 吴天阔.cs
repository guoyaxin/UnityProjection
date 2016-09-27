/********************************************
***************** 男孩无衣 *******************
* 功能描述: 数据库工具类
*********************************************/

using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class ShareDataBase
{
    // 1.单例化
    public static ShareDataBase sDb = new ShareDataBase ();

    /// <summary>
    /// 数据库连接对象
    /// </summary>
    SqliteConnection m_con;
    /// <summary>
    /// 命令执行对象
    /// </summary>
    SqliteCommand m_command;

    /// <summary>
    /// 接收查询返回值的对象
    /// </summary>
    SqliteDataReader m_reader;

    /// <summary>
    /// 打开相应路径的文件
    /// </summary>
    private ShareDataBase ()
    {
        //数据库连接路径
        string dataPath = "Data Source = " +
                          Application.streamingAssetsPath +
                          "/UserDataBase.sqlite";
        //如果路径为空，则新添加一个路径
        try {
            if (m_con == null) {
                m_con = new SqliteConnection (dataPath);
            }
        } 
        //如果打开错误则抛出一个异常
        catch (SqliteException ex) {
            Debug.Log (ex);
        }
        
        try {
            //连接成功后，调用CreateCommand方法，让连接数据库的对象
            //调用执行的方法，然后用一个执行对象接收（只是为了减少代码量）
            //这个对象里有连接数据库的方法
            m_command = m_con.CreateCommand ();
        } catch (SqliteException ex) {
            Debug.Log (ex);
        }
        sDb = this;
    }


    #region 私有方法

    /// <summary>
    /// 打开数据库
    /// </summary>
    private void OpenConnectDataBase ()
    {
        try {
            m_con.Open ();
        } catch (SqliteException ex) {
            Debug.Log (ex);
        }
    }

    /// <summary>
    /// 关闭数据库
    /// </summary>
    private void CloseConnectDataBase ()
    {
        try {
            m_con.Close ();
        } catch (SqliteException ex) {
            Debug.Log (ex);
        }
    }

    #endregion

    #region 公开方法

    /// <summary>
    /// 适用于没有返回值的sql命令,如:
    /// <创建表><删除表><插入数据><删除数据><更改数据>
    /// </summary>
    /// <param name="query">Query.需要执行的Sql语句</param>
    public void ExecSql (string query)
    {
        // 打开数据库
        OpenConnectDataBase ();

        try {
            //CommandText方法是连接SQLiteManager中SQL面板里的输入框，
            m_command.CommandText = query;
            //执行方法
            m_command.ExecuteNonQuery ();
        } catch (SqliteException ex) {
            Debug.Log (ex);
        }

        // 关闭数据库
        CloseConnectDataBase ();
    }

    /// <summary>
    /// 查询返回一个单元格
    /// <查属性><查条数>
    /// </summary>
    /// <returns>The field sql.</returns>
    /// <param name="query">Query.</param>
    public object SelectFieldSql (string query)
    {
        OpenConnectDataBase ();
        //定义一个返回值类型
        object obj = new object ();

        try {
            //把query的信息复制给命令语句
            m_command.CommandText = query;
            //返回查询结果的第一个值
            obj = m_command.ExecuteScalar ();
        } catch (SqliteException ex) {
            Debug.Log (ex);
        }

        CloseConnectDataBase ();

        return obj;
    }

    /// <summary>
    /// 查询多行多列数据
    /// </summary>
    /// <returns>The result sql.</returns>
    public List<ArrayList> SelectResultSql (string query)
    {
        OpenConnectDataBase ();

        List <ArrayList> list = new List<ArrayList> ();

        try {
            m_command.CommandText = query;
            //查询所有结果
            m_reader = m_command.ExecuteReader ();

            while (m_reader.Read ()) {
                ArrayList alist = new ArrayList ();
                for (int i = 0; i < m_reader.FieldCount; i++) {
                    alist.Add (m_reader.GetValue (i));
                }
                list.Add (alist);
            }
            m_reader.Close ();
        } catch (SqliteException ex) {
            Debug.Log (ex);
        }

        CloseConnectDataBase ();

        return list;
    }

    /// <summary>
    /// 将一个object对象序列化，返回一个byte[]
    /// </summary>
    /// <returns>The to bytes.</returns>
    /// <param name="obj">Object.</param>
    public byte[] OhjectToBytes (object obj)
    {
        //using
        using (MemoryStream ms = new MemoryStream ()) {
            IFormatter formatter = new BinaryFormatter ();
            formatter.Serialize (ms, obj);
            return ms.GetBuffer ();
        }
    }
    //将一个byte[]转换为一个object
    public object BytesToObject (byte[] Bytes)
    {
        using (MemoryStream ms = new MemoryStream (Bytes)) {
            IFormatter formatter = new BinaryFormatter ();
            return formatter.Deserialize (ms);
        }
    }

    #endregion

    // Command的查询方法
    // ExecuteNonQuery () :返回受到影响的行数
    // ExecuteScalar () : 返回查询结果的第一个值.
    // ExecuteReader () : 返回所有查询结果
    /////////////////////////////////////////
    public void InsertData (string name, int age, byte[] info)
    {

        OpenConnectDataBase ();

        string sql = String.Format ("insert into tmp1 (name,age ,info)values (@name,@age,@info)");
        SqliteParameter[] parameters = new SqliteParameter[] {
            new SqliteParameter ("@name", name),
            new SqliteParameter ("@age", age.ToString ()),
            new SqliteParameter ("@info", info)
        };
        m_command.Parameters.AddRange (parameters);
        m_command.CommandText = sql;
        m_command.ExecuteNonQuery ();
        CloseConnectDataBase ();

    }

}
