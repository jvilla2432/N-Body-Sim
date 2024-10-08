﻿//particle format
// mass x y z vx vy vz ax ay az (etc for more dimensions) length of dimensions * 3 + 1
// particles is [numParticles][dimensions * 3 + 1]
using ComputeSharp;
namespace computeService
{
    public struct Particle
    {
        public float Mass;
        public float2 Pos;
        public float2 Vel;
        public float2 Accel;
        public float2 AccelNxt;

        public Particle(float mass, float x, float y, float xv, float xy, float xa, float ya)
        {
            Mass = mass;
            Pos = new float2(x, y);
            Vel = new float2(xv, xy);
            Accel = new float2(xa, ya);
            AccelNxt = new float2(xa, ya);
        }

    }

    [ThreadGroupSize(DefaultThreadGroupSizes.X)]
    [GeneratedComputeShaderDescriptor]
    public readonly partial struct ProcessFrame(ReadWriteBuffer<Particle> buffer,
int numParticles, float smooth) : IComputeShader
    {
        private static readonly float G = Hlsl.Pow(6.6743f, -11f);
        private readonly float smoothSqr = Hlsl.Pow(smooth, 2);
        public void Execute()
        {
            float distSqr = 0;
            Particle curr = buffer[ThreadIds.X];
            curr.AccelNxt = 0;
            float force = 0;
            for (int i = 0; i < numParticles; i++)
            {
                Particle next = buffer[i];
                distSqr = Hlsl.Distance(curr.Pos, next.Pos) + 1e-10f;
                //Accel is temp used as force here
                force = (G * curr.Mass * next.Mass) / (distSqr + smoothSqr);
                curr.AccelNxt += force * (next.Pos - curr.Pos) * Hlsl.Rsqrt(distSqr);
            }
            distSqr = Hlsl.Distance(curr.Pos, curr.Pos) + 1e-10f;
            force = (G * curr.Mass * curr.Mass) / (distSqr + smoothSqr);
            curr.AccelNxt -= force * (curr.Pos - curr.Pos) / Hlsl.Rsqrt(distSqr);
            curr.AccelNxt /= curr.Mass;
            buffer[ThreadIds.X] = curr;
        }
    }

    [ThreadGroupSize(DefaultThreadGroupSizes.X)]
    [GeneratedComputeShaderDescriptor]
    public readonly partial struct UpdatePosition(ReadWriteBuffer<Particle> buffer, float timestep) : IComputeShader
    {
        private readonly float deltat = timestep; //Time step
        private readonly float deltat2 = 0.5f * Hlsl.Pow(timestep, 2); //Time step squared * 0.5
        public void Execute()
        {
            var curr = buffer[ThreadIds.X];
            curr.Pos = curr.Pos + curr.Vel * deltat + deltat2 * curr.Accel;
            curr.Vel = curr.Vel + 0.5f * (curr.AccelNxt + curr.Accel) * deltat;
            curr.Accel = curr.AccelNxt;
            buffer[ThreadIds.X] = curr;
        }
    }

    class ShaderSimulator
    {
        private static double G = Math.Pow(6.6743d, -11d);
        public static int dimensions = 2;
        private double soft;
        private List<double[][]> frames = new List<double[][]>();
        private Particle[] initalFrame;
        private ReadWriteBuffer<Particle> buffer;

        public ShaderSimulator(double softFactor, double[][] startingFrame)
        {
            soft = softFactor;
            Particle[] startFrame = new Particle[startingFrame.Length];
            Parallel.For(0, startingFrame.Length, i =>
            {
                startFrame[i] = new Particle((float)startingFrame[i][0], (float)startingFrame[i][1], (float)startingFrame[i][2]
    , (float)startingFrame[i][3], (float)startingFrame[i][4], (float)startingFrame[i][5], (float)startingFrame[i][6]);
            });
            initalFrame = startFrame;

        }

        public void initializeSim()
        {
            buffer = GraphicsDevice.GetDefault().AllocateReadWriteBuffer(initalFrame);
        }

        //The buffer contains the next frame to read 
        public void runSim(int numFrames, float timeStep, float physicsSteps)
        {
            Particle[] readFrame = new Particle[initalFrame.Length];
            Double[][] frame = new Double[readFrame.Length][];
            frames.Clear();
            for (int z = 0; z < numFrames; z++)
            {
                buffer.CopyTo(readFrame);
                Console.WriteLine("haiii");
                frame = new Double[readFrame.Length][];
                Parallel.For(0, readFrame.Length, i => {
                    frame[i] = [readFrame[i].Pos.X, readFrame[i].Pos.Y];
                });
                frames.Add(frame);
                for (int i = 0; i < physicsSteps; i++)
                {
                    GraphicsDevice.GetDefault().For(buffer.Length,
                        new ProcessFrame(buffer, buffer.Length, (float)soft));
                    GraphicsDevice.GetDefault().For(buffer.Length,
                        new UpdatePosition(buffer, timeStep));
                }
            }
        }

        public double[][][] getData()
        {
            return frames.ToArray();
        }

    }

}