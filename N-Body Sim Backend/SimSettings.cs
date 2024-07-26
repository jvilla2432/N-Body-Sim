namespace N_Body_Sim_Backend
{

    public class Settings
    {
        public string name { get; set; }
        public int frames { get; set; }
        public int bodies { get; set; }
        public int smooth { get; set; }
        public int timestep { get; set; }
        public int initial { get; set; }

        public int stepsPerFrame { get; set; }
    }

}