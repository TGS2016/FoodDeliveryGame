using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomSample : MonoBehaviour {
	public Image cbody;
	public Image cface;
	public Image chair;
	public Image ckit;
	public Sprite[] body;
	public Sprite[] face;
	public Sprite[] hair;
	public Sprite[] kit;
	public Color[] background;

	// Use this for initialization
	void Start () {
		//RandomizeCharacter();
	}
	
	public void RandomizeCharacter(){
		cbody.sprite = body[Random.Range(0,body.Length)];
		cface.sprite = face[Random.Range(0,face.Length)];
		chair.sprite = hair[Random.Range(0,hair.Length)];
		ckit.sprite = kit[Random.Range(0,kit.Length)];
	}

	public List<Sprite> GetClientPic()
	{
		List<Sprite> ClientFeatures = new List<Sprite>();

		ClientFeatures.Add(body[Random.Range(0, body.Length)]);
		ClientFeatures.Add(face[Random.Range(0, face.Length)]);
		ClientFeatures.Add(hair[Random.Range(0, hair.Length)]);
		ClientFeatures.Add(kit[Random.Range(0, kit.Length)]);
		return ClientFeatures;
	}

    /*public int[] RandomizeValues()
    {
        int[] RandomParts = new int[4];
        RandomParts[0] = Random.Range(0, body.Length);
        RandomParts[1] = Random.Range(0, face.Length);
        RandomParts[2] = Random.Range(0, hair.Length);
        RandomParts[3] = Random.Range(0, kit.Length);
        return RandomParts;
    }*/
}
