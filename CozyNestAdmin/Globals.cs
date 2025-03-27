using static CozyNestAdmin.GlobalMethods;
using static CozyNestAdmin.GlobalEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;

namespace CozyNestAdmin
{
    public class GlobalEnums
    {
        public enum AccountEndpoints { Login, Register, Logout, Introspect, RenewToken, UpdateData, DeleteAccount, LogoutEverywhere }
        public enum AdminEndpoints { GetUsers, GetRoles, ModifyUser, AddUser, AddReservation, CancelReservation, GetReservations, AddService, ModifyService }
        public enum ReservationEndpoints { GetReservations, Reserve, Cancel, GetRooms }
        public enum RoomEndpoints { List, Create, Delete, Modify }
        public enum ServiceEndpoints { Services }
        public enum TokenDeclaration { AccessToken, RefreshToken }
    }
    public class GlobalMethods
    {
        static readonly string BASE_URL = "http://localhost:5232/api";
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
            HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            if (tokenDeclaration != null)
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokens[tokenDeclaration.Value]);
            }
            return client;
        }
    }
}
