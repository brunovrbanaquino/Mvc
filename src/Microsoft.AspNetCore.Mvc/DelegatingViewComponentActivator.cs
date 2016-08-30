// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// Wrapper around delegates that allow creation and releasing ViewComponent instances.
    /// </summary>
    public sealed class DelegatingViewComponentActivator : IViewComponentActivator
    {
        private readonly Func<ViewComponentContext, object> viewComponentCreator;
        private readonly Action<ViewComponentContext, object> viewComponentReleaser;

        /// <summary>Creates a new <see cref="DelegatingViewComponentActivator"/> instance.</summary>
        /// <param name="viewComponentCreator"
        /// >The delegate that creates a view component based on the supplied <see cref="ViewComponentContext"/>.</param>
        /// <param name="viewComponentReleaser">An optional delegate that allows releasing created view component instances.</param>
        public DelegatingViewComponentActivator(Func<ViewComponentContext, object> viewComponentCreator,
            Action<ViewComponentContext, object> viewComponentReleaser = null)
        {
            if (viewComponentCreator == null)
            {
                throw new ArgumentNullException(nameof(viewComponentCreator));
            }
            
            this.viewComponentCreator = viewComponentCreator;
            this.viewComponentReleaser = viewComponentReleaser ?? ((_, __) => { });
        }

        /// <summary>Instantiates a ViewComponent.</summary>
        /// <param name="context">The <see cref="ViewComponentContext"/> for the executing <see cref="ViewComponent"/>.</param>
        public object Create(ViewComponentContext context) => this.viewComponentCreator(context);

        /// <summary>Releases a ViewComponent instance.</summary>
        /// <param name="context">The <see cref="ViewComponentContext"/> associated with the <paramref name="viewComponent"/>.</param>
        /// <param name="viewComponent">The <see cref="ViewComponent"/> to release.</param>
        public void Release(ViewComponentContext context, object viewComponent)
            => this.viewComponentReleaser(context, viewComponent);
    }
}