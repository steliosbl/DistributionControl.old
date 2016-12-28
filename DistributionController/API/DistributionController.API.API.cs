namespace DistributionController.API
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal sealed class API
    {
        private Config config;
        private NetListener listener;

        public API(Config config)
        {
            this.config = config;
            this.listener = new NetListener(this.config.Port, DistributionCommon.Constants.DistributionController.API.SubDirs, this.RequestHandler);
            this.listener.Start();
        }

        //public string RequestHandler(string subdir)
        //{
        //    switch (subdir)
        //    {
        //        case "locations":
        //            return this.RequestHandler_Locations();
        //        case "states":
        //            return this.RequestHandler_States();
        //        default:
        //            return "NULL";
        //    }
        //}

        //public string RequestHandler_Locations()
        //{
        //    return Newtonsoft.Json.JsonConvert.SerializeObject(new Responses.Locations(this.database.GetLights().Values.ToArray()));
        //}

        //public string RequestHandler_States()
        //{
        //    return Newtonsoft.Json.JsonConvert.SerializeObject(new Responses.States(this.database.GetStates()));
        //}
    }
}
