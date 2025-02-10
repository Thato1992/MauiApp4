using Microsoft.Maui.Media;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;

namespace MauiApp4
{
    public class ProfileViewModel : BindableObject
    {
        private string _profilePicture;
        private string _name;
        private string _surname;
        private string _email;
        private string _bio;

        private const string ProfileFileName = "profile.json"; // File name for storing profile data

        public string ProfilePicture
        {
            get => _profilePicture;
            set
            {
                _profilePicture = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Surname
        {
            get => _surname;
            set
            {
                _surname = value;
                OnPropertyChanged();
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        public string Bio
        {
            get => _bio;
            set
            {
                _bio = value;
                OnPropertyChanged();
            }
        }

        public Command PickPhotoCommand { get; }
        public Command SaveProfileCommand { get; }

        public ProfileViewModel()
        {
            PickPhotoCommand = new Command(async () => await PickPhotoAsync());
            SaveProfileCommand = new Command(async () => await SaveProfileAsync());
        }

        private async Task PickPhotoAsync()
        {
            try
            {
                if (MediaPicker.IsCaptureSupported)
                {
                    var photo = await MediaPicker.PickPhotoAsync();
                    if (photo != null)
                    {
                        var filePath = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);
                        using var stream = await photo.OpenReadAsync();
                        using var newStream = File.OpenWrite(filePath);
                        await stream.CopyToAsync(newStream);

                        ProfilePicture = filePath;
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Media Picker is not supported on this device.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        private async Task SaveProfileAsync()
        {
            try
            {
                // Create a profile object to store all the data
                var profile = new
                {
                    Name = this.Name,
                    Surname = this.Surname,
                    Email = this.Email,
                    Bio = this.Bio,
                    ProfilePicture = this.ProfilePicture // Save the profile picture file path
                };

                // Serialize the profile to JSON
                string json = JsonConvert.SerializeObject(profile);

                // Get the path to save the JSON file
                string filePath = Path.Combine(FileSystem.AppDataDirectory, ProfileFileName);

                // Write the JSON to a file
                await File.WriteAllTextAsync(filePath, json);

                await Application.Current.MainPage.DisplayAlert("File Path", $"Profile saved at: {filePath}", "OK");

            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        public async Task LoadProfileAsync()
        {
            try
            {
                string filePath = Path.Combine(FileSystem.AppDataDirectory, ProfileFileName);

                if (File.Exists(filePath))
                {
                    string json = await File.ReadAllTextAsync(filePath);
                    var profile = JsonConvert.DeserializeObject<dynamic>(json);

                    // Set the profile data to the properties
                    Name = profile.Name;
                    Surname = profile.Surname;
                    Email = profile.Email;
                    Bio = profile.Bio;
                    ProfilePicture = profile.ProfilePicture;
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Profile file not found.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }
    }
}
