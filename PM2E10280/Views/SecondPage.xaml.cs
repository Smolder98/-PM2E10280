using PM2E10280.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PM2E10280.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SecondPage : ContentPage
    {


        private Sitio Site = null;

        public SecondPage()
        {
            InitializeComponent();

            //Para que al inicializar cree la base de datos y no de error, y no esperar a
            //llamar la instancia para crearla
            if (App.DBase == null) Debug.WriteLine("Creando base de datos");
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();

            LoadData();

            Site = null;
        }


        private void listSites_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            try
            {

                Site = e.Item as Sitio;

            }
            catch (Exception ex)
            {
                Message("Error:", ex.Message);
            }

        }

        private async void btnDelete_Clicked(object sender, EventArgs e)
        {
            try
            {


                if (Site == null)
                {
                    Message("Aviso", "Seleccione un sitio");
                    return;
                }

                var status = await DisplayAlert("Aviso", $"¿Desea eliminar el sitio con descripcion: {Site.descripcion}?", "SI", "NO");

                if (status)
                {
                    //El usuario dijo que si
                    var result = await App.DBase.deleteSitio(Site);

                    if (result > 0)
                    {
                        Message("Aviso", "El sitio fue eliminado correctamente");
                        Site = null;
                        LoadData();
                    }
                    else
                    {

                        Message("Aviso", "No se pudo eliminar el sitio");
                    }
                }
                else
                {
                    //El usuario dijo que no
                }

            }
            catch (Exception ex)
            {
                Message("Error:", ex.Message);
            }
        }

        private async void btnViewMapa_Clicked(object sender, EventArgs e)
        {
            try
            {


                if (Site == null)
                {
                    Message("Aviso", "Seleccione un sitio");
                    return;
                }

                var status = await DisplayAlert("Aviso", $"¿Desea ir a la ubicacion indicada?", "SI", "NO");

                if (status)
                {
                    //El usuario dijo que si
                    await Navigation.PushAsync(new MapPage(Site));
                }
                else
                {
                    //El usuario dijo que no
                }

            }
            catch (Exception ex)
            {
                Message("Error:", ex.Message);
            }
        }


        private async void LoadData()
        {
            try
            {
                listSites.ItemsSource = await App.DBase.getListSitio();

             
            }
            catch (Exception ex)
            {
                Message("Error: ", ex.Message);
            }
        }


        #region Metodos Utiles


        private async void Message(string title, string message)
        {
            await DisplayAlert(title, message, "OK");
        }

        #endregion Metodos Utiles

        
    }
}