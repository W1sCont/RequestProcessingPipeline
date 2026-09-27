namespace RequestProcessingPipeline
{
    public class FromThousandToTenThousandMiddleware
    {
        private readonly RequestDelegate _next;

        public FromThousandToTenThousandMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string? token = context.Request.Query["number"]; // Отримуємо число з контексту запиту
            string[] thousands = { "one thousand", "two thousand", "three thousand", "four thousand", "five thousand",
                "six thousand", "seven thousand", "eight thousand", "nine thousand" };
            if (!int.TryParse(token, out int number))
            {
                // Видаємо остаточну відповідь клієнту
                await context.Response.WriteAsync("Incorrect parameter");
                return;
            }
            number = Math.Abs(number);
            if (number < 1000)
            {
                // Передаємо контекст запиту наступному компоненту
                await _next.Invoke(context);
            }
            else if (number > 10000)
            {
                int num = number % 10000;
                if (num >= 1000 && num < 10000)
                {
                    string current = thousands[num / 1000 - 1];

                    if (num % 10000 == 0)
                    {
                        context.Session.SetString("number", current);
                    }
                    else
                    {
                        await _next.Invoke(context);
                        string? unitPart = context.Session.GetString("number");
                        context.Session.SetString("number", $"{current} {unitPart}");
                    }
                }
                else await _next.Invoke(context);
            }
            else if (number == 10000)
            {
                // Видаємо остаточну відповідь клієнту
                await context.Response.WriteAsync("Your number is ten thousand");
            }
            else
            {
                if (number % 1000 == 0)
                {
                    // Видаємо остаточну відповідь клієнту
                    await context.Response.WriteAsync($"Your number is {thousands[number / 1000 - 1]}");
                }
                else
                {
                    // Передаємо контекст запиту наступному компоненту
                    await _next.Invoke(context);
                    string? result = context.Session.GetString("number");
                    // Видаємо остаточну відповідь клієнту
                    await context.Response.WriteAsync($"Your number is {thousands[number / 1000 - 1]} {result}");
                }
            }
        }
    
    }
}
