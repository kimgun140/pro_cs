using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp4
{
    /// <summary>
    /// SignUp.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SignUp : Page
    {
        public SignUp()
        {
            InitializeComponent();
        }
        public int Signup(string id, string pw, string name) // 고객 회원가입
        {
            try
            {
                List<string> aaaa = new List<string>();
                byte[] data;
                //Mainmenu.clients
                //TcpClient client = new TcpClient("10.10.21.111", 5558); //연결객체
                NetworkStream stream =Mainmenu.clients.GetStream(); //데이터 전송에 사용된 스트림
                //string message = "Hello from test";

                string send_msg;
                //data = Encoding.ASCII.GetBytes(aaaa[0]);
                //Console.WriteLine("-----");
                //Console.WriteLine(data);
                //Console.WriteLine("-----");
                //stream.Write(data, 0, data.Length);//전송할 데이터의 바이트 배열, 전송을 시작할 배열의 인덱스, 전송할 데이터의 길이.
                // 회원가입 플래그 보내기 
                send_msg = "_for_abc_signup_";
                data = null;
                data = Encoding.UTF8.GetBytes(send_msg);
                stream.Write(data, 0, data.Length);//전송할 데이터의 바이트 배열, 전송을 시작할 배열의 인덱스, 전송할 데이터의 길이.
                Thread.Sleep(100);
                send_msg = "";

                //이름 보내기
                data = null;
                data = new byte[256];
                send_msg = name;// 이름
                data = Encoding.UTF8.GetBytes(send_msg);
                stream.Write(data, 0, data.Length);//전송할 데이터의 바이트 배열, 전송을 시작할 배열의 인덱스, 전송할 데이터의 길이.
                send_msg = "";
                Thread.Sleep(100);
                //아이디 보내기
                data = null;
                data = new byte[256];
                send_msg = id; // id
                data = Encoding.UTF8.GetBytes(send_msg);
                stream.Write(data, 0, data.Length);//전송할 데이터의 바이트 배열, 전송을 시작할 배열의 인덱스, 전송할 데이터의 길이.
                send_msg = "";
                Thread.Sleep(100);
                //비밀번호 보내기
                data = null;
                data = new byte[256];
                send_msg = pw; // pw
                data = Encoding.UTF8.GetBytes(send_msg);
                stream.Write(data, 0, data.Length);//전송할 데이터의 바이트 배열, 전송을 시작할 배열의 인덱스, 전송할 데이터의 길이.
                send_msg = "";


                //_for_abc_login_
                // 채팅아니라서 주석처리 
                //회원가입을 하려면 3가지를 전송해야함 플래그 아이디 비번 이름 
                data = null;
                data = new byte[256];
                int bytes = stream.Read(data, 0, data.Length);//받는 데이터의 바이트배열, 인덱스, 길이
                string responses = Encoding.UTF8.GetString(data, 0, bytes);
                MessageBox.Show(responses);
                if (responses == "회원가입을 축하합니다.\n")
                {
                    NavigationService.Navigate(
                                            new Uri("/signup.xaml", UriKind.Relative));
                    return 1;
                }
                //txtbox_chat1.Text += ("Received: " + responses + "\n");

                //Thread.Sleep(100);


                //stream.Close();
                //client.Close();
                return -1;


            }
            catch (Exception e)
            {
                MessageBox.Show("Exception: " + e.Message);
                return -2;
            }
        }

        private void btn_login_Click(object sender, RoutedEventArgs e)
        {
            string id = MyTextBoxid.Text;
            string pw = MyTextBoxpw.Text;
            string name = MyTextBoxname.Text;
            int result;
            result = Signup(id, pw, name);// 성공하면 1 실패하면 -1 catch에 잡히면 -2
            //if (result == 1)
            //{
            //    MyTextBoxname.Clear();
            //    MyTextBoxpw.Clear();
            //    MyTextBoxid.Clear();
            //}
            //정보 입력하고 회원가입 버튼 누르면 서버로 데이터 전송되고 전송된 고객회원정보를 통해서 회원가입 로직 돌리고 회원가입 성공 실패 메세지를 고객 클라이언트에게 전달
            // 고객에게 해당 메세지를 출력해준다. 성공이면 성공 메세지 출력하고 페이지 로그인 페이지로 이동 
            // 실패면 실패 메세지 출력하고 다시 입력할 수 있게 기존에 입력 했던거 지우기  회원가입 페이지 다했네 그러면 좀있다가 합체시킬때 
        }

        public void MyTextBoxid_GotFocus(object sender, RoutedEventArgs e)
        {
            if (MyTextBoxid.Text == "ID")
            {
                MyTextBoxid.Clear();
            }

        }

        private void MyTextBoxpw_GotFocus(object sender, RoutedEventArgs e)
        {
            if (MyTextBoxpw.Text == "PassWord")
            {
                MyTextBoxpw.Clear();
            }

        }

        private void MyTextBoxname_GotFocus(object sender, RoutedEventArgs e)
        {
            if (MyTextBoxname.Text == "Name")
            {
                MyTextBoxname.Clear();
            }

        }

        private void MyTextBoxid_LostFocus(object sender, RoutedEventArgs e)
        {
            if (MyTextBoxid.Text == "")
            {
                MyTextBoxid.Text = "ID";
            }
        }

        private void MyTextBoxpw_LostFocus(object sender, RoutedEventArgs e)
        {
            if (MyTextBoxpw.Text == "")
            {
                MyTextBoxpw.Text = "PassWord";
            }
        }

        private void MyTextBoxname_LostFocus(object sender, RoutedEventArgs e)
        {
            if (MyTextBoxname.Text == "")
            {
                MyTextBoxname.Text = "Name";
            }
        }

    }
}
