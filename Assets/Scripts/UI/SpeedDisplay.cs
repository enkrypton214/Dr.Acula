using TMPro;
using UnityEngine;

public class SpeedDisplay : MonoBehaviour
{
    public TextMeshProUGUI speed;
    public Rigidbody player;
    void Update()
    {
        float movementSpeed = player.velocity.magnitude;
        speed.text = "Speed: "+ movementSpeed.ToString("F2");
    }
}
