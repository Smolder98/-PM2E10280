using Android.Content;
using Android.Locations;
using PM2E10280.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

[assembly: Dependency(typeof(LocationShare))]
namespace PM2E10280.Services
{
    public class LocationShare : ILocSettings
    {
        public bool isGpsAvailable()
        {
            bool value = false;
            LocationManager manager = (LocationManager)Android.App.Application.Context.GetSystemService(Context.LocationService);
            if (!manager.IsProviderEnabled(LocationManager.GpsProvider))
            {
                //gps disable
                value = false;
            }
            else
            {
                //Gps enable
                value = true;
            }
            return value;
        }

        public void OpenSettings()
        {
            Intent intent = new Intent(Android.Provider.Settings.ActionLocat‌​ionSourceSettings);
            intent.AddFlags(ActivityFlags.NewTask);
            Android.App.Application.Context.StartActivity(intent);
        }
    }
}
