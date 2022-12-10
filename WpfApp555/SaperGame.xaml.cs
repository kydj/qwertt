using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections;

namespace WpfApp555
{
    /// <summary>
    /// Логика взаимодействия для SaperGame.xaml
    /// </summary>
    public partial class Difficult : Window
    {
        public const int x = 12;
        public const int y = 12;
        public const int CountBomb = 10;
    }
    public partial class SaperGame : Window
    {
        public SaperGame()
        {
            InitializeComponent();
            InitGame();
        }

        Difficult difficult = new Difficult();
        const int x = Difficult.x;
        const int y = Difficult.y;
        const int CountBomb = Difficult.CountBomb;

        Uri uriBomb = new Uri("pack://application:,,,/bomb.png");
        Uri uriFlag = new Uri("pack://application:,,,/flag.png");
        static int ActualStep = 0;
        BitArray GameArray = new BitArray(x * y);
        Button[] ButtonList = new Button[x * y];
        Random rnd = new Random();

        void InitGame(bool breaks = false, int count = 0)
        {
            for (int i = 0; i < GameArray.Length; i++)
            {
                if (count == CountBomb) break;
                if (!GameArray[i]) GameArray[i] = rnd.Next(0, 1000) < 50 && rnd.Next(0, 1000) > 800 ? true : false;
                else continue;
                if (GameArray[i]) count++;
            }

            if (count < CountBomb) InitGame(true, count);
            if (breaks) return;

            this.Width = (playField.Width = 20 * x) + 20;
            this.Height = (playField.Height = 20 * y) + 40;
            CreateButton(x * y);
        }

        void CreateButton(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Button b = new Button()
                {
                    Width = 20,
                    Height = 20,
                    Margin = new Thickness(0),
                    Tag = i
                };
                //if (GameArray[i]) b.Content = i;
                b.Click += new RoutedEventHandler(b_Click);
                b.MouseRightButtonUp += new MouseButtonEventHandler(b_MouseRightButtonUp);
                playField.Children.Add(b);
                ButtonList[i] = b;
            }
        }
        void b_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            Button button = (Button)sender;
            int index = (int)button.Tag;
            if (button.Content == null) SetButton(button, TypeButton.Flag);
            else button.Content = null;
        }
        void b_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int index = (int)button.Tag;
            if (GameArray[index])
            {
                for (int i = 0; i < GameArray.Length; i++)
                    if (GameArray[i]) SetButton(ButtonList[i], TypeButton.Bomb);
                MessageBox.Show("YOU LOSE! \nPress OK to restart", "Result", MessageBoxButton.OK);

                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();

            }
            else
            {
                Step(index);
                this.Title = ActualStep.ToString();
                if (ActualStep == x * y - CountBomb)
                {
                    MessageBox.Show("YOU WIN! \nPress OK to restart", "Result", MessageBoxButton.OK);

                    System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                    Application.Current.Shutdown();
                }
            }

        }
        void Step(int index)
        {
            int[] t = new int[8];
            int row = index / x;
            int column = row * x - index;

            t[0] = x * (row - 1) + (index % x);
            t[1] = x * (row + 1) + (index % x);
            t[2] = t[0] + 1;
            t[3] = t[0] - 1;
            t[4] = t[1] + 1;
            t[5] = t[1] - 1;
            t[6] = index + 1;
            t[7] = index - 1;

            int count = 0;
            for (int i = 0; i < t.Length; i++)
                if (ContainsIndex(t[i], index, row, column))
                    if (GameArray[t[i]])
                        count++;

            if (count == 0)//если рядом с выбранной клеткой нет мин
            {
                ButtonList[index].IsEnabled = false;
                ButtonList[index].Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                ActualStep += 1;
                for (int i = 0; i < t.Length; i++)
                    if (ContainsIndex(t[i], index, row, column))
                        if (ButtonList[t[i]].IsEnabled)
                            Step(t[i]);
            }
            else
            {
                SetButton(ButtonList[index], TypeButton.Number, count);
                ButtonList[index].Background = new SolidColorBrush(Color.FromRgb(210, 210, 210));
                ButtonList[index].Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 255));
                ActualStep += 1;
            }
        }

        bool ContainsIndex(int index, int baseInd, int baseRow, int baseCol)
        {
            if (index >= 0 && index < GameArray.Length)
            {
                int row = index / x;
                int column = row * x - index;
                if (Math.Abs(baseRow - row) > 1) return false;
                if (Math.Abs(baseCol - column) > 1) return false;
                return true;
            }
            else return false;
        }

        void SetButton(Button button, TypeButton type, int number = 0)
        {
            BitmapImage bitmap = new BitmapImage();

            if (type == TypeButton.None)
            {
                button.IsEnabled = false;
                return;
            }

            if (type == TypeButton.Bomb)
            {
                bitmap = new BitmapImage(uriBomb);
                button.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                button.IsEnabled = false;
            }

            if (type == TypeButton.Flag)
                bitmap = new BitmapImage(uriFlag);

            if (type != TypeButton.Number)
            {
                Image im = new Image() { Source = bitmap };
                Grid grid = new Grid();
                grid.Children.Add(im);
                button.Content = grid;
            }

            if (type == TypeButton.Number)
            {
                button.Content = number;
                button.IsEnabled = false;
            }

        }
    }

    public enum TypeButton
    {
        Bomb,
        Flag,
        Number,
        None
    }
}

