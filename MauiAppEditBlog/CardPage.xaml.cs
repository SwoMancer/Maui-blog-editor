using MauiAppEditBlog.ViewModels;

namespace MauiAppEditBlog
{
    public enum CardOperation { Create, Update }

    public partial class CardPage : ContentPage
	{
		private CardViewModel _card;
		private CardOperation _operation;
        public CardPage(CardViewModel card, CardOperation operation)
		{
			InitializeComponent();

            _operation = operation;

            //If the card is new, prepare the button
            
            if (operation == CardOperation.Create)
            {
                card.Date = DateTime.Now;
                SaveCardButton.Text = "Add the new card";
                DeleteCardButton.IsVisible = false;
            }

            _card = card;
            BindingContext = _card;
            ValidateInputForEnablingButton();
        }
        private async void SaveAndUpdateCardButton_Clicked(object sender, EventArgs e)
        {
            //Create a new card or update a existing card
            switch (_operation)
            {
                case CardOperation.Create:
                    await _card.CreateAsync();
                    break;
                case CardOperation.Update:
                    await _card.UpdateAsync();
                    break;
                default:
                    break;
            }

            //The card exists and should be updated instead
            _operation = CardOperation.Update;
			await Navigation.PopAsync();
            DeleteCardButton.IsVisible = true;
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _card.ActiveStateAsync();
            //If the card already exists, find its links
            await _card.LoadLinksAsync();
        }
        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            await _card.SlumberStateAsync();
        }
        private async void DeleteCardButton_Clicked(object sender, EventArgs e)
        {
            await _card.DeleteAsync();
            await Navigation.PopAsync();
        }        
        private void ValidateInputForEnablingButton()
        {
            if (string.IsNullOrEmpty(TextEditor.Text) || string.IsNullOrEmpty(TitleEditor.Text))
            {
                SaveCardButton.IsEnabled = false;  
            }
            else
            {
                SaveCardButton.IsEnabled = true;
            }
        }
        private void TitleEditor_TextChanged(object sender, TextChangedEventArgs e) => ValidateInputForEnablingButton();
        private void TextEditor_TextChanged(object sender, TextChangedEventArgs e) => ValidateInputForEnablingButton();
    }
}