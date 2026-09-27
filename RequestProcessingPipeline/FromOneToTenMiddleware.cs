namespace RequestProcessingPipeline;

public class FromOneToTenMiddleware
{
    private readonly RequestDelegate _next;

    public FromOneToTenMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string? token = context.Request.Query["number"]; // Отримуємо число з контексту запиту

        if (!int.TryParse(token, out int number))
        {
            // Видаємо остаточну відповідь клієнту
            await context.Response.WriteAsync("Incorrect parameter");
            return;
        }

        number = Math.Abs(number);

        if (number == 0)
        {
            await context.Response.WriteAsync("Your number is zero");
            return;
        }

        if (number == 10)
        {
            // Видаємо остаточну відповідь клієнту
            await context.Response.WriteAsync("Your number is ten");
        }
        else
        {
            string[] ones = { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

            // Будь-які числа більші за 20, але не кратні 10
            if (number > 20)
            {
                int lastTwoDigits = number % 100; 
                int unit = number % 10;          

                if (lastTwoDigits == 10)
                {
                    context.Session.SetString("number", "ten");
                }
                else if (unit > 0)
                {
                    context.Session.SetString("number", ones[unit - 1]);
                }
            }
            else
            {
                // Видаємо остаточну відповідь клієнту (від 1 до 9)
                await context.Response.WriteAsync($"Your number is {ones[number - 1]}");
            }
        }
    }
}