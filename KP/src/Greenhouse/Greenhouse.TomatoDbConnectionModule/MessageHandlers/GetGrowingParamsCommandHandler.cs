using Greenhouse.MessageBus.Abstractions;
using Greenhouse.MessageBus.Messages.TomatoDbConnectionModule;
using Greenhouse.Share;
using Greenhouse.TomatoDbConnectionModule.Models;
using System.Text.Json;

namespace Greenhouse.TomatoDbConnectionModule.MessageHandlers
{
    public class GetGrowingParamsCommandHandler : IIntegrationMessageHandler<GetGrowingParamsCommand>
    {
        private readonly ILogger<GetGrowingParamsCommandHandler> _logger;
        private readonly IMessageBus _messageBus;
        private readonly HttpClient _httpClient;
        
        public GetGrowingParamsCommandHandler(ILogger<GetGrowingParamsCommandHandler> logger,
            IMessageBus messageBus,
            HttpClient httpClient)
        {
            _logger = logger;
            _messageBus = messageBus;
            _httpClient = httpClient;
        }

        public async Task Handle(GetGrowingParamsCommand message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Получение параметров выращивания из базы данных");

            await Task.Delay(5000);

            var result = await _httpClient.GetAsync($"/tomatos/growing-params/{message.ParamsId}");
            if (!result.IsSuccessStatusCode)
            {
                _logger.LogError("Ошибка получения параметров выращивания");
                return;
            }

            var contentStream = await result.Content.ReadAsStreamAsync();

            var growingParams = JsonSerializer.Deserialize<GrowingParams>(contentStream);
            if (growingParams is null)
            {
                _logger.LogError("Ошибка сериализации параметров");
                return;
            }

            await _messageBus.SendAsync(GreenhouseServicesNames.CoordinatorModule, new GrowingParamsResultCommand
            {
                TomatoId = growingParams.TomatoId,
                LightIntensity = growingParams.LightIntensity,
                LightDuration = growingParams.LightDuration,
                TemperatureDay = growingParams.TemperatureDay,
                TemperatureNight = growingParams.TemperatureNight,
                HumidityLevel = growingParams.HumidityLevel,
                WateringFrequency = growingParams.WateringFrequency,
                FertilizerType = growingParams.FertilizerType
            }, cancellationToken);
        }
    }
}
