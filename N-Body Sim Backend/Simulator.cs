
//particle format
// mass x y z vx vy vz ax ay az (etc for more dimensions) length of dimensions * 3 + 1
// particles is [numParticles][dimensions * 3 + 1]

namespace N_Body_Sim_Backend
{

    class Simulator
    {
        private static double G = Math.Pow(6.6743d, -11d);
        public static int dimensions = 2;
        private double soft;
        private static double gSoft;

        public Simulator(double softFactor)
        {
            soft = softFactor;
        }
        public double[][] stepFrame(double deltaT, double[][] particles)
        {
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
            return nextStep;
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