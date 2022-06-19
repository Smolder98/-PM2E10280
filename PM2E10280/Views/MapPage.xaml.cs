using Android.Graphics;
using Plugin.Media.Abstractions;
using PM2E10280.Models;
using PM2E10280.Services;
using System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;
using Path = System.IO.Path;

namespace PM2E10280.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        Sitio Sitio = null;

        public MapPage(Sitio sitio)
        {
            InitializeComponent();

            Sitio = sitio;
        }


        protected async override void OnAppearing()
        {
            base.OnAppearing();

            try
            {

                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();


                if (status == PermissionStatus.Granted)
                {
                    var gpsStatus = DependencyService.Get<ILocSettings>().isGpsAvailable();

                    if (!gpsStatus)
                    {
                        Message("Aviso", "Su gps no esta activo: por favor activelo");

                        //Una espera para que se pueda leer el mensaje 
                        await Task.Delay(4000);

                        DependencyService.Get<ILocSettings>().OpenSettings();
                        return;
                    }

                    var localizacion = await Geolocation.GetLocationAsync();

                    if (localizacion != null)
                    {
                        
                        var pin = new Pin()
                        {   
                            Type = PinType.SavedPin,
                            Position = new Position(Sitio.latitude, Sitio.longitude),
                            Label = "Descripcion",
                            Address = Sitio.descripcion
                            
                        };


                        mapa.Pins.Add(pin);
                        
                        //mapa.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(localizacion.Latitude, localizacion.Longitude), Distance.FromMeters(100)));
                        
                        
                        mapa.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(Sitio.latitude, Sitio.longitude), Distance.FromMeters(100)));

                    }

                }
                else
                {
                    await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                }
            }
            catch (Exception e)
            {
                Message("Error", e.Message);
            }

        }


        private async void btnShare_Clicked(object sender, EventArgs e)
        {


            try
            {
                //"/storage/emulated/0/Android/data/com.companyname.pm2e10280/files/Pictures/MisUbicaciones/IMG_2022061…"
                //folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), Sitio.nameImage);
                
                if(!File.Exists(Path.GetFileName(Sitio.pathImage)))
                    File.WriteAllBytes(Sitio.pathImage, Sitio.image);

                var image = new ShareFile(Sitio.pathImage);


                if (image == null)
                {
                    Message("Aviso", "No se pudo compartir la imagen");
                    return;
                }

                
                await Share.RequestAsync(new ShareFileRequest{
                    Title = Sitio.descripcion,
                    File = image
                });

            }
            catch (Exception ex)
            {

                Message("Error: ", ex.Message);
            }
            
        }

        private ImageSource ByteArrayToImage(Byte[] array)
        {
            ImageSource retSource = null; //Se valida que el objeto  a convertir no vallan a ser nulos
            if (array != null)
            {
                byte[] imageAsBytes = (byte[])array;
                retSource = ImageSource.FromStream(() => new MemoryStream(imageAsBytes));
            }
            return retSource;
        }

        public static Bitmap bytesToBitmap(byte[] imageBytes)
        {

            Bitmap bitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);

            return bitmap;
        }





        private async void Message(string title, string message)
        {
            await DisplayAlert(title, message, "OK");
        }

        
    }
}