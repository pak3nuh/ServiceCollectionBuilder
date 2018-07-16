﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NCaro.ServiceCollectionBuilder.Util;

namespace NCaro.ServiceCollectionBuilder.Builder
{
    
    /// <summary>
    /// Utility to create contextualized scopes for better service configuration. Uses the same principle
    /// of the configuration pattern to inject an instance of IScopeCtx configured with the necessary context
    /// on each scope creation.
    /// The problem with the options patters is that is binds a singleton service to a service collection and
    /// we loose the ability to provide different options on each scope.
    /// </summary>
    /// <typeparam name="TCtx">The context type</typeparam>
    public class ContextSpiBuilder<TCtx> where TCtx : class
    {
        private readonly IServiceProvider _spi;

        public ContextSpiBuilder(IServiceCollection sc)
        {
            sc.AddScoped<IScopeCtx<TCtx>, ScopeContext<TCtx>>();
            _spi = sc.BuildServiceProvider();
        }

        public IServiceProvider CreateScope(TCtx context)
        {
            var newSpi = _spi.CreateScope().ServiceProvider;
            //Explicit cast to access the set accessor
            var contextService = (ScopeContext<TCtx>)newSpi.GetRequiredService<IScopeCtx<TCtx>>();
            contextService.Context = context;
            return newSpi;
        }
    }

    /// <summary>
    /// Service interface accessible for scope context data
    /// </summary>
    /// <typeparam name="TC">Context type</typeparam>
    public interface IScopeCtx<out TC>
    {
        TC Context { get; }
    }
    
    /// <summary>
    /// The scope context implementation. This class should not be visible out of the assembly.
    /// Cannot segregate the set accessor in different interface because 2 instances would be created.
    /// </summary>
    /// <typeparam name="TCtx"></typeparam>
    internal class ScopeContext<TCtx> : IScopeCtx<TCtx>
    {
        
        public TCtx Context { get; set; }

    }
}