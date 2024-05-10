using Newtonsoft.Json;
using RestSharp;
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
using System.Windows.Shapes;


namespace Desktop_Application
{
    /// <summary>
    /// Interaction logic for AvailableClients.xaml
    /// </summary>
    public partial class AvailableClients : Window
    {
        private List<string> dropList = new List<string>();
        private List<Clients> clientList = new List<Clients>();
        public static AvailableClients Instance;
        public string selectedService;
        public AvailableClients()
        {
            InitializeComponent();
            Instance = this;
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            string[] split = { "" };
            string temp = listBox.SelectedItem.ToString();
            for (var i = 0; i < dropList.Count; i++)
            {

                split = temp.Split(':');
                
                if (clientList[i].IP_Address.Equals(split[1]))
                {
                    selectedService = clientList[i].IP_Address;
                }
    


            }
            if (listBox.SelectedIndex == 0)
            {
                Client clientWindow = new Client();
                clientWindow.Show();
            }
            else if (listBox.SelectedIndex == 1)
            {
                Client2 clientWindow = new Client2();
                clientWindow.Show();
            }
            else if (listBox.SelectedIndex == 2)
            {
                Client3 clientWindow = new Client3();
                clientWindow.Show();
            }
            else if (listBox.SelectedIndex == 3)
            {
                Client4 clientWindow = new Client4();
                clientWindow.Show();
            }

        }

        private string useService() 
        {
            string URL = "http://localhost:11662/";
            RestClient restClient = new RestClient(URL);
            RestRequest restRequest = new RestRequest("api/Clients", Method.Get);
            RestResponse restResponse = restClient.Execute(restRequest);
            clientList = JsonConvert.DeserializeObject<List<Clients>>(restResponse.Content);
            foreach (Clients client in clientList)
            {
                dropList.Add(client.Name +":"+ client.IP_Address);
            }
            return "";
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {

            try
            {
     
               
                Task<string> task = new Task<string>(useService);
                task.Start();
                status.Content = "Loading services please wait......";
                string val = await task;
                status.Content = "Done";
                listBox.ItemsSource = dropList;



            }
            catch (System.IndexOutOfRangeException mes)
            {
                status.Content = "Exception caught: " + mes.Message;
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            this.Close();
            mainWindow.Show();
        }
    }
}
