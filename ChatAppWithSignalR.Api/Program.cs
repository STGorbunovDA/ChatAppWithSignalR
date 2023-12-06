global using Microsoft.EntityFrameworkCore;
global using ChatAppWithSignalR.Api.Functions.User;
global using ChatAppWithSignalR.Api.Functions.UserFriend;
global using ChatAppWithSignalR.Api.Functions.Message;
global using ChatAppWithSignalR.Api.Entities;
global using Microsoft.AspNetCore.Mvc.Filters;
global using Microsoft.AspNetCore.Mvc;
global using ChatAppWithSignalR.Api.Helpers;
global using Microsoft.AspNetCore.SignalR;
using ChatAppWithSignalR.Api.Controllers.ChatHub;


namespace ChatAppWithSignalR.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddSignalR();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<ChatAppContext>(options =>
            {
                options.UseSqlServer(builder.Configuration["ConnectionString"]);
            });

            builder.Services.AddTransient<IUserFunction, UserFunction>();
            builder.Services.AddTransient<IUserFriendFunction, UserFriendFunction>();
            builder.Services.AddTransient<IMessageFunction, MessageFunction>();
            builder.Services.AddScoped<UserOperator>();
            builder.Services.AddScoped<ChatHub>();

            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            //app.UseAuthorization();
            app.UseMiddleware<JwtMiddleware>();

            //app.MapControllers();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/ChatHub");
            });

            app.Run();
        }
    }
}
