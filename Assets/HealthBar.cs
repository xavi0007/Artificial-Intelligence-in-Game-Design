using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

	public Slider slider;
	public Image fill;
	public Text division;

	public void SetMaxHealth(int health)
	{
		slider.maxValue = health;
		slider.value = health;
		division.text = health.ToString() + "/" + health.ToString();
	}

	public void SetHealth(int health)
	{
		slider.value = health;
		division.text = health.ToString() + "/" + slider.maxValue.ToString();
	}

}
