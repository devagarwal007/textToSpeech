using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.IO;
using Telegram.Bot.Types.InputFiles;
using NAudio.Wave;
using NAudio.Lame;

namespace firstapp
{
    class Program
    {

        static ITelegramBotClient botClient;

        static async Task Main()
        {

            while (true)
            {
                try
                {
                    
                    botClient = new TelegramBotClient("1844622929:AAFdyx8WNHw_nRQuKHntSjTsHXKdB1beh3Q");

                    var me = botClient.GetMeAsync().Result;
                    Console.WriteLine(
                      $"Hello, World! I am user {me.Id} and my name is {me.FirstName}."
                    );

                    botClient.OnMessage += Bot_OnMessage;
                    botClient.StartReceiving();

                    Console.WriteLine("Press any key to exit");

                    Console.ReadKey();
                    botClient.StopReceiving();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error while program running :- {ex.Message} {ex.InnerException}");
                    Console.WriteLine($"Restart Connection Again in 10 Seconds.....");
                    Console.ResetColor();
                    Thread.Sleep(1000 * 10);
                }
            }
        }

        static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Text != null)
            {
                try
                {
                    if(File.Exists("C:\\ADO\\file.wav"))
                    {
                        File.Delete("C:\\ADO\\file.wav");
                    }
                    
                    string message = e.Message.Text;

                    await SynthesizeAudioAsync(message);

                    Console.WriteLine($"Received a {e.Message.Text} message in chat {e.Message.Chat.FirstName}.");

                    Stream content = new MemoryStream(ConvertWavToMp3(File.ReadAllBytes("C:\\ADO\\file.wav")));
                    InputOnlineFile inputOnlineFile = new InputOnlineFile(content, "AudioFile");
                    await botClient.SendAudioAsync(
                      chatId: e.Message.Chat,
                      audio: inputOnlineFile
                    );
                }
                catch (Exception exception)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error while getting the hashrate :- {exception.Message} {exception.InnerException}");
                    Console.WriteLine($"Please start mining in your machine.");
                    Console.ResetColor();
                }
            }
        }

        static async Task SynthesizeAudioAsync(string text)
        {
            Uri uri = new Uri("https://eastus.api.cognitive.microsoft.com/sts/v1.0/issuetoken");
            var config = SpeechConfig.FromEndpoint(uri, "865d434c1e724721ad6e572fcca96672");
            using var audioConfig = AudioConfig.FromWavFileOutput("C:\\ADO\\file.wav");
            config.SpeechSynthesisLanguage = "hi-IN";
            config.SpeechSynthesisVoiceName = "hi-IN-SwaraNeural";
            using var synthesizer = new SpeechSynthesizer(config, audioConfig);
            await synthesizer.SpeakTextAsync(text);
        }

        public static byte[] ConvertWavToMp3(byte[] wavFile)
        {
            using (var retMs = new MemoryStream())
            using (var ms = new MemoryStream(wavFile))
            using (var rdr = new WaveFileReader(ms))
            using (var wtr = new LameMP3FileWriter(retMs, rdr.WaveFormat, 128))
            {
                rdr.CopyTo(wtr);
                return retMs.ToArray();
            }
        }
    }
}