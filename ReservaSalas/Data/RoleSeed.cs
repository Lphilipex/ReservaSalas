using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ReservaSalas.Data.ReservaSalas.Models;
using ReservaSalas.Models;
using System.Threading.Tasks;

namespace ReservaSalas.Data
{
    public static class RoleSeed
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<Usuario>>();

            string[] roles = { "Admin", "Usuario" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminEmail = "admin@admin.com";
            var adminPassword = "Admin@123";

            var admin = await userManager.FindByEmailAsync(adminEmail);

            if (admin == null)
            {
                var usuario = new Usuario();
                usuario.UserName = adminEmail;
                usuario.Email = adminEmail;
                usuario.EmailConfirmed = true;
                usuario.Nome = "Admin";
                usuario.Sobrenome = "Master";
                usuario.Telefone = "999999999";
                usuario.Sexo = "O";

                await userManager.CreateAsync(usuario, adminPassword);
                await userManager.AddToRoleAsync(usuario, "Admin");
            }
        }
    }
}
