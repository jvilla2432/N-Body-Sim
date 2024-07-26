using System.Text.Json;
namespace N_Body_Sim_Backend
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

        private class simData
        {
            public double[][][] data { get; set; }
            public string id { get; set; }
        }

        public Simulation(Settings settings)
        {
            numFrames = settings.frames;
            id = settings.name;
            timeStep = settings.timestep;
            stepsPerFrame = settings.stepsPerFrame;
            dimensionality = 2;
            sim = new ShaderSimulator(settings.smooth, DefaultSims.getStarting(settings.initial, settings.bodies));
        }

        public void start()
        {
            sim.runSim(numFrames, (float)timeStep, stepsPerFrame);
        }
        

        public string getData()
        {
            double[][][] data = sim.getData();
            return JsonSerializer.Serialize( new simData { data = data, id = id});
        }
    }
}
