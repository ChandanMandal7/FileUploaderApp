using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using FileUploadApp.DataAccessLayer;
using FileUploadApp.Controllers;
using FileUploadApp.rabbitmq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<CrudOperations>(); //because CrudOperations requires IConfiguration

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
    });
}



app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();


    
   rabbitmqReceiver rabbitrecivefilename= new rabbitmqReceiver();


Thread rec = new Thread (new ThreadStart(rabbitrecivefilename.StartListening));
rec.Start();


RabbitMQSubscriber rabbitsub=new RabbitMQSubscriber();
Thread sub = new Thread (new ThreadStart(rabbitsub.StartListening));
sub.Start();

app.Run();