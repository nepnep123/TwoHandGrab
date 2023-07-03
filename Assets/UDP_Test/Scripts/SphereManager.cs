using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereManager : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody characterRigidbody;
    float inputX, inputZ;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start ShpereMove");
        characterRigidbody = GetComponent<Rigidbody>();
        UdpManager udp_manager = new UdpManager("127.0.0.1", 50002);
        udp_manager.Initialize();
        udp_manager.SetMessageCallback(new CallbackDirection(OnDirection));
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = new Vector3(inputX/3, 0, inputZ/3);
        velocity *= speed;
        characterRigidbody.velocity = velocity;
    }

    void OnDirection(JsonData message)
    {
        string direction = message.direction;
        Debug.Log("CharacterMove : " + direction);
        switch (direction)
        {
            case "up":
                inputZ = 1f;
                inputX = 0f;
                break;
            case "down":
                inputZ = -1f;
                inputX = 0f;
                break;
            case "right":
                inputX = 1f;
                inputZ = 0f;
                break;
            case "left":
                inputX = -1f;
                inputZ = 0f;
                break;
        }
    }
}
