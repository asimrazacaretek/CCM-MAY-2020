using CCM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCM.Helpers
{
    public static class LiaisionHelper
    {
        
        public static string GetLiaisonNameFromID(this int? Id)
        {
            var name = "";
            if (Id != null)
            {
                var _db = new ApplicationdbContect();
                _db.Database.Connection.Open();
                var liasion = _db.Liaisons.Where(x => x.Id == Id).FirstOrDefault();
                if (liasion != null)
                {
                    name = liasion.LastName + " " + liasion.FirstName;
                }
                else
                {
                    name = "No Counselor/Translator Found for Id=" + Id;
                }
                _db.Database.Connection.Close();
            }
            else {
                name = "Liaison Id is null";
            }
            return name;
        }
    }
}