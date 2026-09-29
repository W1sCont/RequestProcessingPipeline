namespace RequestProcessingPipeline
{
    public class FromHundredToThousandMiddleware
    {
        private readonly RequestDelegate _next;

        public FromHundredToThousandMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string? token = context.Request.Query["number"]; // Отримуємо число з контексту запиту
            string[] hundreds = { "one hundred", "two hundred", "three hundred", "four hundred", "five hundred", "six hundred", "seven hundred", "eight hundred", "nine hundred" };

            if (!int.TryParse(token, out int number))
            {
                // Видаємо остаточну відповідь клієнту
                await context.Response.WriteAsync("Incorrect parameter");
                return;
            }
            number = Math.Abs(number);
            if (number < 100)
            {
                // Передаємо контекст запиту наступному компоненту
                await _next.Invoke(context);
            }
            else if (number > 1000)
            {
                int num = number % 1000;
                if (num >= 100 && num < 1000)
                {
                    string current = hundreds[num / 100 - 1];

                    if (num % 100 == 0)
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
                else
                {
                    await _next.Invoke(context);
                    return;
                }
            }
            else if (number == 1000)
            {
                // Видаємо остаточну відповідь клієнту
                await context.Response.WriteAsync("Your number is one thousand");
                return;
            }
            else
            {
                if (number % 100 == 0)
                {
                    // Видаємо остаточну відповідь клієнту
                    await context.Response.WriteAsync($"Your number is {hundreds[number / 100 - 1]}");
                }
                else
                {
                    // Передаємо контекст запиту наступному компоненту
                    await _next.Invoke(context);
                    // Отримуємо число від компонента FromTwentyToHundredMiddleware
                    string? result = context.Session.GetString("number");
                    // Видаємо остаточну відповідь клієнту
                    await context.Response.WriteAsync($"Your number is {hundreds[number / 100 - 1]} {result}");
                }
            }
        }
    }
}
