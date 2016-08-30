// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Microsoft.AspNetCore.Mvc
{ 
    /// <summary>
    /// Wrapper around a delegate that allow creation of <see cref="ITagHelper"/> instances.
    /// </summary>
    public sealed class DelegatingTagHelperActivator : ITagHelperActivator
    {
        private readonly Func<Type, object> tagHelperCreator;

        /// <summary>Creates a new <see cref="DelegatingTagHelperActivator"/> instance.</summary>
        /// <param name="tagHelperCreator">
        /// The delegate that allows creating new type helper instances based on their type.
        /// </param>
        public DelegatingTagHelperActivator(Func<Type, object> tagHelperCreator)
        {
            if (tagHelperCreator == null)
            {
                throw new ArgumentNullException(nameof(tagHelperCreator));
            }

            this.tagHelperCreator = tagHelperCreator;
        }

        /// <summary>Creates an <see cref="ITagHelper"/>.</summary>
        /// <typeparam name="TTagHelper">The <see cref="ITagHelper"/> type.</typeparam>
        /// <param name="context">The <see cref="ViewContext"/> for the executing view.</param>
        /// <returns>The tag helper.</returns>
        public TTagHelper Create<TTagHelper>(ViewContext context) where TTagHelper : ITagHelper
        {
            return (TTagHelper)this.tagHelperCreator(typeof(TTagHelper));
        }
    }
}