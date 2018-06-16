using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace MitchellMarshClassLibrary.Database.User
{
    //Authorized information -> On request only authorized user access only
    public class AuthProperty<T>
    {
        public AuthProperty(bool SetDataAuthRequired = false)
        {
            _setAuthReq = SetDataAuthRequired;
        }
        private bool _setAuthReq;
        private T _property;
        public T GetData(ClaimsPrincipal RequestingUser)
        {
            if (RequestingUser == null || !RequestingUser.Identity.IsAuthenticated)
            {
                return default(T);
            }
            else return _property;
        }

        public bool SetData(T data, ClaimsPrincipal RequestingUser = null)
        {
            if(_setAuthReq)
            {
                if (RequestingUser == null || !RequestingUser.Identity.IsAuthenticated)
                {
                    return false;
                }
                else _property = data;
            }
            else
            {
                _property = data;
            }
            return true;
        }
    }
}
