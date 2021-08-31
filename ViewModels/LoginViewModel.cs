﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CMP332.Commands;
using CMP332.Models;
using CMP332.Services;
using CMP332.Stores;
using INavigationService = CMP332.Services.INavigationService;
using NavigateCommand = CMP332.Commands.NavigateCommand;

namespace CMP332.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {

        private string _username;
        private string _password;
        private string _errorMessage;
        private UserStore _userStore;

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }
        
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }


        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public ICommand LoginCommand{ get; }

        public LoginViewModel(UserStore userStore,INavigationService loginNavigationService)
        {
            //_userStore = userStore;
            //NavigateAccountCommand = new NavigateCommand(accountNavigationService);
            LoginCommand = new AsyncRelayCommand(Login,(ex)=>ErrorMessage = ex.Message);
        }

        private async Task Login()
        {
            User user = await new UserService().LoginUser(Username, Password);

            //_userStore.LoggedInUser = user;


        }
    }
}
