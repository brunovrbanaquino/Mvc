using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// Extension methods for adding middleware.
    /// </summary>
    public static class UseExtensions
    {
        // NOTE: This method should probably be part of Microsoft.AspNetCore.Builder.UseExtensions.
        /// <summary>
        /// Adds a middleware delegate defined in-line to the application's request pipeline. The middleware
        /// delegate will be called before the request is handled, while its return value will be disposed
        /// after the request is handled.
        /// </summary>
        /// <param name="builder">The Microsoft.AspNetCore.Builder.IApplicationBuilder instance.</param>
        /// <param name="middleware">A function that allows wrapping the request in a pre and post operation,
        /// where the call to the function will produce the pre-operation, and function's returned
        /// <see cref="IDisposable"/> instance will be called after the request allowing the post-operation.</param>
        /// <returns>The Microsoft.AspNetCore.Builder.IApplicationBuilder instance.</returns>
        public static IApplicationBuilder UseMiddleware(this IApplicationBuilder builder, Func<IDisposable> middleware)
        {
            return builder.Use(async (context, next) =>
            {
                using (middleware())
                {
                    await next();
                }
            });
        }
    }

}
