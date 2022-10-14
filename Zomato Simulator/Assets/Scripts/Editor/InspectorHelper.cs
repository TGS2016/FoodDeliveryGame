using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(OrderDispatcher))]
    public class InspectorHelper : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        OrderDispatcher _OrderDispatcher = (OrderDispatcher)target;

            if (GUILayout.Button("Dispatch Order"))
            {
                _OrderDispatcher.DispatchOrder(0);
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


