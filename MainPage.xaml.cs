﻿namespace MauiApp4
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new ProfileViewModel();
        }
    }
}