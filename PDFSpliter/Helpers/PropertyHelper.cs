using System.Windows;
using System.Windows.Input;

namespace PDFSplitter.Helpers
{
    public static class PropertyHelper
    {
        public static readonly DependencyProperty DropCommandProperty = DependencyProperty.RegisterAttached(
            "DropCommand",
            typeof(ICommand),
            typeof(PropertyHelper),
            new PropertyMetadata(null, OnDropCommandChange));

        public static void SetDropCommand(DependencyObject source, ICommand value)
        {
            source.SetValue(DropCommandProperty, value);
        }

        public static ICommand GetDropCommand(DependencyObject source)
        {
            return (ICommand) source.GetValue(DropCommandProperty);
        }

        private static void OnDropCommandChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ICommand command = e.NewValue as ICommand;
            UIElement uiElement = d as UIElement;
            if (command != null && uiElement != null)
            {
                uiElement.Drop += (sender, args) => command.Execute(args.Data);
            }
        }
    }
}
