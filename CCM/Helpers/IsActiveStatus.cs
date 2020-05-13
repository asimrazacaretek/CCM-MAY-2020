using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CCM.Helpers
{
    public enum IsActiveStatus
    {
        Active=1,
        DeActive=0
    }
    public enum ModelNumber
    {
        CSHM188 = 0,
        CSHM189 = 1,
        CSHM190 = 2,
        CSHM191 = 3,
        CSHM192 = 4,
    }
    public enum DeviceStatus
    {
        [Description("Ready to map")]
        ReadyToMap  = 0,
        [Description("Already Mapped")]
        AlreadyMapped = 1,
    
    }
}