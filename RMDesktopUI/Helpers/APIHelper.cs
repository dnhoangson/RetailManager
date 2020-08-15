﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Configuration;
using RMDesktopUI.Models;

namespace RMDesktopUI.Helpers
{
    public class APIHelper : IAPIHelper
    {
        private HttpClient apiClient;

        public APIHelper()
        {
            InitializeClient();
        }

        private void InitializeClient()
        {
            string api = ConfigurationManager.AppSettings["api"];

            apiClient = new HttpClient();
            apiClient.BaseAddress = new Uri(api);
            apiClient.DefaultRequestHeaders.Accept.Clear();
            apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<AuthenticatedUser> Authenticate(string username, string password)
        {
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password)
            });

            using (HttpResponseMessage response = await apiClient.PostAsync("/Token", data))
            {
                if (response.IsSuccessStatusCode)
                {
                    //If succeed, then grab the information from "Content"
                    //and put it in AuthenticatedUser model
                    //then return that model
                    var result = await response.Content.ReadAsAsync<AuthenticatedUser>();
                    return result;
                }
                else
                {
                    //If fail, return the reason why failed in the exception
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}