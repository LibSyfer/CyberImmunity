namespace Greenhouse.MessageBus.Messages.ClimateControlModule
{
    public class SetClimateControlParamsCommand : IntegrationMessage
    {
        public double TemperatureDay { get; set; }

        public double TemperatureNight { get; set; }

        public double HumidityLevel { get; set; }
    }
}
