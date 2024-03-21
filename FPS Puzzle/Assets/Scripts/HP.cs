using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HP : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI text;
    public PlayerInfo player;
    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
        text = this.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        player = this.transform.parent.transform.parent.GetComponent<PlayerInfo>();
    }

    // Update is called once per frame
    void Update()
    {
        slider.maxValue = player.MaxHP;
        slider.value = player.HP;
        text.text = player.HP + "/" + player.MaxHP;
    }
}
