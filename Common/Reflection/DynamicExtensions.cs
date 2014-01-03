using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace NicCage.Reflection
{
	/// <summary>
	/// Set of extension methods for simple reflection
	/// </summary>
	public static class DynamicExtensions
	{
		private static readonly Dictionary<Type, Dictionary<string, PropertyInfo>> _properties = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
		private static readonly Dictionary<Type, Dictionary<string, MethodInfo>> _methods = new Dictionary<Type, Dictionary<string, MethodInfo>>();

		private static PropertyInfo GetPropertyInfo(this object target, string propertyName)
		{
			var type = target.GetType();
			Dictionary<string, PropertyInfo> properties;
			if (!_properties.TryGetValue(type, out properties))
			{
				properties =
					_properties [type] = new Dictionary<string, PropertyInfo>();
			}

			PropertyInfo property;
			if (!properties.TryGetValue(propertyName, out property))
			{
				property = 
					properties [propertyName] = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
			}

			return property;
		}

		/// <summary>
		/// Invokes a method via its name
		/// </summary>
		/// <param name="target">object instance</param>
		/// <param name="methodName">Method name</param>
		public static void Invoke(this object target, string methodName)
		{
			var type = target.GetType();
			Dictionary<string, MethodInfo> methods;
			if (!_methods.TryGetValue(type, out methods))
			{
				methods = 
					_methods [type] = new Dictionary<string, MethodInfo>();
			}

			MethodInfo method;
			if (!methods.TryGetValue(methodName, out method))
			{
				method =
					methods [methodName] = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
			}

			if (method != null)
				method.Invoke(target, null);
		}

		/// <summary>
		/// Sets a property via its name
		/// </summary>
		/// <param name="target">object instance</param>
		/// <param name="propertyName">Property name</param>
		/// <param name="value">Value</param>
		public static void SetProperty(this object target, string propertyName, object value)
		{
			var property = target.GetPropertyInfo(propertyName);
			if (property != null)
				property.SetValue(target, value);
		}

		/// <summary>
		/// Gets a property value via its name
		/// </summary>
		/// <returns>The value</returns>
		/// <param name="target">object instance</param>
		/// <param name="propertyName">Property name</param>
		public static object GetProperty(this object target, string propertyName)
		{
			return target.GetPropertyInfo(propertyName).GetValue(target);
		}

		/// <summary>
		/// A crazy way to setup weak events, pulled this from here:
		///  http://stackoverflow.com/questions/1747235/weak-event-handler-model-for-use-with-lambdas (The Answer)
		/// </summary>
		public static void SetWeakHandler<TSubscriber, TDelegate, TArgs>(
			this TSubscriber subscriber,
			Func<EventHandler<TArgs>, TDelegate> converter,
			Action<TDelegate> add, Action<TDelegate> remove,
			Action<TSubscriber, TArgs> action)
            where TArgs : EventArgs
            where TDelegate : class
            where TSubscriber : class
		{
			var subscriberRef = new WeakReference(subscriber);
			TDelegate handler = null;
			handler = converter(new EventHandler<TArgs>(
				(s, e) =>
				{
					var subs_strong_ref = subscriberRef.Target as TSubscriber;
					if (subs_strong_ref != null)
					{
						action(subs_strong_ref, e);
					}
					else
					{
						remove(handler);
						handler = null;
					}
				}));
			add(handler);
		}

		/// <summary>
		/// A crazy way to setup weak events, pulled this from here:
		///  http://stackoverflow.com/questions/1747235/weak-event-handler-model-for-use-with-lambdas ('The' Answer)
		/// 
		/// Simplified version for INotifyPropertyChanged
		/// </summary>
		public static void SetWeakHandler(
			this INotifyPropertyChanged subscriber,
			Action<PropertyChangedEventHandler> add,
			Action<PropertyChangedEventHandler> remove,
			Action<object, PropertyChangedEventArgs> action)
		{
			SetWeakHandler<INotifyPropertyChanged, PropertyChangedEventHandler, PropertyChangedEventArgs>(
				subscriber, h => (o, e) => h(o, e), add, remove, action);
		}

		/// <summary>
		/// A crazy way to setup weak events, pulled this from here:
		///  http://stackoverflow.com/questions/1747235/weak-event-handler-model-for-use-with-lambdas ('The' Answer)
		/// 
		/// Simplified version for EventHandler
		/// </summary>
		public static void SetWeakHandler<TSubscriber, TArgs>(
			this TSubscriber subscriber, 
			Action<EventHandler<TArgs>> add, 
			Action<EventHandler<TArgs>> remove, 
			Action<TSubscriber, TArgs> action)
            where TArgs : EventArgs
            where TSubscriber : class
		{
			SetWeakHandler<TSubscriber, EventHandler<TArgs>, TArgs>(subscriber, h => h, add, remove, action);
		}
	}
}

