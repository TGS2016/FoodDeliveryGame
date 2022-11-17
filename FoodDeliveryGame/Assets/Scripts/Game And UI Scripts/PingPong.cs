using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPong : MonoBehaviour
{
    [SerializeField] float timer = 0.75f;
    public void OnEnable()
    {
        LeanTween.moveLocalY(this.gameObject, 5, timer).setFrom(4).setLoopPingPong();
        LeanTween.scaleY(this.gameObject, 4 * 1.25f, timer).setFrom(4).setLoopPingPong();
    }

    public void OnDisable()
    {
        LeanTween.cancel(this.gameObject);
    }
}
