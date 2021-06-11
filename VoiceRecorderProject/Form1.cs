using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using NAudio.Wave;

namespace VoiceRecorderProject
{
    public partial class Form1 : Form
    {
        int saatt, dakikaa, saniyee;//Geri sayım için gerekli değişkenlerimizi tanımladık.

        public Form1()
        {
            InitializeComponent();
        }
        [DllImport("winmm.dll")] //Gerekli kütüphaneyi import ettik.

        
        private static extern int mciSendString(string MciComando, string MciRetrno, int MciRetrnoLeng, int CallBack);
        //Kayıt için özel metot tanımladık. Extern sözcüğü değiştiricidir. Dışarıdan uygulanan bir yöntemi bildirmek için kullanılır.

        string record = "";
        /// <summary>
        /// buton1 ses kaydını başlatan butondur.button1 e tıklandığında ses kayıt işlemi, 
        /// butonların visible ve enabled yani görünürlük ve aktiflik özelliklerinde  düzenleme, kayıt sırasında gösterilecek 
        /// geçen süreyi görüntüleme, timerların işlem durumlarını belirleme,kaydettiğimiz bir sesi dinlerken kapatmak,kayıt sırasında 
        /// kullanıcıya gösterilecek görsel  amaçlı progresbar tanımlama işlemlerinin yapıldığı metottur.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            button2.Enabled = true;//button2nin(save tuşu) button1e(ses kaydını başlatma butonu) basıldığı zaman aktif(kullanılabilir) olmasını sağladık.
            mciSendString("close " + record, null, 0, 0);//yürütülen ses kaydının özel metot ile kapatılması.
            timer2.Stop();//timer2 durduruldu.
            label2.Text = "Duration: " + string.Format("{0:00}:{1:00}:{2:00}", 00, 00, 00);//Bir ses kaydı dinlediğimiz sırada ses kaydı başlatma(buton1) tuşuna 
                                                                                           //bastığımızda geri sayımın sıfırlanmasını sağladık.
            label2.Visible = false;  //Butonların görünürlükleri ve aktiflikleri düzenlendi.Örneğin buton1(ses kaydı başlatma butonu) e basınca 
            button4.Visible = true;  //buton3(play butonu) e ait butonlar gizleniyor.
            button4.Enabled = true;
            button5.Visible = false;
            button5.Enabled = false;
            button9.Visible = true;
            button6.Visible = false;
            button7.Visible = false;            
            button10.Visible = false;


            mciSendString("open new type waveaudio alias Som", null, 0, 0);//Ses kaydı için yönlendireceğimiz komutumuzu tanımladık.
            mciSendString("record Som", null, 0, 0);//kayıt başlattık
            label1.Text = "Recording...";//Ses kaydederken uygulama ekranında gösterilecek yazıyı belirttik..
            label1.ForeColor = System.Drawing.Color.Turquoise;
            timer1.Start();   //ses kayıt sırasında zaman göstermek için kullandığımız timer1 i ve stpwtch nesnemizin
            stpwtch.Start();  //çalışmasını başlattık.

            progressBar1.Style = ProgressBarStyle.Marquee;


        }

        /// <summary>
        /// Save butonuna(buton2) bastığımızda çalışacak olan metotumuz.Sesimizi wav uzantılı bir dosya olarak kaydetmeyi sağladık.Ses kayıt sırasında geçen zamanı 
        /// göstermesi için kullandığımız timer1 i durdurduk, stpwtch nesnemizi durdurup Reset() metoduyla ilk haline dönüştürdük.  
        /// SaveFileDialog sınıfının özelliklerini kullanabilmek için bir nesne oluşturduk ve dosyayı wave dosyası olarak kaydetmek 
        /// istediğimizi belirttik. Eğer açılan pencereye okey dersek dosyamız verilen isimle kaydedilecek yoksa kaydedilmeyecek ve ekranda 
        /// ses dosyamızın kaydedilmediğini bildiren bir mesaj görüntülenecek.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            button4.Visible = false; //buton2(save butonu) tıklandığında button1(ses kaydını başlatan button)e ait tuşların 
            button5.Visible = false; //gizlenmesini sağladık.
            button9.Visible = false;
            label1.Text = "Voice Recorder";
            progressBar1.Style = ProgressBarStyle.Blocks;
            timer1.Stop();
            stpwtch.Stop();
            stpwtch.Reset();
            lbl_timer.Text = "Duration: " + string.Format("{0:00}:{1:00}:{2:00}", 00, 00, 00);//Kayıt yaparken gösterilen zamanı sıfırladık.

