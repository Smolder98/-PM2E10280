using PM2E10280.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PM2E10280.Controller
{
    public class DataBase
    {

        readonly SQLiteAsyncConnection dbase;

        public DataBase(string dbpath)
        {
            dbase = new SQLiteAsyncConnection(dbpath);

            //Creacion de las tablas de la base de datos

            dbase.CreateTableAsync<Sitio>(); //Creando la tabla Sitio

        }

        #region OperacionesSitio
        //Metodos CRUD - CREATE
        public Task<int> insertUpdateSitio(Sitio sitio)
        {
            if (sitio.id != 0)
            {
                return dbase.UpdateAsync(sitio);
            }
            else
            {
                return dbase.InsertAsync(sitio);
            }
        }

        //Metodos CRUD - READ
        public Task<List<Sitio>> getListSitio()
        {
            return dbase.Table<Sitio>().ToListAsync();
        }

        public Task<Sitio> getSitio(int id)
        {
            return dbase.Table<Sitio>()
                .Where(i => i.id == id)
                .FirstOrDefaultAsync();
        }

        //Metodos CRUD - DELETE
        public Task<int> deleteSitio(Sitio sitio)
        {
            return dbase.DeleteAsync(sitio);
        }

        #endregion OperacionesSitio

    }
}

