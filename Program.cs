using Microsoft.OpenApi.Models;
using ToDoApi;
using Microsoft.AspNetCore.Mvc;
var  MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy  =>
                      {
                          policy.AllowAnyOrigin()
                        //   .WithOrigins("localhost:3000")  
                           .AllowAnyHeader()
                           .AllowAnyMethod();;
                      });
});
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ToDoDbContext>();
builder.Services.AddSwaggerGen();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
{
    options.SerializeAsV2 = true;
});
   app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});
}
app.MapGet("/", () => "Hello World!");
app.MapGet("/todos",(ToDoDbContext context) =>{
    return context.Items.ToList();
 });
app.MapPost("/todos", async (ToDoDbContext context,Item item) => {
    context.Add(item);
await context.SaveChangesAsync();
return;
});
app.MapPut("/todos/{id:int}", async
    (int id, ToDoDbContext context, [FromBody]Item item) => {
       Item i= await context.Items.FindAsync(id);
       if(i!=null)
       {
        i.IsComplete=item.IsComplete;
        await context.SaveChangesAsync();
        return Results.NoContent();
       }
        return Results.NotFound();
    });
 app.MapDelete("/todos/{id:int}",async (int id,ToDoDbContext context) =>{
    Item i=context.Items.Find(id);
    if(i!=null)
    {context.Items.Remove(i);
    await context.SaveChangesAsync();
    return Results.NoContent();
    }
  return Results.NotFound();
 });
app.UseCors(MyAllowSpecificOrigins);
app.Run();
