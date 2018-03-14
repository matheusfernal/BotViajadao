using System;
using System.Configuration;
using System.IO;
using System.Net;

namespace BotViajadao.Services
{
    public class ServicoReconhecimentoFala
    {
        public string ConverterAudio(string urlAudio)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create("https://api.cognitive.microsoft.com/sts/v1.0");
            request.SendChunked = true;
            request.Accept = @"application/json;text/xml";
            request.Method = "POST";
            request.ProtocolVersion = HttpVersion.Version11;
            request.ContentType = @"audio/wav; codec=audio/pcm; samplerate=16000";
            request.Headers["Ocp-Apim-Subscription-Key"] = ConfigurationManager.AppSettings["ReconhecimentoFalaApiKey"];

            ////

            // Send an audio file by 1024 byte chunks
            using (var fs = HttpWebRequest.Create(urlAudio).GetResponse().GetResponseStream())
            {

                /*
                * Open a request stream and write 1024 byte chunks in the stream one at a time.
                */
                byte[] buffer = null;
                var bytesRead = 0;
                using (var requestStream = request.GetRequestStream())
                {
                    /*
                    * Read 1024 raw bytes from the input audio file.
                    */
                    buffer = new byte[checked((uint)Math.Min(1024, (int)fs.Length))];
                    while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        requestStream.Write(buffer, 0, bytesRead);
                    }

                    // Flush
                    requestStream.Flush();
                }
            }

            string responseString;
            /*
            * Get the response from the service.
            */
            using (var response = request.GetResponse())
            {
                Console.WriteLine(((HttpWebResponse)response).StatusCode);

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    responseString = sr.ReadToEnd();
                }

            }

            return responseString;
        }
    }
}