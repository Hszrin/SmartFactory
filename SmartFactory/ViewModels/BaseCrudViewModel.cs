using CommunityToolkit.Mvvm.ComponentModel;
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

        protected BaseCrudViewModel(
            ICommonRepository<T, TKey> commonRepository)
        {
            _commonRepository = commonRepository;
        }
        // 자식 ViewModel의 실제 Collection과 연결
        protected abstract ObservableCollection<T> Items { get; }
        // 자식 ViewModel의 실제 SelectedXXX와 연결
        protected abstract T? SelectedItem { get; set; }
        // PK를 가져오는 방법
        protected abstract TKey GetKey(T item);
        // 각 Repository마다 Include 등이 다를 수 있으므로
        // 실제 조회 방법은 자식이 결정
        protected abstract Task<List<T>> GetAllAsync(CancellationToken token);


        // =========================
        // Refresh
        // =========================
        protected async Task RefreshItemsAsync(CancellationToken token)
        {
            await LoadCollectionAsync(
                ()=>GetAllAsync(token),
                Items);
        }
        [RelayCommand]
        private async Task Refresh()
        {
            await RefreshItemsAsync(CancellationToken.None);
        }
        // =========================
        // Delete
        // =========================
        [RelayCommand]
        private async Task Delete()
        {
            if (SelectedItem == null)
                return;

            var result = MessageBox.Show(
                GetDeleteConfirmMessage(SelectedItem),
                "삭제 확인",
                MessageBoxButton.YesNo);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                await _commonRepository.DeleteAsync(
                    GetKey(SelectedItem));

                SelectedItem = null;

                await RefreshItemsAsync(CancellationToken.None);

                ClearInput();
            }
            catch (DbUpdateException)
            {
                MessageBox.Show(GetDeleteErrorMessage());
            }
        }
        // =========================
        // 자식에서 필요하면 변경
        // =========================
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