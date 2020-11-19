using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SplineInterpolation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<TextBox> textBoxesX = new List<TextBox>();
        private List<TextBox> textBoxesY = new List<TextBox>();
        private List<TextBlock> answers = new List<TextBlock>();

        public MainWindow()
        {
            InitializeComponent();
            ChangeTable(3);
            counter.ValueChanged += OnCounterChange;
        }

        private void ChangeTable(int columns)
        {
            foreach (var textBox in textBoxesX.Concat(textBoxesY))
            {
                table.Children.Remove(textBox);
            }
            textBoxesX.Clear();
            textBoxesY.Clear();

            foreach (var textBlock in answers)
            {
                answerPanel.Children.Remove(textBlock);
            }
            answers.Clear();

            
            for (int i = 1; i < columns + 1; i++)
            {
                var textBoxX = TextBoxes.CreateTextBox();
                textBoxesX.Add(textBoxX);
                table.Children.Add(textBoxX);
                Grid.SetColumn(textBoxX, i);
                
                var textBoxY = TextBoxes.CreateTextBox();
                textBoxesY.Add(textBoxY);
                table.Children.Add(textBoxY);
                Grid.SetColumn(textBoxY, i);
                Grid.SetRow(textBoxY, 1);
            }

            for (int i = 0; i < columns - 1; i++)
            {
                var textBlock = new TextBlock()
                {
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 16,
                    Margin = new Thickness(5)
                };
                answers.Add(textBlock);
                answerPanel.Children.Add(textBlock);
            }
        }

        private void OnCounterChange(object sender, RoutedEventArgs args)
        {
            ChangeTable(counter.Value ?? 3);
        }

        private void OnClickEvaluateButton(object sender, RoutedEventArgs args)
        {
            Evaluate();
        }

        private void Evaluate()
        {
            try
            {
                var x = textBoxesX
                    .Select(box => double.Parse(box.Text.Replace(',', '.'), CultureInfo.InvariantCulture))
                    .ToArray();
                var y = textBoxesY
                    .Select(box => double.Parse(box.Text.Replace(',', '.'), CultureInfo.InvariantCulture))
                    .ToArray();

                var result = Interpolation.Evaluate(x, y);
                for (int i = 0; i < result.Length; i++)
                {
                    answers[i].Text = result[i].ToString();
                }

                CheckSplines(result, x, y);
            }
            catch (FormatException)
            {
                MessageBox.Show("Неккоретные вводные данные! Должны быть только действительные числа",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CheckSplines(Polynomial[] splines, double[] x, double[] y)
        {
            var builder = new StringBuilder();
            
            for (int i = 0; i < splines.Length; i++)
            {
                builder.Append($"Погрешность {i + 1}-го сплайна для x = {x[i]}:\t {y[i] - splines[i].Evaluate(x[i]):F}\n");
                builder.Append($"Погрешность {i + 1}-го сплайна для x = {x[i + 1]}:\t {y[i + 1] - splines[i].Evaluate(x[i + 1]):F}\n");
                builder.Append("\n");
            }

            MessageBox.Show(builder.ToString(), "Погрешности", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}