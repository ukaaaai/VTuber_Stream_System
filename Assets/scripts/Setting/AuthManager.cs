using Firebase.Auth;
using UnityEngine;

namespace Setting
{
    public sealed class AuthManager
    {
        //singleton
        private static AuthManager _instance;
        public static AuthManager Instance
        {
            get
            {
                return _instance ??= new AuthManager();
            }
        }

        private FirebaseUser _user;
        public string DisplayName = string.Empty;
        public string UserID = string.Empty;
        
        public void Login()
        {
            FirebaseAuth.DefaultInstance.SignInAnonymouslyAsync().ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInAnonymouslyAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                    return;
                }
                _user = task.Result;
                Debug.LogFormat("User signed in successfully: {0} ({1})", _user.DisplayName, _user.UserId);
            });
        }
        
        public void Logout()
        {
            //delete user
            FirebaseAuth.DefaultInstance.CurrentUser.DeleteAsync().ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("DeleteAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("DeleteAsync encountered an error: " + task.Exception);
                    return;
                }
                Debug.Log("User deleted successfully");
            });
            _user = null;
        }
    }
}
