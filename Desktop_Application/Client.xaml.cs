using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Security.Cryptography;

namespace Desktop_Application
{
    /// <summary>
    /// Interaction logic for Client.xaml
    /// </summary>
    public partial class Client : Window
    {
        private string val = "";
        private static Clients currentClient;
        private string pythonCode = "";
        private string result = "";
        private static Client Instance;
        private static ServiceHost host;
        private static Random rng = new Random();


        private ServerInterface foob;

        public Client()
        {
            InitializeComponent();
            Instance = this;
            val = AvailableClients.Instance.selectedService;

            //code to get a Client object that matches the ip address in the val variable
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(val);
            string base64String = System.Convert.ToBase64String(plainTextBytes);
            string URL = "http://localhost:11662/";
            RestClient restClient = new RestClient(URL);
            RestRequest restRequest = new RestRequest("api/Clients/{id}", Method.Get);
            restRequest.AddUrlSegment("id", base64String);
            RestResponse restResponse = restClient.Execute(restRequest);
            currentClient = JsonConvert.DeserializeObject<Clients>(restResponse.Content);

            //code to set connected to YES
            var plainTextBytes2 = System.Text.Encoding.UTF8.GetBytes(val);
            string base64String2 = System.Convert.ToBase64String(plainTextBytes2);
            string URL2 = "http://localhost:11662/";
            RestClient restClient2 = new RestClient(URL2);
            RestRequest restRequest2 = new RestRequest("api/Update/{ip}/{connected}", Method.Put);
            restRequest2.AddUrlSegment("ip", base64String2);
            restRequest2.AddUrlSegment("connected", "YES");
            RestResponse restResponse2 = restClient.Execute(restRequest2);

            //MessageBox.Show(restResponse.Content);

            //serverthread
            Thread maintherad = Thread.CurrentThread;
            Thread thr1 = new Thread(() => serverThread());
            thr1.Start();


            // Networking Thread
            Thread thr2 = new Thread(() => networkingThread());
            thr2.Start();

            this.Closing += new CancelEventHandler(this.Window_Closing);

        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)//code to set connected to No when client closes and to close the client server
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(val);
            string base64String = System.Convert.ToBase64String(plainTextBytes);
            string URL = "http://localhost:11662/";
            RestClient restClient = new RestClient(URL);
            RestRequest restRequest = new RestRequest("api/Update/{ip}/{connected}", Method.Put);
            restRequest.AddUrlSegment("ip", base64String);
            restRequest.AddUrlSegment("connected", "NO");
            RestResponse restResponse = restClient.Execute(restRequest);
            host.Close();
            //MessageBox.Show(restResponse.Content);
        }


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {

            try
            {


                Task<string> task = new Task<string>(useService);
                task.Start();
                string val = await task;
                MessageBox.Show("Client currently handling a job:  " + currentClient.Currently_Doing_Any_Job + "\nNumber Of Jobs done:  " + currentClient.No_Of_Job_Completed.ToString());




            }
            catch (System.IndexOutOfRangeException mes)
            {
                statusLabel.Content = "Exception caught: " + mes.Message;
            }
        }
        private string useService() //code to get uptodate info about client
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(val);
            string base64String = System.Convert.ToBase64String(plainTextBytes);
            string URL = "http://localhost:11662/";
            RestClient restClient = new RestClient(URL);
            RestRequest restRequest = new RestRequest("api/Clients/{id}", Method.Get);
            restRequest.AddUrlSegment("id", base64String);
            RestResponse restResponse = restClient.Execute(restRequest);
            currentClient = JsonConvert.DeserializeObject<Clients>(restResponse.Content);

