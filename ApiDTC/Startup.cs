﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiDTC.Data;
using ApiDTC.Services;
using ApiDTC.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ApiDTC
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
            services.AddCors();
            services.AddScoped<ApiLogger>();
            services.AddScoped<BotDb>();
            services.AddScoped<CalendarioDb>();
            services.AddScoped<UserDb>();
            services.AddScoped<SqlResult>();
            services.AddScoped<DtcDataDb>();
            services.AddScoped<ComponentDb>();
            services.AddScoped<LoginDb>();
            services.AddScoped<MantenimientoPdfDb>();
            services.AddScoped<PdfConsultasDb>();
            services.AddScoped<TypeDescriptionsDb>();
            services.AddScoped<RequestedComponentDb>();
            services.AddScoped<SquaresCatalogDb>();
            services.AddScoped<EmailDb>();
            services.AddScoped<MantenimientoDb>();
            services.AddScoped<DiagnosticoDb>();
            services.AddScoped<FichaTecnicaDb>();
            services.AddScoped<InventarioDb>();
            services.AddScoped<ReporteFotograficoDB>();
            services.AddScoped<ComentarioDb>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:key"])),
                    ClockSkew = TimeSpan.Zero
                });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors(x =>
            {
                x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
            });

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}