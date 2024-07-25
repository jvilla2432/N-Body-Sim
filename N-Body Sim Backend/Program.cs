

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options => options.AddPolicy("Everything", policy =>
{
    policy
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin();
}));
var app = builder.Build();
var server = new N_Body_Sim_Backend.Server(app);
server.StartServer();