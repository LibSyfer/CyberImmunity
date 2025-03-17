namespace TomatoDatabase.Web.Models
{
    public class GrowingParams
    {
        public Guid Id { get; set; }

        public Guid TomatoId { get; set; }

        public double LightIntensity { get; set; }                 //Интенсивность света в люксах

        public int LightDuration { get; set; }                      //Продолжительность светового дня в часах
        
        public double TemperatureDay { get; set; }                  //Температура днем в °C

        public double TemperatureNight { get; set; }                //Температура ночью в °C
        
        public double HumidityLevel { get; set; }                   //Уровень влажности в %
        
        public double WateringFrequency { get; set; }               //Частота полива в днях

        public string FertilizerType { get; set; } = string.Empty;  //Тип удобрения
    }
}
