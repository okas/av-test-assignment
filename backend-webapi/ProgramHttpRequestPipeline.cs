namespace Backend.WebApi;

public static class ProgramHttpRequestPipeline
{
    public static WebApplication ConfigureHttpRequestPipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            SetupFirstOrderDevelopmentOnlyMiddlewares(app);
        }
        else
        {
            SetupFirstOrderProductionOnlyMiddlewares(app);
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        return app;
    }

    private static WebApplication SetupFirstOrderDevelopmentOnlyMiddlewares(this WebApplication app)
    {
        app.UseSwagger();

        app.UseSwaggerUI(c =>
        {
            c.DisplayOperationId();
        });

        return app;
    }

    private static void SetupFirstOrderProductionOnlyMiddlewares(this WebApplication app)
    {
        throw new NotImplementedException();
    }
}
