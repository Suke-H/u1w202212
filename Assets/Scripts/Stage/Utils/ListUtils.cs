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

    public (List<List<int>>, int) orderingNodes(List<List<int>> NodeMap)
    {
        // Listを-1で初期化
        List<List<int>> tmpList = initList(NodeMap[0].Count, NodeMap.Count, -1);

        int count = 0;
        
        // 「左から→上から」の順で番号を振っていく
        for (int x = 0; x < NodeMap[0].Count; x++) {
            for (int y = 0; y < NodeMap.Count; y++) {

                if (NodeMap[y][x] != -1)
                {
                    tmpList[y][x] = count;
                    count++;
                }
            }
        }

        return (tmpList, count);
    }


    public Vector2Int searchNodePos(List<List<int>> NodeOrders, int order)
    {
        for (int y = 0; y < NodeOrders.Count; y++){
            for (int x = 0; x < NodeOrders[0].Count; x++){
                if (NodeOrders[y][x] == order){
                    return new Vector2Int(x, y);
                }
            }
        }

        Debug.Log("エラーが来るぞ！！！");
        return new Vector2Int(-1, -1);
    }

    public List<int> searchNextNodeOrders(List<List<int>> EdgeMap, int currentOrder){
        List<int> nodes = new List<int>();

        for (int y = 0; y < EdgeMap.Count; y++){
            if (EdgeMap[y][0] == currentOrder){
                nodes.Add(EdgeMap[y][1]);
            }
        }

        return nodes;
    }

    // public List<Vector2Int> searchNextNodePositions(List<List<int>> NodeMap, int turn){

    //     List<Vector2Int> nodes = new List<Vector2Int>();

    //     for (int y = 0; y < NodeMap.Count; y++){
    //         if (NodeMap[y][turn+1] != -1){
    //             nodes.Add(new Vector2Int(turn+1, y));
    //         }
    //     }

    //     return nodes;
    // }

    // public List<Vector2Int> searchNextNodePositions(List<List<int>> NodeMap, List<List<int>> EdgeMap, int currentOrder){
    //     List<Vector2Int> nodes = new List<Vector2Int>();

    //     for (int y = 0; y < EdgeMap.Count; y++){
    //         if (EdgeMap[y][0] == currentOrder){
    //             nodes.Add(searchNodePos(NodeMap, EdgeMap[y][1]));
    //         }
    //     }

    //     return nodes;
    // }

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
