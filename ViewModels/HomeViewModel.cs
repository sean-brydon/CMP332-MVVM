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
using System.Collections.ObjectModel;

namespace CMP332.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        public ICommand NavigateToLoginCommand { get; }
        private UserStore _userStore;

        private  ObservableCollection<Property> _propertiesForUser;
        private  ObservableCollection<Inspection> _inspectionsForUser;
        private  ObservableCollection<Invoice> _overduePayments;


        public bool IsLoggedIn => _userStore.IsLoggedIn;

        private string _username;

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        private string _role;

        public string Role
        {
            get => _role;
            set
            {
                _role = value;
                OnPropertyChanged(nameof(Role));
            }
        }

        public ObservableCollection<Property> PropertiesForUser
        {
            get => _propertiesForUser;
            set
            {
                _propertiesForUser = value;
            }
        }

        public ObservableCollection<Inspection> InspectionsForUser
        {
            get => _inspectionsForUser;
            set
            {
                _inspectionsForUser = value;
            }
        }
        public ObservableCollection<Invoice> OverDuePayments
        {
            get => _overduePayments;
            set
            {
                _overduePayments = value;
            }
        }

        

        public HomeViewModel(UserStore userStore, INavigationService loginNavigationService)
        {
            NavigateToLoginCommand = new NavigateCommand(loginNavigationService);
            _userStore = userStore;

            _userStore.LoggedInUserChanged += OnCurrentUserChanged;

        }

        private void OnCurrentUserChanged() 
        {
            // rerender the value in this viewmodel when this action fires
            OnPropertyChanged(nameof(IsLoggedIn));
            this.Username = _userStore.LoggedInUser?.Username;
            this.Role = _userStore.LoggedInUser?.Role.Name;
            // Return null here to prevent a nested if statement - using guard clauses is easier to read.
            if (_userStore.LoggedInUser == null) return;
            // Load properties on inital load
            if (_userStore.IsAdmin)
            {
                List<Property> propertiesAsList = new PropertyService().GetAll();
                List<Inspection> inspectionsAsList = new InspectionService().GetAll();
                PropertiesForUser = new ObservableCollection<Property>(propertiesAsList);
                InspectionsForUser = new ObservableCollection<Inspection>(inspectionsAsList);
            }
            else
            {
                List<Property> propertiesAsList = new PropertyService().GetAllPropertiesByUser(_userStore.LoggedInUser);
                List<Inspection> inspectionsAsList = new InspectionService().GetInspectionsFromUser(_userStore.LoggedInUser);
                PropertiesForUser = new ObservableCollection<Property>(propertiesAsList);
                InspectionsForUser = new ObservableCollection<Inspection>(inspectionsAsList);
            }
            // Needs to happen for everyone
            List<Invoice> overduePayments = new InvoiceService().FindAllOverdue().ToList();
            OverDuePayments = new ObservableCollection<Invoice>(overduePayments);

            OnPropertyChanged(nameof(PropertiesForUser));
            OnPropertyChanged(nameof(InspectionsForUser));
            OnPropertyChanged(nameof(OverDuePayments));

        }

        public override void Dispose()
        {
            // Remove the listener when we navigate away from the page
            _userStore.LoggedInUserChanged -= OnCurrentUserChanged;
            base.Dispose();
        }
    }
}
