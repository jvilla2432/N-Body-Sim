using computeService;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.VisualStudio.Threading;
using System.Threading.Channels;
using System.Text.Json;

namespace N_Body_Sim_Backend
{
    public class Distributor
    {
        // private Queue<Simulation> sims; //used to order the sims (not currently in place
        
        private readonly AsyncQueue<ComputeSim.ComputeSimClient> computeClusters = new AsyncQueue<ComputeSim.ComputeSimClient>();
        private int clusterCount;
        public Distributor(String[] clusters)
        {
            for (int i = 0; i < clusters.Length; i++)
            {
                var channel = GrpcChannel.ForAddress(clusters[i]);
                var client = new ComputeSim.ComputeSimClient(channel);
                computeClusters.Enqueue(client);
            }
            clusterCount = computeClusters.Count;
        }

        //async = callback?
        public async Task<string> RequestSimAsync(Settings settings)
        {
            var request = new SimulationRequest
            {
                Name = settings.name,
                Frames = settings.frames,
                Bodies = settings.bodies,
                Smooth = settings.smooth,
                Timestep = settings.timestep,
                Intial = settings.initial,
                StepsPerFrame = settings.stepsPerFrame
            };
            var cluster = await computeClusters.DequeueAsync();
            if (cluster == null)
            {
                throw new Exception("No clusters!! Total server failure...");
            }
            double[][][] frameData = new double[settings.frames][][];
            using var call = cluster.SendSimulation(request);
            int index = 0;
            await foreach (var simResponse in call.ResponseStream.ReadAllAsync())
            {
                double[][][] parsedData = JsonSerializer.Deserialize<simData>(simResponse.Data).data;
                Console.WriteLine("Greeting: ");
                for (int i = 0; i < simResponse.Frames; i++)
                {
                    Console.WriteLine("ok so like im writing into frame" + (i + index) + " but like at " + i);
                    Console.WriteLine(parsedData[i][0][0]);
                    frameData[i + index] = parsedData[i];
                }
                index = index + simResponse.Frames;
            }
            simData totalData = new simData { data = frameData, id = settings.name };
            return JsonSerializer.Serialize<simData>(totalData);
        }
    }
}
