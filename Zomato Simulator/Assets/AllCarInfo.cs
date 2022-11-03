using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllCarInfo : MonoBehaviour
{
    public int carIndex;
    public string carFolder;
    public int subIndex;
    public bool addNew=false;
    public string oldReplace;
    public string[] newReplace;
    public string[] folderName;

    public List<CarInfo> allCarInfo;
    // Start is called before the first frame update
    void Start()
    {
        if (addNew)
        {

            for (int j = 0; j < folderName.Length; j++)
            {
                CarSprites newCar = new CarSprites();
                newCar.car_sprites = new List<Sprite>();
                //int currentLength= allCarInfo[carIndex].allColorSprite.Count;
                for (int i = 0; i < allCarInfo[carIndex].allColorSprite[0].car_sprites.Count; i++)
                {
                    string orignalCarSpriteName = allCarInfo[carIndex].allColorSprite[0].car_sprites[i].name;

                    string newName = orignalCarSpriteName.Replace(oldReplace, newReplace[j]);
                    Debug.Log(newName);
                    newCar.car_sprites.Add(Resources.Load<Sprite>(folderName[j] + newName) as Sprite);
                }
                allCarInfo[carIndex].allColorSprite.Add(newCar);
            }
           
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

