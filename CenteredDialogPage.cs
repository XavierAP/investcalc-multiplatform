using Xamarin.Forms;

namespace JP.InvestCalc
{
	public class CenteredDialogPage : ContentPage
	{
		readonly Grid table;
		readonly RowDefinition layoutEntryRows = new RowDefinition { Height = GridLength.Auto };

		public CenteredDialogPage()
		{
			var layoutCol = new ColumnDefinition { Width = GridLength.Star };
			table = new Grid
			{
				HorizontalOptions = LayoutOptions.Center,
				ColumnDefinitions = new ColumnDefinitionCollection { layoutCol, layoutCol },
			};
			this.Content = new ScrollView
			{
				Content = table,
				HorizontalOptions = LayoutOptions.Center,
			};
		}

		public Label
		AddTextRow(string text)
		{
			Label ans;
			var currentRow = DefineNewRowAndReturnIndex(new RowDefinition { Height = GridLength.Auto });
			table.Children.Add(
				ans = new Label { Text = text },
				0, 2, currentRow, currentRow+1);
			return ans;
		}

		public (Entry Entry, Label Label)
		AddEntryRow(string labelText)
		{
			(Entry Entry, Label Label) ans;
			var currentRow = DefineNewRowAndReturnIndex(layoutEntryRows);
			table.Children.Add(
				ans.Label = new Label { Text = labelText },
				0, currentRow);
			table.Children.Add(
				ans.Entry = new Entry(),
				1, currentRow);
			return ans;
		}

		public (Button Left, Button Right)
		AddButtonsRow(string leftText, string rightText)
		{
			(Button Left, Button Right) ans;

		}

		private int
		DefineNewRowAndReturnIndex(RowDefinition format)
		{
			var currentRow = table.RowDefinitions.Count;
			table.RowDefinitions.Add(format);
			return currentRow;
		}
	}
}
