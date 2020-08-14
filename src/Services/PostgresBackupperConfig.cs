using System;

namespace WebUI.Services
{
    /// <summary>
    /// PostgresBackupper config properties
    /// </summary>
    public class PostgresBackupperConfig
    {
        public string Host { get; set; }
        
        public string Port { get; set; }
        
        public  string Username { get; set; }
        
        public string Password { get; set; }

        public PostgresBackupperConfig()
        {
            
        }

        public PostgresBackupperConfig(string host, string port, string username, string password)
        {
            Host = host ?? throw new ArgumentNullException(nameof(host));
            Port = port ?? throw new ArgumentNullException(nameof(port));
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Password = password ?? throw new ArgumentNullException(nameof(password));
        }                      
    }
}