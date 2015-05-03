/*
The MIT License (MIT)

Copyright (c) 2015 Oliver Scheel

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

 * @author    Oliver Scheel <bloggeroliver@web.de>
 * @copyright 2015 Oliver Scheel
 * @license   http://www.opensource.org/licenses/mit-license.html  MIT License
 * @link      http://csharp-tricks.blogspot.de/p/c-crypto-chat.html
 
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net;
using System.IO;
using System.Security.Cryptography;

namespace SimpleChatClient
{
  
    
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SimpleChatClient Client;
        bool LoggedIn = false;
        Dictionary<string, Chat> Chats = new Dictionary<string, Chat>(); // stores all active chats

        private void button1_Click(object sender, EventArgs e)
        {
            if (!LoggedIn)
            {
                Client = new SimpleChatClient();

                bool Login = (Client.Login(textBox1.Text, textBox2.Text));
                if (Login)
                {
                    // Login successful, enable chatting etc.
                    textBox3.Enabled = true;
                    button3.Enabled = true;
                    this.Height = 570;
                    tabControl1.Visible = true;
                    LoggedIn = true;
                    button1.Text = "Logout";
                    button2.Enabled = false;
                }
                else
                    MessageBox.Show("Login failed.");
            }
            else
            {
                // logout, disable chatting etc.
                Client = new SimpleChatClient();
                Chats = new Dictionary<string, Chat>();
                tabControl1.TabPages.Clear();
                textBox1.Text = "";
                textBox2.Text = "";
                button2.Enabled = true;
                textBox3.Enabled = false;
                button3.Enabled = false;
                this.Height = 176;
                tabControl1.Visible = false;
                LoggedIn = false;
                button1.Text = "Login";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Client = new SimpleChatClient();
            string Register = Client.Register(textBox1.Text, textBox2.Text);
            MessageBox.Show(Register);
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            ProvideChat(textBox3.Text);
        }

        private void ProvideChat(string name)
        {
            Chat Dummy;
            bool ChatExisting = Chats.TryGetValue(name, out Dummy);
            if (ChatExisting)
            {   // if chat already exists, make its tabpage active
                tabControl1.SelectedTab = Dummy.ChatPage;
                return;
            }
            else
            {
                // create new tabpage for the conversation
                TabPage NewPage = new TabPage(name);

                TextBox ChatWindow = new TextBox();
                ChatWindow.Left = 10;
                ChatWindow.Top = 10;
                ChatWindow.Width = 532;
                ChatWindow.Height = 180;
                ChatWindow.Multiline = true;
                ChatWindow.ScrollBars = ScrollBars.Vertical;
                ChatWindow.ReadOnly = false;
                NewPage.Controls.Add(ChatWindow);

                TextBox SendBox = new TextBox();
                SendBox.Left = 10;
                SendBox.Top = 200;
                SendBox.Width = 450;
                NewPage.Controls.Add(SendBox);
                SendBox.Name = "snd" + name;
                SendBox.Click += new EventHandler(SendBox_Click);
                SendBox.TextChanged += new EventHandler(SendBox_TextChanged);

                Button SendButton = new Button();
                SendButton.Left = 470;
                SendButton.Top = 200;
                SendButton.Text = "Send";
                SendButton.Name = "btn" + name;
                SendButton.Click += new EventHandler(SendButton_Click);
                NewPage.Controls.Add(SendButton);

                Chat NewChat = new Chat();
                NewChat.ChatWindow = ChatWindow;
                NewChat.SendBox = SendBox;
                NewChat.ChatPage = NewPage;
                NewChat.Partner = name;
                NewChat.SendButton = SendButton;

                NewPage.Name = "tpg" + name;
                tabControl1.SelectedIndexChanged += new EventHandler(tabControl1_SelectedIndexChanged);

                Chats.Add(name, NewChat);

                Client.Add(name);

                this.AcceptButton = NewChat.SendButton;

                if (tabControl1.InvokeRequired)
                {
                    tabControl1.Invoke(new Action(() =>
                    {
                        tabControl1.TabPages.Add(NewPage);
                        tabControl1.SelectedTab = NewPage;
                    }));
                }
                else
                {
                    tabControl1.TabPages.Add(NewPage);
                    tabControl1.SelectedTab = NewPage;
                }

                this.ActiveControl = NewChat.SendBox;
            }
        }

        private void tabControl1_SelectedIndexChanged(Object sender, EventArgs e)
        {
            // change tabpages / chats
            if (((TabControl)sender).SelectedIndex != -1)
            {
                string Receiver = ((TabControl)sender).TabPages[((TabControl)sender).SelectedIndex].Name.ToString().Substring(3, ((TabControl)sender).TabPages[((TabControl)sender).SelectedIndex].Name.ToString().Length - 3);
                this.AcceptButton = Chats[Receiver].SendButton;
                this.ActiveControl = Chats[Receiver].SendBox;
            }
        }

        private void SendBox_Click(Object sender, EventArgs e)
        {
            // click in textbox for sending
            string Receiver = ((TextBox)sender).Name.Substring(3, ((TextBox)sender).Name.Length - 3);
            Chat CurrentChat = Chats[Receiver];
            CurrentChat.NewMessage = false;
            CurrentChat.ChatPage.Text = CurrentChat.Partner;
        }

        private void SendBox_TextChanged(Object sender, EventArgs e)
        {
            string Receiver = ((TextBox)sender).Name.Substring(3, ((TextBox)sender).Name.Length - 3);
            Chat CurrentChat = Chats[Receiver];
            CurrentChat.NewMessage = false;
            CurrentChat.ChatPage.Text = CurrentChat.Partner;
        }

        private void SendButton_Click(Object sender, EventArgs e)
        {
            // send message
            string Receiver = ((Button)sender).Name.Substring(3, ((Button)sender).Name.Length - 3);
            Chat CurrentChat = Chats[Receiver];
            Client.Send(Receiver, CurrentChat.SendBox.Text);
            CurrentChat.ChatWindow.Text += Client.GetUsername() + ": " + CurrentChat.SendBox.Text + Environment.NewLine;
            CurrentChat.SendBox.Text = "";
            Chats[Receiver].ChatWindow.SelectionStart = Chats[Receiver].ChatWindow.TextLength;
            Chats[Receiver].ChatWindow.ScrollToCaret();
        }

        private void IncomingMessage(string NameSender, string message)
        {
            // rceive an incoming message and display it in the correct chat window
            ProvideChat(NameSender);

            Chats[NameSender].NewMessage = true;
            Chats[NameSender].ChatWindow.Invoke(new Action(() =>
            {
                Chats[NameSender].ChatWindow.Text += NameSender + ": " + message + Environment.NewLine;
                Chats[NameSender].ChatWindow.SelectionStart = Chats[NameSender].ChatWindow.TextLength;
                Chats[NameSender].ChatWindow.ScrollToCaret();
            }));
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // periodically poll messages from server
            if (Client != null)
            {
                List<SimpleChatClient.Message> Messages = Client.Receive();
                foreach (SimpleChatClient.Message m in Messages)
                {
                    IncomingMessage(m.Sender, m.Msg);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Height = 176;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            // timer used for the blinking effect for new messages
            foreach (Chat c in Chats.Values)
            {
                if (c.NewMessage)
                {
                    string BlankName = "";
                    BlankName = BlankName.PadLeft(c.Partner.Length, ' ');
                    if (c.ChatPage.Text == BlankName)
                        c.ChatPage.Text = c.Partner;
                    else
                        c.ChatPage.Text = BlankName;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex != -1)
            {
                string Current = tabControl1.TabPages[tabControl1.SelectedIndex].Name.ToString().Substring(3, tabControl1.TabPages[tabControl1.SelectedIndex].Name.ToString().Length - 3);
                tabControl1.TabPages.RemoveAt(tabControl1.SelectedIndex);
                Chats.Remove(Current);
            }
        }
    }

    public class Chat
    {
        public TextBox ChatWindow;
        public TextBox SendBox;
        public bool NewMessage = false;
        public TabPage ChatPage;
        public string Partner;
        public Button SendButton;
    }

    public class SimpleChatClient
    {
        public static class Crypto
        {
            static public byte[] RSAEncrypt(byte[] DataToEncrypt, RSAParameters RSAKeyInfo)
            {
                byte[] encryptedData;

                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSA.ImportParameters(RSAKeyInfo);

                encryptedData = RSA.Encrypt(DataToEncrypt, true);

                return encryptedData;
            }

            static public byte[] RSADecrypt(byte[] DataToDecrypt, RSAParameters RSAKeyInfo)
            {
                byte[] decryptedData;

                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSA.ImportParameters(RSAKeyInfo);

                decryptedData = RSA.Decrypt(DataToDecrypt, true);

                return decryptedData;
            }

            static public void CreateSymmetricKey(string password, string salt, out byte[] Key, out byte[] IV)
            {
                if (salt.Length < 8)
                    salt = salt.PadRight(8);

                Rfc2898DeriveBytes Generator = new Rfc2898DeriveBytes(password, System.Text.Encoding.UTF8.GetBytes(salt), 10000);
                Key = Generator.GetBytes(16);
                IV = Generator.GetBytes(16);
            }

            static public string  CreateRSA(string filename, string password, string folder)
            {
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                byte[] Key;
                byte[] IV;
                CreateSymmetricKey(password, filename, out Key, out IV);
                string RSAKey = RSA.ToXmlString(true);
                byte[] EncryptedKey = AESEncode(RSAKey, Key, IV);

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                if (File.Exists(folder + "/" + filename))
                    return "Existing";

                File.WriteAllBytes(folder + "/" + filename, EncryptedKey);
                return RSA.ToXmlString(false);
            }

            static public RSACryptoServiceProvider GetRSA(string filename, string password, string folder)
            {
                byte[] EncryptedKey = File.ReadAllBytes(folder + "/" + filename);
                byte[] Key;
                byte[] IV;
                CreateSymmetricKey(password, filename, out Key, out IV);
                string RSAKey = AESDecode(EncryptedKey, Key, IV);
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSA.FromXmlString(RSAKey);
                return RSA;
            }

            static public string AESDecode(byte[] encryptedBytes, byte[] key, byte[] IV)
            {
                Rijndael AESCrypto = Rijndael.Create();

                AESCrypto.Key = key;
                AESCrypto.IV = IV;

                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, AESCrypto.CreateDecryptor(), CryptoStreamMode.Write);

                cs.Write(encryptedBytes, 0, encryptedBytes.Length);
                cs.Close();

                byte[] DecryptedBytes = ms.ToArray();
                return System.Text.Encoding.UTF8.GetString(DecryptedBytes);
            }

            static public byte[] AESEncode(string plaintext, byte[] key, byte[] IV)
            {
                Rijndael AESCrypto = Rijndael.Create();
                AESCrypto.Key = key;
                AESCrypto.IV = IV;

                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, AESCrypto.CreateEncryptor(), CryptoStreamMode.Write);

                byte[] PlainBytes = System.Text.Encoding.UTF8.GetBytes(plaintext);
                cs.Write(PlainBytes, 0, PlainBytes.Length);
                cs.Close();

                byte[] EncryptedBytes = ms.ToArray();
                return EncryptedBytes;
            }
        }

        public class User
        {
            public string Username;
            public RSACryptoServiceProvider RSASend;

            public User(string username)
            {
                Username = username;
            }
        }

        public class Message
        {
            public string Sender;
            public string Msg;

            public Message(string sender, string message)
            {
                Sender = sender;
                Msg = message;
            }
        }

        User CurrentUser = null;
        CookieContainer Cookie = null;
        string ServerUrl = "http://bloggeroliver.bplaced.net/Chat/V4/";
        public RSACryptoServiceProvider RSAServer;

        List<User> ActiveChats = new List<User>();

        public string GetUsername()
        {
            return CurrentUser.Username;
        }

        private string HTTPPost(string url, string postparams)
        {
            string responseString = "";

            try
            {
                // performs the desired http post request for the url and parameters
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.CookieContainer = Cookie; // explicitely use the cookiecontainer to save the session

                string postData = postparams;
                byte[] data = Encoding.UTF8.GetBytes(postData);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();

                responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                return responseString;
            }
            catch
            {
                // MessageBox.Show("error" + url + postparams + responseString);
                return null;
            }
        }

        public string Register(string username, string password)
        {
            // register a new user
            if (HTTPPost(ServerUrl + "checkuser.php", "username=" + username) == "No")
            {
                string LoginPubKey = Crypto.CreateRSA(username, password, "users");
                if (LoginPubKey == "Existing")
                    return "Password file already existing on computer.";
                string SendPubKey = Crypto.CreateRSA(username, password, "send");

                return HTTPPost(ServerUrl + "register.php", "username=" + username + "&loginkey=" + Uri.EscapeDataString(LoginPubKey) + "&sendkey=" + Uri.EscapeDataString(SendPubKey));
            }
            else
                return "Already existing.";
        }

        public bool Login(string username, string password)
        {
            // login
            Cookie = new CookieContainer();
            string Challenge = HTTPPost(ServerUrl + "prelogin.php", "username=" + username);
            RSACryptoServiceProvider RSALogin = Crypto.GetRSA(username, password, "users");
            string ClearChallenge = Encoding.UTF8.GetString(Crypto.RSADecrypt(Convert.FromBase64String(Challenge), RSALogin.ExportParameters(true)));
            string Login = HTTPPost(ServerUrl + "login.php", "username=" + username + "&challenge=" + Uri.EscapeDataString(ClearChallenge));
            
            if (Login == "LoginBad")
            {
                Cookie = null;
                return false;
                
            }
            else
            {
                CurrentUser = new User(username);
                CurrentUser.RSASend = Crypto.GetRSA(username, password, "send");

                RSAServer = new RSACryptoServiceProvider();
                string[] LoginParts = Login.Split(new string[] { "<br />" }, StringSplitOptions.RemoveEmptyEntries);
                string ClearLogin = "";
                foreach (string s in LoginParts)
                {
                    ClearLogin += Encoding.UTF8.GetString(Crypto.RSADecrypt(Convert.FromBase64String(s), RSALogin.ExportParameters(true)));
                }
                RSAServer.FromXmlString(ClearLogin);

                return true;
            }
        }

        public void Logout()
        {
            // logout
            CurrentUser = null;
            Cookie = null;
        }

        public string Send(string recipient, string message)
        {
            string EncryptedMessage = "";
            foreach (User u in ActiveChats)
            {
                if (u.Username == recipient)
                {
                    EncryptedMessage = Uri.EscapeDataString(Convert.ToBase64String(Crypto.RSAEncrypt(Encoding.UTF8.GetBytes(message), u.RSASend.ExportParameters(false))));
                    break;
                }
            }
            return (HTTPPost(ServerUrl + "send.php", "Recipient=" + recipient + "&Message=" + (EncryptedMessage)));
        }

        public string GetSendKey(string user)
        {
            return (HTTPPost(ServerUrl + "getsendkey.php", "user=" + user));
        }

        public List<Message> Receive()
        {
            // receive messages
            if (CurrentUser == null)
                return new List<Message>();
            string Messages = HTTPPost(ServerUrl + "receive.php", "");
            if (Messages == null)
                return new List<Message>();

            // message format is: sender<br />message<br />, split regarding to this in single messages
            string[] Splits = Messages.Split(new string[] { "<br />" }, StringSplitOptions.None);
            List<Message> Received = new List<Message>();
            for (int i = 0; i < Splits.Length - 1; i += 2)
            {
                Received.Add(new Message(Splits[i], Encoding.UTF8.GetString(Crypto.RSADecrypt(Convert.FromBase64String(Splits[i + 1]), CurrentUser.RSASend.ExportParameters(true)))));
            }
            return Received;
        }

        public void Add(string name)
        {
            User NewUser = new User(name);
            RSACryptoServiceProvider NewRSA = new RSACryptoServiceProvider();
            NewRSA.FromXmlString(GetSendKey(name));
            NewUser.RSASend = NewRSA;
            ActiveChats.Add(NewUser);
        }
    }
}
