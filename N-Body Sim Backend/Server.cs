using System.Diagnostics;
using computeService;
using Grpc.Net.Client;

namespace N_Body_Sim_Backend
{
    public class Server
    {
        private WebApplication app;
       
        public Server(WebApplication startingApp, String[] clusters)
        {
            app = startingApp;
            var distrubtor = new Distributor(clusters);

            app.MapPost("/startSim", async (Settings settings) => {
                Task<string> dist = distrubtor.RequestSimAsync(settings);
                string data = await dist;
                Console.WriteLine("Reply! " + data);   
                return Results.Ok(data);
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
