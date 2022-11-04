using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllCarInfo : MonoBehaviour
{
    #region Singleton
    public static AllCarInfo Instance;
    private void Awake()
    {
        Instance = this;
    }
    #endregion


  

    public List<CarInfo> allCarInfo;
    // Start is called before the first frame update
    private void Start()
    {
        
    }
}



[System.Serializable]
public class CarInfo
{
    public float carSpeed;
    public float carTurnTime;
    public float maxFuelCapacity;
    public float FuelBURNAmount;
    public List<CarSprites> allColorSprite = new List<CarSprites>();
}

[System.Serializable]
public class CarSprites
{
    public List<Sprite> car_sprites = new List<Sprite>();
}
