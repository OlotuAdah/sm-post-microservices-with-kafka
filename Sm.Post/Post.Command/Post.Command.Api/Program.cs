using Confluent.Kafka;
using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using CQRS.Core.producers;
using MongoDB.Bson.Serialization;
using Post.Command.Api;
using Post.Command.Api.Mappings;
using Post.Command.Domain.Aggregates;
using Post.Command.Infrastructure.config;
using Post.Command.Infrastructure.Handlers;
using Post.Command.Infrastructure.producers;
using Post.Command.Infrastructure.Repositories;
using Post.Command.Infrastructure.stores;
using Post.Common.Events;
using Post.Common.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(typeof(CommandToDtoProfile));

//MogoDB does not support abtract class by default,
//  so we need to map the abstact BaseEvent class to our concrete classes that inherit from it
BsonClassMap.RegisterClassMap<BaseEvent>();
BsonClassMap.RegisterClassMap<CommentAddedEvent>();
BsonClassMap.RegisterClassMap<CommentRemovedEvent>();
BsonClassMap.RegisterClassMap<CommentUpdatedEvent>();
BsonClassMap.RegisterClassMap<MessageUpdatedEvent>();
BsonClassMap.RegisterClassMap<PostCreatedEvent>();
BsonClassMap.RegisterClassMap<PostLikedEvent>();
BsonClassMap.RegisterClassMap<PostRemovedEvent>();

// 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<MongoDbConfig>(builder.Configuration.GetSection(nameof(MongoDbConfig)));
builder.Services.Configure<ProducerConfig>(builder.Configuration.GetSection(nameof(ProducerConfig)));
builder.Services.Configure<KafkaTopics>(builder.Configuration.GetSection(nameof(KafkaTopics)));
builder.Services.AddScoped<IEventStoreRepository, EventStoreRepository>();
builder.Services.AddScoped<IEventProducer, EventProducer>();
builder.Services.AddScoped<IEventStore, EventStore>();
builder.Services.AddScoped<IEventSourcingHandler<PostAggregate>, EventSourcingHandler>();

#region Register Command Handlers
builder.Services.AddCommandHandlers();
#endregion Register Command Handlers

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
app.UseRouting();


app.Run();

