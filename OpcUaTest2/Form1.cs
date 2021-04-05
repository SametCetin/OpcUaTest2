using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Client.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpcUaTest2
{
    public partial class Form1 : Form
    {
        private Session UASession;
        private bool UAConnectedOnce;
        private Subscription UASubscription;
        private ConnectServerCtrl UAConnServerCtrl;
        private ApplicationConfiguration UAConfiguration;

        public Form1()
        {
            InitializeComponent();
        }

        public Form1(ApplicationConfiguration configuration)
        {
            InitializeComponent();
            InitUAConnection(configuration);
        }

        private void InitUAConnection(ApplicationConfiguration configuration)
        {
            UAConnServerCtrl = new ConnectServerCtrl();
            UAConnServerCtrl.DisableDomainCheck = false;
            UAConnServerCtrl.PreferredLocales = null;
            UAConnServerCtrl.UserIdentity = null;
            UAConnServerCtrl.Configuration = UAConfiguration = configuration;
            UAConnServerCtrl.Configuration.SecurityConfiguration.AutoAcceptUntrustedCertificates = true;
            UAConnServerCtrl.ServerUrl = "opc.tcp://localhost:4840/";
            UAConnServerCtrl.UseSecurity = false;
            UAConnServerCtrl.ConnectComplete += new EventHandler(UASConnServerCtrl_ConnectComplete);
            
        }

        private void UASConnServerCtrl_ConnectComplete(object sender, EventArgs e)
        {
            try
            {
                UASession = UAConnServerCtrl.Session;

                if (UASession == null)
                {
                    return;
                }

                // set a suitable initial state.
                if (UASession != null && !UAConnectedOnce)
                {
                    UAConnectedOnce = true;
                }
            }
            catch
            {

            }

            MessageBox.Show(UAConnectedOnce.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                UAConnServerCtrl.Connect();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (UASession == null)
                    return;

                if (UASubscription !=null)
                {
                    UASession.RemoveSubscription(UASubscription);
                    UASubscription = null;
                }

                UASubscription = new Subscription();
                UASubscription.PublishingEnabled = true;
                UASubscription.PublishingInterval = 1000;
                UASubscription.Priority = 1;
                UASubscription.KeepAliveCount = 10;
                UASubscription.LifetimeCount = 20;
                UASubscription.MaxNotificationsPerPublish = 1000;

                UASession.AddSubscription(UASubscription);
                UASubscription.Create();

                MonitoredItem monitoredItem = new MonitoredItem();
                monitoredItem.StartNodeId = new NodeId("::AsGlobalPV:count", 6);
                monitoredItem.AttributeId = Attributes.Value;
                monitoredItem.Notification += MonitoredItem_Notification;
                UASubscription.AddItem(monitoredItem);
                UASubscription.ApplyChanges();

                label1.Text = "abcd";
                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MonitoredItem_Notification(MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MonitoredItemNotificationEventHandler(MonitoredItem_Notification), monitoredItem, e);
                return;
            }
            try
            {
                string a = ((MonitoredItemNotification)e.NotificationValue).Value.ToString();

                //MessageBox.Show(a.ToString());
                label1.Text = a;
            }
            catch
            {

            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            UAConnServerCtrl.Disconnect();
        }
    }
}
