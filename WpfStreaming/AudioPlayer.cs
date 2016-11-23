using NAudio.Wave;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;

namespace WpfStreaming
{
    sealed class AudioPlayer
    {
        private IWavePlayer _waveOut;
        private Stream ms = new MemoryStream();
        Mp3FileReader reader;
        

        /// <summary>
        /// 再生を開始します。
        /// </summary>
        public void PlayStreaming(string url)
        {
            if (_waveOut == null)
            {
                new Thread(delegate (object o)
                {
                    var response = WebRequest.Create(url).GetResponse();
                    using (var stream = response.GetResponseStream())
                    {
                        byte[] buffer = new byte[65536]; // 64KB chunks

                        int read;

                        while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            var pos = ms.Position;
                            ms.Position = ms.Length;
                            ms.Write(buffer, 0, read);
                            ms.Position = pos;
                        }
                    }
                }).Start();
            }


            // Pre-buffering some data to allow NAudio to start playing
            while (ms.Length < 65536 * 10)
            {
                Thread.Sleep(1000);
            }

            int errorCont = 0;

            try
            {
                using (reader = new Mp3FileReader(ms))
                using (WaveStream pcm = WaveFormatConversionStream.CreatePcmStream(reader))
                using (WaveStream blockAlignedStream = new BlockAlignReductionStream(pcm))
                {
                    using (_waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
                    {
                        _waveOut.Init(blockAlignedStream);
                        _waveOut.Play();
                        while (_waveOut.PlaybackState == PlaybackState.Playing)
                        {
                            Console.WriteLine(reader.CurrentTime);
                            System.Threading.Thread.Sleep(100);
                        }
                        
                    }
                }
            }
            catch
            {
                if (errorCont > 10)
                {
                    roop = false;
                    MessageBox.Show("10回Tryしましたが再生に失敗しました。");
                }
                errorCont++;
            }



        }
        

        /// <summary>
        /// 再生を一時停止します。
        /// </summary>
        public void Pause()
        {
            this._waveOut.Pause();
        }

        /// <summary>
        /// 再生を停止します。
        /// </summary>
        public void Stop()
        {
            this._waveOut.Stop();
            this.ms.Position = 0;
        }

        public void AddSec(int sec)
        {
            this.reader.Position += (long)(reader.WaveFormat.AverageBytesPerSecond * 10);
        }

        public void BackSec(int sec)
        {
            this.reader.Position += (long)(reader.WaveFormat.AverageBytesPerSecond * -10);
        }
    }
}
