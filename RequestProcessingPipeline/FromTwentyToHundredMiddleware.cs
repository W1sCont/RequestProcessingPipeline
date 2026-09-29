namespace RequestProcessingPipeline;

public class FromTwentyToHundredMiddleware
{
    private readonly RequestDelegate _next;

    public FromTwentyToHundredMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string? token = context.Request.Query["number"];
        string[] tens = { "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

        if (!int.TryParse(token, out int number))
        {
            await context.Response.WriteAsync("Incorrect parameter");
            return;
        }

        number = Math.Abs(number);

        if (number < 20)
        {
            // Передаємо контекст запиту наступному компоненту
            await _next.Invoke(context);
        }
        else if (number > 100)
        {
            int num = number % 100;
            if(num >= 20 && num < 100)
            {
                string currentTen = tens[num / 10 - 2];

                if (num % 10 == 0)
                {
                    context.Session.SetString("number", currentTen);
                }
                else
                {
                    await _next.Invoke(context);
                    string? unitPart = context.Session.GetString("number");
                    context.Session.SetString("number", $"{currentTen} {unitPart}");
                }
            }
            else await _next.Invoke(context);
        }
        else if (number == 100)
        {
            // Видаємо остаточну відповідь клієнту
            await context.Response.WriteAsync("Your number is one hundred");
        }
        else
        {
            if (number % 10 == 0)
            {
                // Видаємо остаточну відповідь клієнту
                await context.Response.WriteAsync($"Your number is {tens[number / 10 - 2]}");
            }
            else
            {
                // Передаємо контекст запиту наступному компоненту
                await _next.Invoke(context);

                // Отримуємо число від компонента FromOneToTenMiddleware
                string? result = context.Session.GetString("number");

                // Видаємо остаточну відповідь клієнту
                await context.Response.WriteAsync($"Your number is {tens[number / 10 - 2]} {result}");
            }
        }
    }
}