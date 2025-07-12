using Microsoft.OpenApi.Models;
using Softin_HotelBookingAPI.Data;
using Softin_HotelBookingAPI.Interfaces;
using Softin_HotelBookingAPI.Services;
using Microsoft.EntityFrameworkCore;
using Softin_HotelBookingAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("HotelBookingDb"));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IBookingService, BookingService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Hotel Booking API", Version = "v1" });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    //seed 5 data in the db to create rooms data 
    if (!context.Rooms.Any())
    {
        context.Rooms.AddRange(
            new Room { Id = 1, Name = "101", Type = "Deluxe Room", IsAvailable = true },
            new Room { Id = 2, Name = "201", Type = "Executive Room", IsAvailable = true },
            new Room { Id = 3, Name = "301", Type = "Premier Room", IsAvailable = true },
            new Room { Id = 4, Name = "401", Type = "Grand Suite", IsAvailable = true },
            new Room { Id = 5, Name = "501", Type = "Penthouse Suite", IsAvailable = true }
        );
        context.SaveChanges();
    }
}

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
