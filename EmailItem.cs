using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmailClient
{
    public class EmailItem
    {
        public string host;
        public int port;
        public string username;
        public string password; 
        public string from;
        public string[] to;
        public string subject; 
        public string body;
        public ICollection<string> attachedFiles;

        public EmailItem(string host, int port, string username, string password, string from, string to, string subject, string body, ICollection<string> attachedFiles)
        {
            // TODO: Complete member initialization
            this.host = host;
            this.port = port;
            this.username = username;
            this.password = password;
            this.from = from;
            this.to = to.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            this.subject = subject;
            this.body = body;
            this.attachedFiles = attachedFiles;
        }
    }
}
