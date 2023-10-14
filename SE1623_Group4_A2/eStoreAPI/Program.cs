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
            ODataConventionModelBuilder edmBuilder = new ODataConventionModelBuilder();
            edmBuilder.EntitySet<Category>("Category");
            edmBuilder.EntitySet<Member>("Member");
            edmBuilder.EntitySet<Order>("Order");
            edmBuilder.EntitySet<OrderDetail>("OrderDetail");
            edmBuilder.EntitySet<Product>("Product");

            IEdmModel edmModel = edmBuilder.GetEdmModel();

            var builder = WebApplication.CreateBuilder(args);

            //Add Service InMemoryData
            builder.Services.AddDbContext<EStoreContext>();

            // Add services to the container.
            builder.Services.AddControllers();

            //Add Service Odata
            builder.Services.AddControllers().AddOData(opt
                => opt.Select().Filter().Count().OrderBy().Expand().SetMaxTop(10)
                .AddRouteComponents("odata", edmModel));

            //Setup for json parsing
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.DictionaryKeyPolicy = null;
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

            app.UseAuthorization();

            app.UseODataBatching();

            app.MapControllers();

            app.Run();
        }
    }
}