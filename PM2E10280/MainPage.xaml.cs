using Plugin.Media;
using Plugin.Media.Abstractions;
using PM2E10280.Models;
using PM2E10280.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PM2E10280
{
    public partial class MainPage : ContentPage
    {
        MediaFile FileFoto = null;

        public MainPage()
        {
            InitializeComponent();

            //Para que al inicializar cree la base de datos y no de error, y no esperar a
            //llamar la instancia para crearla
            if (App.DBase == null) Debug.WriteLine("Creando base de datos");
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            getLatitudeAndLongitude();
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {

            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();


            if (status == PermissionStatus.Granted)
            {
                try
                {
                    FileFoto = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                    {

                        Directory = "MisUbicaciones",
                        AllowCropping = true,
                        CustomPhotoSize = 30,
                        CompressionQuality = 30
                        
                    });


                    if (FileFoto != null)
                    {
                        imgFoto.Source = ImageSource.FromStream(() => {
                            return FileFoto.GetStream();
                        });
                    }

                    //await DisplayAlert("Direcctorio", FileFoto.Path, "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", ex.Message, "OK");
                }

            }
            else
            {
                await Permissions.RequestAsync<Permissions.Camera>();
            }

            
        }

        private void btnExit_Clicked(object sender, EventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
        }

        private async void btnAdd_Clicked(object sender, EventArgs e)
        {
            if (FileFoto == null)
            {
                Message("Aviso", "Aun no se a tomado una foto: Presione la imagen de ejemplo para capturar una imagen");
                return;
            }


            if (string.IsNullOrEmpty(txtLatitude.Text) || string.IsNullOrEmpty(txtLongitude.Text))
            {
                Message("Aviso", "Aun no se obtiene la ubicacion");
                getLatitudeAndLongitude();
                return;
            }


            if (string.IsNullOrEmpty(txtDescription.Text.Trim()))
            {
                Message("Aviso", "Debe escribir una breve descripcion");
                return;
            }


            try
            {
                var sitio = new Sitio()
                {
                    id = 0,
                    latitude = double.Parse(txtLatitude.Text),
                    longitude = double.Parse(txtLongitude.Text),
                    descripcion = txtDescription.Text,
                    image = ConvertImageToByteArray(),
                    pathImage = FileFoto.Path
                };

                var result = await App.DBase.insertUpdateSitio(sitio);

                if (result > 0)
                {
                    Message("Aviso", "Sitio agregado correctamente");
                    clearComp();
                }
                else
                {
                    Message("Aviso", "No se pudo agregar el sitio");
                }

            }
            catch (Exception ex)
            {

                Message("Error: ", ex.Message);
            }

        }
        
        private async void btnList_Clicked(object sender, EventArgs e)
        {
           await Navigation.PushAsync(new SecondPage());
        }

        private async void getLatitudeAndLongitude()
        {
            try
            {

                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();


                if (status == PermissionStatus.Granted)
                {
                    
                    var localizacion = await Geolocation.GetLocationAsync();

                    txtLatitude.Text = Math.Round(localizacion.Latitude, 5) + "";
                    txtLongitude.Text = Math.Round(localizacion.Longitude, 5) + "";

                }
                else
                {
                    
                    await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                }
            }
            catch (Exception e)
            {
                


                if (e.Message.Equals("Location services are not enabled on device."))
                {

                    Message("Error", "Servicio de localizacion no encendido");
                }
                else
                {
                    Message("Error", e.Message);

                }

            }
        }

        private void clearComp()
        {
            imgFoto.Source = "imgMuestra.png";
            txtDescription.Text = "";
            FileFoto = null;
            getLatitudeAndLongitude();
        }


        #region Metodos Utiles

        private Byte[] ConvertImageToByteArray()
        {
            if (FileFoto != null)
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    Stream stream = FileFoto.GetStream();

                    stream.CopyTo(memory);

                    return memory.ToArray();
                }
            }

            return null;
        }

        private async void Message(string title, string message)
        {
            await DisplayAlert(title, message, "OK");
        }

        #endregion Metodos Utiles
    }
}
