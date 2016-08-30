// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// Wrapper around delegates that allow creation and releasing controller instances.
    /// </summary>
    public sealed class DelegatingControllerActivator : IControllerActivator
    {
        private readonly Func<ControllerContext, object> controllerCreator;
        private readonly Action<ControllerContext, object> controllerReleaser;

        /// <summary>Creates a new <see cref="DelegatingControllerActivator"/> instance.</summary>
        /// <param name="controllerCreator"
        /// >The delegate that creates a controller based on the supplied <see cref="ControllerContext"/>.</param>
        /// <param name="controllerReleaser">An optional delegate that allows releasing created controller instances.</param>
        public DelegatingControllerActivator(Func<ControllerContext, object> controllerCreator,
            Action<ControllerContext, object> controllerReleaser = null)
        {
            if (controllerCreator == null)
            {
                throw new ArgumentNullException(nameof(controllerCreator));
            }

            this.controllerCreator = controllerCreator;
            this.controllerReleaser = controllerReleaser ?? ((_, __) => { });
        }

        /// <summary>Creates a controller.</summary>
        /// <param name="context">The <see cref="ControllerContext"/> for the executing action.</param>
        public object Create(ControllerContext context) => this.controllerCreator(context);

        /// <summary>Releases a controller.</summary>
        /// <param name="context">The <see cref="ControllerContext"/> for the executing action.</param>
        /// <param name="controller">The controller to release.</param>
        public void Release(ControllerContext context, object controller) => this.controllerReleaser(context, controller);
    }
}