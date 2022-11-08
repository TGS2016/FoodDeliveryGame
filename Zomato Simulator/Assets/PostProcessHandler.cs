using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessHandler : MonoBehaviour
{

    [SerializeField] Volume volume;
    [SerializeField] Bloom bloom;





    private void OnEnable()
    {

        DayNightSystem2D.OnBloomChanged += HandleBloomChange;
    }
    private void OnDisable()
    {
        DayNightSystem2D.OnBloomChanged -= HandleBloomChange;
    }

    private void HandleBloomChange(float value)
    {

        if (volume.profile.TryGet<Bloom>(out Bloom b))
        {
            bloom = b;

            if (bloom.intensity == value) return;

            LeanTween.value(bloom.intensity.value, value, 3f).setOnUpdate((v) =>
            {
                bloom.intensity.Override(v);

            });
        }      
    }

    // Start is called before the first frame update
    void Start()
    {

    }

}
