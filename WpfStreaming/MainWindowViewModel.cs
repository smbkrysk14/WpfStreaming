﻿using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfStreaming
{
    public class MainWindowViewModel:ViewModelBase
    {

        AudioPlayer audio = new AudioPlayer();

        public MainWindowViewModel()
        {
            this.EnclosureUrl = "http://cache.rebuild.fm/podcast-ep162a.mp3";
        }

        
        private async void Play()
        {
            string enclosureUrl = this.EnclosureUrl;

            await Task.Run(() =>
            {
                audio.PlayStreaming(enclosureUrl);
            });
        }

        private void Stop()
        {
            audio.Stop();
        }

        private void Pause()
        {
            audio.Pause();
        }

        private async void AddSec()
        {
            await Task.Run(() =>
                {
                    audio.AddSec(10);
                });
        }

        private async void BackSec()
        {
            await Task.Run(() =>
            {
                audio.BackSec(10);
            });
        }


        private string _enclosureUrl;
        public string EnclosureUrl
        {
            get
            {
                return this._enclosureUrl;
            }
            set
            {
                this._enclosureUrl = value;
                base.RaisePropertyChanged("EnclosureUrl");
            }
        }

        ICommand _playCommand;
        public ICommand PlayCommand
        {
            get
            {
                return _playCommand ?? (_playCommand = new DelegateCommand(Play));
            }
        }

        ICommand _stopCommand;
        public ICommand StopCommand
        {
            get
            {
                return _stopCommand ?? (_stopCommand = new DelegateCommand(Stop));
            }
        }

        ICommand _pauseCommand;
        public ICommand PauseCommand
        {
            get
            {
                return _pauseCommand ?? (_pauseCommand = new DelegateCommand(Pause));
            }
        }

        ICommand _addSecCommand;
        public ICommand AddSecCommand
        {
            get
            {
                return _addSecCommand ?? (_addSecCommand = new DelegateCommand(AddSec));
            }
        }

        ICommand _backSecCommand;
        public ICommand BackSecCommand
        {
            get
            {
                return _backSecCommand ?? (_backSecCommand = new DelegateCommand(BackSec));
            }
        }
    }
}
