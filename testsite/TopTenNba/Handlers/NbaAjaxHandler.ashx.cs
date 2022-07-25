using Commbox.Controllers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CommboxAssignment.Handlers
{
    public class NbaAjaxHandler : HttpTaskAsyncHandler
    {
        public readonly static string PRM_AJAX_COMMAND = "ac";

        public enum AjaxCommand : short
        {
            GetTopTen = 1
        }
        public override async Task ProcessRequestAsync(HttpContext context)
        {
            short commandId = short.Parse(context.Request[PRM_AJAX_COMMAND]);

            switch ((AjaxCommand)commandId)
            {
                case AjaxCommand.GetTopTen:
                    context.Response.Write(JsonConvert.SerializeObject(await GetTopTen(context)));
                    break;
                default:
                    break;
            }        
        }

        public async Task<List<NBAPlayerFullData>> GetTopTen(HttpContext context)
        {
            string year = context.Request["year"];
            NBAController nc = new NBAController();
            List<NBAPlayerFullData> topTen = await nc.GetAsync(year);
            return topTen;
        }

    }
}