            mciSendString("pause Som", null, 0, 0);//Ses kaydetmeyi duraklattık.
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "wave|*.wav";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                mciSendString("save Som " + saveFileDialog.FileName, null, 0, 0);//Kaydettiğimiz sesimizi dosya olarak kaydettik.
                mciSendString("close Som", null, 0, 0);
                //Ses kaydımızı kapattık,sonlandırdık.Kayıt işlemimizi durdurma şartı olmadan ses kaydımız sürerken save ile direkt kaydedebilmek için.


            }
            else
            {
                mciSendString("close Som", null, 0, 0);
                MessageBox.Show("KAYDEDİLMEDİ ", "Kayıt Durumu", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }


        }


        /// <summary>
        /// buton3 Play butonudur.Kaydet butonuna tıklandığı zaman eğer kayıt nesnemiz boşsa OpenFileDialog sınıfından oluşturduğumuz nesne üzerinden kaydettiğimiz ses kaydını oynatmak için 
        /// nesnemizi bir koşul ifadesine sokuyoruz. Eğer açılan pencerede ses kaydı seçildikten sonra "OK" butonuna tıklanırsa ses kaydımız oynatılmaya başlanacak.
        /// Kayıt nesnemiz boş değilse de aynı işlemler gerçekleştirilecek.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {   mciSendString("close " + record, null, 0, 0);//Hali hazırda play butonu yardımıyla bir ses kaydı dinlediğimiz sırada başka bir kayıt dinlemek için
                                                         // play tuşuna tekrar bastığımızda eski dinlenilen sesi kapattık.Ses dosyaları üst üste çalışmasın diye.
            label1.Text = "Voice Recorder";
            lbl_timer.Text = "Duration: " + string.Format("{0:00}:{1:00}:{2:00}", 00, 00, 00);
            //Ses kaydını başlatma butonu çalışırken gösterilen zamanı sıfırladık.
                                                                                              
            timer1.Stop();
            stpwtch.Stop();
            stpwtch.Reset();
            timer2.Stop();//Kayıt ettiğimiz bir sesi dinlerken geri sayım yapması için kullandığımız timer2 yi durdurduk.
            label2.Text = "Duration: " + string.Format("{0:00}:{1:00}:{2:00}", 00, 00, 00);//Geri sayım göserimini sıfırladık.
            button2.Enabled = false;//Play tuşuna basıldığında save butonunu kullanılamaz(inaktif) yaptık.          
            progressBar1.Style = ProgressBarStyle.Blocks;
            mciSendString("close Som", null, 0, 0);//Ses kaydederken Play tuşuna bastığımızda ses kayıt işleminin kapatılmasını sağladık.
            button4.Visible = false; //Button1 e ait tuşları gizledik.Playe basıldığında button6(durdurma butonu)yı görünürür ve aktif yaptık.
            button5.Visible = false; 
            button9.Visible = false;
            button6.Visible = true;
            button6.Enabled = true;
            button7.Visible = false;//Button7(devam et butonu)yi gizleyip inaktif yaptık çünkü button6 ya tıklandığında görünür olmasını istiyoruz.
            button7.Enabled = false;
            button10.Visible = true;//Kayıt dinlemeyi iptal etmeye yarayan butonu görünür yaptık.
            if (record == "")
            {
                
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "wave|*.wav";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    label2.Visible = true;//Kayıt dinlerken kaydın uzunluğundan geri sayımı gösteren labelı görünür yaptık.

                    record = openFileDialog.FileName;

                    TimeSpan a = GetWavFileDuration(record);//Dosyamızın ismini dosya uzunluğ ölçmek için kullandığımız metoda gönderdik
                                                            //ve geri dönüş değerini TimeSpan sınıfından türettiğimiz nesneye atadık.
                    int dakika = Convert.ToInt32(a.Minutes);               
                    int saat = Convert.ToInt32(Math.Floor(a.TotalHours)); //Geri dödürülen değerdeki toplam saat,dakika ve saniyeyi bulduk.
                    int saniye = Convert.ToInt32(a.Seconds);
                    
                    saniyee = saniye;
                    dakikaa = dakika;
                    saatt = saat;

                    timer2.Start();
                    timer2.Enabled = true; //geri sayım için kullandığımız timer2 ayarlarını yaptık.
                    timer2.Interval = 1000;//1 sn için ayarladık.
                    mciSendString("play " + record, null, 0, 0);//Kayıt dosyasını çalmaya başladık.
                    label1.ForeColor = System.Drawing.Color.Turquoise;
                    label1.Text = "Playing...";
                }

                else //Kayıt çalmaktan vazgeçtiğimiz durumda olacakları yazıyoruz.
                {
                    mciSendString("close " + record, null, 0, 0);//Ses dosyamızı kapattık.
                    label1.Text = "Voice Recorder";
                    button6.Visible = false;  //Durdur,devam ve iptal etme tuşları gizlendi.
                    button7.Visible = false;                
                    button10.Visible = false;
                }


            }
            else
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "wave|*.wav";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    
                    label2.Visible = true;
                    record = openFileDialog.FileName;

                    TimeSpan a = GetWavFileDuration(record);


                    int dakika = Convert.ToInt32(a.Minutes);
                    int saat = Convert.ToInt32(Math.Floor(a.TotalHours));
                    int saniye = Convert.ToInt32(a.Seconds);

                    saniyee = saniye;
                    dakikaa = dakika;
                    saatt = saat;

                    timer2.Start();
                    timer2.Enabled = true;
                    timer2.Interval = 1000;
                    mciSendString("play " + record, null, 0, 0);
                    label1.ForeColor = System.Drawing.Color.Turquoise;
                    label1.Text = "Playing...";
                }
                else
                {
                    label1.Text = "Voice Recorder";
                    button6.Visible = false;
                    button7.Visible = false;

                    button10.Visible = false;
                }
            }
           
            



        }

        /// <summary>
        /// Parametre olarak aldığı isme karşılık gelen ses dosyasının uzunluğunu veren metottur.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns> lengt.TotalTime</returns>
        public static TimeSpan GetWavFileDuration(string fileName)
        {
            WaveFileReader lengt = new WaveFileReader(fileName);
            return lengt.TotalTime;
        }
        private void lbl_timer_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// Form ilk açıldığında gerçekleşmesini istediğimiz,geçerli olmasını istediğimiz işlemleri bu metot içinde belirttik.
        /// Gözükmesini ve kullanilabilir olup olmamasını istediğimiz butonların özelliklerini
        /// ayarladık.Sadece ana butonlarımız buton1(kayıt başlatan) buton2(ses dosyasını bilgisayara kaydeden)
        /// buton3(ses oynatmayı sağlayan) butonlarımızın görünür olmasını sağladık.Ayrıca sadece buton2 yi inaktif yaptık.Çünkü buton2nin 
        /// buton1 e basıldığı taktirde kullanılabilir olmasını istedik.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            button4.Visible = false;   
            button4.Enabled = false;
            button5.Visible = false; 
            button5.Enabled = false;
            button6.Visible = false;
            button7.Visible = false;         
            button9.Visible = false;
            button10.Visible = false;
            button2.Enabled = false;
            label2.Visible = false;

        }
        System.Diagnostics.Stopwatch stpwtch = new System.Diagnostics.Stopwatch();
        //Stopwatch sınıfından bir nesne tanımladık  sayım yapıp zamanı ölçmek için.

        /// <summary>
        /// Geçen zamanı uygulamamızın içinde yazdırmak için timerin her tick durumunda gerçekleşmesini istedğimiz işlemleri"timer_Tick" 
        /// metodunun içine yazdık. TimeSpan sınıfının özelliklerini kullanmak için TimeSpan sınıfından nesnemizi oluşturduk. Daha sonra oluşturduğumuz
        /// nesneyi Stopwatch sınıfından oluşturduğumuz stpwtch nesnesi üzerindeki Elapsed özelliğine yani nesnemizin geçen zaman özelliğine eşitledik.
        /// Daha sonra uygulamamızdaki text e saat, dakika ve saniye formatında yazdırdık.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsedtime = stpwtch.Elapsed;
            lbl_timer.Text = "Duration: " + string.Format("{0:00}:{1:00}:{2:00}", Math.Floor(elapsedtime.TotalHours), elapsedtime.Minutes, elapsedtime.Seconds);

        }

        /// <summary>
        /// button4 buton1 e ait olan durdurma butonudur.Ses kaydetme tuşuna(button1) ait olan , ses kaydetme işlemini durduran butonun(button4) click metodudur.
        /// button4 e basınca buton4 görünmez ve inaktif oldu.button5(ses kaydetmeyi devam ettiren buton) görünür ve aktif oldu."pause Som"
        /// komutuyla ses kaydetme işlemi duraklatıldı.Kaydetme sırasında zaman ilerlemesini öğrenmek ve göstermek için kullandığımız
        /// timer1 in işlevi ve stpwtch nesnemiz durduruldu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            button4.Visible = false;
            button4.Enabled = false;
            button5.Visible = true;
            button5.Enabled = true;

            mciSendString("pause Som", null, 0, 0);
            timer1.Stop();
            stpwtch.Stop();
            timer1.Enabled = false;

            label1.Text = "Waiting...";

            progressBar1.Style = ProgressBarStyle.Blocks;//prograssBaeımızdaki ilerleme durmuş oldu.

        }
        /// <summary>
        /// button5 button1e ait olan devam ettirme butonudur. Bu metot buton5 e tıklandığında gerçekleşmesini istediğimiz olayları içerir.
        /// buton5 e tıklandığında buton5 gizlenir ve button4 görünür olup aktifleşir.Buton5 e tıkladığımızda ses kaydetmeye 
        /// kaldığımız yerden devam ederiz.stpwtch ve timer1i başlatarak zamanıda devam ettirmiş oluruz.progressBarın ilerleyişi başlar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            button4.Visible = true;
            button4.Enabled = true;
            button5.Visible = false;
            button5.Enabled = false;
            mciSendString("open new type waveaudio alias Som", null, 0, 0);
            mciSendString("record Som", null, 0, 0);
            label1.Text = "Recording...";//Ses kaydederken uygulamada yazmasını istediğimiz yazıyı belirttik.
            label1.ForeColor = System.Drawing.Color.Turquoise;

            stpwtch.Start();
            timer1.Start();
            timer1.Enabled = true;

            progressBar1.Style = ProgressBarStyle.Marquee;

        }
        /// <summary>
        /// button6 button3(play butonu)e ait olan durdurma butonudur.Bu metot buton6 ya tıklandığında gerçekleşmesini 
        /// istediğimiz olayları içerir.button6 gizlendi ve buton7 görünür olup aktifleştirildi.Ses kaydı oynatma sırasında geri sayım için
        /// kullandığımız timer2nin işlevi durduruldu.Dinliyor olduğumuz ses kaydı durduruldu.Ses kaydımız dururken kullanıcıya durumu bildirecek olan
        /// yazı belirtildi.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            button6.Visible = false;
            button6.Enabled = false;
            button7.Visible = true;
            button7.Enabled = true;

            
            timer2.Enabled = false;
            mciSendString("pause " + record, null, 0, 0);//Ses dosyasının çalmasını duraklatma işlemi.
            label1.Text = "Waiting...";

        }
        /// <summary>
        /// buton7 buton3e ait olan devam ettirme butonudur.Buton7 ye tıklandığında buton7nin gizlenip ve buton6 nın görünür olup aktifleşmesi
        /// sağlandı.Geri sayım için kullandığımız timer2 aktifleştirildi.Ses kaydımızın çalmaya devam etmesi sağlandı.Ses çalınırken 
        /// uygulamada "Playing..." yazısı gösterildi.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            button6.Visible = true;
            button6.Enabled = true;
            button7.Visible = false;
            button7.Enabled = false;
            
            timer2.Enabled = true;

            mciSendString("play " + record, null, 0, 0);
            label1.Text = "Playing...";


        }

        
        /// <summary>
        /// timer2 nin tick olayında gerçekleşmesini istediğimiz işlemleri içeren motuttur.timer2 ses oynatılırken kullanıcıya 
        /// çalan sesin uzunluğuna bağlı olarak bir geri sayım gösterilmesi için kullanıldı.İf else şart bloklarında koşullara bağlı
        /// olarak geri sayımın nasıl yaptırılacağı belirtildi.En son da geri sayımın saat dakika ve saniye cinsinden uygulama ekranında gösterilmesini 
        /// sağlayan kod yazıldı.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer2_Tick(object sender, EventArgs e)
        {
            
            
            saniyee -= 1;
            if (saniyee == 00)
            {
                if (dakikaa != 00)
                {
                    dakikaa--;
                    saniyee = 60;
                }
                else
                {
                    if (saatt != 00)
                    {
                        saatt--;
                        dakikaa = 60;
                        saniyee = 60;
                    }
                    else
                    {
                        timer2.Stop();                        //Süre sonlandığında gerçekleşmesini istediğimiz olayları belirttik.
                        button6.Enabled = false;                        
                        button7.Enabled = false;
                        label1.Text = "Voice Recorder";

                    }
                }
            }
            label2.Text = "Duration: " + string.Format("{0:00}:{1:00}:{2:00}", (saatt).ToString("00"), (dakikaa).ToString("00"), (saniyee).ToString("00"));
                

        }

        /// <summary>
        /// buton9 buton1e ait olan ses kaydetme işlemini iptal ettirme butonudur.Bu metot buton9a tıklanıldığında 
        /// gerçekleşecek işlemleri içerir. buton1e ait olan durdurma butonu(buton4) inaktif ve görünür yapıldı;  
        /// devam ettir butonu(button5) hem inaktif hem görünmez yapıldı.Ses kayıt sırasında geçen zamanı 
        /// göstermesi için kullandığımız timer1 i durdurduk, stpwtch nesnemizi durdurup Reset() özelliğiyle ilk haline döndürdük.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button9_Click(object sender, EventArgs e)
        {
            button4.Visible = true;
            button4.Enabled = false;
            button5.Visible = false;
            button5.Enabled = false;
            button2.Enabled = false;

            mciSendString("close Som", null, 0, 0);//Ses kayıt işlemi kapatıldı.
            label1.Text = "Voice Recorder";
            progressBar1.Style = ProgressBarStyle.Blocks;
            timer1.Stop();
            stpwtch.Stop();
            stpwtch.Reset();
            lbl_timer.Text = "Duration: " + string.Format("{0:00}:{1:00}:{2:00}", 00, 00, 00);//Zaman göstergesini sıfırladık.
        }

        
        /// <summary>
        /// buton10 buton3e ait olan ses kaydı dinlemeyi iptal etme butonudur.timer2 durduruldu.label2 geri sayımı göteren labeldır.label2 görünmez yapıldı.
        /// Label2 nin gösterdiği süre sıfırlandı.durdurma(buton6) butonu görünür,inaktif;devam (buton7) butonu görünmez ve inaktif yapıldı.
        /// Çalmakta olan ses kapatıldı.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button10_Click(object sender, EventArgs e)
        {
            
            timer2.Stop();
            label2.Visible = false;
            label2.Text = "Duration: " + string.Format("{0:00}:{1:00}:{2:00}", 00, 00, 00);
            button6.Visible = true;
            button6.Enabled = false;
            button7.Visible = false;
            button7.Enabled = false;

            mciSendString("close " + record, null, 0, 0);
            label1.Text = "Voice Recorder";//Ses çalmıyorken label1 de yazmasını istediğimiz yazıyı belirttik.
            timer1.Stop();
            stpwtch.Stop();
            stpwtch.Reset();
            lbl_timer.Text = "Duration: " + string.Format("{0:00}:{1:00}:{2:00}", 00, 00, 00);
        }
       
    }//Projeyi Yapanlar: Melike ÇELİKEL 201913709038 ve Osman Erdem KILIÇ 201913709005
}
