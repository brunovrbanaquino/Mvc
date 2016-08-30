// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for setting up MVC services in an <see cref="IServiceCollection" />.
    /// </summary>
    public static class MvcServiceCollectionExtensions
    {
        /// <summary>
        /// Adds MVC services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <returns>An <see cref="IMvcBuilder"/> that can be used to further configure the MVC services.</returns>
        public static IMvcBuilder AddMvc(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var builder = services.AddMvcCore();

            builder.AddApiExplorer();
            builder.AddAuthorization();

            AddDefaultFrameworkParts(builder.PartManager);

            // Order added affects options setup order

            // Default framework order
            builder.AddFormatterMappings();
            builder.AddViews();
            builder.AddRazorViewEngine();
            builder.AddCacheTagHelper();

            // +1 order
            builder.AddDataAnnotations(); // +1 order

            // +10 order
            builder.AddJsonFormatters();

            builder.AddCors();

            return new MvcBuilder(builder.Services, builder.PartManager);
        }

        private static void AddDefaultFrameworkParts(ApplicationPartManager partManager)
        {
            var mvcTagHelpersAssembly = typeof(InputTagHelper).GetTypeInfo().Assembly;
            if (!partManager.ApplicationParts.OfType<AssemblyPart>().Any(p => p.Assembly == mvcTagHelpersAssembly))
            {
                partManager.ApplicationParts.Add(new AssemblyPart(mvcTagHelpersAssembly));
            }

            var mvcRazorAssembly = typeof(UrlResolutionTagHelper).GetTypeInfo().Assembly;
            if (!partManager.ApplicationParts.OfType<AssemblyPart>().Any(p => p.Assembly == mvcRazorAssembly))
            {
                partManager.ApplicationParts.Add(new AssemblyPart(mvcRazorAssembly));
            }
        }

        /// <summary>
        /// Adds MVC services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="setupAction">An <see cref="Action{MvcOptions}"/> to configure the provided <see cref="MvcOptions"/>.</param>
        /// <returns>An <see cref="IMvcBuilder"/> that can be used to further configure the MVC services.</returns>
        public static IMvcBuilder AddMvc(this IServiceCollection services, Action<MvcOptions> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            var builder = services.AddMvc();
            builder.Services.Configure(setupAction);

            return builder;
        }

        /// <summary>
        /// Replaces the default <see cref="IControllerActivator"/> with one that resolves controllers using
        /// the supplied <paramref name="activator"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add the custom activator to.</param>
        /// <param name="activator">The activator to add.</param>
        public static void AddCustomControllerActivation(this IServiceCollection services, ISupportRequiredService activator)
        {
            if (activator == null)
            {
                throw new ArgumentNullException(nameof(activator));
            }

            services.AddCustomControllerActivation(activator.GetRequiredService);
        }

        /// <summary>
        /// Replaces the default <see cref="IControllerActivator"/> with one that resolves controllers using
        /// the supplied <paramref name="activator"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add the custom activator to.</param>
        /// <param name="activator">The activator to add.</param>
        public static void AddCustomControllerActivation(this IServiceCollection services, Func<Type, object> activator)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (activator == null)
            {
                throw new ArgumentNullException(nameof(activator));
            }

            services.AddSingleton<IControllerActivator>(
                new DelegatingControllerActivator(context => context.ActionDescriptor.ControllerTypeInfo.AsType()));
        }

        /// <summary>
        /// Replaces the default <see cref="IViewComponentActivator"/> with one that resolves view components using
        /// the supplied <paramref name="activator"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add the custom activator to.</param>
        /// <param name="activator">The activator to add.</param>
        public static void AddCustomViewComponentActivation(this IServiceCollection services, ISupportRequiredService activator)
        {
            if (activator == null)
            {
                throw new ArgumentNullException(nameof(activator));
            }

            services.AddCustomViewComponentActivation(activator.GetRequiredService);
        }

        /// <summary>
        /// Replaces the default <see cref="IViewComponentActivator"/> with one that resolves view components using
        /// the supplied <paramref name="activator"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add the custom activator to.</param>
        /// <param name="activator">The activator to add.</param>
        public static void AddCustomViewComponentActivation(this IServiceCollection services, Func<Type, object> activator)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (activator == null)
            {
                throw new ArgumentNullException(nameof(activator));
            }

            services.AddSingleton<IViewComponentActivator>(
                new DelegatingViewComponentActivator(c => activator(c.ViewComponentDescriptor.TypeInfo.AsType())));
        }

        /// <summary>
        /// Replaces the default <see cref="ITagHelperActivator"/> with one that resolves tag helpers using
        /// the supplied <paramref name="activator"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add the custom activator to.</param>
        /// <param name="activator">The activator to add.</param>
        public static void AddCustomTagHelperActivator(this IServiceCollection services, ISupportRequiredService activator)
        {
            if (activator == null)
            {
                throw new ArgumentNullException(nameof(activator));
            }

            services.AddCustomTagHelperActivation(activator.GetRequiredService);
        }

        /// <summary>
        /// Replaces the default <see cref="ITagHelperActivator"/> with one that resolves tag helpers using
        /// the supplied <paramref name="activator"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add the custom activator to.</param>
        /// <param name="activator">The activator to add.</param>
        public static void AddCustomTagHelperActivation(this IServiceCollection services, Func<Type, object> activator)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (activator == null)
            {
                throw new ArgumentNullException(nameof(activator));
            }

            services.AddSingleton<ITagHelperActivator>(
                new DelegatingTagHelperActivator(activator));
        }


    }
}