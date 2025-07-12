using UnityEngine;
using UnityEngine.UI;

public class SpeedOMeter : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    

    

    private void Update()
    {
        SpeedMeter();

    }
    public void SpeedMeter()
    {
        float speedMeter = Input.GetAxis("Vertical");

        slider.value = speedMeter;


        fill.color = gradient.Evaluate(1f);
    }

    
}
