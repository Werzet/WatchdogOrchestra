using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using WatchdogOrchestra.Configuration;
using WatchdogOrchestra.Infrastructure;
using WatchdogOrchestra.Secrets;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

SecretsManager.CreateTokenKey();

builder.Services
	.Configure<ServersConfiguration>(builder.Configuration.GetSection("Servers"));
builder.Services
	.Configure<TokenConfiguration>(builder.Configuration.GetSection("Security"))
	.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJWTBearerAuthorization>();
builder.Services
	.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme);

builder.Services
	.AddControllers(opt =>
	{
		opt.Filters.Add<ExceptionFilter>();
	});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();