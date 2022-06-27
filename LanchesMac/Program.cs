using LanchesMac.Context;
using Microsoft.EntityFrameworkCore;
using LanchesMac.Areas.Admin.Servicos;
using LanchesMac.Models;
using LanchesMac.Repositories;
using LanchesMac.Repositories.Interfaces;
using LanchesMac.Services;
using ReflectionIT.Mvc.Paging;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
         options.UseSqlServer(connection));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

builder.Services.Configure<ConfigurationImagens>(builder.Configuration.GetSection("ConfigurationPastaImagens"));

/* inicio sobrescrever as diretivas de senha do Identity */
//builder.Services.Configure<IdentityOptions>(options =>
//{
//    //Default Password settings.
//    options.Password.RequireDigit = true;
//    options.Password.RequireLowercase = true;
//    options.Password.RequireNonAlphanumeric = true;
//    options.Password.RequireUppercase = true;
//    options.Password.RequiredLength = 6;
//    options.Password.RequiredUniqueChars = 1;
//});
/* fim sobrescrever as diretivas de senha do Identity */


builder.Services.AddTransient<ILancheRepository, LancheRepository>();
builder.Services.AddTransient<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddTransient<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<ISeedUserRoleInitial, SeedUserRoleInitial>();
builder.Services.AddScoped<RelatorioVendasService>();
builder.Services.AddScoped<GraficoVendasService>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin",
        politica =>
        {
            politica.RequireRole("Admin");
        });
});




builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped(sp => CarrinhoCompra.GetCarrinho(sp));

builder.Services.AddControllersWithViews();

builder.Services.AddPaging(options =>
{
    options.ViewName = "Bootstrap4";
    options.PageParameterName = "pageindex";
});

//builder.Services.AddDistributedMemoryCache();
builder.Services.AddMemoryCache();
builder.Services.AddSession();


//Add services to the container.
//builder.Services.AddControllersWithViews();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

CriarPerfisUsuario(app);

//seedUserRoleInitial.SeedRoles();    //cria os perfis
//seedUserRoleInitial.SeedUsers();    //cria os usuários e atribui ao perfil

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

//app.Run(async (context) =>
//{
//    await context.Response.WriteAsync(msg.GetMensagem());
//});

app.UseEndpoints(endpoints =>
{

    endpoints.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}"
    );

    endpoints.MapControllerRoute(
        name: "categoriaFiltro",
        pattern: "Lanche/{action}/{categoria?}",
        defaults: new { Controller = "Lanche", action = "List" });

    //endpoints.MapControllerRoute(
    //    name: "Produtos",
    //    //areaName: "Produtos",
    //    pattern: "Produtos/{controller=Admin}/{action=Index}/{id?}");
    //
    //endpoints.MapControllerRoute(
    //    name: "AdminArea",
    //    pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}");

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

//Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();
//app.UseRouting();
//app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}"
//    );

//configurar o pipeline do request da aplicação
app.Run();

void CriarPerfisUsuario(WebApplication app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();
    using (var scope = scopedFactory.CreateScope())
    {
        var service = scope.ServiceProvider.GetService<ISeedUserRoleInitial>();
        service.SeedUsers();
        service.SeedRoles();
    }
}


//------------------------------------------------

//namespace LanchesMac;

//public class Program
//{
//    public static void Main(string[] args)
//    {
//        CreateHostBuilder(args).Build().Run();
//    }

//    public static IHostBuilder CreateHostBuilder(string[] args) =>
//        Host.CreateDefaultBuilder(args)
//            .ConfigureWebHostDefaults(webBuilder =>
//            {
//                webBuilder.UseStartup<Startup>();
//            });
//}