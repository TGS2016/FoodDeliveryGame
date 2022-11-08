using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;



public enum DayCycles // Enum with day and night cycles, you can change or modify with whatever you want
{
    Sunrise = 0,
    Day = 1,
    Sunset = 2,
    Night = 3,
    Midnight = 4
}


public class DayNightSystem2D : MonoBehaviour
{
    [Header("Controllers")]
    
    [Tooltip("Global light 2D component, we need to use this object to place light in all map objects")]
    public Light2D globalLight; // global light
    
    [Tooltip("This is a current cycle time, you can change for private float but we keep public only for debug")]
    public float cycleCurrentTime = 0; // current cycle time
    
    [Tooltip("This is a cycle max time in seconds, if current time reach this value we change the state of the day and night cyles")]
    public float cycleMaxTime = 60; // duration of cycle

    [Tooltip("Enum with multiple day cycles to change over time, you can add more types and modify whatever you want to fits on your project")]
    public DayCycles dayCycle = DayCycles.Sunrise; // default cycle

    [Header("Cycle Colors")]
    
    [Tooltip("Sunrise color, you can adjust based on best color for this cycle")]
    public Gradient sunrise; // Eg: 6:00 at 10:00
    
    [Tooltip("(Mid) Day color, you can adjust based on best color for this cycle")]
    public Color day; // Eg: 10:00 at 16:00
    
    [Tooltip("Sunset color, you can adjust based on best color for this cycle")]
    public Gradient sunset; // Eg: 16:00 20:00
    
    [Tooltip("Night color, you can adjust based on best color for this cycle")]
    public Color night; // Eg: 20:00 at 00:00
    
    [Tooltip("Midnight color, you can adjust based on best color for this cycle")]
    public Color midnight; // Eg: 00:00 at 06:00

    [Header("Objects")]
    [Tooltip("Objects to turn on and off based on day night cycles, you can use this example for create some custom stuffs")]
    public Light2D[] mapLights; // enable/disable in day/night states
    public Light2D[] normalPoleLights; // enable/disable in day/night states
    public Light2D[] diffPoleLights_Circle; // enable/disable in day/night states
    public Light2D[] diffPoleLights_Freeform;

    public static Action<float> OnBloomChanged;

    void Start() 
    {
        dayCycle = DayCycles.Sunrise; // start with sunrise state
        globalLight.color = sunrise.Evaluate(1); // start global color at sunrise
    }
    bool hasToChangeBloom=true;
     void Update()
     {
        // Update cycle time
        cycleCurrentTime += Time.deltaTime;

        // Check if cycle time reach cycle duration time
        if (cycleCurrentTime >= cycleMaxTime) 
        {
            cycleCurrentTime = 0; // back to 0 (restarting cycle time)
            dayCycle++; // change cycle state
        }

        // If reach final state we back to sunrise (Enum id 0)
        if(dayCycle > DayCycles.Midnight)
            dayCycle = 0;

        // percent it's an value between current and max time to make a color lerp smooth
        float percent = cycleCurrentTime / cycleMaxTime;

        // Sunrise state (you can do a lot of stuff based on every cycle state, like enable animals only in sunrise )
        if(dayCycle == DayCycles.Sunrise)
        {
            if (percent > 0.8f) {
                if (hasToChangeBloom)
                {
                    hasToChangeBloom = false;
                    OnBloomChanged?.Invoke(0f);
                }
            }
            else
            {
                hasToChangeBloom = true;
            }
            globalLight.color = Color.Lerp(sunrise.Evaluate(1), day, percent);
        }

        // Mid Day state
        if (dayCycle == DayCycles.Day)
        {
            globalLight.color = Color.Lerp(day, sunset.Evaluate(percent), percent);            
        }

        // Sunset state
        if (dayCycle == DayCycles.Sunset)
        {
            globalLight.color = Color.Lerp(sunset.Evaluate(1), night, percent);

            if (currentStatus != true)
            {
                if (percent > 0.55f)
                {
                    ControlLightMaps(true); // disable map light (keep enable only at night)
                    if (hasToChangeBloom)
                    {
                        hasToChangeBloom = false;
                        OnBloomChanged?.Invoke(7f);
                    }

                }
                else
                {
                    hasToChangeBloom = true;

                }
            }
        }
        // Night state
        if (dayCycle == DayCycles.Night)
        {
              
            
            globalLight.color = Color.Lerp(night, midnight, percent);        
        }

        // Midnight state
        if (dayCycle == DayCycles.Midnight)
        {

            if (currentStatus != false )
            {
                if (percent > 0.75f)
                {
                    ControlLightMaps(false); // disable map light (keep enable only at night)                    
                }
            }

            globalLight.color = Color.Lerp(midnight, sunrise.Evaluate(percent), percent);
        }
     }

