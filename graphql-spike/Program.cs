using GraphQL;
using GraphQL.Server;
using GraphQL.SystemTextJson;
using GraphQL.Types;
using GraphQL.Spike;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IRepository<int, Person>, PersonRepository>();
builder.Services.AddSingleton<PersonType>();
builder.Services.AddSingleton<PersonQuery>();
builder.Services.AddSingleton<ISchema, PersonSchema>();
builder.Services.AddGraphQL(opt => opt.EnableMetrics = false).AddSystemTextJson();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseGraphQLGraphiQL();
app.UseGraphQL<ISchema>();
app.Run();

public class PersonType : ObjectGraphType<Person>
{
    public PersonType()
    {
        Field(p => p.Id);
        Field(p => p.Name);
        Field(p => p.Surname);
        Field(p => p.Email);
    }
}

public class PersonQuery : ObjectGraphType<Person>
{
    public PersonQuery(IRepository<int, Person> personRepository)
    {
        Field<ListGraphType<PersonType>>(
            "people",
            arguments: new QueryArguments(new QueryArgument<IdGraphType> { Name = "offset" }, new QueryArgument<IdGraphType> { Name = "limit" }),
            resolve: context =>
            {
                int offset = context.GetArgument("offset", 0);
                int limit = context.GetArgument("limit", int.MaxValue);

                return personRepository.GetAll().Skip(offset).Take(limit);
            }
        );


        Field<PersonType>(
            "person",
            arguments: new QueryArguments(new QueryArgument<IdGraphType> { Name = "id" }),
            resolve: context =>
            {
                var id = context.GetArgument<int>("id");
                return personRepository.GetById(id);
            }
        );

    }
}

public class PersonSchema : Schema
{
    public PersonSchema(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        Query = serviceProvider.GetRequiredService<PersonQuery>();
    }
}
