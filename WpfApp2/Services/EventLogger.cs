using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Services
{
    public class EventLogger : INotifyPropertyChanged
    {
        private ObservableCollection<string> _events;

        public ObservableCollection<string> Events
        {
            get => _events;
            set { _events = value; OnPropertyChanged(); }
        }

        public EventLogger()
        {
            Events = new ObservableCollection<string>();
        }

        public void AddEvent(string message)
        {
            Events.Insert(0, $"[{System.DateTime.Now:HH:mm:ss}] {message}");
            OnPropertyChanged(nameof(Events));
        }

        public void Clear()
        {
            Events.Clear();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
