using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp4
{
    /// <summary>
    /// Choice.xaml에 대한 상호 작용 논리
    /// </summary>
    public class Liv_info_binding
    {
        public string statnNm {  get; set; }
        public string updnLine { get; set; }
        public string statnTnm { get; set; }
        public string trainSttus { get; set; }
        public string directAt { get; set; }
        public string lstcarAt { get; set; }
    } 
    public partial class Choice : Page
    {
        static readonly HttpClient API = new HttpClient();
        //List<JObject> list_jobject = new List<JObject>();
        List<Liv_info_binding> str_list_jobject = new List<Liv_info_binding>();
        List<string> ment1 = new List<string>()
        {
            "1호선",
            "2호선",
            "3호선",
            "4호선",
            "5호선",
            "6호선",
            "7호선",
            "8호선",
            "9호선",
            "수인분당선",
            "신분당선",
            "경춘선",
            "서해선"
        };
        byte[] data = new byte[256];
        public Choice()
        {
            InitializeComponent();
            cb_1.ItemsSource = ment1;
            //cb_1.SelectedIndex = 0;
        }
        public async void info(string selecteditem)
        {
            await Console.Out.WriteLineAsync(selecteditem);
            str_list_jobject.Clear();//리스트뷰에 들어갈 배열 초기화
            
            //서울시 지하철 실시간 열차 위치정보 (json으로 파싱한거) https://www.data.go.kr/data/15058569/openapi.do
            string url = "http://swopenapi.seoul.go.kr/api/subway/65724277537568763738636a587a61/json/realtimePosition/";
            url += "0/";
            url += "100/";
            url += selecteditem;

            // API에 HTTP GET 요청을 보내고 응답을 받음
            HttpResponseMessage response = await API.GetAsync(url);

            // 응답이 성공적인지 확인
            if (response.IsSuccessStatusCode)
            {
                // JSON 형식의 응답 데이터를 문자열로 읽어옴
                string jsonString = await response.Content.ReadAsStringAsync();

                // 문자열로 읽어온 JSON 데이터를 JObject로 파싱
                JObject jObject = JObject.Parse(jsonString);

                //Console.WriteLine(jObject["realtimePositionList"]); //다뽑기

                // 파싱된 데이터에서 recptnDt 필드 값만 추출하여 출력
                JArray realtimePositionList = (JArray)jObject["realtimePositionList"];
                
                if(realtimePositionList != null)
                {
                    foreach (JObject item in realtimePositionList)
                    {
                        Liv_info_binding obje = new Liv_info_binding();
                        obje.statnNm = Convert.ToString(item["statnNm"]);// 현재위치 추가
                        obje.updnLine = Convert.ToString(item["updnLine"]) == "0" ? "상행" : "하행"; // 상행/하행 추가
                        obje.statnTnm = Convert.ToString(item["statnTnm"]); //역 이름 추가(도착역)
                        switch (Convert.ToString(item["trainSttus"])) // 상태 추가
                        {
                            case "0":
                                obje.trainSttus = "진입";
                                break;
                            case "1":
                                obje.trainSttus = "도착";
                                break;
                            case "2":
                                obje.trainSttus = "출발";
                                break;
                            case "3":
                                obje.trainSttus = "전역출발";
                                break;
                            default:
                                obje.trainSttus = "";
                                break;
                        }
                        switch (Convert.ToString(item["directAt"]))// 급행/완행/특급 추가
                        {
                            case "1":
                                obje.directAt = "급행";
                                break;
                            case "0":
                                obje.directAt = "완행";
                                break;
                            case "7":
                                obje.directAt = "특급";
                                break;
                            default:
                                obje.directAt = "";
                                break;
                        }
                        obje.lstcarAt = Convert.ToString(item["lstcarAt"]) == "1" ? "O" : "X";//막차 확인
                        str_list_jobject.Add(obje);
                    }
                    liv_info.ItemsSource = str_list_jobject;
                    liv_info.Items.Refresh();
                }
                else
                {
                    Liv_info_binding obje = new Liv_info_binding();
                    
                    obje.statnNm = "Anything.";
                    str_list_jobject.Add(obje);
                    liv_info.ItemsSource = str_list_jobject;
                    liv_info.Items.Refresh();
                }

            }
            else
            {
                // 응답이 실패한 경우 오류 메시지 출력
                liv_info.ItemsSource ="Failed to retrieve subway position data.";
            }


        }

        // GridViewColumnHeader 클릭 시 마지막으로 클릭된 헤더를 저장하는 변수
        private GridViewColumnHeader _lastHeaderClicked = null;

        // 마지막으로 사용된 정렬 방향을 저장하는 변수
        private ListSortDirection _lastDirection = ListSortDirection.Ascending;
        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            // 이벤트의 원본이 GridViewColumnHeader인지 확인
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;

            // headerClicked가 null이 아닌 경우 (즉, GridViewColumnHeader가 클릭된 경우)
            if (headerClicked != null)
            {
                // 마지막으로 클릭된 헤더와 현재 클릭된 헤더가 다르면 정렬 방향을 오름차순으로 설정
                if (headerClicked != _lastHeaderClicked)
                {
                    direction = ListSortDirection.Ascending;
                }
                else
                {
                    // 마지막 정렬 방향이 오름차순이면 내림차순으로, 내림차순이면 오름차순으로 변경
                    if (_lastDirection == ListSortDirection.Ascending)
                    {
                        direction = ListSortDirection.Descending;
                    }
                    else
                    {
                        direction = ListSortDirection.Ascending;
                    }
                }
                // 클릭된 헤더의 컬럼에 바인딩된 데이터를 가져옴
                var columnBinding = headerClicked.Column.DisplayMemberBinding as Binding;
                // 바인딩된 데이터가 있으면 그 경로를, 없으면 헤더 문자열을 사용하여 정렬 기준을 설정
                var sortBy = columnBinding?.Path.Path ?? headerClicked.Column.Header as string;

                // 정렬을 수행하는 메서드 호출
                Sort(sortBy, direction);

                // 마지막으로 클릭된 헤더와 정렬 방향을 저장
                _lastHeaderClicked = headerClicked;
                _lastDirection = direction;
            }
        }
        private void Sort(string sortBy, ListSortDirection direction)
        {
            // 정렬을 수행하는 메서드
            ICollectionView dataView = CollectionViewSource.GetDefaultView(liv_info.ItemsSource);

            // 이전 정렬 조건을 모두 제거
            dataView.SortDescriptions.Clear();

            // 새로운 정렬 조건을 추가
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);

            // 데이터를 새로고침하여 정렬된 결과를 반영
            dataView.Refresh();
        }
        private void cb_1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            info(Convert.ToString(cb_1.SelectedItem));
        }
        private void btn_go_CS_Click(object sender, RoutedEventArgs e)
        {
            NetworkStream stream = Mainmenu.clients.GetStream(); //데이터 전송에 사용된 스트림
            data = null;
            data = Encoding.UTF8.GetBytes("_for_go_cs_");
            stream.Write(data, 0, data.Length);//전송할 데 이터의 바이트 배열, 전송을 시작할 배열의 인덱스, 전송할 데이터의 길이.

            byte[] data1 = new byte[256];
            int bytes = stream.Read(data1, 0, data1.Length);//받는 데이터의 바이트배열, 인덱스, 길이
            string responses = Encoding.UTF8.GetString(data, 0, bytes);
            stream.Flush();
            if (responses == "_for_go_cs_")
            {
                NavigationService.Navigate(new Uri("/CS_CC.xaml", UriKind.Relative));
            }
        }
        private void btn_first_station_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("05시 30분\n(상황에 따라 유동적으로 변할 수 있음)", "첫 차 정보", MessageBoxButton.OK, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {

            }
            else
            {
                //현재 창 유지
            }
        }

        private void btn_last_btn_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("00시 05분\n(상황에 따라 유동적으로 변할 수 있음)", "막차 정보", MessageBoxButton.OK, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {

            }
            else
            {
                //현재 창 유지
            }
        }

        private void btn_anything_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("실시간 상황을 반영하고 있습니다.\n아직 운행중인 열차가 없거나, 공공데이터포털 내부적으로 문제가있을 수 있습니다. 잠시 후에 다시 시도주세요.", "", MessageBoxButton.OK, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {

            }
            else
            {
                //현재 창 유지
            }
        }
    }
}
