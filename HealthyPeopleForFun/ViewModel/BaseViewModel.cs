using System.ComponentModel;

namespace HealthyPeopleForFun.ViewModel
{
	public class BaseViewModel : INotifyPropertyChanged
    {
        #region Properties
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion Properties

        #region Functions
        /// <summary>
        /// Triggeres the PropertyChanged event
        /// </summary>
        /// <param name="propertyName">Name of the property that's changed</param>
        public void Notify(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion Functions
    }
}
