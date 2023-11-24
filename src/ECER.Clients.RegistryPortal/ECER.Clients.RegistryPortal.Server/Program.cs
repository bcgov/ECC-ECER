using System.Reflection;
using ECER.Clients.RegistryPortal.Server;
using ECER.Utilities.Hosting;

var builder = WebApplication.CreateBuilder(args);

HostConfigurer.Configure(builder.Services, builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opts =>
{
    opts.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});

var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

ApplicationsEndpoints.Map(app);

app.Run();