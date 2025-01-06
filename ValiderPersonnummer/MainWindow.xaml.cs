using System.Text;
using System.Text.Unicode;
using System.Windows;
using System.Windows.Media;

namespace ValiderPersonnummer;

public partial class MainWindow : Window
{
  public MainWindow()
  {
    InitializeComponent();
  }

  private void InputÆndret(object sender, RoutedEventArgs e)
  {
    SvarLabel.Content = "";
  }

  private void Valider_Click(object sender, RoutedEventArgs e)
  {
    if(Validator.Validate(Input.Text))
    {
      SvarLabel.Content = "GYLDIGT";
      SvarLabel.Foreground = new SolidColorBrush(Colors.LawnGreen);
      return;
    }

    SvarLabel.Content = "IKKE GYLDIGT";

    SvarLabel.Foreground = new SolidColorBrush(Colors.Red);
  }
}