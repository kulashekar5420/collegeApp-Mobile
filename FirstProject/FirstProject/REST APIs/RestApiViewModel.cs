using FirstProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace FirstProject.REST_APIs
{
    public class RestApiViewModel : INotifyBaseViewModel
    {
        private bool isNoDataVisible;
        private bool isRefreshing;

        public bool IsNoDataVisible
        {
            get { return isNoDataVisible; }
            set
            {
                if (isNoDataVisible != value)
                {
                    isNoDataVisible = value;
                    OnPropertyChanged(nameof(IsNoDataVisible));
                }
            }
        }
        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set
            {
                if (isRefreshing != value)
                {
                    isRefreshing = value;
                    OnPropertyChanged(nameof(IsRefreshing));
                }
            }
        }

        public RestApiViewModel() 
        {
            
        }
    }
}
