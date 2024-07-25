using System.Text.Json;

namespace N_Body_Sim_Backend
{

    public class Simulation
    {
        //Frames
        public List<double[][]> frames;
        private string id;
        private double timeStep;
        private Simulator sim;
        private int numFrames;
        private int dimensionality = 2;

        private class simData
        {
            public double[][][] data { get; set; }
            public string id { get; set; }
        }

        public Simulation(Settings settings)
        {
            frames = new List<double[][]>();
            frames.Add(DefaultSims.getStarting(settings.initial,settings.bodies));
            numFrames = settings.frames;
            id = settings.name;
            timeStep = settings.timestep;
            dimensionality = 2;
            sim = new Simulator(settings.smooth);
        }

        public void start()
        {
            while (frames.Count < numFrames)
            {
                 addFrame();
            }
        }
        
        private void addFrame()
        {
            double[][] currentFrame = frames.Last();
            double[][] nextFrame = sim.stepFrame(timeStep, currentFrame);
            frames.Add(nextFrame);
        }

        public string getData()
        {
            double[][][] data = new double[frames.Count][][];
            for(int i = 0; i < data.Length; i++)
            {
                var frame = frames[i];
                double[][] frameData = new double[frame.Length][];
                for(int z = 0; z < frame.Length; z++)
                {
                    var particle = frame[z];
                    double[] particleData = new double[dimensionality];
                    for(int y = 1; y < 1+ dimensionality; y++)
                    {
                        particleData[y - 1] = particle[y];
                    }
                    frameData[z] = particleData;
                }
                data[i] = frameData;
            }
            return JsonSerializer.Serialize( new simData { data = data, id = id});
        }
    }
}
