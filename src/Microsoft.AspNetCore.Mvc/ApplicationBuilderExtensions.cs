using System;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// Application builder extensions.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Builds a delegate that will resolve the given <typeparamref name="T"/> as request service for the
        /// current HTTP request. The instance will be created according to its defined lifestyle.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="builder">The <see cref="IApplicationBuilder"/> instance.</param>
        /// <returns>A delegate that allows resolving the given <typeparamref name="T"/> according to
        /// its defined lifestyle.</returns>
        public static Func<T> GetRequestServiceProvider<T>(this IApplicationBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var accessor = builder.ApplicationServices.GetService<IHttpContextAccessor>();

            if (accessor == null)
            {
                throw new InvalidOperationException(
                    string.Format(CultureInfo.InvariantCulture,
                        "Type '{0}' is not available in the IApplicationBuilder.ApplicationServices collection. " +
                        "Please make sure it is registered by registering it in the ConfigureServices method " +
                        "as follows: services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();",
                        typeof(IHttpContextAccessor).FullName));
            }

            return () =>
            {
                var context = accessor.HttpContext;

                if (context == null)
                {
                    throw new InvalidOperationException(
                        "No HttpContext. Please make sure this method is " +
                        "called in the context of an active HTTP request.");
                }

                return context.RequestServices.GetRequiredService<T>();
            };
        }

        /// <summary>Returns the list of known controllers known to the current application.</summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/> instance.</param>
        /// <returns>The list of <see cref="Type"/> instances.</returns>
        public static Type[] GetApplicationControllers(this IApplicationBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var manager = builder.ApplicationServices.GetRequiredService<ApplicationPartManager>();

            var feature = new ControllerFeature();
            manager.PopulateFeature(feature);

            return feature.Controllers.Select(t => t.AsType()).ToArray();
        }

        /// <summary>Returns the list of known view components known to the current application.</summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/> instance.</param>
        /// <returns>The list of <see cref="Type"/> instances.</returns>
        public static Type[] GetApplicationViewComponents(this IApplicationBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var componentProvider = 
                builder.ApplicationServices.GetRequiredService<IViewComponentDescriptorProvider>();

            return componentProvider.GetViewComponents()
                .Select(description => description.TypeInfo.AsType())
                .ToArray();
        }
    }
}