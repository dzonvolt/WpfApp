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

        int CountRecords;
        int CountPage;
        int CurrentPage = 0;

        List<Service> CurrentPageList = new List<Service>();
        List<Service> TableList;

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

            if(ComboType.SelectedIndex == 1) services = services.Where(p => (p.FixedDiscount >= 0 && p.FixedDiscount < 5)).ToList();
            else if (ComboType.SelectedIndex == 2) services = services.Where(p => (p.FixedDiscount >= 5 && p.FixedDiscount < 15)).ToList();
            else if (ComboType.SelectedIndex == 3) services = services.Where(p => (p.FixedDiscount >= 15 && p.FixedDiscount < 30)).ToList();
            else if (ComboType.SelectedIndex == 4) services = services.Where(p => (p.FixedDiscount >= 30 && p.FixedDiscount < 70)).ToList();
            else if (ComboType.SelectedIndex == 5) services = services.Where(p => (p.FixedDiscount >= 70 && p.FixedDiscount < 100)).ToList();

            services = services.Where(p => p.Title.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();

            ServiceListView.ItemsSource = services;

            if (RButtonDown.IsChecked.Value)ServiceListView.ItemsSource = services.OrderByDescending(p => p.Cost).ToList();
            if (RButtonUp.IsChecked.Value) ServiceListView.ItemsSource = services.OrderBy(p => p.Cost).ToList();

            ServiceListView.ItemsSource = services;
            TableList = services;
            ChangePage(0, 0);
        }


        private void ChangePage(int direction, int? selectedPage)
        {
            CurrentPageList.Clear();
            CountRecords = TableList.Count;
            if (CountRecords % 10 > 0)
            {
                CountPage = CountRecords / 10 + 1;
            }
            else
            {
                CountPage = CountRecords / 10;
            }
            Boolean Ifupdate = true;

            int min;

            if (selectedPage.HasValue)
            {
                if (selectedPage >= 0 && selectedPage <= CountPage)
                {
                    CurrentPage = (int)selectedPage;
                    min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                    for (int i = CurrentPage * 10; i < min; i++)
                    {
                        CurrentPageList.Add(TableList[i]);
                    }
                }
            }
            else
            {
                switch (direction)
                {
                    case 1:
                        if (CurrentPage > 0)
                        {
                            CurrentPage--;
                            min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                            for (int i = CurrentPage * 10; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
                            }
                        }
                        else
                        {
                            Ifupdate = false;
                        }
                        break;

                    case 2:
                        if (CurrentPage < CountPage - 1)
                        {
                            CurrentPage++;
                            min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                            for (int i = CurrentPage * 10; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
                            }
                        }
                        else
                        {
                            Ifupdate = false;
                        }
                        break;

                }

            }
            if (Ifupdate)
            {
                PageListBox.Items.Clear();

                for (int i = 1; i <= CountPage; i++)
                {
                    PageListBox.Items.Add(i);
                }
                PageListBox.SelectedIndex = CurrentPage;
                ServiceListView.Items.Refresh();
                min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                TBCount.Text = min.ToString();
                TBAllRecords.Text = " из " + CountRecords.ToString();
                ServiceListView.ItemsSource = CurrentPageList;
            }
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

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var currentServices = (sender as Button).DataContext as Service;

            var currentClientServices = chirkov_autoserviceEntities.GetContext().ClientService.ToList();
            currentClientServices = currentClientServices.Where(p => p.ServiceID == currentServices.ID).ToList();

            if (currentClientServices.Count != 0)
                MessageBox.Show("Невозможно удалить, так как уже существуют записи на эту услугу");
            else
            {
                if (MessageBox.Show("Вы точно хоите выполнить удалить?", "Внимание!",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        chirkov_autoserviceEntities.GetContext().Service.Remove(currentServices);
                        chirkov_autoserviceEntities.GetContext().SaveChanges();
                        ServiceListView.ItemsSource = chirkov_autoserviceEntities.GetContext().Service.ToList();
                        UpdateServices();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }

        }

        private void LeftDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(1, null);
        }

        private void RightDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(2, null);

        }

        private void PageListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ChangePage(0, Convert.ToInt32(PageListBox.SelectedItem.ToString()) - 1);
        }
    }
}
