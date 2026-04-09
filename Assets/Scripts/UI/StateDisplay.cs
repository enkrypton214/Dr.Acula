using TMPro;
using UnityEngine;

public class StateDisplay : MonoBehaviour
{
    public TextMeshProUGUI state;
    public GameObject player;
    void Update()
    {
        
        state.text = "State: "+ player.GetComponent<PlayerMovement>().state;
    }
}
