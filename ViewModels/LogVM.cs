using System.ComponentModel;

namespace RPG_MV_MZ_Decoder
{
    public class LogVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Поля
        /// <summary>
        /// Модель с данными для вывода
        /// </summary>
        private ViewModel sourcemodel;
        /// <summary>
        /// Выводимое сообщение
        /// </summary>
        private string text;
        #endregion

        #region Свойства
        /// <summary>
        /// Модель с данными для вывода
        /// </summary>
        public ViewModel SourceModel
        {
            get => sourcemodel;
            set
            {
                sourcemodel = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SourceModel)));
            }
        }
        /// <summary>
        /// Выводимое сообщение
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

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="model">Модель с данными для вывода</param>
        public LogVM(ViewModel model)
        {
            SourceModel = model;
        }
    }
}