    bool currentStatus;
     void ControlLightMaps(bool status)
     {
        currentStatus = status;
        // loop in light array of objects to enable/disable
        if (mapLights.Length > 0)

            if (status)
            {
                LeanTween.value(0, 0.5f, 1f).setOnUpdate((value) => {
                    foreach (Light2D _light in mapLights)
                    {
                        _light.intensity = value;

                    }
                });

                LeanTween.value(0, 0.4f, 1f).setOnUpdate((value) => {
                    foreach (Light2D _light in normalPoleLights)
                    {
                        _light.intensity = value;
                    }
                });

                LeanTween.value(0, 0.18f, 1f).setOnUpdate((value) => {
                    foreach (Light2D _light in diffPoleLights_Freeform)
                    {
                        _light.intensity = value;
                    }
                });

                LeanTween.value(0, 0.6f, 1f).setOnUpdate((value) => {
                    foreach (Light2D _light in diffPoleLights_Circle)
                    {
                        _light.intensity = value;
                    }
                });

            }
            else
            {
                LeanTween.value(0.5f, 0f, 1f).setOnUpdate((value) => {
                    foreach (Light2D _light in mapLights)
                    {
                        _light.intensity = value;
                    }
                });

                LeanTween.value(0.4f, 0f, 1f).setOnUpdate((value) => {
                    foreach (Light2D _light in normalPoleLights)
                    {
                        _light.intensity = value;
                    }
                });

                LeanTween.value(0.18f, 0f, 1f).setOnUpdate((value) => {
                    foreach (Light2D _light in diffPoleLights_Freeform)
                    {
                        _light.intensity = value;
                    }
                });

                LeanTween.value(0.6f, 0, 1f).setOnUpdate((value) => {
                    foreach (Light2D _light in diffPoleLights_Circle)
                    {
                        _light.intensity = value;
                    }
                });
            }
     }

    /*void Test_ControlLightMaps(bool status)
     {
        currentStatus = status;
        // loop in light array of objects to enable/disable
        if (mapLights.Length > 0)

            if (status)
            {
               
                    foreach (Light2D _light in mapLights)
                    {
                        _light.intensity = 0.5f;

                    }
              

                
                    foreach (Light2D _light in normalPoleLights)
                    {
                        _light.intensity = 0.4f;
                    }
             

            
                    foreach (Light2D _light in diffPoleLights_Freeform)
                    {
                        _light.intensity = 0.26f;
                    }
               
                
                    foreach (Light2D _light in diffPoleLights_Circle)
                    {
                        _light.intensity = 0.5f;
                    }
            

            }
            else
            {
               
                    foreach (Light2D _light in mapLights)
                    {
                        _light.intensity = 0;
                    }
                    foreach (Light2D _light in normalPoleLights)
                    {
                        _light.intensity = 0;
                    }
                    foreach (Light2D _light in diffPoleLights_Freeform)
                    {
                        _light.intensity = 0;
                    }
                    foreach (Light2D _light in diffPoleLights_Circle)
                    {
                        _light.intensity = 0;
                    }
               
            }
     }*/

  /*  [ContextMenu("Turn On Lights")]
    private void TurnOnLights()
    {
        Test_ControlLightMaps(false);
        globalLight.color = sunrise.Evaluate(1);
    }*/
}

