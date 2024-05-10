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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static IronPython.Modules._ast;

namespace Desktop_Application
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private string name;
        private string ip_address;
        private string portNumber;
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                name = ClientName.Text;
                ip_address = IP_Address.Text;
                portNumber = PortNumber.Text;

                Task<string> task = new Task<string>(useRegisterService);
                task.Start();
                statusLabel.Content = "Registering new client please wait......";
                string val = await task;
                if (val.Equals("0") )
                {
                    statusLabel.Content = "Error client already exists";
                }
                else
                {
                    statusLabel.Content = "Client Registered";
                }
            }
            catch (System.FormatException message)
            {
                statusLabel.Content = "Exception caught: " + message.Message + " Please fill all input boxes";
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            this.Close();
            mainWindow.Show();
        }


        private string useRegisterService()//Connect to the Authenticator server an access the register service.
        {
            string URL = "http://localhost:11662/";
            RestClient restClient = new RestClient(URL);
            RestRequest restRequest = new RestRequest("api/Register/RegisterClient/{ipAddress}/{portNo}/{name}", Method.Get);
            restRequest.AddUrlSegment("ipAddress", ip_address);
            restRequest.AddUrlSegment("portNo", portNumber);
            restRequest.AddUrlSegment("name", name);
            RestResponse response = restClient.Execute(restRequest);
            string res = response.Content.Trim(new char[] { '"', '”' });
            return res;
        }


    }
}
