namespace RequestProcessingPipeline
{
    public class FromThousandToHundredThousandMiddleware
    {
        private readonly RequestDelegate _next;

        public FromThousandToHundredThousandMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string? token = context.Request.Query["number"];
            string[] ones = { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
            string[] from10kTo19k = { "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen",
                "sixteen", "seventeen", "eighteen", "nineteen" };
            string[] from20kTo90k = { "twenty", "thirty", "forty", "fifty",
                "sixty", "seventy", "eighty", "ninety" };

            if (!int.TryParse(token, out int number))
            {
                await context.Response.WriteAsync("Incorrect parameter");
                return;
            }
            number = Math.Abs(number);

            int thousandsCount = number / 1000;
            string thousandsText = "";

            if (number > 100000)
            {
                await context.Response.WriteAsync("Number greater than one hundred thousand");
                return;
            }

            if (number == 100000)
            {
                await context.Response.WriteAsync("Your number is one hundred thousand");
                return;
            }

            if(number >= 1000)
            {
                if (thousandsCount >= 1 && thousandsCount <= 9)
                {
                    thousandsText = $"{ones[thousandsCount - 1]} thousand";
                }
                else if (thousandsCount >= 10 && thousandsCount <= 19)
                {
                    thousandsText = $"{from10kTo19k[thousandsCount - 10]} thousand";
                }
                else if (thousandsCount >= 20 && thousandsCount < 100)
                {
                    int tens = thousandsCount / 10;
                    int units = thousandsCount % 10;

                    if (units == 0)
                        thousandsText = $"{from20kTo90k[tens - 2]} thousand";
                    else
                        thousandsText = $"{from20kTo90k[tens - 2]} {ones[units - 1]} thousand";
                }
            }

            if (number % 10000 == 0)
            {
                await context.Response.WriteAsync($"Your number is {thousandsText}");
                return;
            }
            else if(number > 1000 && number < 100000)
            {
                await _next.Invoke(context);
                string? lowerPart = context.Session.GetString("number");
                await context.Response.WriteAsync($"Your number is {thousandsText} {lowerPart}");
                return;
            }
            else
            {
                await _next.Invoke(context);
            }
        }
    }
}