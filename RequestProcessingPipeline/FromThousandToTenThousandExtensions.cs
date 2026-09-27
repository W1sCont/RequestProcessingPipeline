namespace RequestProcessingPipeline
{
    public static class FromThousandToTenThousandExtensions
    {
        public static IApplicationBuilder UseFromThousandToTenThousandMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<FromThousandToTenThousandMiddleware>();
        }
    }
}
