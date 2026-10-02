using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using SmartFactory.Repositories.Interface;
using System.Collections.ObjectModel;
using System.Windows;

namespace SmartFactory.ViewModels
{
    public abstract partial class BaseCrudViewModel<T, TKey> : BaseViewModel
        where T : class
    {
        protected readonly ICommonRepository<T, TKey> _commonRepository;

        protected BaseCrudViewModel(ICommonRepository<T, TKey> commonRepository)
        {
            _commonRepository = commonRepository;
        }

        protected abstract ObservableCollection<T> Items { get; }
        protected abstract T? SelectedItem { get; set; }
        protected abstract TKey GetKey(T item);

        // Include 등 조회 방식이 엔티티마다 다르므로 실제 조회는 자식 ViewModel이 결정한다.
        protected abstract Task<List<T>> GetAllAsync(CancellationToken token);

        protected Task RefreshItemsAsync(CancellationToken token)
        {
            return LoadCollectionAsync(() => GetAllAsync(token), Items);
        }

        [RelayCommand]
        private Task Refresh()
        {
            return RefreshItemsAsync(CancellationToken.None);
        }

        [RelayCommand]
        private async Task Delete()
        {
            if (SelectedItem == null)
                return;

            var result = MessageBox.Show(
                GetDeleteConfirmMessage(SelectedItem),
                "삭제 확인",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                await _commonRepository.DeleteAsync(GetKey(SelectedItem));
                SelectedItem = null;
                await RefreshItemsAsync(CancellationToken.None);
                ClearInput();
            }
            catch (DbUpdateException)
            {
                ShowError(GetDeleteErrorMessage());
            }
        }

        protected virtual string GetDeleteConfirmMessage(T item)
        {
            return "선택한 데이터를 삭제하시겠습니까?";
        }

        protected virtual string GetDeleteErrorMessage()
        {
            return "다른 데이터에서 사용 중이라 삭제할 수 없습니다.";
        }

        protected virtual void ClearInput()
        {
        }
    }
}
