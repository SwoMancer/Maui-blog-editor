using MauiAppEditBlog.DB;
using MauiAppEditBlog.ViewModels;
using System.Reflection;

namespace MauiAppEditBlog
{

    public partial class MainPage : ContentPage
    {
        private bool _isNewlyCreated;
        private MySqlConnectorRetriever _connectorRetriever;
        private TabelsViewModel _tabelsVM;
        public MainPage()
        {
            InitializeComponent();
            _connectorRetriever = new MySqlConnectorRetriever();
            _tabelsVM = new TabelsViewModel(_connectorRetriever.ConnectionString);
            BindingContext = _tabelsVM;
            _isNewlyCreated = true;
        }

        private async void ReloadCardsButton_Clicked(object sender, EventArgs e)
        {
            await _tabelsVM.PopulateDataFromDatabaseAsync();
        }

        private async void CardsListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            CardViewModel card = e.Item as CardViewModel;
            await Navigation.PushAsync(new CardPage(card, CardOperation.Update));
        }

        private async void AddCardButton_Clicked(object sender, EventArgs e)
        {
            CardViewModel cardViewModel = new CardViewModel(_connectorRetriever.ConnectionString);
            _tabelsVM.Cards.Add(cardViewModel);
            CardPage cardPage = new CardPage(cardViewModel, CardOperation.Create);
            await Navigation.PushAsync(cardPage);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _tabelsVM.ActiveStateAsync();
            _tabelsVM.UpdateLocalCards();
            await FirstTimeAppering();
        }

        private async Task FirstTimeAppering()
        {
            if (_isNewlyCreated)
            {
                _isNewlyCreated = false;
                await _tabelsVM.PopulateDataFromDatabaseAsync();
            }
        }

        protected override async void OnDisappearing() 
        { 
            base.OnDisappearing();
            await _tabelsVM.SlumberStateAsync();
        }

    }

}