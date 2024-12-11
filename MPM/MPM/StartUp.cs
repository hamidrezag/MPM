
using Creadit.WebApi.ServiceRegistering;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerUI;
using Infrastructure.Context;
using Infrastructure.Utils;

namespace Creadit.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddControllersWithViews();

            services.RegisterCors(Configuration);
            services.AddControllers()
                .AddJsonOptions(opt => opt.JsonSerializerOptions.PropertyNamingPolicy = null);
            services.AddHttpContextAccessor();
            services.AddDbContext<AppDbContext>(option =>
            {
                option
                .UseLazyLoadingProxies()
                .UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                option.EnableSensitiveDataLogging();
            });
            services.AddScoped<AppDbContext>();
            services.RegisterSwagger();
            services.ConfigureOptions<ConfigureSwaggerOptions>();
            //services.RegisterMiddlewares();
            //services.RegisterValidations();
            //services.RegisterAuthentication(Configuration);
            //services.RegisterApiVersionning();
            //services.RegisterRefitClient(Configuration);
            //services.RegisterConfigs(Configuration);
            /////services.AddAllElasticApm();
            //services.RegisterMassTransit(Configuration);
            //services.AddInfrastructureServiceWithAttribute();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {


            app.UseCors("CorsPolicy");
            app.UseRouting();
            //app.RegisterMiddlewares();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "shortlink", pattern: "IPay/{phonenumber?}/{iid?}", defaults: new
                {
                    controller = "ShortLinks",
                    action = "IPay"
                });
                endpoints.MapControllers();
            });
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.RoutePrefix = string.Empty;
                options.DocumentTitle = "Creadit Api";
#if DEBUG
                options.Interceptors = new InterceptorFunctions
                {
                    //RequestInterceptorFunction = "function (req) { req.headers['Authorization'] = 'Bearer eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjJFMEY4RUFELUYyREItNDdCMy05NUNGLTJBRjZDRjhBNUNDQSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiIwOTE5MjAzNzI0OSIsImV4cCI6MTc2MDM1NjM2MywiaXNzIjoiaHR0cHM6Ly9zZXRhcmV5ZWsuaXIiLCJhdWQiOiJodHRwczovL3NldGFyZXllay5pciJ9.8cSsI25MjM127UF1Kn_XcZnV0JzLCOVG6gjIjbbasKw'; return req; }"
                    // ashkan
                    //RequestInterceptorFunction = "function (req) { req.headers['Authorization'] = 'Bearer eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6ImQyZTMyY2VhLTFiYWMtNDcxOC05NmMzLWRlZmVjOGI5ZDZiOCIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiIwOTE5NjU0NjM0NiIsImV4cCI6MTc3NjI0NDI1NSwiaXNzIjoiaHR0cHM6Ly9zZXRhcmV5ZWsuaXIiLCJhdWQiOiJodHRwczovL3NldGFyZXllay5pciJ9.8LBVYHprYgnLZ0iXvTHu3Lz5Gyzn2rbPdwR5Zhn63A0'; return req; }"
                    // hamidreza golmohammadi
                    RequestInterceptorFunction = "function (req) { req.headers['Authorization'] = 'Bearer eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IkQ0RDQ2MUQ3LUZEQkItNDZEQi1CNjBCLUE5NUU3NEU3NENGMCIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiIwOTMwNzM3NDc5NyIsImV4cCI6MTc2MTQ4MDEzOCwiaXNzIjoiaHR0cHM6Ly9zZXRhcmV5ZWsuaXIiLCJhdWQiOiJodHRwczovL3NldGFyZXllay5pciJ9.BAAbW4CutPe_f-ozuZ0yzLKgfsCoKe0Qvi91qjg4Jl8'; return req; }"
                    // mohammad rahmati
                    //RequestInterceptorFunction = "function (req) { req.headers['Authorization'] = 'Bearer eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjMxZjYxN2U4LTQzODMtNDI2Ni04ODFiLThhZTczZThiMDdhMSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiIwOTE5ODgyNzg4MSIsImV4cCI6MTc3NjA3Mjg4MSwiaXNzIjoiaHR0cHM6Ly9zZXRhcmV5ZWsuaXIiLCJhdWQiOiJodHRwczovL3NldGFyZXllay5pciJ9.7wEnUe5nq5Olha4uZ1Leuy8XUQvWuogcYsUMLNhXNYs'; return req; }"
                    // shayan vosoughi                   
                    //RequestInterceptorFunction = "function (req) { req.headers['Authorization'] = 'Bearer eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjI0Q0Y5RDY0LUU0NzQtNDRDQy05RkE1LUNBMkUwRTk4OTREOCIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiIwOTkxMzQxMzYyMSIsImV4cCI6MTc3NTM3MzEwNSwiaXNzIjoiaHR0cHM6Ly9zZXRhcmV5ZWsuaXIiLCJhdWQiOiJodHRwczovL3NldGFyZXllay5pciJ9.5TgmXdKv1cjT8ZXtSexXKvUaA07adV-gxzln4BGjUlc'; return req; }"
                };

#endif
                foreach (var description in app.ApplicationServices.GetService<IApiVersionDescriptionProvider>().ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint(
                        $"/swagger/{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant());
                }
            });
            InitializeDatabase(app);
        }
        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                context.Database.EnsureCreated();
            }
        }
    }
}
