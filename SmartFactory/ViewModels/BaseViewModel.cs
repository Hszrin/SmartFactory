using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SmartFactory.ViewModels
{
    public abstract partial class BaseViewModel : ObservableObject, IAsyncInitializable
    {
        public abstract Task InitializeAsync(CancellationToken token);
        protected async Task LoadCollectionAsync<TItem>(
            Func<Task<List<TItem>>> getAllAsync,
            ObservableCollection<TItem> collection)
        {
            var items = await getAllAsync();

            collection.Clear();

            foreach (var item in items)
            {
                collection.Add(item);
            }
        }
        protected void ShowError(string message)
        {
            MessageBox.Show(
                message,
                "오류",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        protected void ShowInfo(string message)
        {
            MessageBox.Show(
                message,
                "알림",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}
