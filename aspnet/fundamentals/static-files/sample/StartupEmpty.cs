//#define First
#if First
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace empty
{
    public class StartupEmpty
    {
        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDefaultFiles();
            app.UseStaticFiles();
        }
    }
}

#endif