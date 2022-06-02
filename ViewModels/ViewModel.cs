using Microsoft.Win32;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace RPG_MV_MZ_Decoder
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        #region Поля
        /// <summary>
        /// Текст выводимый в лог.
        /// </summary>
        private string text;
        /// <summary>
        /// Указать и декодировать проект
        /// </summary>
        private RelayCommand? decriptcomm;
        #endregion

        #region Свойства
        /// <summary>
        /// Текст выводимый в лог.
        /// </summary>
        public string Text
        {
            get => text;
            set
            {
                text = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Text)));
            }
        }
        #endregion

        #region Команды
        /// <summary>
        /// Указать и декодировать проект
        /// </summary>
        public RelayCommand DecriptComm => decriptcomm ?? (decriptcomm = new RelayCommand(obj => Decript()));
        #endregion

        #region Авто-Свойства
        /// <summary>
        /// Коллекция потоков для дешифровки.
        /// </summary>
        private List<Task> LO { get; set; }
        /// <summary>
        /// Коллекция файлов для дешифровки.
        /// </summary>
        private List<string> DecFiles { get; set; }
        /// <summary>
        /// Окно программы.
        /// </summary>
        private MainWindow MainWin { get; set; }
        /// <summary>
        /// Счётчик.
        /// </summary>
        private int Count { get; set; }
        #endregion

        /// <summary>
        /// Конструктор.
        /// </summary>
        public ViewModel(MainWindow mainWin)
        {
            MainWin = mainWin;
        }

        #region Методы
        /// <summary>
        /// Указать и декодировать проект
        /// </summary>
        private void Decript()
        {
            OpenFileDialog OFD = new OpenFileDialog()
            {
                FileName = "Game.exe",
                Filter = "(*.exe)|*.exe",
                Title = "Укажите файл игры"
            };
            if (OFD.ShowDialog() == true)
            {
                DecFiles = new List<string>();
                LO = new List<Task>();
                DecodeProject(OFD.FileName);
            }
        }
        /// <summary>
        /// Расшифровать проект.
        /// </summary>
        public async void DecodeProject(string file)
        {
            string dir = Path.GetDirectoryName(file);
            DecFiles.Clear();
            LO.Clear();
            if (Directory.Exists($"{dir}\\www")) dir = $"{dir}\\www";
            Decriptor.SystemBackupText = File.ReadAllText($"{dir}\\data\\System.json");
            Decriptor.GetEncode();
            MainWin.Hide();
            if (Decriptor.encriptcode.Length > 0)
            {
                ReadDirectories(Directory.GetDirectories($"{dir}\\audio"));
                ReadDirectories(Directory.GetDirectories($"{dir}\\img"));
                if (DecFiles.Count > 0)
                {
                    LogWin logWin = new LogWin();
                    logWin.DataContext = new LogVM(this);
                    logWin.Show();
                    Count = 0;
                    await Task.Run(() => Waiting());
                    string temp = File.ReadAllText($"{dir}\\data\\System.json");
                    temp = temp.Substring(0, temp.IndexOf(",\"hasEncrypted")) + "}";
                    File.WriteAllText($"{dir}\\data\\System.json", temp);
                    logWin.Close();
                    MessageBox.Show($"Дешифровано {DecFiles.Count} файлов");
                }
                else { MessageBox.Show($"Не найдено зашифрованных файлов."); }
            }
            else { MessageBox.Show($"Отсутствует ключ шифрования."); }
            Application.Current.Shutdown();

        }
        /// <summary>
        /// Дешифровка файлов в многопоточном режиме.
        /// </summary>
        private void Waiting()
        {
            for (int i = 0; i < DecFiles.Count; i++)
            {
                int idx = i;
                LO.Add(Task.Factory.StartNew(() => DecriptById(idx)));
            }
            Task.WaitAll(LO.ToArray());
        }
        /// <summary>
        /// Поиск файлов для дешифровки по папкам.
        /// </summary>
        /// <param name="dirs">Папка для поиска</param>
        private void ReadDirectories(string[] dirs)
        {
            for (int i = 0; i < dirs.Length; i++)
            {
                string[] files = Directory.GetFiles(dirs[i]);
                for (int j = 0; j < files.Length; j++)
                {
                    if (files[j].ToLower().Contains(".rpgmvo") || files[j].Contains(".rpgmvp") || files[j].EndsWith("_"))
                    {
                        DecFiles.Add(files[j]);
                    }
                }
                ReadDirectories(Directory.GetDirectories(dirs[i]));
            }
        }
        /// <summary>
        /// Дешифровка файла.
        /// </summary>
        /// <param name="id">id файла</param>
        private void DecriptById(int id)
        {
            Decriptor.Decriptfile(DecFiles[id]);
            Count++;
            Text = $"{Count} из {DecFiles.Count}";
        }
        #endregion
    }
}


