using CCM.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CCM.Helpers
{
    public static class ModalStateValidator
    {
        internal static List<ErrorResult> ValidateModelState(ModelStateDictionary ModelState)
        {
            List<ErrorResult> Errors = new List<ErrorResult>();
            foreach (KeyValuePair<string, ModelState> modelStateDD in ModelState)
            {
                string key = modelStateDD.Key;
                ModelState modelState = modelStateDD.Value;

                foreach (ModelError error in modelState.Errors)
                {
                    ErrorResult er = new ErrorResult();
                    er.ErrorMessage = error.ErrorMessage;
                    er.Field = key;
                    Errors.Add(er);
                }
            }
            return Errors;
        }
    }
}