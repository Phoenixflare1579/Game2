using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextUI : MonoBehaviour
{
    TextMeshProUGUI text;
    Slider slider;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        slider = transform.parent.GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = slider.value + "%";
    }
}
