using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCM
{
    public class CustomUserIdProvider:IUserIdProvider
    {
        public string GetUserId(IRequest request)
        {
            // your logic to fetch a user identifier goes here.

            // for example:

            var userId = HttpContext.Current.User.Identity.GetUserId(); 
            return userId.ToString();
        }
    }
}