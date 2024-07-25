using System.Diagnostics;

namespace N_Body_Sim_Backend
{
    public class Server
    {
        private WebApplication app;
        public Server(WebApplication startingApp)
        {
            app = startingApp;
            Stopwatch sw = new Stopwatch();
            app.MapPost("/startSim", async (Settings settings) => {
                Simulation sim = new Simulation(settings);
                sw.Restart();
                sim.start();
                await Task.Run(() => sim.start());
                sw.Stop();
                Console.WriteLine(settings.name + "took " + sw.Elapsed.ToString());
                return Results.Ok(sim.getData());
               
            });
        }

        public void StartServer()
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            }
            app.Run("http://localhost:3001");
        }
    }
}
