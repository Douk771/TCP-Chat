using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CommonLib
{
    public class ObservableObject : INotifyPropertyChanged
    {
        /// <summary>
        /// Вызывается при изменении любого свойства
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Вызывает событие PropertyChanged
        /// </summary>
        /// <param name="propertyName">Имя свойства</param>
        protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// Проверяет является ли self и value равными и, если они не равны, self присваивается value. После присвоения вызывается OnPropertyChanged c параметром propertyName.
        /// </summary>
        /// <param name="self">Ссылка на объект которому будет присвоено value</param>
        /// <param name="value">Значение</param>
        /// <param name="propertyName">Имя свойства вызывающего этот метод</param>
        /// <returns>Было ли присвоено значение</returns>
        protected virtual bool OnPropertyChanged<T>(ref T self, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(self, value))
                return false;

            self = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
