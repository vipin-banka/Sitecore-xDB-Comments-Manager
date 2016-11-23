#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Sitecore.Diagnostics;
using Sitecore.StringExtensions;

#endregion

namespace xDBCommentsManager
{
    public class Recaptcha
    {
        public bool Validate(string response,string str="")
        {
            bool responseObject = RecaptchaIsValid(response,str);
            return responseObject;
        }

        public bool RecaptchaIsValid(string captchaResponse,string str="")
        {
            bool valid = false;
           GetSettings objGetSettings=new GetSettings();
           var settings = objGetSettings.GetSetting(str);
            var secretKey = settings.SecretKey;
            if (!secretKey.IsNullOrEmpty())
            {
                //Request to Google Server
                HttpWebRequest req =
                    (HttpWebRequest)
                        WebRequest.Create("https://www.google.com/recaptcha/api/siteverify?secret=" + secretKey +
                                          "&response=" + captchaResponse);
                try
                {
                    //Google recaptcha Response
                    using (WebResponse wResponse = req.GetResponse())
                    {
                        using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                        {
                            string jsonResponse = readStream.ReadToEnd();

                            JavaScriptSerializer js = new JavaScriptSerializer();
                            // Deserialize Json
                            RecaptchaResponse data = js.Deserialize<RecaptchaResponse>(jsonResponse);
                            valid = Convert.ToBoolean(data.Success);
                        }
                    }

                    return valid;
                }
                catch (WebException ex)
                {
                    Log.Error(ex.Message + ": Not contacting Google Response server", this);
                    return false;
                }
            }
            return valid;
        }
        //private static string GetUserIp()
        //{
        //    return System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        //}
    }
    public class RecaptchaResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("error-codes")]
        public List<string> ErrorCodes { get; set; }
    }
}
