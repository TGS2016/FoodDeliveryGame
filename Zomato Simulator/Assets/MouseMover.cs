using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMover : MonoBehaviour
{
    public static bool drag;
    [SerializeField] Transform mainCam;
    [SerializeField] float sensi;
    [SerializeField] float lerpSpeed;

    Vector2 mouseMovement;
    PlayerInput _input;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (CommonReferences.Instance.myPlayer == null) return;

        if (_input == null)
        {
            _input = CommonReferences.Instance.myPlayer.GetComponent<PlayerInput>();
            return;
        }
        if (drag) {
            this.transform.position =Vector3.Lerp(transform.position,transform.position+ ((Vector3)_input.GetMouseDelta() )* sensi * Time.deltaTime, Time.deltaTime * lerpSpeed);
        }
        else
        {
            this.transform.position = mainCam.transform.position;
        }
    }
}
