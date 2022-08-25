using Microsoft.EntityFrameworkCore; //EfCore Eklenmesi
using Todo.Models; //Modallar buraya taþýnmasý
using Todo.Data;
using Todo.Auth;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase("Todo"));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

int getUserIdFunction(HttpContext httpContext, TodoContext db)
{

    string[] userCredential = Auth.getUserId(httpContext);

    string userEmail = "";
    string password = "";
    int userId = 0;

    if (userCredential[0] != "InvalidCreadentials")
    {
        userEmail = userCredential[0];
        password = userCredential[1];

        var helper = async () =>
        {
            try
            {
                userId = (await db.Users.Where(user => user.Email == userEmail && user.Password == password)
                .ToListAsync())[0].Id;
            }
            catch (Exception e)
            {
                //Bir þerler gönder...
            }
        };
        helper();
    }

    return userId;
};

//Ana Sayfa-------------

app.MapGet("", (HttpContext httpContext, TodoContext db) =>
{

    //Auth.getUserId(httpContext);

    return;
});

//Users ile ilgili olan End-points

app.MapGet("api/users", async (TodoContext db) =>
    await db.Users.ToListAsync());

app.MapGet("api/users/{id}", async (int id, TodoContext db) =>
    await db.Users.FindAsync(id) is User user
            ? Results.Ok(user)
            : Results.NotFound());

app.MapPost("api/users", async (User user, TodoContext db) =>
{
    db.Users.Add(user);
    await db.SaveChangesAsync();

    return Results.Created($"/users/{user.Id}", user);
});
//Users Update EndPoint
app.MapPut("api/users/{id}", async (int id, User yeniUser, TodoContext db) =>
{
    var user = await db.Users.FindAsync(id);

    if (user is null) return Results.NotFound();

    user.Name = yeniUser.Name;
    user.Email = yeniUser.Email;
    user.Password = yeniUser.Password;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

//User delete EndPoint
app.MapDelete("api/users/{id}", async (int id, TodoContext db) =>
{
    if (await db.Users.FindAsync(id) is User user)
    {
        db.Users.Remove(user);
        await db.SaveChangesAsync();
        return Results.Ok(user);
    }

    return Results.NotFound();
});

//END USERS------


//Todo Enpoint--------

app.MapGet("api/todoitems",
    async (HttpContext httpContext, TodoContext db) =>
    {
        //user Id request'ten çekiyoruz...
        int userId = getUserIdFunction(httpContext, db);
        var user = await db.Users.FindAsync(userId);

        // Kullanýcý bilgileri yanlýþ ise
        if (user is null) return Results.NotFound(
            @"Authorization kýsýmda kullanýncý bilgileri girmeniz gerek. ya da girdikleriniz yanlýþtýr"
        );

        return Results.Ok(await db.TodoItems.Where(todo => todo.UserId == userId).ToListAsync());
    }
);

// GET todoitems/{id}
app.MapGet("api/todoitems/{id}",
    async (int id, HttpContext httpContext, TodoContext db) =>
    {
        //user Id request'ten çekiyoruz...
        int userId = getUserIdFunction(httpContext, db);
        var user = await db.Users.FindAsync(userId);
        var todo = await db.TodoItems.FindAsync(id);

        // Kullanýcý bilgileri yanlýþ ise
        if (user is null) return Results.NotFound(
            @"Authorization kýsýmda kullanýncý bilgileri girmeniz gerek. ya da girdikleriniz yanlýþtýr"
        );

        if (todo is null) return Results.NotFound();

        return Results.Ok(todo);
    }
);

// POST todoitems/{id}
app.MapPost("api/todoitems", async (TodoItem todo, HttpContext httpContext, TodoContext db) =>
{

    int userId = getUserIdFunction(httpContext, db);
    var user = await db.Users.FindAsync(userId);

    //Boyle bir kullanýcý yoksa
    if (user is null) return Results.NotFound(
        @"Authorization kýsýmda kullanýncý bilgileri girmeniz gerek. ya da girdikleriniz yanlýþtýr"
    );

    //todo.UserId = userId;

    db.TodoItems.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/todoitems/{todo.Id}", todo);

});

//PUT /todoitems/{id}
app.MapPut("api/todoitems/{id}", async (int id, TodoItem inputTodo, HttpContext httpContext, TodoContext db) =>
{
    int userId = getUserIdFunction(httpContext, db);
    var user = await db.Users.FindAsync(userId);
    var todo = await db.TodoItems.FindAsync(id);

    if (todo is null || user is null) return Results.NotFound();

    todo.Description = inputTodo.Description;
    todo.IsCompleted = inputTodo.IsCompleted;
    todo.UserId = userId;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

//DELETE /todoitems/{id}
app.MapDelete("api/todoitems/{id}", async (int id, HttpContext httpContext, TodoContext db) =>
{

    int userId = getUserIdFunction(httpContext, db);
    var user = await db.Users.FindAsync(userId);
    var todo = await db.TodoItems.FindAsync(id);

    //Boyle bir kullanýcý yoksa
    if (user is null) return Results.NotFound(
        @"Authorization kýsýmda kullanýncý bilgileri girmeniz gerek. ya da girdikleriniz yanlýþtýr"
    );

    //
    if (!(todo == null))
    {
        db.TodoItems.Remove(todo);
        await db.SaveChangesAsync();
        return Results.Ok(todo);
    }

    return Results.NotFound();
});

//END ----------------

//Start listening on a random port...

app.Run();

