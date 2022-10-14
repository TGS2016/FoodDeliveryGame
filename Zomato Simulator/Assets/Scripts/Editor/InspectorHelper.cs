using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Photon.Pun;

[CustomEditor(typeof(PlayerZomatoApp))]
public class InspectorHelper : Editor
{
    public override void OnInspectorGUI()
    {
        
        DrawDefaultInspector();
        PlayerZomatoApp Player = (PlayerZomatoApp)target;

        if (GUILayout.Button("Dispatch Order"))
        {
            Player.GetAnOrder();
        }
    }
}

[CustomEditor(typeof(Restaurant))]
public class RestaurantHelper : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Restaurant _restaurantSomething = (Restaurant)target;

        if (GUILayout.Button("Recieved Order"))
        {
            _restaurantSomething.OrderRecieved(0);
        }
    }
}