            return "";
        }

        private async void Button_Click_3(object sender, RoutedEventArgs e) //code to get the python code input from the text box and display a result when a result is given 
        {
            pythonCode = code.Text;
            resul.Text = "";
            statusLabel.Content = "Job uploaded";
            Task<string> task = new Task<string>(getResult);
            task.Start();
            statusLabel.Content = "Job processing......";
            string val = await task;
            resul.Text = result;
            statusLabel.Content = "Result Loaded";
            result = "";
            pythonCode = "";
        }


        private static void serverThread() // thread hosting the client server 
        {


            string url = "net.tcp://0.0.0.0:" + currentClient.Port_NO.ToString() + "/" + currentClient.Name;
            NetTcpBinding tcp = new NetTcpBinding();
            host = new ServiceHost(typeof(ServerImplementaion));
            host.AddServiceEndpoint(typeof(ServerInterface), tcp, url);
            host.Open();
            for (; ; )
            {

            }



        }

        private string getResult()
        {
            while (result.Equals(""))
            {

            }
            return " ";
        }


        public void networkingThread()
        {
            ChannelFactory<ServerInterface> foobFactory;
            NetTcpBinding tcp = new NetTcpBinding();

            string URL = "http://localhost:11662/";
            RestClient restClient = new RestClient(URL);


            // Infinite Loop

            for (; ; )
            {
                /* Get the Client List */

                RestRequest request = new RestRequest("api/ClientList/GetListOfClients");
                RestResponse response = restClient.Get(request);

                List<Clients> clientList = JsonConvert.DeserializeObject<List<Clients>>(response.Content);

                List<Clients> clientListWithoutcurrent = new List<Clients>();

                // Get the List Of Connected Clients
                clientListWithoutcurrent.Clear();

                foreach (Clients client in clientList)
                {
                    if (!(client.IP_Address.Equals(val)) && !(client.Connected.Equals("NO")))
                    {
                        clientListWithoutcurrent.Add(client);
                    }
                }

                clientListWithoutcurrent = clientListWithoutcurrent.OrderBy(a => rng.Next()).ToList();
                foreach (Clients client in clientListWithoutcurrent)
                {
                    //Set the URL and create the connection!

                    string URL2 = "net.tcp://localhost:" + client.Port_NO.ToString() + "/" + client.Name;


                    foobFactory = new ChannelFactory<ServerInterface>(tcp, URL2);
                    foob = foobFactory.CreateChannel();

                    string base64String = null;
                    string decodedPythonCode = null;

                    /* Download the Job */
                    try
                    {
                        DownloadObject downloadObjectRR = new DownloadObject();
                        downloadObjectRR = foob.Download();
                        base64String = downloadObjectRR.code;
                        SHA256 sha256 = new SHA256Managed();
                        byte[] hash2 = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(base64String));

                        if (!hash2.SequenceEqual(downloadObjectRR.data))
                        {
                            base64String = null;
                        }
                    }
                    catch (Exception ex)
                    {

                    }


                    /* Decode the String */

                    if (!String.IsNullOrEmpty(base64String))
                    {


                        byte[] encodedBytes = Convert.FromBase64String(base64String);
                        decodedPythonCode = Encoding.UTF8.GetString(encodedBytes);

                    }


                    /* Execute the Python Code */

                    string strResult = null;

                    if (!String.IsNullOrEmpty(decodedPythonCode))
                    {
                        var plainTextBytes2 = System.Text.Encoding.UTF8.GetBytes(val);
                        string base64String2 = System.Convert.ToBase64String(plainTextBytes2);

                        string URL3 = "http://localhost:11662/";
                        RestClient restClient2 = new RestClient(URL3);
                        RestRequest restRequest2 = new RestRequest("api/UpdateCurrentlyDoing/{ip}/{busy}", Method.Put);
                        restRequest2.AddUrlSegment("ip", base64String2);
                        restRequest2.AddUrlSegment("busy", "YES");
                        RestResponse restResponse2 = restClient2.Execute(restRequest2);

                        int var1, var2;

                        var1 = 100;
                        var2 = 20;

                        /* Exception handling for invalid code and program failures */

                        try
                        {
                            ScriptEngine engine = Python.CreateEngine();
                            ScriptScope scope = engine.CreateScope();

                            engine.Execute(decodedPythonCode, scope);

                            // Using C# Dynamic

                            dynamic testFunction = scope.GetVariable("test_func");
                            var result = testFunction(var1, var2);

                            // Convert the actual result to string value

                            strResult = result.ToString();
                        }
                        catch (Exception ex)
                        {
                            // Do nothing
                        }

                    }


                    /* Post the Answer back to the Client that hosted the job */

                    if (!String.IsNullOrEmpty(strResult))
                    {
                        foob.Upload(strResult);
                    }


                    /* Post to the WebServer After finishing the Job */

                    if (!String.IsNullOrEmpty(strResult))
                    {
                        // Increase the count of completed Job
                        int jobCount = currentClient.No_Of_Job_Completed + 1;

                        // Update the result of last completed Job
                        string lastJobResult = strResult;
                        string lastJobName = client.Name;

                        var plainTextBytes2 = System.Text.Encoding.UTF8.GetBytes(val);
                        string base64String2 = System.Convert.ToBase64String(plainTextBytes2);
                        string URL4 = "http://localhost:11662/";
                        RestClient restClient2 = new RestClient(URL4);

                        RestRequest restRequest = new RestRequest("api/UpdateDetails/{ip}/{jobName}/{jobCount}/{jobResult}", Method.Put);
                        restRequest.AddUrlSegment("ip", base64String2);
                        restRequest.AddUrlSegment("jobName", lastJobName);
                        restRequest.AddUrlSegment("jobCount", jobCount);
                        restRequest.AddUrlSegment("jobResult", lastJobResult);

                        RestResponse restResponse = restClient2.Execute(restRequest);






                        RestRequest restRequest2 = new RestRequest("api/UpdateCurrentlyDoing/{ip}/{busy}", Method.Put);
                        restRequest2.AddUrlSegment("ip", base64String2);
                        restRequest2.AddUrlSegment("busy", "NO");
                        RestResponse restResponse2 = restClient2.Execute(restRequest2);

                    }

                }

            }

        }
        public class ServerImplementaion : ServerInterface
        {

            public DownloadObject Download()
            {
                DownloadObject downloadObject = new DownloadObject();
                string val = Instance.pythonCode;
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(val);
                string base64String = System.Convert.ToBase64String(plainTextBytes);
                SHA256 sha256 = new SHA256Managed();
                byte[] hash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(base64String));
                downloadObject.code = base64String;
                downloadObject.data = hash;

                return downloadObject;
            }

            public void Upload(string result)
            {

                Instance.result = result;
            }
        }

        [ServiceContract]
        public interface ServerInterface
        {
            [OperationContract]
            DownloadObject Download();

            [OperationContract]
            void Upload(string result);
        }
    }
}
