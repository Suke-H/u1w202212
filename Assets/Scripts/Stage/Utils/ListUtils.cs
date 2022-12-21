using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;  
using System.IO;

public class ListUtils
{
    public void printMap(List<List<int>> oriList, string text){
        Debug.Log(text);

        foreach (List<int> row in oriList){
            Debug.Log(string.Join(", ",row.Select(obj => obj.ToString())));
        }
    }

    public List<List<int>> initList(int x, int y, int value){

        List<List<int>> tmpList = new List<List<int>>();

        for(int i=0; i<y; i++){
            List<int> tmp = new List<int>();
            for(int j=0; j<x; j++){ tmp.Add(value);}
            tmpList.Add(tmp);
        }

        return tmpList;
    }

    public List<string[]> readCSV(string path)
    {
        //ResourcesからCSVを読み込むのに必要
        TextAsset csvFile;

        //読み込んだCSVファイルを格納
        List<string[]> csvDatas = new List<string[]>();

        /* Resouces/CSV下のCSV読み込み */
        csvFile = Resources.Load(path) as TextAsset;
        StringReader reader = new StringReader(csvFile.text);
        while (reader.Peek() > -1){
            string line = reader.ReadLine();
            csvDatas.Add(line.Split(','));
        }

        return csvDatas;

    }

    public List<List<int>> read2DListFromCSV(string path)
    {
        //一時入力用で毎回初期化する構造体とリスト
        List<List<int>> tmpList = new List<List<int>>();

        // CSVの読み込み
        List<string[]> csvDatas = readCSV(path);

        // List<List<int>>形式に移す
        for (int i = 0; i < csvDatas.Count; i++)
        {
            // 一行ずつ
            List<int> tmpRow = new List<int>();
            for (int j=0; j<csvDatas[i].Length; j++){
                tmpRow.Add(int.Parse(csvDatas[i][j]));
            }

            // 追加
            tmpList.Add(tmpRow);
        }

        return tmpList;
    }
}
