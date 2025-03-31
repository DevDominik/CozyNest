using static CozyNestAdmin.GlobalMethods;
using static CozyNestAdmin.GlobalEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using CozyNestAdmin.ResponseTypes;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace CozyNestAdmin
{
    public class GlobalVariables
    {
        public static MainWindow MainWindow { get; set; }
    }
    public class GlobalEnums
    {
        public enum AccountEndpoints { Login, Register, Logout, Introspect, RenewToken, UpdateData, DeleteAccount, LogoutEverywhere }
        public enum AdminEndpoints { GetUsers, GetRoles, ModifyUser, AddUser, AddReservation, CancelReservation, GetReservations, AddService, ModifyService }
        public enum ReservationEndpoints { GetReservations, Reserve, Cancel, GetRooms }
        public enum RoomEndpoints { List, Create, Delete, Modify }
        public enum ServiceEndpoints { Services }
        public enum TokenDeclaration { AccessToken, RefreshToken }
    }
    public class Session
    {
        public static string Username { get; set; }
        public static int Id { get; set; }
        public static string FirstName { get; set; }
        public static string LastName { get; set; }
        public static string Address { get; set; }
        public static bool Closed { get; set; }
        public static string Email { get; set; }
        public static DateTime JoinDate { get; set; }
        public static string RoleName { get; set; }
    }
    public class GlobalMethods
    {
        static readonly string BASE_URL = "https://localhost:7290/api";
        static readonly Dictionary<Enum, string> endpoints = new Dictionary<Enum, string>() {
            [AccountEndpoints.Login] = "/account/login",
            [AccountEndpoints.Register] = "/account/register",
            [AccountEndpoints.Logout] = "/account/logout",
            [AccountEndpoints.Introspect] = "/account/introspect",
            [AccountEndpoints.RenewToken] = "/account/renewtoken",
            [AccountEndpoints.UpdateData] = "/account/updatedata",
            [AccountEndpoints.DeleteAccount] = "/account/deleteaccount",
            [AccountEndpoints.LogoutEverywhere] = "/account/logouteverywhere",
            [AdminEndpoints.GetUsers] = "/admin/getusers",
            [AdminEndpoints.GetRoles] = "/admin/getroles",
            [AdminEndpoints.ModifyUser] = "/admin/modifyuser",
            [AdminEndpoints.AddUser] = "/admin/adduser",
            [AdminEndpoints.AddReservation] = "/admin/addreservation",
            [AdminEndpoints.CancelReservation] = "/admin/cancelreservation",
            [AdminEndpoints.GetReservations] = "/admin/getreservations",
            [AdminEndpoints.AddService] = "/admin/addservice",
            [AdminEndpoints.ModifyService] = "/admin/modifyservice",
            [ReservationEndpoints.GetReservations] = "/reservation/getreservations",
            [ReservationEndpoints.Reserve] = "/reservation/reserve",
            [ReservationEndpoints.Cancel] = "/reservation/cancel",
            [ReservationEndpoints.GetRooms] = "/reservation/getrooms",
            [RoomEndpoints.List] = "/room/list",
            [RoomEndpoints.Create] = "/room/create",
            [RoomEndpoints.Delete] = "/room/delete",
            [RoomEndpoints.Modify] = "/room/modify",
            [ServiceEndpoints.Services] = "/service/services"
        };
        static readonly Dictionary<Enum, string> tokens = new Dictionary<Enum, string>() 
        {
            [TokenDeclaration.AccessToken] = "",
            [TokenDeclaration.RefreshToken] = ""
        };
        public static string GetEndpoint(Enum endpoint)
        {
            return BASE_URL + endpoints[endpoint];
        }
        public static HttpClient CreateHTTPClient(TokenDeclaration? tokenDeclaration = null) 
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            if (tokenDeclaration != null)
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokens[tokenDeclaration.Value]);
            }
            return client;
        }
        public static async Task<(bool, string)> Authenticate(string username, string password)
        {
            using HttpClient client = CreateHTTPClient();
            var res = await client.PostAsync(
                GetEndpoint(AccountEndpoints.Login), 
                new StringContent
                (
                    JsonConvert.SerializeObject(new
                    {
                        username = username,
                        password = password,
                    }),
                    Encoding.UTF8,
                    "application/json"
                )
            );
            
            if (res.StatusCode == HttpStatusCode.OK)
            {
                LoginResponse loginResponse = JsonConvert.DeserializeObject<LoginResponse>(await res.Content.ReadAsStringAsync());
                Session.Username = loginResponse.UserData.Username;
                Session.Id = loginResponse.UserData.Id;
                Session.FirstName = loginResponse.UserData.FirstName;
                Session.LastName = loginResponse.UserData.LastName;
                Session.RoleName = loginResponse.UserData.RoleName;
                Session.Address = loginResponse.UserData.Address;
                Session.Closed = loginResponse.UserData.Closed;
                Session.Email = loginResponse.UserData.Email;
                SetAccessToken(loginResponse.AccessToken);
                SetRefreshToken(loginResponse.RefreshToken);
                return (true, loginResponse.Message);
            }
            MessageResponse messageResponse = JsonConvert.DeserializeObject<MessageResponse>(await res.Content.ReadAsStringAsync());
            return (false, messageResponse.Message);
        }
        private static void SetAccessToken(string token)
        {
            tokens[TokenDeclaration.AccessToken] = token;
        }
        private static void SetRefreshToken(string token)
        {
            tokens[TokenDeclaration.RefreshToken] = token;
        }
        public static async Task<bool> Introspect() 
        {
            using (HttpClient client = CreateHTTPClient(TokenDeclaration.AccessToken))
            {
                var res = await client.GetAsync(GetEndpoint(AccountEndpoints.Introspect));
                if (res.StatusCode == HttpStatusCode.OK)
                {
                    IntrospectResponse introspectResponse = JsonConvert.DeserializeObject<IntrospectResponse>(await res.Content.ReadAsStringAsync());
                    Session.Username = introspectResponse.UserData.Username;
                    Session.Id = introspectResponse.UserData.Id;
                    Session.FirstName = introspectResponse.UserData.FirstName;
                    Session.LastName = introspectResponse.UserData.LastName;
                    Session.RoleName = introspectResponse.UserData.RoleName;
                    Session.Address = introspectResponse.UserData.Address;
                    Session.Closed = introspectResponse.UserData.Closed;
                    Session.Email = introspectResponse.UserData.Email;
                    return true;
                }
            }
            using (HttpClient client = CreateHTTPClient(TokenDeclaration.RefreshToken))
            {
                var res = await client.GetAsync(GetEndpoint(AccountEndpoints.RenewToken));
                if (res.StatusCode == HttpStatusCode.OK)
                {
                    TokenResponse tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(await res.Content.ReadAsStringAsync());
                    SetAccessToken(tokenResponse.AccessToken);
                    SetRefreshToken(tokenResponse.RefreshToken);
                    return await Introspect();
                }
            }
            return false;
        }
        public static void ResetSession()
        {
            SetAccessToken("");
            SetRefreshToken("");
        }
        public static void ReturnToLogin()
        {
            CreateHTTPClient(TokenDeclaration.RefreshToken).GetAsync(GetEndpoint(AccountEndpoints.Logout));
            ResetSession();
            Auth auth = new Auth(false);
            auth.Show();
            GlobalVariables.MainWindow.Close();
        }
    }
}
