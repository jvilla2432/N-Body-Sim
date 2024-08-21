

using System.Numerics;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options => options.AddPolicy("Everything", policy =>
{
    policy
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin();
}));
var app = builder.Build();
var clusters = new String[] { "http://localhost:5000" };
var server = new N_Body_Sim_Backend.Server(app, clusters);
server.StartServer();