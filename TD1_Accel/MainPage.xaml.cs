using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Sensors;
using Windows.UI.Core;
using Windows.UI.Popups;
using TD1_Accel.Helpers;
using TD1_Accel.Model;
using System.Diagnostics;


// Pour en savoir plus sur le modèle d'élément Page vierge, consultez la page http://go.microsoft.com/fwlink/?LinkId=391641

namespace TD1_Accel
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Boolean tracking;
        Accelerometer accelerometer;
        private uint desiredReportInterval;
        int i = 0;
        TypedEventHandler<Accelerometer, AccelerometerReadingChangedEventArgs> Event = null;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
            tracking = false;
            accelerometer = Accelerometer.GetDefault();

            barX.Height = 0;
            barY.Height = 0;
            barZ.Height = 0;
            barXneg.Height = 0;
            barYneg.Height = 0;
            barZneg.Height = 0;

        }

        /// <summary>
        /// Invoqué lorsque cette page est sur le point d'être affichée dans un frame.
        /// </summary>
        /// <param name="e">Données d'événement décrivant la manière dont l'utilisateur a accédé à cette page.
        /// Ce paramètre est généralement utilisé pour configurer la page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

        }


        private void AddDataEntry(double x, double y, double z)
        {
            DatabaseHelperClass Db_Helper = new DatabaseHelperClass();
                Db_Helper.Insert(new Model.AccelData(x,y,z));     
        }



        private async void ReadingChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                AccelerometerReading reading = args.Reading;
                i++;

                txtXvalue.Text = String.Format("{0,5:0.00}", reading.AccelerationX);
                txtYvalue.Text = String.Format("{0,5:0.00}", reading.AccelerationY);
                txtZvalue.Text = String.Format("{0,5:0.00}", reading.AccelerationZ);


                this.AddDataEntry(reading.AccelerationX, reading.AccelerationY, reading.AccelerationZ);

                return;

            });
        }


        private void ReadData_Loaded()
        {
            DatabaseHelperClass Db_Helper = new DatabaseHelperClass();
            List<AccelData> myData =  Db_Helper.ReadAllAccelData().ToList();
            if (myData.Count > 0)
            {
                foreach (AccelData item in myData)
                {
                    Debug.WriteLine("X " + item.X + " Y " + item.Y + " Z " + item.Z + " \n");
                }   
            }
        }


        private void btnStart_Click(object sender, RoutedEventArgs e)
        {

            if (Event == null)
            {
                Event = new TypedEventHandler<Accelerometer, AccelerometerReadingChangedEventArgs>(ReadingChanged);
            }


            if (tracking)
            {
                accelerometer.ReadingChanged -= Event;
               
                tracking = false;
                btnStart.Content = "Start";

                txtXvalue.Text = "A";
                txtYvalue.Text = "B";
                txtZvalue.Text = "C";
            }
            else
            {
                if (accelerometer != null)
                {
                    btnStart.IsEnabled = false;
                    btnStart.IsEnabled = true;

                    tracking = true;
                    btnStart.Content = "Pause";

                    // Select a report interval that is both suitable for the purposes of the app and supported by the sensor.
                    // This value will be used later to activate the sensor.
                    uint minReportInterval = accelerometer.MinimumReportInterval;
                    desiredReportInterval = minReportInterval > 16 ? minReportInterval : 16;
                    accelerometer.ReportInterval = 100;
                    //add event for accelerometer readings
                    accelerometer.ReadingChanged += Event;

                }
                else
                {
                    MessageDialog ms = new MessageDialog("No accelerometer Found");
                    ms.ShowAsync();
                }
            }
        }

        private async void btnRead_Click(object sender, RoutedEventArgs e)
        {

            MessageDialog ms = new MessageDialog("Shutting Down");
            await ms.ShowAsync();
            ReadData_Loaded();

        }
    }
}
