using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
[System.Serializable]
public class CustomerData : ScriptableObject {
    public string customerName;
    public int[] demandLv;
}

