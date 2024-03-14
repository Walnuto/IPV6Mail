using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Timers;
using System.Threading;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using System.Net.Mail;

namespace MailService
{
    public partial class Service1 : ServiceBase
    {
        private string smtpaddr = ConfigurationManager.AppSettings["smtpaddr"];
        private int smtpport = Convert.ToInt32(ConfigurationManager.AppSettings["smtpport"]);
        private string username = ConfigurationManager.AppSettings["username"];
        private string password = ConfigurationManager.AppSettings["password"];
        private string mailfrom = ConfigurationManager.AppSettings["mailfrom"];
        private string mailrcpt = ConfigurationManager.AppSettings["mailrcpt"];
        private int interval = Convert.ToInt32(ConfigurationManager.AppSettings["interval"]);
        private System.Threading.Timer timer;
        private string ipv6last = string.Empty;
        public Service1()
        {
            InitializeComponent();
        }
        private string GetIpv6Address()
        {
            string strIpv6 = string.Empty;
            string name = Dns.GetHostName();
            IPAddress[] ipadrlist = Dns.GetHostAddresses(name);
            foreach (IPAddress ipa in ipadrlist)
            {
                string ip = ipa.ToString();
                if ((ipa.AddressFamily == AddressFamily.InterNetworkV6) &&
                    (!ip.StartsWith("fe80")) && 
                    (!ip.StartsWith("::1")))
                {
                    strIpv6 = strIpv6 + "[" + ip + "]    ";
                    strIpv6 = strIpv6 + ip.Replace(':','-') + ".ipv6-literal.net\n";
                }
            }
            return strIpv6;
        }
        private bool SendMail(string text)
        {
            MailAddress from = new MailAddress(mailfrom);
            MailAddress to = new MailAddress(mailrcpt);
            MailMessage mailMessage = new MailMessage(from, to);
            mailMessage.Subject = "IPV6";
            mailMessage.SubjectEncoding = Encoding.UTF8;
            mailMessage.Body = text;
            mailMessage.BodyEncoding = Encoding.UTF8;
            try
            {
                SmtpClient smtpClient = new SmtpClient(smtpaddr, smtpport);
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(username, password);
                smtpClient.EnableSsl = true;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Send(mailMessage);
                return true;
            }
            catch (FormatException ex)
            {
                return false;
            }
            catch (SmtpException ex2)
            {
                return false;
            }
        }
        private void TimerProc(object state)
        {
            string ip = GetIpv6Address();
            if (!ip.Equals(ipv6last))
            {
                if (SendMail(ip))
                {
                    ipv6last = ip;
                }
            }
        }
        protected override void OnStart(string[] args)
        {
            timer = new System.Threading.Timer(new TimerCallback(TimerProc));
            timer.Change(0,interval*1000);
        }

        protected override void OnStop()
        {
            timer.Dispose();
        }
    }
}
