//particle format
// mass x y z vx vy vz ax ay az (etc for more dimensions) length of dimensions * 3 + 1
// particles is [numParticles][dimensions * 3 + 1]
using ComputeSharp;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace computeService
{

    class Simulator
    {
        private static double G = Math.Pow(6.6743d, -11d);
        public static int dimensions = 2;
        private double soft;
        private List<double[][]> frames = new List<double[][]>();


        public Simulator(double softFactor, double[][] startingFrame)
        {
            soft = softFactor;
            frames.Add(startingFrame);
        }

        public void runSim(int numFrames, double timeStep, double stepPerFrame)
        {
            while (frames.Count < numFrames)
            {
                stepFrame(timeStep);
            }
        }

        public double[][][] getData()
        {
            double[][][] data = new double[frames.Count][][];
            for (int i = 0; i < data.Length; i++)
            {
                var frame = frames[i];
                double[][] frameData = new double[frame.Length][];
                for (int z = 0; z < frame.Length; z++)
                {
                    var particle = frame[z];
                    double[] particleData = new double[dimensions];
                    for (int y = 1; y < 1 + dimensions; y++)
                    {
                        particleData[y - 1] = particle[y];
                    }
                    frameData[z] = particleData;
                }
                data[i] = frameData;
            }
            return data;
        }
        private void stepFrame(double deltaT)
        {
            double[][] particles = frames.Last();
            int numParticles = particles.Length;
            double[][] nextStep = new double[numParticles][];
            Parallel.For(0, particles.Length, currentParticle => {
                double[] particle1 = particles[currentParticle];
                nextStep[currentParticle] = new double[dimensions * 3 + 1];
                double[] nextParticle = nextStep[currentParticle];
                nextParticle[0] = particle1[0];
                foreach (var particle2 in particles)
                {
                    if (particle1 != particle2)
                    {
                        addForce(particle1, particle2, nextParticle, dimensions * 2 + 1);
                    }
                }
                //Calculate acceleration(placed in force)
                for (int i = 0; i < dimensions; i++)
                {
                    nextParticle[i + dimensions * 2 + 1] = nextParticle[i + dimensions * 2 + 1] / particle1[0];
                }
                //Leapfrog integration
                for (int i = 0; i < dimensions; i++)
                {
                    nextParticle[i + 1] = particle1[i + 1] + particle1[i + dimensions + 1] * deltaT + 0.5 * nextParticle[i + dimensions * 2 + 1] * deltaT * deltaT;
                    nextParticle[i + dimensions + 1] = particle1[i + dimensions + 1] + 0.5 * (nextParticle[i + dimensions * 2 + 1] + particle1[i + dimensions * 2 + 1]) * deltaT;
                    nextParticle[i + dimensions * 2 + 1] = nextParticle[i + dimensions * 2 + 1];
                }

            });
            frames.Add(nextStep);
        }

        private void addForce(double[] particle1, double[] particle2, double[] forceVec, int offset = 0)
        {
            double sum = 0;
            for (int i = 0; i < dimensions; i++)
            {
                sum += Math.Pow((particle2[1 + i] - particle1[1 + i]), 2);
            }
            double distSqr = sum;
            double dist = Math.Sqrt(distSqr);
            double force = (G * particle1[0] * particle2[0]) / (distSqr + Math.Pow(soft, 2));
            for (int i = 0; i < dimensions; i++)
            {
                forceVec[i + offset] += force * (particle2[i + 1] - particle1[i + 1]) / dist;
            }
        }

    }

}