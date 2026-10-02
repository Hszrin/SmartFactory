using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;

namespace SmartFactory.ViewModels
{
    public abstract partial class BaseViewModel : ObservableObject, IAsyncInitializable
    {
        public abstract Task InitializeAsync(CancellationToken token);

        protected static async Task LoadCollectionAsync<TItem>(
            Func<Task<List<TItem>>> getAllAsync,
            ObservableCollection<TItem> collection)
        {
            var items = await getAllAsync();

            collection.Clear();
            foreach (var item in items)
                collection.Add(item);
        }

        protected static void ShowError(string message)
        {
            MessageBox.Show(
                message,
                "오류",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}
