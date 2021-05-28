using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using UnityEngine.Events;

public class WeatherController : MonoBehaviour
{
    private static WeatherController instance;
    public static WeatherController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<WeatherController>();
            }
            return instance;
        }
    }

    public UnityAction<Weather> onWeatherChanged;

    public WeatherControl weather;
    [Space]
    public Transform weatherTransform;

    public WeatherForecast CurrentForecast => weather.Forecast;

    private void Awake()
    {
        weather.onWeatherUpdated = onWeatherChanged;

        weather.Setup(GeneralAvailability.Player.Camera.Transform, weatherTransform, transform);
    }

    private void Update()
    {
        weather.RefreshWeather();
    }

    public void SetForecast(WeatherForecast forecast)
    {
        weather.Forecast = forecast;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        if (weatherTransform)
        {
            Gizmos.DrawSphere(weatherTransform.position, 0.25f);
        }
    }

    /// <summary>
    /// TODO: засунуть частицы в forecast
    /// </summary>
    [System.Serializable]
    public class WeatherControl
    {
        public UnityAction<Weather> onWeatherUpdated;

        [SerializeField] private WeatherForecast forecast;

        [Space]    
        [SerializeField] private Times updateTime;
        [ReadOnly] [SerializeField] private Weather currentWeather;

        [Space]
        public WeatherPressetSD pressetClear;
        public WeatherPressetSD pressetSnowFall;
        public WeatherPressetSD pressetFog;
        public WeatherPressetSD pressetBlizzard;

        public WeatherForecast Forecast
        {
            get
            {
                if (forecast.IsEmpty)
                {
                    forecast.Generate();
                }
                return forecast;
            }
            set
            {
                forecast = value;
            }
        }

        [ReadOnly]
        [ShowInInspector] public float Temperature => currentWeather.Temperature;

        public WeatherAir Air => currentWeather.air;
        public WeatherWind Wind => currentWeather.wind;

        private Transform currentWeatherTransform;
        private ParticleSystem currentWeatherParticle;

        private Transform origin;
        private Transform weatherTransform;
        private Transform playerTransform;

        public void Setup(Transform playerTransform, Transform weatherTransform, Transform origin)
        {
            GeneralTime.TimeUnityEvent unityEvent = new GeneralTime.TimeUnityEvent();
            unityEvent.AddEvent(GeneralTime.TimeUnityEvent.EventType.ExecuteEveryTime, updateTime, null, UpdateWeather);

            GeneralTime.Instance.AddEvent(unityEvent);

            GeneralTime.Instance.onState += RandomWeather;

            UpdateWeather(GeneralTime.Instance.globalTime);

            this.origin = origin;
            this.playerTransform = playerTransform;
            this.weatherTransform = weatherTransform;

            SetWeather(pressetClear);
        }

        public WeatherForecast GenerateForecast()
        {
            forecast.Generate();

            return forecast;
        }

        public void UpdateWeather(Times time)
        {
            currentWeather = forecast.GetWeatherByTime(time.GetSkyPercent());

            onWeatherUpdated?.Invoke(currentWeather);
        }

        public void RefreshWeather()
        {
            if (currentWeatherTransform != null)
            {
                currentWeatherTransform.position = weatherTransform.position;
                currentWeatherTransform.rotation = Quaternion.Euler(0, playerTransform.rotation.eulerAngles.y, 0);
            }
        }

        public void RandomWeather()
        {
            int value = Random.Range(0, 4);
            if (value == 0)
            {
                TransitionWeather(pressetClear);
            }
            else if (value == 1)
            {
                TransitionWeather(pressetSnowFall);
            }
            else if (value == 2)
            {
                TransitionWeather(pressetFog);
            }
            else if (value == 3)
            {
                TransitionWeather(pressetBlizzard);
            }
        }

        [Button]
        public void Clear()
        {
            TransitionWeather(pressetClear);
        }
        [Button]
        public void SnowFall()
        {
            TransitionWeather(pressetSnowFall);
        }
        [Button]
        public void Fog()
        {
            TransitionWeather(pressetFog);
        }
        [Button]
        public void Blizzard()
        {
            TransitionWeather(pressetBlizzard);
        }


        public void SetWeather(WeatherPressetSD weather)
        {
            if (currentWeatherTransform != null && currentWeatherTransform.gameObject.activeInHierarchy)
            {
                currentWeatherParticle.Stop();
                currentWeatherParticle.GetComponent<SelfDestruct>().StartDestruct();

                currentWeatherTransform = null;
                currentWeatherParticle = null;
            }

            if (weather.randomParticle)
            {
                if (weather.particles.Count > 0)
                {
                    currentWeatherTransform = ObjectPool.GetObject(weather.particles.GetRandomItem().gameObject).transform;
                    currentWeatherTransform.SetParent(origin);

                    currentWeatherParticle = currentWeatherTransform.GetComponent<ParticleSystem>();
                    currentWeatherParticle.Play();
                }
            }
            else
            {
                if (weather.particle != null)
                {
                    currentWeatherTransform = ObjectPool.GetObject(weather.particle.gameObject).transform;
                    currentWeatherTransform.SetParent(origin);

                    currentWeatherParticle = currentWeatherTransform.GetComponent<ParticleSystem>();
                    currentWeatherParticle.Play();
                }
            }


            if (weather.randomFog)
            {
                if (weather.fogs.Count > 0)
                {
                    FogPressetSD fog = weather.fogs.GetRandomItem();
                    FogController.Instance.SetFog(fog);
                }
                else
                {
                    FogController.Instance.SetFog(null);
                }
            }
            else
            {
                if (weather.fog != null)
                {
                    FogController.Instance.SetFog(weather.fog);
                }
                else
                {
                    FogController.Instance.SetFog(null);
                }
            }
        }
        public void TransitionWeather(WeatherPressetSD weather)
        {
            if (currentWeatherTransform != null && currentWeatherTransform.gameObject.activeInHierarchy)
            {
                currentWeatherParticle.Stop();
                currentWeatherParticle.GetComponent<SelfDestruct>().StartDestruct();
            }

            if (weather.randomParticle)
            {
                if (weather.particles.Count > 0)
                {
                    currentWeatherTransform = ObjectPool.GetObject(weather.particles.GetRandomItem().gameObject).transform;
                    currentWeatherTransform.SetParent(origin);

                    currentWeatherParticle = currentWeatherTransform.GetComponent<ParticleSystem>();
                    currentWeatherParticle.Play();
                }
            }
            else
            {
                if (weather.particle != null)
                {
                    currentWeatherTransform = ObjectPool.GetObject(weather.particle.gameObject).transform;
                    currentWeatherTransform.SetParent(origin);

                    currentWeatherParticle = currentWeatherTransform.GetComponent<ParticleSystem>();
                    currentWeatherParticle.Play();
                }
            }

            if (weather.randomFog)
            {
                if (weather.fogs.Count > 0)
                {
                    FogPressetSD fog = weather.fogs.GetRandomItem();

                    if (fog == null)
                    {
                        FogController.Instance.ClearFog();
                    }
                    else
                    {
                        FogController.Instance.TransitionTo(fog);
                    }
                }
                else
                {
                    FogController.Instance.ClearFog();
                }
            }
            else
            {
                if (weather.fog != null)
                {
                    FogController.Instance.TransitionTo(weather.fog);
                }
                else
                {
                    FogController.Instance.ClearFog();
                }
            }
        }
    }

    [System.Serializable]
    public class WeatherForecast
    {
        [Title("Temperature")]
        [SerializeField] private Vector2 expectedTemperature = Vector2.zero;
        [Min(0)]
        [SerializeField] private Vector2 deviation;

        [SerializeField] private AnimationCurve expectedTemperatureCurve;

        [Title("Wind")]
        [SerializeField] private AnimationCurve expectedWindCurve;

        [Space]
        [SerializeField] private int daysCount = 32;
        [ListDrawerSettings(NumberOfItemsPerPage = 5)]
        [SerializeField] private List<WeatherDayForecast> daysForecasts = new List<WeatherDayForecast>();
        public WeatherDayForecast this[int index] => daysForecasts[index % daysCount];
        [Space]
        [SerializeField] private AnimationCurve temperatureDayCurve;
        [SerializeField] private AnimationCurve temperatureNightCurve;

        public bool IsEmpty => daysForecasts.Count == 0;

        private WeatherDayForecast day0;
        private WeatherDayForecast day1;
        private int currentIndex = 0;

        public Weather GetWeatherByTime(float percent)
        {
            int dayIndex = (int)percent;
            float dayPercent = percent - dayIndex;

            return this[dayIndex].GetWeatherByTime(dayPercent);
        }

        public List<Weather> GetAllWeathers()
        {
            List<Weather> weathers = new List<Weather>();

            for (int i = 0; i < daysForecasts.Count; i++)
            {
                weathers.AddRange(daysForecasts[i].GetAllWeathers());
            }

            return weathers;
        }


        [Button]
        public void Generate()
        {
            ClearAll();

            float minTemp = expectedTemperature.x;
            float maxTemp = expectedTemperature.y;
            float curveTimeStepNormalized = (float) 1f / daysCount;

            Keyframe[] framesDay = new Keyframe[daysCount];
            Keyframe[] framesNight = new Keyframe[daysCount];
            for (int i = 0; i < daysCount; i++)
            {
                float stepCurve = curveTimeStepNormalized * i;

                WeatherDayForecast dayForecast = new WeatherDayForecast();
                dayForecast.New();

                float curveTemperature = Mathf.Lerp(minTemp, maxTemp, expectedTemperatureCurve.Evaluate(stepCurve));

                float curveWind = expectedWindCurve.Evaluate(stepCurve) * 120f;//km/h
                
                //temperatures
                float minTemperature = Mathf.Clamp(curveTemperature - Random.Range(deviation.x, deviation.y), minTemp, maxTemp);
                float maxTemperature = Mathf.Clamp(curveTemperature + Random.Range(deviation.x, deviation.y), minTemp, maxTemp);

                dayForecast.morning.SetTemperature(maxTemperature, WeatherWind.GetRandomWind(maxTemperature, curveWind));
                dayForecast.afternoon.SetTemperature(Random.Range(minTemperature, maxTemperature), WeatherWind.GetRandomWind(maxTemperature, curveWind));
                dayForecast.evening.SetTemperature(Random.Range(minTemperature, maxTemperature), WeatherWind.GetRandomWind(maxTemperature, curveWind));
                dayForecast.night.SetTemperature(minTemperature, WeatherWind.GetRandomWind(maxTemperature, curveWind));

                //curves
                framesDay[i] = new Keyframe(i, maxTemperature);
                framesNight[i] = new Keyframe(i, minTemperature);

                daysForecasts.Add(dayForecast);
            }
            //curves
            temperatureDayCurve = new AnimationCurve(framesDay);
            temperatureDayCurve.preWrapMode = WrapMode.Clamp;
            temperatureDayCurve.postWrapMode = WrapMode.Clamp;

            temperatureNightCurve = new AnimationCurve(framesNight);
            temperatureNightCurve.preWrapMode = WrapMode.Clamp;
            temperatureNightCurve.postWrapMode = WrapMode.Clamp;
        }

        [Button]
        private void ClearAll()
        {
            daysForecasts.Clear();
        }


        [System.Serializable]
        public class WeatherDayForecast
        {
            [HideInInspector] public float maxWindStrength;

            public Forecast morning;
            public Forecast afternoon;
            public Forecast evening;
            public Forecast night;

            //private float morningStart = 6f / 24f;
            //private float afternoonStart = 12f / 24f;
            //private float eveningStart = 17f / 24f;
            //private float nightStart = 20f / 24f;


            private Forecast start;
            private Forecast end;
            private float t = 0;
            private Weather current;

            //public Weather GetWeatherByTime(float dayPercent)
            //{
            //    Forecast oldstart = start;
            //    Forecast oldend = end;

            //    if ((dayPercent > 0f && dayPercent < afternoonStart))
            //    {
            //        start = night;
            //        end = morning;
            //    }
            //    else if (dayPercent >= morningStart && dayPercent < afternoonStart)
            //    {
            //        start = night;
            //        end = morning;
            //    }
            //    else if (dayPercent >= afternoonStart && dayPercent <= eveningStart)
            //    {
            //        start = morning;
            //        end = afternoon;
            //    }
            //    else if (dayPercent > eveningStart && dayPercent <= nightStart)
            //    {
            //        start = afternoon;
            //        end = evening;
            //    }
            //    else if(dayPercent > nightStart)
            //    {
            //        start = evening;
            //        end = night;
            //    }

            //    if (oldstart != start || oldend != end)
            //        t = 0;


            //    if(t < 1f)
            //    {
            //        current = Weather.Lerp(start.weather, end.weather, t);

            //        t += Time.deltaTime * 1.5f;
            //    }

            //    return current;
            //}
            public Weather GetWeatherByTime(float dayPercent)
            {
                Weather weather1 = Weather.Lerp(morning.weather, afternoon.weather, dayPercent);
                Weather weather2 = Weather.Lerp(afternoon.weather, evening.weather, dayPercent);
                Weather weather3 = Weather.Lerp(evening.weather, night.weather, dayPercent);

                return Weather.Lerp(weather1, weather3, dayPercent);
            }

            public List<Weather> GetAllWeathers()
            {
                List<Weather> weathers = new List<Weather>();

                weathers.Add(morning.weather);
                weathers.Add(afternoon.weather);
                weathers.Add(evening.weather);
                weathers.Add(night.weather);

                return weathers;
            }

            public void New()
            {
                morning = new WeatherDayForecast.Forecast();
                afternoon = new WeatherDayForecast.Forecast();
                evening = new WeatherDayForecast.Forecast();
                night = new WeatherDayForecast.Forecast();

                morning.New();
                afternoon.New();
                evening.New();
                night.New();
            }

            [System.Serializable]
            public class Forecast
            {
                [HideLabel]
                public Weather weather;

                public void New()
                {
                    weather = new Weather();
                    weather.air = new WeatherAir();
                    weather.wind = new WeatherWind();
                }

                public void SetTemperature(WeatherAir air, WeatherWind wind)
                {
                    weather.air = air;
                    weather.wind = wind;
                }
                public void SetTemperature(float air, WeatherWind wind)
                {
                    weather.air.airTemperature = air;
                    weather.wind = wind;

                    weather.humidity = Random.Range(0f, 100f);
                    weather.precipitation = Random.Range(0f, 100f);
                }
            }
        }
    }
}

public enum WeatherType
{
    Clear,
    Aurora,
    Cloudy,
    Fog,
    Snowfall,
    Blizzard,
}
public enum FogType
{
    None,
    Light,
    Medium,
    Heavy,
}