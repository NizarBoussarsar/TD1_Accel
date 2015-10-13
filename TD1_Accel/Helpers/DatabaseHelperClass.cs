using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TD1_Accel.Model;

namespace TD1_Accel.Helpers
{
    class DatabaseHelperClass
    {
        SQLiteConnection dbConn;

        //Create Tabble 
        public async Task<bool> onCreate(string DB_PATH)
        {
            try
            {
                if (!CheckFileExists(DB_PATH).Result)
                {
                    using (dbConn = new SQLiteConnection(DB_PATH))
                    {
                        dbConn.CreateTable<AccelData>();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


        private async Task<bool> CheckFileExists(string fileName)
        {
            try
            {
                var store = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Retrieve the specific contact from the database. 
        public AccelData ReadAccelData(int id)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                var existingconact = dbConn.Query<AccelData>("select * from AccelData where Id =" + id).FirstOrDefault();
                return existingconact;
            }
        }


        // Retrieve the all contact list from the database. 
        public ObservableCollection<AccelData> ReadAllAccelData()
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                List<AccelData> myCollection = dbConn.Table<AccelData>().ToList<AccelData>();
                ObservableCollection<AccelData> AccelDataList = new ObservableCollection<AccelData>(myCollection);
                return AccelDataList;
            }
        }

        //Update existing conatct 
        public void UpdateAccelData(AccelData accelData)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                var existing = dbConn.Query<AccelData>("select * from AccelData where Id =" + accelData.Id).FirstOrDefault();
                if (existing != null)
                {
                    existing.X = accelData.X;
                    existing.Y = accelData.Y;
                    existing.Z = accelData.Z;

                    dbConn.RunInTransaction(() =>
                    {
                        dbConn.Update(existing);
                    });
                }
            }
        }

        // Insert the new contact in the Contacts table. 
        public void Insert(AccelData newAccelData)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                dbConn.RunInTransaction(() =>
                {
                    dbConn.Insert(newAccelData);
                });
            }
        }

        //Delete specific contact 
        public void DeleteAccelData(int Id)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                var existing = dbConn.Query<AccelData>("select * from AccelData where Id =" + Id).FirstOrDefault();
                if (existing != null)
                {
                    dbConn.RunInTransaction(() =>
                    {
                        dbConn.Delete(existing);
                    });
                }
            }
        }
        

        //Delete all contactlist or delete Contacts table 
        public void DeleteAllAccelData()
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                //dbConn.RunInTransaction(() => 
                //   { 
                dbConn.DropTable<AccelData>();
                dbConn.CreateTable<AccelData>();
                dbConn.Dispose();
                dbConn.Close();
                //}); 
            }
        }

    }
}
