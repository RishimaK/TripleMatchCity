using UnityEngine;

public class WeatherControl : MonoBehaviour
{
    public GameObject Clouds;
    public GameObject Rain;
    int randomInt = 0;

    public void SetWeather()
    {
        randomInt = Random.Range(0, 2);

        if(randomInt == 0)
        {
            Clouds.SetActive(true);
        }
        else if(randomInt == 1)
        {
            Rain.SetActive(true);
            Rain.GetComponent<ParticleSystem>().Play();
            // Rain.GetComponent<ParticleSystem>().l
        }
    }

    public void TurnOffWeather()
    {
        if(randomInt == 0)
        {
            Clouds.SetActive(false);
        }
        else if(randomInt == 1)
        {
            Rain.SetActive(false);
            Rain.GetComponent<ParticleSystem>().Stop();
            // Rain.GetComponent<ParticleSystem>().l
        }
    }
}
