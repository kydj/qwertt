using System;
using System.Collections.Generic;
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
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace WpfApp555
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private SqlConnection SqlConnection = null;
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            SqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["Database1"].ConnectionString);
            SqlConnection.Open();
        }
        public Window1()
        {
            InitializeComponent();
            this.TextBox1.PreviewTextInput += new TextCompositionEventHandler(TextBox1_PreviewTextInput);
            this.TextBox2.PreviewTextInput += new TextCompositionEventHandler(TextBox2_PreviewTextInput);
            this.TextBox3.PreviewTextInput += new TextCompositionEventHandler(TextBox3_PreviewTextInput);
        }

        void TextBox1_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsLetter(e.Text, 0)) e.Handled = true;
        }

        void TextBox2_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0)) e.Handled = true;
        }

        void TextBox3_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0)) e.Handled = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["Database1"].ConnectionString);
            SqlConnection.Open();
            SqlDataAdapter dataAdapter = new SqlDataAdapter("SELECT * FROM Table1", SqlConnection);
            SqlCommand select = new SqlCommand("SELECT * FROM Table1", SqlConnection);
            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet);
            datagrid.SetBinding(ItemsControl.ItemsSourceProperty, new Binding { Source = dataSet.Tables[0] });
            select.ExecuteReader();
        }




        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SqlCommand command = new SqlCommand($"INSERT INTO [Table2] (Вакансия, Зарплата, Телефон) VALUES (N'{TextBox1.Text}', N'{TextBox2.Text}', N'{Region.Text+TextBox3.Text}')", SqlConnection);

            if (TextBox1.Text == "Сапер")
            {
                SaperGame taskWindow = new SaperGame();
                taskWindow.Show();
            }    

                else if (string.IsNullOrWhiteSpace(TextBox1.Text) || string.IsNullOrWhiteSpace(TextBox2.Text) || string.IsNullOrWhiteSpace(TextBox3.Text))
            {
                MessageBox.Show("Некоторые поля пусты", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            else if (int.Parse(TextBox2.Text) <= 12000)
            {
                MessageBox.Show("Заработная плата введена меньше МРОТа", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            else
            {
                MessageBox.Show("Занесено в базу", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                command.ExecuteNonQuery();
            }
        }

    }
}
