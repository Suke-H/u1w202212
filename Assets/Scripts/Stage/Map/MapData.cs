using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
[System.Serializable]
public class MapData : ScriptableObject {
    public int[] nodeOrders;
    public CustomerData[] customerDatas;
}
