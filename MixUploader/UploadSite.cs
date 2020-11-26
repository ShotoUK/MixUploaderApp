using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Globalization;
using System.IO;
using System.Net.Http.Headers;
using Windows.Web.Http.Headers;
using Windows.UI.Popups;

namespace MixUploader
{
    public class UploadSite
    {
        public string responsebody { get; set; }

        public static string UploadTitle { get; set; }

        public static string UploadDescription { get; set; }

        public static string UploadUnlisted  { get; set; } 

        public static byte[] audiofile { get; set; }

        public static string filename { get; set; }

        public static string MixcloudAuthCode { get; set; } = "3aLMAp4mbrN5SSb5sp4tAcP3sq7naRck";

        public static long MixcloudMaxBytes { get; set; } = 4294967296;



        public async Task UploadToMixcloudAsync(byte[] file)
        {
            try
            {
                /*Live Mixcloud URL*/
                //Uri uri = new Uri($"https://api.mixcloud.com/upload/?access_token={MixcloudAuthCode}");

                /*Test Dump URL*/
                Uri uri = new Uri($"http://ptsv2.com/t/vmygd-1602012853/post?access_token={MixcloudAuthCode}");

                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMinutes(10);

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                    client.DefaultRequestHeaders.AcceptEncoding.TryParseAdd("gzip");
                    client.DefaultRequestHeaders.AcceptEncoding.TryParseAdd("deflate");
                    client.DefaultRequestHeaders.AcceptEncoding.TryParseAdd("br");


                    using (var formcontent = new MultipartFormDataContent("Upload----" /*+ DateTime.Now.ToString(CultureInfo.InvariantCulture)*/))
                    {

                        formcontent.Headers.ContentType.MediaType = "multipart/form-data";

                        formcontent.Add(new ByteArrayContent(file, 0, file.Length), "mp3", filename);
                        formcontent.Add(new StringContent(UploadTitle), "name");
                        formcontent.Add(new StringContent(UploadUnlisted), "unlisted");
                        formcontent.Add(new StringContent(UploadDescription), "description");

                        HttpResponseMessage httpreponse = new HttpResponseMessage();

                        if (Convert.ToInt64(file.Length) > MixcloudMaxBytes)
                        {
                            Console.WriteLine("Filesize too large");
                            responsebody = "Mixcloud: Filesize too large";
                        }
                        else
                        {
                            using (var Upload = await client.PostAsync(uri, formcontent))
                            {


                                try
                                {
                                    Upload.EnsureSuccessStatusCode();

                                    responsebody = "Mixcloud: Upload Successful";
                                }
                                catch (HttpRequestException ex)
                                {
                                    responsebody = $"{ex.Source} says {ex.Message} - Response: {Upload.ReasonPhrase.ToString()}";
                                }
                            }
                        }


                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
 
        }

        public async Task GetMixcloudAsync()
        {
            HttpClient client = new HttpClient();
            Uri uri = new Uri("https://api.mixcloud.com/shoto_uk/in-the-lab-talks-1-with-nurtured-beatz-recordings-kalm-spindall/");
            HttpResponseMessage httpreponse = new HttpResponseMessage();
            httpreponse = await client.GetAsync(uri);
            //httpreponse.EnsureSuccessStatusCode();
            responsebody = await httpreponse.Content.ReadAsStringAsync();

        }
    }
}
