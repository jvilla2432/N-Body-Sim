
//particle mass x,y,z vx,vy,vz (7 fields)

//enum

enum particle { mass,x,y,z,vx,vy,vz} 
class NBody
{
    private double G = Math.Pow(6.6743d,-11d);
    private double[][] particles;
    public NBody(int particleCount)
    {
        particles = new double[particleCount][];
    }
    public void stepTime(double deltaT)
    {
        foreach(var particle1 in particles)
        {
            double fx = 0;
            double fy = 0;
            double fz = 0;
            foreach(var particle2 in particles)
            {
                if(particle1 != particle2)
                {
                    double dx = particle2[(int)particle.x] - particle1[(int)particle.x];
                    double dy = particle2[(int)particle.y] - particle1[(int)particle.y];
                    double dz = particle2[(int)particle.z] - particle1[(int)particle.z];
                    double distSqr = dx * dx + dy * dy + dz * dz;
                    double dist = Math.Sqrt(distSqr);
                    double force = G * particle1[(int)particle.mass] * particle2[(int)particle.mass] / distSqr;
                    fx += force * dx / dist;
                    fy += force * dy / dist;
                    fz += force * dz / dist;
                }
            }
            particle1[(int)particle.vx] += fx / particle1[(int)particle.mass] * deltaT;
            particle1[(int)particle.vy] += fy / particle1[(int)particle.mass] * deltaT;
            particle1[(int)particle.vz] += fz / particle1[(int)particle.mass] * deltaT;

            particle1[(int)particle.x] += particle1[(int)particle.vx] * deltaT;
            particle1[(int)particle.y] += particle1[(int)particle.vy] * deltaT;
            particle1[(int)particle.z] += particle1[(int)particle.vz] * deltaT;
        }
    }

}