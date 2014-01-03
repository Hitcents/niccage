using System;
using System.ComponentModel;
using System.Reflection;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using NicCage.Reflection;

namespace NicCage
{
	public static class BindingExtensions
	{
		public static PropertyObserver Bind(this object bindable, INotifyPropertyChanged viewModel)
		{
			var observer = new PropertyObserver(viewModel);
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
						observer.Add<string>(property.Name, text => label.Text = text ?? string.Empty);
						continue;
					}

					//Button
					var button = view as UIButton;
					if (button != null)
					{
						observer.Add<bool>("Can" + property.Name, value => button.Enabled = value);

						button.TouchUpInside += (sender, e) => observer.InvokeMethod(property.Name);

						continue;
					}

					//TextField
					var textField = view as UITextField;
					if (textField != null)
					{
						observer.Add<string>(property.Name, text => textField.Text = text ?? string.Empty);

						NSNotificationCenter.DefaultCenter.AddObserver(UITextField.TextFieldTextDidChangeNotification, n =>
						{
							viewModel.SetProperty(property.Name, textField.Text);

						}, textField);

						continue;
					}

					//No conventions hit
				}
			}

			return observer;
		}
	}
}

