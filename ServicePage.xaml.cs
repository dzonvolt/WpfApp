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

namespace WpfApp
{
    /// <summary>
    /// Логика взаимодействия для ServicePage.xaml
    /// </summary>
    public partial class ServicePage : Page
    {
        public ServicePage()
        {
            InitializeComponent();
            var services = chirkov_autoserviceEntities.GetContext().Service.ToList();
            ServiceListView.ItemsSource = services;
            ComboType.SelectedIndex = 0;
            UpdateServices();
        }

        public void UpdateServices() {
            var services = chirkov_autoserviceEntities.GetContext().Service.ToList();

            if(ComboType.SelectedIndex == 1) services = services.Where(p => (p.FixedDiscount >= 0 && p.FixedDiscount <= 5)).ToList();
            else if (ComboType.SelectedIndex == 2) services = services.Where(p => (p.FixedDiscount >= 5 && p.FixedDiscount <= 15)).ToList();
            else if (ComboType.SelectedIndex == 3) services = services.Where(p => (p.FixedDiscount >= 15 && p.FixedDiscount <= 30)).ToList();
            else if (ComboType.SelectedIndex == 4) services = services.Where(p => (p.FixedDiscount >= 30 && p.FixedDiscount <= 70)).ToList();
            else if (ComboType.SelectedIndex == 5) services = services.Where(p => (p.FixedDiscount >= 70 && p.FixedDiscount <= 100)).ToList();

            services = services.Where(p => p.Title.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();

            ServiceListView.ItemsSource = services;

            if (RButtonDown.IsChecked.Value)ServiceListView.ItemsSource = services.OrderByDescending(p => p.Cost).ToList();
            if (RButtonUp.IsChecked.Value) ServiceListView.ItemsSource = services.OrderBy(p => p.Cost).ToList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new EditPage());
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateServices();
        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateServices();
        }

        private void RButtonUp_Checked(object sender, RoutedEventArgs e)
        {
            UpdateServices();
        }

        private void RButtonDown_Checked(object sender, RoutedEventArgs e)
        {
            UpdateServices();
        }
    }
}
