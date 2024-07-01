var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");


//Get data of a simulation(get)
app.MapGet("simData", () => { });

//Get current sim options
app.MapGet("simOptions", () => { });

//Set sim options
app.MapPost("setOptions", () => { });

//Run a simulation on the server with a pregenerated set(post)
app.MapPost("simRunDefault", () => { });

//Run a simulation with a custom set of bodies
app.MapPost("simRunDefault", () => { });

//Cancel a simulation (put)
app.MapPut("simDelete", () => { });

app.Run();
