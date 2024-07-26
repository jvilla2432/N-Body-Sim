namespace N_Body_Sim_Backend
{
    public static class DefaultSims
    {
        public static double[][] getStarting(int starting, int bodies)
        {
            if(starting == 0)
            {
                return Circle(bodies);
            }
            return start;
        }
        public static double[][] Circle(int num)
        {
            var rand = new Random();
            var circles = new double[num][];
            for (int i = 0; i < num; i++)
            {
                circles[i] = new double[7];
                //center of circle at 300, radius 100
                var angle = rand.NextDouble() * 2 * Math.PI;
                circles[i][1] = Math.Sin(angle) * 100;
                circles[i][2] = Math.Cos(angle) * 100;
                circles[i][0] = 0.001;
                for(int x =  3; x< 7; x++)
                {
                    circles[i][x] = 0;
                }
            }
            return circles;
        }

        public static double[][] start = new double[3][] { 
            new double[7]{ 10, 0, 0 , 0, 0, 0, 0},
            new double[7]{ 10, 50, 50, 0, 0 ,0 ,0 },
            new double[7]{ 10, 100, 100, 0, 0, 0 ,0}
        };
    }
}
