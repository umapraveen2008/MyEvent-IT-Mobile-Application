using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class DomainList : ContentView
    {
        private Command loadDomainCommand;

        public DomainList()
        {
            InitializeComponent();
            domainSearch.TextChanged += OnSearchTextChange;
            domainParent.RowHeight = 100;
            domainParent.RefreshCommand = LoadDomainCommand;
            domainParent.ItemTemplate = new DataTemplate(typeof(DomainTemplate));
        }



        public void OnSearchTextChange(object sender, EventArgs e)
        {            
            CreateDomain();
        }

        public Command LoadDomainCommand
        {
            get
            {
                return loadDomainCommand ?? (loadDomainCommand = new Command(PullToRefresh));
            }
        }

        public async void PullToRefresh()
        {
            //if (contactsParent.IsRefreshing)
            //  return;
            domainParent.IsRefreshing = true;
            SetDomainDetails(await App.serverData.SearchDomain(domainSearch.Text));
            domainParent.IsRefreshing = false;
        }

        public async void CreateDomain()
        {
            if (!string.IsNullOrEmpty(domainSearch.Text))
               SetDomainDetails(await App.serverData.SearchDomain(domainSearch.Text));
            else
                SetDomainDetails(new List<ServerDomain>());
           await ((HomeLayout)App.Current.MainPage).SetLoading(false, "Loading event sessions...");
        }

        public void SetDomainDetails(IList<ServerDomain> domains)
        {         
            List<ServerDomain> filterList = new List<ServerDomain>();           
            filterList = domains as List<ServerDomain>;
            filterList.RemoveAll(x => x == null);
            if (filterList.Count > 0)
            {
                filterList = new List<ServerDomain>(filterList.OrderBy(a => GetSort(a)));
                emptyList.IsVisible = false;
                domainParent.IsVisible = true;
            }
            else
            {
                domainParent.IsVisible = false;
                emptyList.IsVisible = true;
            }
            domainParent.RowHeight = 80;
            domainParent.ItemsSource = filterList;
        }

        public string GetSort(ServerDomain s)
        {
            if (s != null)
            {
                return s.domainName;
            }
            return string.Empty;
        }


    }
}
