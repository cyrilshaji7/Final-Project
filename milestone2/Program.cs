using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using milestone2.Data;
using milestone2.Services;
using milestone2.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();

// Add controllers and services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Swagger setup
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "milestone2", Version = "v1" });
    options.AddServer(new OpenApiServer { Url = "http://localhost:5062" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token as: Bearer {your_token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// DB context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// JWT Auth
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// CORS: Allow frontend on localhost:5249
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5249")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Middleware
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowFrontend"); // Applied policy here

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed users
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!db.Users.Any())
    {
        db.Users.AddRange(
            new User { Email = "admin", Role = "Admin" },
            new User { Email = "staff@example.com", Role = "Admin" },
            new User { Email = "patron1@example.com", Role = "Patron" },
            new User { Email = "patron2@example.com", Role = "Patron" }
        );

        db.SaveChanges();
        Console.WriteLine("Seeded test users.");
    }
}

// Seed books
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!db.Books.Any()) 
    {
        var books = new List<Book>
        {
            new() { Title = "To Kill a Mockingbird", Author = "Harper Lee", Genre = "Fiction" },
            new() { Title = "1984", Author = "George Orwell", Genre = "Sci-Fi" },
            new() { Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", Genre = "Fiction" },
            new() { Title = "Sapiens: A Brief History of Humankind", Author = "Yuval Noah Harari", Genre = "History" },
            new() { Title = "The Hobbit", Author = "J.R.R. Tolkien", Genre = "Fantasy" },
            new() { Title = "Pride and Prejudice", Author = "Jane Austen", Genre = "Fiction" },
            new() { Title = "The Catcher in the Rye", Author = "J.D. Salinger", Genre = "Fiction" },
            new() { Title = "Dune", Author = "Frank Herbert", Genre = "Sci-Fi" },
            new() { Title = "The Da Vinci Code", Author = "Dan Brown", Genre = "Mystery" },
            new() { Title = "Educated", Author = "Tara Westover", Genre = "Biography" },
            new() { Title = "Atomic Habits", Author = "James Clear", Genre = "Education" },
            new() { Title = "Thinking, Fast and Slow", Author = "Daniel Kahneman", Genre = "Education" },
            new() { Title = "Becoming", Author = "Michelle Obama", Genre = "Biography" },
            new() { Title = "The Lord of the Rings", Author = "J.R.R. Tolkien", Genre = "Fantasy" },
            new() { Title = "Fahrenheit 451", Author = "Ray Bradbury", Genre = "Sci-Fi" },
            new() { Title = "The Silent Patient", Author = "Alex Michaelides", Genre = "Mystery" },
            new() { Title = "The Alchemist", Author = "Paulo Coelho", Genre = "Fiction" },
            new() { Title = "The Road", Author = "Cormac McCarthy", Genre = "Fiction" },
            new() { Title = "Steve Jobs", Author = "Walter Isaacson", Genre = "Biography" },
            new() { Title = "Harry Potter and the Sorcerer's Stone", Author = "J.K. Rowling", Genre = "Fantasy" },
        };

        for (int i = 0; i < books.Count; i++)
        {
            books[i].IsAvailable = (i + 1) % 5 != 0;
        }

        db.Books.AddRange(books);
        db.SaveChanges();
        Console.WriteLine("Seeded 20 real-world books into the database.");
    }
    else
    {
        Console.WriteLine("Books already exist. Skipping seed.");
    }
}


// Seed loans
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!db.Loans.Any())
    {
        var users = db.Users.ToList();
        var books = db.Books.ToList();

        if (users.Count == 0 || books.Count < 10)
        {
            Console.WriteLine("Not enough users or books to seed loans.");
        }
        else
        {
            var random = new Random();
            var loans = new List<Loan>();

            for (int i = 0; i < 10; i++)
            {
                var user = users[i % users.Count];
                var book = books[i];

                var borrowedAt = DateTime.UtcNow.AddDays(-random.Next(1, 20));
                var dueDate = borrowedAt.AddDays(14);

                loans.Add(new Loan
                {
                    PatronEmail = user.Email,
                    UserId = user.Id,
                    BookId = book.Id,
                    BorrowedAt = borrowedAt,
                    DueDate = dueDate,
                    IsReturned = false
                });

                // Optional: mark that this book is now loaned out
                book.IsAvailable = false;
            }

            db.Loans.AddRange(loans);
            db.SaveChanges();
            Console.WriteLine(" Seeded 10 loans into the database.");
        }
    }
    else
    {
        Console.WriteLine(" Loans already exist. Skipping loan seed.");
    }
}


app.Run();
