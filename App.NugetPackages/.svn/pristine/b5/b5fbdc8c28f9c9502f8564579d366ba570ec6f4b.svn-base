using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using PushNotifyLocal.Plugin;
using System.Globalization;

namespace SamplePlugin
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            int count = 0;
            //CrossLocalNotifications.Current.PushSnackbar("Title", "Body s", null,
            //    result =>
            //    {
            //        count++;
            //        Console.WriteLine("XXX:" + count.ToString() + result.Action.ToString());
            //    });

            CrossLocalNotifications.Current.PushSnackbar("Title", "Body s", null);
            //int gio = (!string.IsNullOrWhiteSpace(entryGio.Text)) ? Convert.ToInt32(entryGio.Text) : DateTime.Now.Hour;
            //int phut = (!string.IsNullOrWhiteSpace(entryPhut.Text)) ? Convert.ToInt32(entryPhut.Text) : DateTime.Now.Minute;
            //int id = (!string.IsNullOrWhiteSpace(entryID.Text)) ? Convert.ToInt32(entryID.Text) : 0;
            //int ngay = (!string.IsNullOrWhiteSpace(entryNgay.Text)) ? Convert.ToInt32(entryNgay.Text) : 0;

            //DateTime ngayHen = new DateTime(2019, 8, ngay, gio, phut, 1);
            //CrossLocalNotifications.Current.AddSchedule(id, "Báo thức", "Đây là id:" + id.ToString(), ngayHen, null);
            //DependencyService.Get<IMessage>().LongAlert("Đã hẹn giờ: "+ ngayHen.ToString("HH:mm dd/MM/yyyy"));
        }

        private void Button1_Clicked(object sender, EventArgs e)
        {
            int id = (!string.IsNullOrWhiteSpace(entryID.Text)) ? Convert.ToInt32(entryID.Text) : 0;
            CrossLocalNotifications.Current.RemoveSchedule(id);
        }
    }
}
