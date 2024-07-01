
//particle format
// mass x y z vx vy vz ax ay az (etc for more dimensions)

//enum

class NBody
{
    private static double G = Math.Pow(6.6743d,-11d);
    private double[][] particles;
    private static int dimensions = 3;
    public NBody(double[][] particlesList)
    {
        if(particlesList.Length == 0)
        {
            throw new ArgumentException("Particles empty");
        }
        if(particlesList[0].Length != dimensions * 3 + 1)
        {
            throw new ArgumentException("Particles incorrect dimensionality");
        }
        particles = particlesList;
    }


    public static double[] calculateForce(double[] particle1, double[] particle2)
    {
        double[] f = new double[dimensions];
        double sum = 0;
        for (int i = 0; i < dimensions; i++)
        {
            sum += Math.Pow((particle2[1 + i] - particle1[1 + i]),2);
        }
        double distSqr = sum;
        double dist = Math.Sqrt(distSqr);
        double force = (G * particle1[0] * particle2[0]) / distSqr;
        for(int i = 0; i < dimensions; i++)
        {
            f[i] = force * (particle2[i + 1] - particle1[i + 1]) / dist;
        }
        return f;
    }

    public void stepTime(double deltaT)
    {
        foreach(var particle1 in particles)
        {
            double[] f = new double[dimensions];
            foreach(var particle2 in particles)
            {
                if(particle1 != particle2)
                {
                    f = calculateForce(particle1, particle2);
                }
            }

            double[] a = new double[dimensions];
            for(int i = 0; i < dimensions; i++)
            {
                a[i] = f[i] / particle1[0];
            }
            //Leapfrog integration
            for(int i = 0; i < dimensions; i++)
            {
                particle1[i+1] = particle1[i+1] + particle1[i + dimensions + 1] * deltaT + 0.5 * a[i] * deltaT * deltaT;
                particle1[i + dimensions + 1] = particle1[i + dimensions + 1] + 0.5 * (a[i] + particle1[i + dimensions*2 + 1]) * deltaT;
                particle1[i + dimensions * 2 + 1] = a[i];
            }

        }
    }

}