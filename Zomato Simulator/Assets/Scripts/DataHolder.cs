using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHolder : MonoBehaviour
{

    public static DataHolder Instance;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
    private int _coinCount;
    public int CoinCount { get { return _coinCount; } 
        set
        {
            _coinCount = value;
            UIManager.Instance.CoinCountText.text = CoinCount.ToString();
        }
    }
}
