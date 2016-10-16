using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;

namespace DimaCalc.ViewModels
{
    sealed class MainViewModel: INotifyPropertyChanged
    {

        #region Commands
        public ICommand CloseAppCommand { get; private set; }

        public void CloseApp()
        {
            App.Current.MainWindow.Close();
        }

        public void CloseHandler(object sender, EventArgs e)
        {
            App.Current.MainWindow.Close();
        }


        #endregion

        #region Bindable properties
        private string _statusText;

        /// <summary>
        /// Состояние, которое будет отражено в status bar
        /// </summary>
        public string StatusText
        {
            get { return _statusText; }
            set
            {
                if (_statusText != value)
                {
                    _statusText = value;
                    OnPropertyChanged("StatusText");
                }
            }
        }
        #endregion

        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        /// <remarks>
        /// В том числе, используется и самой студией в режиме дизайна окна
        /// </remarks>
        public MainViewModel()
        {
            CloseAppCommand = new SimpleCommand((x) => CloseApp());
            StatusText = "Ready";
        }

        #region INotifyPropertyChanged
        /// <summary>
        /// Вспомогательная функция для вызова события "изменение значения свойства"
        /// </summary>
        /// <param name="propName">Имя изменившегося свойства</param>
        private void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        /// <summary>
        /// Событие "изменение значения свойства". 
        /// Другие классы могут регистрировать подписку на него, присваивая обработчик события:
        /// PropertyChanged += new PropertyChangedEventHandler(_имя_обработчика)
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

    }
}
