using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollowPlayer : MonoBehaviour
{
    public GameObject player;
    public static bool cameraIsShaking;

    public static float shakeAmount;

    void LateUpdate()
    {
        if (cameraIsShaking)
        {
            Vector3 pos = shakeAmount * Time.fixedDeltaTime * Random.insideUnitSphere + player.transform.position;
            transform.position = pos;
        }
        else
        {
            transform.position = player.transform.position;
        }
    }
}
