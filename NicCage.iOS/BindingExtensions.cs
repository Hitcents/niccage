using System;
using System.ComponentModel;
using System.Reflection;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using NicCage.Reflection;
using Praeclarum.Bind;

namespace NicCage
{
	public static class BindingExtensions
	{
		public static void Bind(this object bindable, INotifyPropertyChanged viewModel)
		{
			var properties = bindable.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic);
			foreach (var property in properties)
			{
				var outlet = property.GetCustomAttribute<OutletAttribute>();
				if (outlet != null)
				{
					var view = property.GetValue(bindable) as UIView;

					//TODO: These are our conventions, which need to be configurable

					//Label
					var label = view as UILabel;
					if (label != null)
					{
						Binding.Create(() => label.Text == viewModel.GetProperty(property.Name));
						continue;
					}

					//Button
					var button = view as UIButton;
					if (button != null)
					{
						Binding.Create(() => button.Enabled == (bool)viewModel.GetProperty("Can" + property.Name));

						button.TouchUpInside += (sender, e) => viewModel.Invoke(property.Name);

						continue;
					}

					//TextField
					var textField = view as UITextField;
					if (textField != null)
					{
						Binding.Create(() => textField.Text == viewModel.GetProperty(property.Name));

						continue;
					}

					//No conventions hit
				}
			}
		}
	}
}

