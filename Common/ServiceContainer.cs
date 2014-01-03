using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NicCage
{
	/// <summary>
	/// A simple service container implementation, singleton only
	/// </summary>
	public static class ServiceContainer
	{
		private static readonly Dictionary<Type, Lazy<object>> _services = new Dictionary<Type, Lazy<object>>();

		/// <summary>
		/// Register the specified service with an instance
		/// </summary>
		public static void Register<T>(T service)
		{
			_services [typeof(T)] = new Lazy<object>(() => service);
		}

		/// <summary>
		/// Register the specified service for a class with a default constructor
		/// </summary>
		public static void Register<T>() where T : new()
		{
			_services [typeof(T)] = new Lazy<object>(() => new T());
		}

		/// <summary>
		/// Register the specified service with a callback to be invoked when requested
		/// </summary>
		public static void Register<T>(Func<T> function)
		{
			_services [typeof(T)] = new Lazy<object>(() => function());
		}

		/// <summary>
		/// Register the specified service with an instance
		/// </summary>
		public static void Register(Type type, object service)
		{
			_services [type] = new Lazy<object>(() => service);
		}

		/// <summary>
		/// Register the specified service with a callback to be invoked when requested
		/// </summary>
		public static void Register(Type type, Func<object> function)
		{
			_services [type] = new Lazy<object>(function);
		}

		/// <summary>
		/// Resolves the type, throwing an exception if not found
		/// </summary>
		public static T Resolve<T>()
		{
			return (T)Resolve(typeof(T));
		}

		/// <summary>
		/// Resolves the type, throwing an exception if not found
		/// </summary>
		public static object Resolve(Type type)
		{
			Lazy<object> service;
			if (_services.TryGetValue(type, out service))
			{
				return service.Value;
			}
			else
			{
				throw new KeyNotFoundException(string.Format("Service not found for type '{0}'", type));
			}
		}

		/// <summary>
		/// Mainly for testing, clears the entire container
		/// </summary>
		public static void Clear()
		{
			_services.Clear();
		}
	}
}