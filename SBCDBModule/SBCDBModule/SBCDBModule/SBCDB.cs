using Microsoft.EntityFrameworkCore;
using SBCDBModule.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace SBCDBModule
{
    public class SBCDB
    {
      public static  string ConnectionString = "Data Source = SBCDB.db";
    
        /// <summary>
        /// Add new user
        /// </summary>
        /// <param name="user"></param>
        public void AddUser(User user)
        {
            using(DataContext db = new DataContext())
            {
                db.Add(user);
                db.SaveChanges();
            }
        }

         public List<DeviceType> DeviceTypeList()
        {
            IQueryable<DeviceType> data = null;
            List<DeviceType> datatypelist = null;

            var db = new DataContext();
            try
            {
                
                data = (from dt in db.DeviceTypes select dt) ;

                 datatypelist = new List<DeviceType>(data);
               
            }
            catch (Exception ex)
            {
                string s = ex.ToString();

            }
            return datatypelist;
        }
        /// <summary>
        /// return loggedin user
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public User GetLoggedinUser(string userid)
        {
            IQueryable<User> data = null;
            User loggedinuser = null;
            var db = new DataContext();
            try
            {               
                data = (from user in db.Users 
                        where user.UserID == userid
                        select  user);

                if (data != null)
                    loggedinuser = (User)data.FirstOrDefault();
            }
            catch(Exception ex)
            {
                string s = ex.ToString();
              
            }
            

            if(db != null)
                db.Dispose();

            return loggedinuser; 
        }

        public void CopyDBToLocalFolder()
        {
          var v =   CopyDB();
        }
        private    async Task CopyDB()
        {
            var dbFile = await ApplicationData.Current.LocalFolder.TryGetItemAsync("C:\\Users\\Sujitpc\\Pictures\\DB\\SBCDB.db") as StorageFile;

            if (null == dbFile)
            {
                var localFolder = ApplicationData.Current.LocalFolder;
                var originalDbFileUri = new Uri("ms-appx:///Assets/SBCDB.db");
                var originalDbFile = await StorageFile.GetFileFromPathAsync("Assets/SBCDB.db");//GetFileFromApplicationUriAsync(originalDbFileUri);

                if (null != originalDbFile)
                {
                  var db =await  originalDbFile.CopyAsync(localFolder, "SBCDB.db", NameCollisionOption.ReplaceExisting);
                }
            }
        }
        ///
    }
}
