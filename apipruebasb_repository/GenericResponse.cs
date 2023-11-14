using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apipruebasb_repository
{
    public class GenericResponse<T>
    {
        #region [ Properties ]

        public bool Success { get; set; }
        public T? Data { get; set; }
        // public Pager? Pager { get; set; }
        public Exception? Messages { get; set; }
        private List<string> notifications;

        public IReadOnlyList<string> Notifications => notifications.AsReadOnly();


        #endregion

        public GenericResponse()
        {
            Success = true;
            notifications = new List<string>();
        }

        public void SetFail(Exception messages)
        {
            Success = false;
            Messages = messages;
        }

        public void AddNotification(string message)
        {
            Success = false;
            notifications.Add(message);
        }

    }

}