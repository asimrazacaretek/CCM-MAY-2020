using CCM.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CCM.Helpers
{
    public static class DataManipulation
    {
        public static void CategoryStatusManipulation()
        {
         ApplicationdbContect _db = new ApplicationdbContect();
           var categorystatus= _db.CategoriesStatuses.Where(p=>p.BillingCategoryId==BillingCodeHelper.G0506BillingCatagoryid).ToList();
          var duplicatelist=  categorystatus.GroupBy(p => p.PatientId).Where(p => p.Count() > 1).Select(p => p.ToList().OrderBy(x=>x.PatientId));
           var duplicatecount= duplicatelist.Count();
          
            foreach (var item in duplicatelist)
            {
                if (item.Where(p => p.Status == "Claims Submission").Count() > 0)
                {
                    var claim=item.Where(p => p.Status == "Claims Submission").FirstOrDefault();
                    claim.Cycle = 1;
                    _db.Entry(claim).State = EntityState.Modified;
                    _db.SaveChanges();
                    var removelist= _db.CategoriesStatuses.Where(p => p.PatientId == claim.PatientId && p.Id != claim.Id).ToList();
                    _db.CategoriesStatuses.RemoveRange(removelist);
                    _db.SaveChanges();
                   
                }
                else
                {
                    if (item.Where(p => p.Status == "Clinical Sign-Off").Count() > 0)
                    {
                        var clinical = item.Where(p => p.Status == "Clinical Sign-Off").FirstOrDefault();
                        clinical.Cycle = 1;
                        _db.Entry(clinical).State = EntityState.Modified;
                        _db.SaveChanges();
                        var removelist = _db.CategoriesStatuses.Where(p => p.PatientId == clinical.PatientId && p.Id != clinical.Id).ToList();
                        _db.CategoriesStatuses.RemoveRange(removelist);
                        _db.SaveChanges();
                    }
                    else
                    {
                        if (item.Where(p => p.Status == "Enrolled").Count() > 0)
                        {
                           var des= item.OrderByDescending(p => p.UpdatedOn == null ? p.CreatedOn : p.UpdatedOn);
                         
                            
                            var Enrolled = des.Where(p => p.Status == "Enrolled" ).FirstOrDefault();
                            if (Enrolled != null)
                            {
                                Enrolled.Cycle = 1;
                            _db.Entry(Enrolled).State = EntityState.Modified;
                            _db.SaveChanges();
                           
                            var removelist = _db.CategoriesStatuses.Where(p => p.PatientId == Enrolled.PatientId && p.Id != Enrolled.Id).ToList();
                            _db.CategoriesStatuses.RemoveRange(removelist);
                            _db.SaveChanges();

                            }
                        }

                    }

                }




            }

    }




}
}