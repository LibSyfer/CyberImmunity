﻿namespace Greenhouse.Coordinator.Models
{
    public class GrowingParams
    {
        public Guid Id { get; set; }

        public Guid TomatoId { get; set; }

        public double LightIntensity { get; set; }                  // Интенсивность света в люксах

        public int LightDuration { get; set; }                      //Продолжительность светового дня в часах

        public double TemperatureDay { get; set; }                  //Температура днем в цельсиях

        public double TemperatureNight { get; set; }                //Температура ночью в цельсиях

        public double HumidityLevel { get; set; }                   //Уровень влажности в процентах

        public double WateringFrequency { get; set; }               //Частота полива в днях

        public string FertilizerType { get; set; } = string.Empty;  //Тип удобрения
    }
}
