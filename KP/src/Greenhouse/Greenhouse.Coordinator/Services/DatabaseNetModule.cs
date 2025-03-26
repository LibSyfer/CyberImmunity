using Greenhouse.Coordinator.Models;

namespace Greenhouse.Coordinator.Service
{
    public class DatabaseNetModule
    {
        private readonly ILogger<DatabaseNetModule> _logger;
        private const string DatabaseUrl = "http://TomatoDatabase:8080";

        public DatabaseNetModule(ILogger<DatabaseNetModule> logger)
        {
            _logger = logger;
        }

        public Task<GrowingParams> GetGrowingParamsAsync(Guid paramsId, CancellationToken cancellationToken)
        {

            
            return Task.FromResult(new GrowingParams
            {
                Id = Guid.NewGuid()
            });
        }
    }
}
