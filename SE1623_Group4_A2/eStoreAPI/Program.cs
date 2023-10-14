using eStoreAPI.Models;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace eStoreAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Get EDM 
            ODataConventionModelBuilder edmBuilder = new();
            edmBuilder.EntitySet<OrderDetail>("OrderDetails");
            edmBuilder.EntitySet<Order>("Orders");            
            edmBuilder.EntitySet<Product>("Products");
            edmBuilder.EntitySet<Member>("Members");
            edmBuilder.EntitySet<Category>("Categories");

            var builder = WebApplication.CreateBuilder(args);

            //Add Service InMemoryData
            builder.Services.AddDbContext<EStoreContext>();

            builder.Services.AddControllers();

            //Add Service Odata
            builder.Services.AddControllers().AddOData(opt
                => opt.Select().Filter().Count().OrderBy().Expand().SetMaxTop(10)
                .AddRouteComponents("odata", edmBuilder.GetEdmModel()));

            //Setup for json parsing
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseODataBatching();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());

            app.Run();
        }
    }
}