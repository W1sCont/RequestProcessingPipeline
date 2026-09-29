namespace RequestProcessingPipeline;

public class FromElevenToNineteenMiddleware
{
    private readonly RequestDelegate _next;

    public FromElevenToNineteenMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string? token = context.Request.Query["number"];

        if (!int.TryParse(token, out int number))
        {
            await context.Response.WriteAsync("Incorrect parameter");
            return;
        }

        number = Math.Abs(number);

        if (number < 11 || number > 19)
        {
            await _next.Invoke(context);
        }
        else
        {
            string[] numbers = { "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
            await context.Response.WriteAsync($"Your number is {numbers[number - 11]}");
        }
    }
}