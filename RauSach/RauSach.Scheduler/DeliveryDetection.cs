using Microsoft.Extensions.Logging;
using Quartz;
using RauSach.Application.Repositories;
using RauSach.Application.Services;

namespace RauSach.Scheduler
{
    [DisallowConcurrentExecution]
    public class DeliveryDetection : IJob
    {
        private readonly ILogger<DeliveryDetection> _logger;
        private readonly IDeliveryService _deliveryService;

        public DeliveryDetection(ILogger<DeliveryDetection> logger,
                                 IDeliveryService deliveryService)
        {
            _logger = logger;
            _deliveryService = deliveryService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _deliveryService.BuildDeliveryAsync();
            _logger.LogDebug("DeliveryDetection Job executed");
        }
    }
}