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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///  



public partial class MainWindow : Window
    {

        Computer computer;
        public MainWindow()
        {
            InitializeComponent();
            computer = new Computer();

            Page_Fault_Rate.DataContext = computer;
            current.DataContext = computer;
            next.DataContext = computer;
            Block0.DataContext = computer;
            Block1.DataContext = computer;
            Block2.DataContext = computer;
            Block3.DataContext = computer;
            AC.DataContext = computer;
            PC.DataContext = computer;
           


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var task = new Task(() =>
            {
                computer.start();
            });
            task.Start();

        }
    }




}
