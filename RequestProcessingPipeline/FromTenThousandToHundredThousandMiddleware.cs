namespace RequestProcessingPipeline
{
    public class FromTenThousandToHundredThousandMiddleware
    {
        private readonly RequestDelegate _next;

        public FromTenThousandToHundredThousandMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string? token = context.Request.Query["number"]; // Отримуємо число з контексту запиту
            string[] thousands = { "ten thousand", "twenty thousand", "thirty thousand", "forty thousand", "fifty thousand",
                "sixty thousand", "seventy thousand", "eighty thousand", "ninety thousand" };
            if (!int.TryParse(token, out int number))
            {
                // Видаємо остаточну відповідь клієнту
                await context.Response.WriteAsync("Incorrect parameter");
                return;
            }
            number = Math.Abs(number);
            if (number < 10000)
            {
                // Передаємо контекст запиту наступному компоненту
                await _next.Invoke(context);
            }
            else if (number > 100000)
            {
                await context.Response.WriteAsync("Your number is greater than one hundred thousand");
            }
            else if (number == 100000)
            {
                // Видаємо остаточну відповідь клієнту
                await context.Response.WriteAsync("Your number is one hundred thousand");
            }
            else
            {
                if (number % 100000 == 0)
                {
                    // Видаємо остаточну відповідь клієнту
                    await context.Response.WriteAsync($"Your number is {thousands[number / 10000 - 1]}");
                }
                else
                {
                    // Передаємо контекст запиту наступному компоненту
                    await _next.Invoke(context);
                    string? result = context.Session.GetString("number");
                    // Видаємо остаточну відповідь клієнту
                    await context.Response.WriteAsync($"Your number is {thousands[number / 10000 - 1]} {result}");
                }
            }

        }
    }
}
