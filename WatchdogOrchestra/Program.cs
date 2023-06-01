using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Options;
using WatchdogOrchestra.Configuration;
using WatchdogOrchestra.Controllers.Login;
using WatchdogOrchestra.Infrastructure;
using WatchdogOrchestra.Secrets;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
	opt.CustomOperationIds(operationOpts =>
	{
		if (operationOpts.ActionDescriptor is ControllerActionDescriptor actionDescriptor)
		{
			return $"{actionDescriptor.ControllerName}_{actionDescriptor.ActionName}";
		}

		return null;
	});
});

SecretsManager.CreateTokenKey();

builder
	.Configuration
	.AddJsonFile(SecretsManager.FileName);

builder.Services
	.Configure<ServersConfiguration>(builder.Configuration.GetSection("Servers"));

builder.Services
	.Configure<LoginInfoConfiguration>(builder.Configuration.GetSection("LoginInfo"));

builder.Services
	.Configure<TokenConfiguration>(builder.Configuration.GetSection("Security"))
	.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJWTBearerAuthorization>();

builder.Services
	.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme);

builder.Services.AddHttpClient<LoginController>().ConfigurePrimaryHttpMessageHandler(() =>
new HttpClientHandler
{
	AllowAutoRedirect = true,
	UseDefaultCredentials = true
});

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