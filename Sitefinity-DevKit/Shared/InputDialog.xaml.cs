using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Sitefinity_DevKit.Shared
{
    public partial class InputDialog : Window
    {
        private const int ElementHeight = 23;
        private const int LabelWidth = 110;
        private const int InputWidth = 210;
        private const int LabelMarginLeft = 25;
        private const int InputMarginLeft = LabelMarginLeft + 120;
        private const int ElementInitialMarginTop = 50;
        private const int ElementAdditionalMarginTop = 30;
        private const int ElementInitialMarginBottom = 50;

        public InputDialog(Command commandConfig)
        {
            InitializeComponent();
            this.Title = commandConfig.Title;
            int numberOfArgs = 0;
            for (int i = 0; i < commandConfig.Args.Count; i++, numberOfArgs++)
            {
                var argument = commandConfig.Args[i];
                this.CreateInput(i, null);
                this.CreateLabel(i, argument);
            }

            for (int i = 0; i < commandConfig.Options.Count; i++)
            {
                this.CreateInput(i + numberOfArgs, commandConfig.Options[i].DefaultValue);
                this.CreateLabel(i + numberOfArgs, commandConfig.Options[i].Title);
            }
        }

        public List<string> ResponseText { get; set; }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (this.ResponseText == null)
            {
                this.ResponseText = new List<string>();
            }

            foreach (var element in this.Grid.Children)
            {
                if (element.GetType() == typeof(TextBox))
                {
                    this.ResponseText.Add(((TextBox)element).Text);
                }
            }

            this.DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void CreateInput(int index, string defaultValue)
        {
            TextBox newInput = new TextBox();

            newInput.HorizontalAlignment = HorizontalAlignment.Left;
            newInput.Height = ElementHeight;
            newInput.Width = InputWidth;
            newInput.Margin = new Thickness(InputMarginLeft, ElementInitialMarginTop + index * ElementAdditionalMarginTop, 0, ElementInitialMarginBottom);
            newInput.TextWrapping = TextWrapping.Wrap;
            newInput.VerticalAlignment = VerticalAlignment.Top;

            if (defaultValue != null)
            {
                newInput.Text = defaultValue;
            }

            this.Grid.Children.Add(newInput);
        }

        private void CreateLabel(int index, string text)
        {
            TextBlock newLabel = new TextBlock();

            newLabel.Text = text;
            newLabel.HorizontalAlignment = HorizontalAlignment.Left;
            newLabel.Height = ElementHeight;
            newLabel.Width = LabelWidth;
            newLabel.Margin = new Thickness(LabelMarginLeft, ElementInitialMarginTop + index * ElementAdditionalMarginTop, 0, ElementInitialMarginBottom);
            newLabel.VerticalAlignment = VerticalAlignment.Top;

            this.Grid.Children.Add(newLabel);
        }
    }
}
