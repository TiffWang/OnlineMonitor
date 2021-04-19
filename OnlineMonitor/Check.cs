using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineMonitor
{
    public class Check
    {
        public static void CheckOnline()
        {
            string corpId = "ww8216c019edc9fc4a";
            string corpSecret = "PS5wUqe8ICPQRj5_YyFNS5_nFzOrQ8qZs8etL5FQ82E";
            string agentId = "1000002";

            string msgUrl = "https://tiffmsg.herokuapp.com/api/Send?corpId=" + corpId + "&corpSecret=" + corpSecret + "&agentId=" + agentId + "&text=";
            string checkUrl = "http://hpidc.gicp.net:9999/login";

            try
            {
                string result = HttpClientHelper.Get(checkUrl);
                if (result == "")
                {
                    Console.WriteLine("XXXX 机器掉线--------" + DateTime.Now.ToString());
                    HttpClientHelper.Get(msgUrl + "机器掉线");

                    Thread.Sleep(3 * 60 * 1000);
                }
                else if (result.Contains("入口"))
                {
                    Console.WriteLine("√√ 机器正常--------" + DateTime.Now.ToString());

                    Thread.Sleep(5 * 60 * 1000);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("XXXX 机器掉线--------" + DateTime.Now.ToString());
                HttpClientHelper.Get(msgUrl + "机器掉线");
                Console.WriteLine(e.Message);
                Thread.Sleep(3 * 60 * 1000);
            }
        }
    }
}
