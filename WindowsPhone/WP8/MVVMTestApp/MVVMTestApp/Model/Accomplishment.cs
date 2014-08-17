using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMTestApp.Model
{
    class Accomplishment : INotifyPropertyChanged
    {
        // The name of the accomplishment.
        public string Name { get; set; }

        // The type of the accomplishment, Item or Level.
        public string Type { get; set; }

        // The number of each item that has been collected.
        private int _count;
        public int Count
        {
            get
            {
                return _count;
            }
            set
            {
                _count = value;
                RaisePropertyChanged("Count");
            }
        }

        // Whether a level has been completed or not
        private bool _completed;
        public bool Completed
        {
            get
            {
                return _completed;
            }
            set
            {
                _completed = value;
                RaisePropertyChanged("Completed");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        // Create a copy of an accomplishment to save.
        // If your object is databound, this copy is not databound.
        public Accomplishment GetCopy()
        {
            Accomplishment copy = (Accomplishment)this.MemberwiseClone();
            return copy;
        }
    }
}
