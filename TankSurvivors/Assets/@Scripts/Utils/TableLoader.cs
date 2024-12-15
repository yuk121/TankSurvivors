using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using Unity.VisualScripting;
using System;

public class TableLoader  : Singleton<TableLoader> 
{
    // 리스트에 불러온 엑셀데이터를 저장해준다 . 엑셀 데이터 저장 형식 : 키, 데이터  / 키 = 칼럼 , 데이터 = 해당 칼럼의 데이터
    List<Dictionary<string, string>> m_table = new List<Dictionary<string, string>>();

    // 바이트 배열로 기반 파일을 읽기 위한것
    public void LoadTable(byte[] buff)
    {
        MemoryStream ms = new MemoryStream(buff);
        BinaryReader br = new BinaryReader(ms);

        //로우 수와 칼럼 수가 텍스트 제일 먼저 있으므로 먼저 값을 얻어온다.
        int rowCount = br.ReadInt32(); // 열
        int colCount = br.ReadInt32();  // 행

        // 분리된 CellData
        string[] stringData = br.ReadString().Split('\t');

        // 키 = 칼럼명
        List<string> listKey = new List<string>();

        // 이전 테이블 값이 남아있다면 삭제 , 다른 파일도 읽어올때 쓰는 리스트 변수 이므로
        m_table.Clear();

        int offset = 1; // 첫 row는 설명란 따라서 1칸 띄어서 검사

        for (int i = 0; i < rowCount; i++)
        {
            // 증가한 이유 : 엑셀데이터에 띄어져있는 칸이있어서.
            offset++;
           
            //칼럼의 행
            if (i == 0)
            {
                for (int j = 0; j < colCount - 1; j++)  // colCount - 1 인 이유 : 마지막은 줄바꿈(null)이기 때문에 불러올 필요가 없다.
                {
                    string data = stringData[offset].Replace("\\t", "\t");
                    data = data.Replace(Environment.NewLine, "");

                    listKey.Add(data);
                    offset++;
                }
            }
            // 데이터가 들어있는 행
            else
            {
                // Dictionary 하나가 컬럼의 데이터.
                Dictionary<string, string> dicData = new Dictionary<string, string>();
                for (int j = 0; j < colCount - 1; j++)
                {
                    string data = stringData[offset].Replace("\\t", "\t");   // 만일 뉴라인이 들어있는 경우 간혹 \t로 저장되어있을 수도 있기에 바꾸는 과정, 체크 역할
                    data = data.Replace(Environment.NewLine, "");

                    // 공백이 있더라도 다 때려넣어준다.
                    dicData.Add(listKey[j], data);
                    offset++;
                }
                m_table.Add(dicData);
            }
        }
        ms.Close();
        br.Close();
    }

    public string GetData(string key , int index)
    {
        string data = m_table[index][key];
        return data;
    }

    // 전체 로우의 갯수
    public int GetLength()
    {
        return m_table.Count;
    }

    public void Clear()
    {
        m_table.Clear();
    }

    // 해당 데이터를 변환해서 가져오기 위함
    // 예외 처리 안하는 이유 : 어디서 오류났는지 확인하기 위해서

    public string GetString(string key, int index)
    {
        string getStr = GetData(key, index);
        return getStr;
    }

    // Enum을 썼다는것은 문자열을 넣은것
    public T GetEnum<T> (string key , int index)
    {
        T getEnum = Utils.ToEnum<T>(GetData(key, index));
        return getEnum;
    }

    public byte GetByte(string key, int index)
    {
        byte getByte = byte.Parse(GetData(key, index));
        return getByte;
    }

    public int GetInt(string key, int index)
    {
        int getInt = int.Parse(GetData(key, index));
        return getInt;
    }

    public float GetFloat(string key , int index)
    {
        float getFloat = float.Parse(GetData(key, index));
        return getFloat;
    }

    public List<int> GetList_Int(string key, int index)
    {
        List<int> getList = new List<int>();
        string data = GetData(key, index);

        string[] splitData = data.Split('&');
        int dataInt;

        for(int i =0; i < splitData.Length; i++)
        {
            if (splitData[i] == null)
                continue;

            dataInt = int.Parse(splitData[i]);
            getList.Add(dataInt);
        }

        return getList;
    }
    public List<float> GetList_Float(string key, int index)
    {
        List<float> getList = new List<float>();
        string data = GetData(key, index);

        string[] splitData = data.Split('&');
        float dataFloat;

        for (int i = 0; i < splitData.Length; i++)
        {
            if (splitData[i] == null)
                continue;

            dataFloat = float.Parse(splitData[i]);
            getList.Add(dataFloat);
        }

        return getList;
    }
} 
