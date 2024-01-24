using Xamarin.Forms;

namespace FirstProject.REST_APIs
{
    public class EmptyValidationBehavior : Behavior<Entry>
    {
        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(entry);
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(entry);
        }

        private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = (Entry)sender;
            bool isValid = !string.IsNullOrWhiteSpace(e.NewTextValue);
            entry.TextColor = isValid ? Color.Default : Color.Red;
        }
    }
}
