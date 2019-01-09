﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Hk.Core.Util.Contexts;
using Hk.Core.Util.Events.Handlers;
using Hk.Core.Util.Helper;
using Hk.Core.Util.Reflections;
using Microsoft.Extensions.DependencyInjection;

namespace Hk.Core.Util.Dependency
{
    public class Bootstrapper
    {
        /// <summary>
        /// 服务集合
        /// </summary>
        private readonly IServiceCollection _services;
        /// <summary>
        /// 依赖配置
        /// </summary>
        private readonly IConfig[] _configs;
        /// <summary>
        /// 上下文
        /// </summary>
        private readonly IContext _context;
        /// <summary>
        /// 类型查找器
        /// </summary>
        private readonly IFind _finder;
        /// <summary>
        /// 程序集列表
        /// </summary>
        private List<Assembly> _assemblies;
        /// <summary>
        /// 容器生成器
        /// </summary>
        private ContainerBuilder _builder;

        /// <summary>
        /// 初始化依赖引导器
        /// </summary>
        /// <param name="finder">类型查找器</param>
        /// <param name="context">上下文</param>
        /// <param name="services">服务集合</param>
        /// <param name="configs">依赖配置</param>
        public Bootstrapper(IServiceCollection services, IContext context, IConfig[] configs, IFind finder)
        {
            _services = services ?? new ServiceCollection();
            _context = context;
            _configs = configs;
            _finder = finder ?? new Finder();
        }

        /// <summary>
        /// 启动引导
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configs">依赖配置</param>
        /// <param name="finder">类型查找器</param>
        /// <param name="context">上下文</param>
        public static IServiceProvider Run(IServiceCollection services = null, IContext context = null, IConfig[] configs = null, IFind finder = null)
        {
            return new Bootstrapper(services, context, configs, finder).Bootstrap();
        }

        /// <summary>
        /// 启动引导
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="context">上下文</param>
        /// <param name="configs">依赖配置</param>
        public static IServiceProvider Run(IServiceCollection services, IContext context, params IConfig[] configs)
        {
            return Run(services, context, configs, null);
        }

        /// <summary>
        /// 引导
        /// </summary>
        public IServiceProvider Bootstrap()
        {
            _assemblies = _finder.GetAssemblies();
            return Ioc.DefaultContainer.Register(_services, RegisterServices, _configs);
        }

        /// <summary>
        /// 注册服务集合
        /// </summary>
        private void RegisterServices(ContainerBuilder builder)
        {
            _builder = builder;
            RegisterInfrastracture();
            RegisterEventHandlers();
            RegisterDependency();
        }

        /// <summary>
        /// 注册基础设施
        /// </summary>
        private void RegisterInfrastracture()
        {
            EnableAop();
            RegisterFinder();
            RegisterContext();
        }

        /// <summary>
        /// 启用Aop
        /// </summary>
        private void EnableAop()
        {
            _builder.EnableAop();
        }

        /// <summary>
        /// 注册类型查找器
        /// </summary>
        private void RegisterFinder()
        {
            _builder.AddSingleton(_finder);
        }

        /// <summary>
        /// 注册上下文
        /// </summary>
        private void RegisterContext()
        {
            if (_context != null)
                _builder.AddSingleton(_context);
        }

        /// <summary>
        /// 注册事件处理器
        /// </summary>
        private void RegisterEventHandlers()
        {
            RegisterEventHandlers(typeof(IEventHandler<>));
        }

        /// <summary>
        /// 注册事件处理器
        /// </summary>
        private void RegisterEventHandlers(Type handlerType)
        {
            var handlerTypes = GetTypes(handlerType);
            foreach (var handler in handlerTypes)
            {
                _builder.RegisterType(handler).As(handler.FindInterfaces(
                    (filter, criteria) => filter.IsGenericType && ((Type)criteria).IsAssignableFrom(filter.GetGenericTypeDefinition())
                    , handlerType
                )).InstancePerLifetimeScope();
            }
        }

        /// <summary>
        /// 获取类型集合
        /// </summary>
        private Type[] GetTypes(Type type)
        {
            return _finder.Find(type, _assemblies).ToArray();
        }

        /// <summary>
        /// 查找并注册依赖
        /// </summary>
        private void RegisterDependency()
        {
            RegisterSingletonDependency();
            RegisterScopeDependency();
            RegisterTransientDependency();
            ResolveDependencyRegistrar();
        }

        /// <summary>
        /// 注册单例依赖
        /// </summary>
        private void RegisterSingletonDependency()
        {
            _builder.RegisterTypes(GetTypes<ISingletonDependency>()).AsImplementedInterfaces().PropertiesAutowired().SingleInstance();
        }

        /// <summary>
        /// 获取类型集合
        /// </summary>
        private Type[] GetTypes<T>()
        {
            return _finder.Find<T>(_assemblies).ToArray();
        }

        /// <summary>
        /// 注册作用域依赖
        /// </summary>
        private void RegisterScopeDependency()
        {
            _builder.RegisterTypes(GetTypes<IScopeDependency>()).AsImplementedInterfaces().PropertiesAutowired().InstancePerLifetimeScope();
        }

        /// <summary>
        /// 注册瞬态依赖
        /// </summary>
        private void RegisterTransientDependency()
        {
            _builder.RegisterTypes(GetTypes<ITransientDependency>()).AsImplementedInterfaces().PropertiesAutowired().InstancePerDependency();
        }

        /// <summary>
        /// 解析依赖注册器
        /// </summary>
        private void ResolveDependencyRegistrar()
        {
            var types = GetTypes<IDependencyRegistrar>();
            types.Select(type => Reflection.CreateInstance<IDependencyRegistrar>(type)).ToList().ForEach(t => t.Register(_services));
        }
    }
}