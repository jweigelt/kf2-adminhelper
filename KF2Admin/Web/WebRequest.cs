/* 
 * This file is part of kf2 adminhelper. (https://github.com/jweigelt/kf2-adminhelper)
 * Copyright (C) 2017 Jan Weigelt (info@janelo.net)
 * 
 * kf2 adminhelper is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.

 * kf2 adminhelper is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.

 * You should have received a copy of the GNU General Public License
 * along with kf2 adminhelper.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Net;
using System.IO;
using System.Text;

namespace KF2Admin.Web
{
    public class WebRequest
    {
        private HttpWebRequest request = null;
        private HttpWebResponse response = null;

        public virtual string ResponseContentData { get { return responseContentData; } }
        public virtual Uri ResponseUri { get { return (response == null ? null : response.ResponseUri); } }
        public virtual bool ResponseOK { get { return responseOK; } }

        private bool responseOK = false;
        private string responseContentData = null;
        private string postData;

        public WebRequest(string requestUrl, string method, string userAgent, CookieContainer cookieContainer, string postData = "", string contentType = "")
        {
            request = (HttpWebRequest)HttpWebRequest.Create(requestUrl);
            request.Method = method;
            request.UserAgent = userAgent;
            request.CookieContainer = cookieContainer;

            if (method == "POST")
            {
                request.ContentType = contentType;
                request.ContentLength = postData.Length;
                this.postData = postData;
            }
        }

        private byte[] GetBytes(string s)
        {
            return Encoding.UTF8.GetBytes(s);
        }

        public void PerformRequest()
        {
            try
            {
                if (request.Method == "POST")
                {
                    using (Stream str = request.GetRequestStream())
                    {
                        str.Write(GetBytes(postData), 0, postData.Length);
                        str.Close();
                    }
                }

                response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (StreamReader strR = new StreamReader(response.GetResponseStream()))
                    {
                        responseContentData = strR.ReadToEnd();
                        strR.Close();
                    }
                    responseOK = true;
                }
                else
                {
                    responseOK = false;
                    throw new Exception("HTTP != 200 OK");
                }

            }
            catch (WebException e)
            {
                responseOK = false;
                throw e;
            }
        }

    }
}