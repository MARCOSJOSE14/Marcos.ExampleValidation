using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/api/user", async Task<Results<Ok<string>, ValidationProblem>> (EntityUser entityUser, IValidator<EntityUser> validator) =>
{
    var result = await validator.ValidateAsync(entityUser);
    if (result.IsValid) return TypedResults.Ok("Entity pass all the test of fluentValidation");
    return TypedResults.ValidationProblem(result.ToDictionary());
});

app.Run();
 
public class EntityUser
{
    public int UserId { get; set; }
    public string FullName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime Birth { get; set; }
    public decimal Weight { get; set; }
    public int Height { get; set; }
    public string CreditCard { get; set; } = null!;
    public UserStatus Status { get; set; }
    public int RoleId { get; set; }
    public string Type { get; set; } = null!;
}

public enum UserStatus
{
    Active,
    Inactive,
    Pending,
    Completed
}

enum UserType
{
    Saler,
    Builder
}

public class ValidatorUser : AbstractValidator<EntityUser>
{
    public ValidatorUser()
    {
        RuleFor(x => x.FullName).NotNull().WithMessage("It's Null").NotEmpty().WithMessage("It's empty");
        RuleFor(x => x.LastName).NotNull().WithMessage("It's Null").NotEmpty().WithMessage("It's empty");
        RuleFor(x => x.Weight).GreaterThan(30.5m).WithMessage("Nobody weighs 30.5 kilos");
        RuleFor(x => x.Height).GreaterThan(50).WithMessage("Nobody measures 50 cm");
        RuleFor(x => x.CreditCard).CreditCard().WithMessage("The credit card isn't correct");
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Type).IsEnumName(typeof(UserType), caseSensitive:false);
        RuleFor(x => x.Birth).Must(x => x > new DateTime(2001, 1, 1)).WithMessage("The datetime is incorrect");
        RuleFor(x => x.RoleId).MustAsync(async (x, cancellation) => 
        {
            await Task.Delay(300, cancellation);
            
            List<int> numbers = [1, 2, 3, 4];
            return numbers.Contains(x);
        }    
        );
    }
}
