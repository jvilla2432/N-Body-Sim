using computeService;
using Grpc.Core;
using System.Diagnostics;

namespace computeService.Services
{
    public class ComputeService : ComputeSim.ComputeSimBase
    {
        private readonly ILogger<ComputeService> _logger;
        public ComputeService(ILogger<ComputeService> logger)
        {
            _logger = logger;
        }

        public override async Task SendSimulation(SimulationRequest request, IServerStreamWriter<computeService.SimulationData> responseStream,ServerCallContext context)
        {
            Stopwatch sw = new Stopwatch();
            Simulation sim = new Simulation(request);
            sw.Restart();
            await Task.Run(() => sim.start(responseStream));
            sw.Stop();
        }
    }
}
