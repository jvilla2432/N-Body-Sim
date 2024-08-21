using Grpc.Core;
using System.Runtime.CompilerServices;
using System.Text.Json;
namespace computeService
{

    public class Simulation
    {
        //Frames
        private string id;
        private double timeStep;
        private ShaderSimulator sim;
        private int numFrames;
        private int dimensionality = 2;
        private int stepsPerFrame;
        private int framesPerPacket = 5;
        public int currentFrame = 0;

        private class simData
        {
            public double[][][] data { get; set; }
            public string id { get; set; }
        }

        public Simulation(SimulationRequest settings)
        {
            numFrames = settings.Frames;
            id = settings.Name;
            timeStep = settings.Timestep;
            stepsPerFrame = settings.StepsPerFrame;
            dimensionality = 2;
            sim = new ShaderSimulator(settings.Smooth, DefaultSims.getStarting(settings.Intial, settings.Bodies));
        }

        public async Task start(IServerStreamWriter<computeService.SimulationData> responseStream)
        {
            sim.initializeSim();
            while(currentFrame < numFrames)
            {
                framesPerPacket = Math.Min(10, numFrames - currentFrame);
                sim.runSim(framesPerPacket, (float)timeStep, stepsPerFrame);
                currentFrame += framesPerPacket;
                await responseStream.WriteAsync(new SimulationData
                {
                    Data = getData(),
                    Frames = framesPerPacket
                });
            }
\        }
        
        public bool finishedReading()
        {
            return currentFrame == numFrames;
        }

        public string getData()
        {
            double[][][] data = sim.getData();
            return JsonSerializer.Serialize( new simData { data = data, id = id});
        }

    }
}
