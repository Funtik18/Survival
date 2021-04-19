using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

public class GeneralTemperature : MonoBehaviour
{
    private static GeneralTemperature instance;
    public static GeneralTemperature Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GeneralTemperature>();
            }
            return instance;
        }
    }

    public WeatherController weather;

    private void Awake()
    {
        weather.Setup();
    }

    [System.Serializable]
    public class WeatherController 
    {
        [SerializeField] private WeatherForecast forecast;
        [Space]    
        [SerializeField] private Times updateTime;
        [ReadOnly] [SerializeField] private Weather currentWeather;

        [ReadOnly]
        [ShowInInspector] public float Temperature => currentWeather.Temperature;

        public WeatherAir Air => currentWeather.air;
        public WeatherWind Wind => currentWeather.wind;


        public void Setup()
        {
            forecast.Generate();

            GeneralTime.TimeUnityEvent unityEvent = new GeneralTime.TimeUnityEvent();
            unityEvent.AddEvent(GeneralTime.TimeUnityEvent.EventType.ExecuteEveryTime, updateTime, null, UpdateWeather);

            GeneralTime.Instance.AddEvent(unityEvent);
        }

        public void UpdateWeather(Times time)
        {
            currentWeather = forecast.GetWeatherByTime(time.GetSkyPercent());
        }

        private void RefreshWeather()
        {

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

            private float morningStart = 6f / 24f;
            private float afternoonStart = 12f / 24f;
            private float eveningStart = 17f / 24f;
            private float nightStart = 20f / 24f;


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
                }
            }
        }
    }

    [System.Serializable]
    public struct Weather
    {
        public WeatherAir air;
        public WeatherWind wind;
        [Range(0, 100f)]
        public float humidity;

        public float Temperature => air.airTemperature + wind.windchill;

        public static Weather Lerp(Weather from, Weather to, float t)
        {
            Weather weather = new Weather();

            weather.air = weather.air.Lerp(from.air, to.air, t);
            weather.wind = weather.wind.Lerp(from.wind, to.wind, t);
            weather.humidity = Mathf.Lerp(from.humidity, to.humidity, t);

            return weather;
        }
    }

    [InlineProperty]
    [System.Serializable]
    public struct WeatherAir 
    {
        //antarctida -90 , 20
        //alaska -63 , 38
        [Range(-100f, 55f)]
        public float airTemperature;

        public WeatherAir Lerp(WeatherAir from, WeatherAir to, float t)
        {
            airTemperature = Mathf.Lerp(from.airTemperature, to.airTemperature, t);

            return this;
        }
    }

    [InlineProperty]
    [System.Serializable]
    public struct WeatherWind 
    {
        [Space]
        [Range(-100f, 0)]
        public float windchill;
        [Range(0, 117f)]
        [SerializeField] private float windSpeed;
        public float WindSpeed
        {
            get => windSpeed;
            set
            {
                windSpeed = Mathf.Clamp(value, 0, 120f);
                CheckWindSpeed();
            }
        }
        public Vector3 windDirection;
        [Space]
        [ReadOnly] [SerializeField] private float windAngle;
        [Space]
        [ReadOnly] [SerializeField] private WindSpeedType windSpeedType;
        [ReadOnly] [SerializeField] private WindDirectionType windDirectionType;

        public WeatherWind SetSpeed(float value)
        {
            windSpeed = value;
            CheckWindSpeed();

            return this;
        }
        public WeatherWind SetDirection(Vector3 dir)
        {
            windDirection.x = dir.x;
            windDirection.z = dir.y;

            CheckWindDirection();

            return this;
        }

        public WeatherWind Lerp(WeatherWind from, WeatherWind to, float t)
        {
            windchill = Mathf.Lerp(from.windchill, to.windchill, t);
            windSpeed = Mathf.Lerp(from.windSpeed, to.windSpeed, t);
            windDirection = Vector3.Lerp(from.windDirection, to.windDirection, t);

            CheckWindSpeed();
            CheckWindDirection();

            return this;
        }


        public static WeatherWind GetRandomWind(float temperature, float maxWindStrength)
        {
            WeatherWind wind = new WeatherWind();
            wind.SetDirection(Random.insideUnitCircle.normalized);
            wind.WindSpeed = Random.Range(0, maxWindStrength);

            //https://tehtab.ru/Guide/GuideTricks/WindChillingEffect/
            //formule
            float constanta = Mathf.Pow(wind.WindSpeed, 0.16f);
            float teff = 13.12f + (0.6215f * temperature) - (11.37f * constanta) + (0.3965f * temperature * constanta);
            wind.windchill = Mathf.Min(0, teff - temperature);

            return wind;
        }


        private void CheckWindDirection()
        {
            windAngle = Vector3.Angle(Vector3.forward, windDirection);
            if (windAngle>=0 && windAngle <= 22.5f)
            {
                windDirectionType = WindDirectionType.N;
            }
            else if(windAngle > 22.5f && windAngle <= 45f)
            {
                windDirectionType = WindDirectionType.NNE;
            }
            else if (windAngle > 45f && windAngle <= 67.5f)
            {
                windDirectionType = WindDirectionType.NE;
            }
            else if (windAngle > 67.5f && windAngle <= 90f)
            {
                windDirectionType = WindDirectionType.ENE;
            }
            else if(windAngle > 90f && windAngle <= 112.5f)
            {
                windDirectionType = WindDirectionType.E;
            }
            else if (windAngle > 112.5f && windAngle <= 135f)
            {
                windDirectionType = WindDirectionType.ESE;
            }
            else if (windAngle > 135f && windAngle <= 157.5f)
            {
                windDirectionType = WindDirectionType.SE;
            }
            else if (windAngle > 157.5f && windAngle <= 180f)
            {
                windDirectionType = WindDirectionType.SSE;
            }
            else if (windAngle > 180f && windAngle <= 202.5f)
            {
                windDirectionType = WindDirectionType.S;
            }
            else if (windAngle > 202.5f && windAngle <= 225f)
            {
                windDirectionType = WindDirectionType.SSW;
            }
            else if (windAngle > 225f && windAngle <= 247.5f)
            {
                windDirectionType = WindDirectionType.SW;
            }
            else if (windAngle > 247.5f && windAngle <= 270f)
            {
                windDirectionType = WindDirectionType.WSW;
            }
            else if (windAngle > 270f && windAngle <= 292.5f)
            {
                windDirectionType = WindDirectionType.W;
            }
            else if (windAngle > 292.5f && windAngle <= 315f)
            {
                windDirectionType = WindDirectionType.WNW;
            }
            else if (windAngle > 315f && windAngle <= 337.5f)
            {
                windDirectionType = WindDirectionType.NW;
            }
            else if (windAngle > 337.5f && windAngle <= 360f)
            {
                windDirectionType = WindDirectionType.NNW;
            }
            else
            {
                Debug.LogError("ERROR WIND DIRECTION");
            }
        }
        private void CheckWindSpeed()
        {
            if(windSpeed <= 2f)
            {
                windSpeedType = WindSpeedType.Calm;
            }
            else if (windSpeed > 2f && windSpeed <= 5f)
            {
                windSpeedType = WindSpeedType.LightAir;
            }
            else if(windSpeed > 5f && windSpeed <= 11f)
            {
                windSpeedType = WindSpeedType.LightBreeze;
            }
            else if (windSpeed > 11f && windSpeed <= 19f)
            {
                windSpeedType = WindSpeedType.GentleBreeze;
            }
            else if (windSpeed > 19f && windSpeed <= 28f)
            {
                windSpeedType = WindSpeedType.ModerateBreeze;
            }
            else if (windSpeed > 28f && windSpeed <= 39f)
            {
                windSpeedType = WindSpeedType.FreshBreeze;
            }
            else if (windSpeed > 39f && windSpeed <= 49f)
            {
                windSpeedType = WindSpeedType.StrongBreeze;
            }
            else if (windSpeed > 49f && windSpeed <= 61f)
            {
                windSpeedType = WindSpeedType.NearGale;
            }
            else if (windSpeed > 61f && windSpeed <= 74f)
            {
                windSpeedType = WindSpeedType.Gale;
            }
            else if (windSpeed > 74f && windSpeed <= 88f)
            {
                windSpeedType = WindSpeedType.StrongGale;
            }
            else if (windSpeed > 88f && windSpeed <= 102f)
            {
                windSpeedType = WindSpeedType.Storm;
            }
            else if (windSpeed > 102f && windSpeed <= 117f)
            {
                windSpeedType = WindSpeedType.ViolentStorm;
            }
            else
            {
                windSpeedType = WindSpeedType.HurricaneForce;
            }
        }


        public enum WindDirectionType
        {
            N,
            NNE,
            NE,
            ENE,
            E,
            ESE,
            SE,
            SSE,
            S,
            SSW,
            SW,
            WSW,
            W,
            WNW,
            NW,
            NNW,
        }

        /// <summary>
        /// Beaufort number types 0-12
        /// </summary>
        public enum WindSpeedType : int
        {
            Calm = 0,
            LightAir = 1,
            LightBreeze = 2,
            GentleBreeze = 3,
            ModerateBreeze = 4,
            FreshBreeze = 5,
            StrongBreeze = 6,
            NearGale = 7,
            Gale = 8,
            StrongGale = 9,
            Storm = 10,
            ViolentStorm = 11,
            HurricaneForce = 12,
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
