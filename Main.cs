using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Future_Predictor
{
    public partial class Main : Form
    {
        private const string APP_NAME = "Предсказатель будущего";

        // Путь будет актуальным, куда бы мы не поместили ехе файл
        private readonly string PREDICTIONS_CONFIG_PATH = $"{Environment.CurrentDirectory}\\predictionsConfig.json";

        private string[] _predictions;

        private Random _random = new Random();

        public Main()
        {
            InitializeComponent();
        }

        private async void bPredict_Click(object sender, EventArgs e)
        {
            // Выключение кнопки, чтобы нельзя было запустить новое предсказание, пока не покажется текущее
            bPredict.Enabled = false;

            // Анимация загрузки. Используется отдельный поток, чтобы UI не зависал при анимации
            await Task.Run(() =>
            {
                for (int i = 1; i <= 100; i++)
                {
                    // Используется Invoke, чтобы модифицировать элементы UI из другого потока, иначе выбросит исключение
                    Invoke(new Action(() =>
                    {
                        // Танцы с бубном, чтоюы сообщение вывелось ТОЛЬКО после встроенной от винды анимации прогресс бара
                        if (i == progressBar1.Maximum) 
                        {
                            progressBar1.Maximum = i + 1;
                            progressBar1.Value = i + 1;
                            progressBar1.Maximum = i;
                        }
                        else 
                        {
                            progressBar1.Value = i + 1;
                        }

                        progressBar1.Value = i;

                        // Отображение прогресса загрузки в %
                        Text = $"{i}%";
                    }));
                    Thread.Sleep(10);
                }
            });

            // Генерация предсказания
            var index = _random.Next(_predictions.Length);
            var prediction = _predictions[index];

            // Вывод предсказания
            MessageBox.Show(prediction);

            // Сброс значений для следующего предсказания
            progressBar1.Value = 0;
            Text = APP_NAME;
            bPredict.Enabled = true;
        }

        private void Main_Load(object sender, EventArgs e)
        {
            Text = APP_NAME;

            // Считываем текста предсказаний при запуске
            try
            {
                var data = File.ReadAllText(PREDICTIONS_CONFIG_PATH);
                _predictions = JsonConvert.DeserializeObject<string[]>(data);
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
            finally 
            { 
                if (_predictions is null) 
                {
                    Close();
                }
                else if (_predictions.Length == 0) 
                {
                    MessageBox.Show("Предсказания закончились, кина не будет! =)");
                    Close();
                }
            }
        }
    }
}
