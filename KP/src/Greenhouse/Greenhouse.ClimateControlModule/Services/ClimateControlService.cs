using Greenhouse.ClimateControlModule.Models;

namespace Greenhouse.ClimateControlModule.Services
{
    public class ClimateControlService : BackgroundService
    {
        private readonly ILogger<ClimateControlService> _logger;
        private ClimateControlParams _necessaryParams;
        private EnvironmentalClimateParams _currentParams;

        public ClimateControlService(ILogger<ClimateControlService> logger)
        {
            _logger = logger;
            _necessaryParams = new ClimateControlParams
            {
                TemperatureDay = 20,
                TemperatureNight = 16,
                HumidityLevel = 50
            };
            _currentParams = new EnvironmentalClimateParams
            {
                Temperature = 20,
                HumidityLevel = 50
            };
        }

        public void UpdateNecessaryParams(ClimateControlParams newClimateControlParams)
        {
            _necessaryParams = newClimateControlParams;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("[Параметры климата] Температура: {Temperature} °C; Влажность: {Humidity} %", _currentParams.Temperature, _currentParams.HumidityLevel);

                var necessaryTemperature = _necessaryParams.TemperatureDay;
                var nowTime = DateTime.UtcNow.TimeOfDay;
                if (nowTime >= new TimeSpan(22, 0, 0) || nowTime < new TimeSpan(6, 0, 0))
                {
                    necessaryTemperature = _necessaryParams.TemperatureNight;
                }

                double temperatureСhange = 0;
                if (_currentParams.Temperature > necessaryTemperature)
                {
                    temperatureСhange = GetRandomDouble(-0.375, 0.125);
                }
                else if (_currentParams.Temperature < necessaryTemperature)
                {
                    temperatureСhange = GetRandomDouble(-0.125, 0.375);
                }
                else
                {
                    temperatureСhange = GetRandomDouble(-0.25, 0.25);
                }

                double humidityСhange = 0;
                if (_currentParams.HumidityLevel > _necessaryParams.HumidityLevel)
                {
                    humidityСhange = GetRandomDouble(-0.375, 0.125);
                }
                else if (_currentParams.HumidityLevel < _necessaryParams.HumidityLevel)
                {
                    humidityСhange = GetRandomDouble(-0.125, 0.375);
                }
                else
                {
                    humidityСhange = GetRandomDouble(-0.25, 0.25);
                }

                _currentParams.Temperature += temperatureСhange;
                _currentParams.HumidityLevel += humidityСhange;

                await Task.Delay(1000);
            }
        }

        private double GetRandomDouble(double min, double max)
        {
            return min + (Random.Shared.NextDouble() * (max - min));
        }
    }
}
