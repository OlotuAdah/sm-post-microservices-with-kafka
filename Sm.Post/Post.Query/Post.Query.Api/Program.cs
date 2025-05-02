using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Post.Common.Entities;
using Post.Common.Settings;
using Post.Query.Api.Queries;
using Post.Query.Domain.Interfaces.Repositories;
using Post.Query.Infrastructure.Consumers;
using Post.Query.Infrastructure.DataAccess;
using Post.Query.Infrastructure.Dispatchers;
using Post.Query.Infrastructure.Handlers;
using Post.Query.Infrastructure.Repositories;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
// Add services to the container.
Action<DbContextOptionsBuilder> configureDbContext = o => o.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
builder.Services.AddDbContext<ReadDatabaseContext>(configureDbContext);
builder.Services.AddSingleton(new ReadDatabaseContextFactory(configureDbContext));

//Create databse and tables from code
ReadDatabaseContext readDatabseContext = builder.Services.BuildServiceProvider().GetRequiredService<ReadDatabaseContext>();
readDatabseContext.Database.EnsureCreated();

builder.Services.AddScoped<IPostRepo, PostRepo>();
builder.Services.AddScoped<ICommentRepo, CommentRepo>();
builder.Services.AddScoped<IQueryHandler, QueryHandler>(); //register after post repo, becaus it depends on it
builder.Services.AddScoped<IEventHandler, Post.Query.Infrastructure.Handlers.EventHandler>();
builder.Services.Configure<ConsumerConfig>(builder.Configuration.GetSection("ConsumerConfig"));
builder.Services.Configure<KafkaTopics>(builder.Configuration.GetSection("KafkaTopics"));
builder.Services.AddScoped<IEventConsumer, EventConsumer>();

builder.Services.AddControllers();

//Register Query Handlers
IQueryHandler queryHandler = builder.Services.BuildServiceProvider().GetRequiredService<IQueryHandler>();
QueryDispatcher dispatcher = new();
dispatcher.RegisterHandler<FindAllPostQuery>(queryHandler.HandleAsync);
dispatcher.RegisterHandler<FindPostByIdQuery>(queryHandler.HandleAsync);
dispatcher.RegisterHandler<FindPostByAuthorQuery>(queryHandler.HandleAsync);
dispatcher.RegisterHandler<FindPostWithCommentsQuery>(queryHandler.HandleAsync);
dispatcher.RegisterHandler<FindPostWithLikesQuery>(queryHandler.HandleAsync);
builder.Services.AddSingleton<IQueryDispatcher<PostEntity>>(_ => dispatcher);

//Add Service Registrattion for Kafka hosted service
builder.Services.AddHostedService<ConsumerHostedService>();
//swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();
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

