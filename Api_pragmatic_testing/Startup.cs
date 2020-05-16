using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Application_pragmatic_testing.Commands;
using Application_pragmatic_testing.ExternalServices;
using Core_pragmatic_testing.Repositories;
using Infra_pragmatic_testing.Database;
using Infra_pragmatic_testing.Repositories;
using Infra_pragmatic_testing.Services;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Retry;

namespace Api_pragmatic_testing
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();
			services.AddMediatR(typeof(Startup), typeof(ChangePasswordCommand));

			services.AddTransient<IUserBehaviorService, UserBehaviorService>();
			services.AddTransient<IUserBehaviorGateway, UserBehaviorGateway>();
			services.AddTransient<HttpClient, HttpClient>();

			services.AddTransient<IExternalEventPublisherServ, ExternalEventPublisherServ>();
			var policy = Policy
			   .Handle<Exception>()
			   .WaitAndRetryAsync(retryCount: 3,
								  retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
								  (_, __, retryCount, context) => { context["retryCount"] = retryCount; });
			services.AddSingleton(policy);
			services.AddTransient<IEventGridGateway, EventGridGateway>();
			var eventGridClient = new EventGridClient(new TopicCredentials("SomeTopicKey"));
			services.AddSingleton<IEventGridClient>(eventGridClient);

			services.AddTransient<IPasswordHistoryRepository, PasswordHistoryRespository>();
			services.AddSingleton<ISimpleInMemoryDb>(SimpleInMemoryDb.InitializeDbWithDefaultSeedData());
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
