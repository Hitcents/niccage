using System;

namespace NicCage.Tests
{
	public class TestViewModel : PropertyChangedBase
    {
		private string _text;

		public string Text
		{
			get { return _text; }
			set
			{
				if (_text != value)
				{
					_text = value;
					OnPropertyChanged("Text"); 
				}
			}
		}

		private string _textField;

		public string TextField
		{
			get { return _textField; }
			set
			{
				if (_textField != value)
				{
					_textField = value;
					OnPropertyChanged("TextField"); 
				}
			}
		}

		private bool _canSearch = true;

		public bool CanSearch
		{
			get { return _canSearch; }
			set
			{
				if (_canSearch != value)
				{
					_canSearch = value;
					OnPropertyChanged("CanSearch"); 
				}
			}
		}

		public void Search()
		{
			Searched = true;
		}

		public bool Searched;
    }
}

